using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Data;
using System.Globalization;

namespace server
{
    class Server
    {
        private TcpListener tcpListener;
        private int connectedClients = 0;
        private Thread shopThread;
        private List<Client> clientList = new List<Client>();
        private MessageBuilder msgBuilder = new MessageBuilder();

        private DbController dbc = new DbController();
        private EventController evc = new EventController();
        private LocationController lc = new LocationController();
        private OrderController oc = new OrderController();

        private void startServer()
        {
            this.tcpListener = new TcpListener(IPAddress.Loopback, 3000); // Change to IPAddress. Any for internet wide Communication
            // Clients can connect on localhost with 3000 port.

            this.shopThread = new Thread(new ThreadStart(ListenForClients)); // wait for client connections
            this.shopThread.Start();

            evc.readEventsFromDb(dbc);
            lc.readLocationsFromDb(dbc);
            oc.readEveryOder(evc, dbc);
        }

        private void ListenForClients()
        {
            this.tcpListener.Start();
            // server started
            Console.WriteLine(msgBuilder.serverStart());

            while (true) // Never ends until the Server is closed.
            {

                TcpClient client = this.tcpListener.AcceptTcpClient();
                //blocks until a client has connected to the server

                connectedClients++;
                // new client connected
                Client newClient = new Client(client, new Thread(new ParameterizedThreadStart(ReceiveMessageFromClient)));

                clientList.Add(newClient);
                // classes:  client -> user    assistant -> admin

                SendMessage(msgBuilder.welcomeMessage(), newClient);
                SendMessage(msgBuilder.mainMenuForClient(), newClient);

                Console.WriteLine(msgBuilder.newClientConnected(connectedClients));
            }

        }

        private void ReceiveMessageFromClient(object client) //client
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] fromClient = new byte[4096];
            int bytesRead;


            while (true)
            {
                // olvas a klienstől ----- 
                bytesRead = 0;


                Client kliens = Identify(tcpClient);


                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(fromClient, 0, 4096);
                }
                catch (Exception ex)
                {
                    connectedClients--;
                    Console.WriteLine("Client disconnected! Number of clients: " + connectedClients);
                    kliens.Disconnect();
                    clientList.Remove(kliens);
                    break;
                    //a socket error has occured
                }

                //message has been successfully received

                UTF8Encoding encoder = new UTF8Encoding();

                // Convert the Bytes received to a string and display it on the Server Screen
                string csomag = encoder.GetString(fromClient, 0, bytesRead);

                string json = null;
                int jsonHossz = 0;
                int x = 0, y = 0;


                while (csomag[x] != '¤') x++;
                if (csomag.Substring(0, x).Contains("BEGINBEGIN"))
                {
                    x++;
                    y = x + 1;
                    while (csomag[y] != '¤') y++;
                    jsonHossz = Int32.Parse(csomag.Substring(x, y - x));

                    x = y + 1;
                    y = x + 1;

                    while (csomag[y] != '¤') y++;

                    if (csomag.Substring(x, y - x).Length == jsonHossz
                        && csomag.Contains("ENDEND"))
                    {
                        json = csomag.Substring(x, y - x);
                    }
                }

                Message message = null;
                if (json != null)
                {
                    message = JsonConvert.DeserializeObject<Message>(json);
                }
                else
                {
                    Console.WriteLine("Incoming Message Error!");
                }

                if (message != null)
                {
                    if (kliens.UserType == "user")
                    {
                        if (message.head.STATUS.Equals("COMMAND") && message.head.STATUSCODE.Equals("MENU"))
                        {
                            if (message.body.MESSAGE.Equals("1"))
                            {
                                SendMessage(msgBuilder.eventList(evc), kliens);
                            }
                            else if (message.body.MESSAGE.Equals("2"))
                            {
                                SendMessage(msgBuilder.newOrder(), kliens);
                            }
                            else if (message.body.MESSAGE.Equals("3"))
                            {
                                SendMessage(msgBuilder.ordersList(kliens), kliens);
                            }
                            else if (message.body.MESSAGE.Equals("4"))
                            {
                                SendMessage(msgBuilder.payableList(kliens), kliens);
                            }
                            else if (message.body.MESSAGE.Equals("5"))
                            {
                                SendMessage(kliens.Logout(), kliens);
                            }
                            else if (message.body.MESSAGE.Equals("6"))
                            {
                                DisconnectClient(kliens);
                                break;
                            }
                            else if (message.body.MESSAGE.Equals(""))
                            {
                                SendMessage(kliens.showMenu(), kliens);
                            }
                        }
                        else if (message.head.STATUS.Equals("LOGIN"))
                        {
                            string[] userData = Regex.Split(message.body.MESSAGE, ",");

                            SendMessage("Már korábban bejelentkezett, mint " + kliens.Username, kliens);
                        }
                        else if (message.head.STATUS.Equals("SEATMAP"))
                        {
                            SendMessage(msgBuilder.seatMap(Convert.ToInt32(message.body.MESSAGE)), kliens);
                        }
                        else if (message.head.STATUS.Equals("PAY"))
                        {
                            string[] results = (message.body.MESSAGE).Split(new char[] { ',', ':' }, StringSplitOptions.RemoveEmptyEntries);

                            SendMessage(msgBuilder.paymentList(results, dbc, oc, lc, kliens), kliens);
                        }
                        else if (message.head.STATUS.Equals("ORDER"))
                        {
                            string[] orderString = Regex.Split(message.body.MESSAGE, ",");

                            Event ev = evc.getEventByID(Convert.ToInt32(orderString[0]));

                            int row = Convert.ToInt32(orderString[1]);
                            int column = Convert.ToInt32(orderString[2]);

                            if (ev != null)
                            {
                                if (row <= ev.Location.SeatRow && column <= ev.Location.SeatColumn)
                                {
                                    if (dbc.reserveSeat(ev.Id, row, column, kliens.UserID))
                                    {
                                        Order order = new Order(ev.PerformName, ev.Location.Name, ev.Start, kliens.Username);
                                        order.addSeat(new Seat(row, column));

                                        if (kliens.MyOrder.newOrder(order) == false)
                                        {
                                            // KLIENS DIDNT ORDERED FOR THIS EVENT BEFORE
                                            dbc.newOrder(ev.Id, kliens.UserID);
                                        }
                                        oc.newOrder(order);

                                        ev.AvailSeats -= 1;

                                        SendMessage("Sikeresen foglalt jegyet a(z) " + ev.PerformName + " eseményre!", kliens);
                                    }
                                    else
                                    {
                                        SendMessage("A kiválasztott hely már foglalt.", kliens);
                                    }
                                }
                                else
                                {
                                    SendMessage("A megadott hely nem létezik erre az előadásra!", kliens);
                                }
                            }
                            else
                            {
                                SendMessage("Nincs ilyen esemény!", kliens);
                            }
                        }
                    }
                    else if (kliens.UserType == "admin")
                    {
                        if (message.head.STATUS.Equals("COMMAND") && message.head.STATUSCODE.Equals("MENU"))
                        {
                            if (message.body.MESSAGE.Equals("1"))
                            {
                                SendMessage(msgBuilder.eventList(evc), kliens);
                            }
                            else if (message.body.MESSAGE.Equals("2"))
                            {
                                SendMessage("Eladó felvételéhez adja meg a # karakter után a felhasználói nevét, jelszavát, nevét és hogy mióta dolgozik a cégnél.\nPélda: #seller,seller,Nagy Péter,2015-09-30", kliens);
                            }
                            else if (message.body.MESSAGE.Equals("3"))
                            {
                                SendMessage("Helyszín felvételéhez adja meg a + karakter után a helyszín nevét, az ott vásárolható jegyek árát, a nézőtér sorainak és oszlopainak számát.\nPélda: +Színpad,3000,20,18", kliens);
                            }
                            else if (message.body.MESSAGE.Equals("4"))
                            {
                                SendMessage("Esemény felvételéhez adja meg a + karakter után az előadás és helyszín nevét, az esemény időpontját.\nPélda: +Linkin Park,Színpad,2016-05-04 21:00", kliens);
                            }
                            else if (message.body.MESSAGE.Equals("5"))
                            {
                                SendMessage(kliens.Logout(), kliens);
                            }
                            else if (message.body.MESSAGE.Equals("6"))
                            {
                                DisconnectClient(kliens);
                                break;
                            }
                            else if (message.body.MESSAGE.Equals(""))
                            {
                                SendMessage(kliens.showMenu(), kliens);
                            }
                        }
                        else if (message.head.STATUS.Equals("COMMAND") && message.head.STATUSCODE.Equals("NEWSELLER"))
                        {
                            string[] data = Regex.Split(message.body.MESSAGE, ",");

                            DataTable seller = dbc.findUserByName(data[0]);

                            if (seller.Rows.Count > 0)
                            {
                                SendMessage("A megadott felhasználói név foglalt!", kliens);
                            }
                            else
                            {
                                dbc.registerUser(data[0], data[1], data[2]);

                                seller = dbc.findUserByName(data[0]);
                                int userID = Convert.ToInt32(seller.Rows[0].Field<Int64>("UserID"));

                                dbc.registerSeller(data[3], userID);

                                SendMessage("Eladó sikeresen felvéve!\n", kliens);
                            }
                        }
                        else if (message.head.STATUS.Equals("COMMAND") && message.head.STATUSCODE.Equals("NEWLOCATION"))
                        {
                            string[] data = Regex.Split(message.body.MESSAGE, ",");

                            Location loc = lc.findLocationByName(data[0]);

                            if (loc != null)
                            {
                                SendMessage("Ez a helyszín már szerepel!", kliens);
                            }
                            else
                            {
                                lc.addLocation(new Location(lc.numOfLocations() + 1, data[0], Convert.ToInt32(data[1]), Convert.ToInt32(data[2]), Convert.ToInt32(data[3])));
                                dbc.newLocation(data[0], data[1], data[2], data[3]);

                                SendMessage("Helyszín sikeresen hozzáadva!\n", kliens);
                            }
                        }
                        else if (message.head.STATUS.Equals("COMMAND") && message.head.STATUSCODE.Equals("NEWEVENT"))
                        {
                            string[] data = Regex.Split(message.body.MESSAGE, ",");

                            DataTable perform = dbc.findPerformByName(data[0]);

                            if (perform.Rows.Count == 0)
                            {
                                dbc.newPerform(data[0]);
                            }

                            Event ev = evc.findEvent(data[0], data[1], DateTime.ParseExact(data[2], "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture));

                            if (ev != null)
                            {
                                SendMessage("Ez az esemény már szerepel!", kliens);
                            }
                            else
                            {
                                perform = dbc.findPerformByName(data[0]);

                                Location loc = lc.findLocationByName(data[1]);

                                if (loc == null)
                                {
                                    SendMessage("Nem létező helyszín!", kliens);
                                }
                                else
                                {
                                    evc.addEvent(new Event(evc.numOfEvents() + 1, perform.Rows[0].Field<string>("PerformName"), loc, DateTime.ParseExact(data[2], "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), loc.SeatColumn * loc.SeatRow));
                                    dbc.newEvent(Convert.ToInt32(perform.Rows[0].Field<Int64>("PerformID")), loc.Id, data[2], loc.SeatColumn * loc.SeatRow);
                                    SendMessage("Esemény sikeresen hozzáadva!", kliens);
                                }
                            }
                        }
                    }
                    else if (kliens.UserType == "seller")
                    {
                        if (message.head.STATUS.Equals("COMMAND") && message.head.STATUSCODE.Equals("MENU"))
                        {
                            if (message.body.MESSAGE.Equals("1"))
                            {
                                SendMessage(msgBuilder.eventList(evc), kliens);
                            }
                            else if (message.body.MESSAGE.Equals("2"))
                            {
                                SendMessage(msgBuilder.verifyPayment(oc), kliens);
                            }
                            else if (message.body.MESSAGE.Equals("3"))
                            {
                                SendMessage(kliens.Logout(), kliens);
                            }
                            else if (message.body.MESSAGE.Equals("4"))
                            {
                                DisconnectClient(kliens);
                                break;
                            }
                            else if (message.body.MESSAGE.Equals(""))
                            {
                                SendMessage(kliens.showMenu(), kliens);
                            }
                        } else if (message.head.STATUS.Equals("VERIFY"))
                        {
                            if (oc.verifyPayment(Convert.ToInt32(message.body.MESSAGE), dbc))
                            {
                                SendMessage("Sikeres hitelesítés!", kliens);
                            }
                            else
                            {
                                SendMessage("Sikertelen hitelesítés!", kliens);
                            }
                        }
                    }
                    else
                    {
                        if (message.head.STATUS.Equals("COMMAND") && message.head.STATUSCODE.Equals("MENU"))
                        {
                            if (message.body.MESSAGE.Equals("1"))
                            {
                                SendMessage(msgBuilder.eventList(evc), kliens);
                            }
                            else if (message.body.MESSAGE.Equals("2"))
                            {
                                DisconnectClient(kliens);
                                break;
                            }
                            else if (message.body.MESSAGE.Equals(""))
                            {
                                SendMessage(kliens.showMenu(), kliens);
                            }
                        }

                        if (message.head.STATUS.Equals("LOGIN"))
                        {
                            string[] userData = Regex.Split(message.body.MESSAGE, ",");

                            bool newLogin = true;

                            if (kliens.UserID != 0)
                            {
                                newLogin = false;
                            }

                            SendMessage(kliens.Login(userData[0], userData[1], dbc), kliens);

                            if (newLogin && kliens.UserID != 0)
                            {
                                kliens.MyOrder.readOrdersFromDb(kliens.UserID, kliens.Username, evc, dbc);
                            }

                            SendMessage(kliens.showMenu(), kliens);
                        }
                        else if (message.head.STATUS.Equals("REGISTER"))
                        {
                            string[] userData = Regex.Split(message.body.MESSAGE, ",");

                            SendMessage(kliens.Register(userData[0], userData[1], userData[2], dbc), kliens);
                        }
                    }
                }
            }
        }

        private void SendMessage(string msg, Client consignee)
        {
            foreach (Client kliens in clientList)
            {
                if (kliens.Socket == consignee.Socket)
                {
                    NetworkStream clientStream = consignee.Socket.GetStream();

                    UTF8Encoding encoder = new UTF8Encoding();
                    byte[] buffer = encoder.GetBytes(msg);

                    clientStream.Write(buffer, 0, buffer.Length);
                    clientStream.Flush();
                }
            }
        }

        private Client Identify(TcpClient client)
        {
            foreach (Client kliens in clientList)
            {
                if (client == kliens.Socket)
                {
                    return kliens;
                }
            }
            return null;
        }

        private void DisconnectClient(Client kliens)
        {
            connectedClients--;
            Console.WriteLine("Client disconnected! Number of clients: " + connectedClients);
            SendMessage("QUIT", kliens);
            kliens.Disconnect();
            clientList.Remove(kliens);
        }


        static void Main(string[] args)
        {
            Server rf = new Server();

            rf.startServer();
        }
    }
}
