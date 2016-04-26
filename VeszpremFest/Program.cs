using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using System.Data;

namespace server
{
    class Server
    {
        private TcpListener tcpListener;
        private int connectedClients = 0;
        private Thread shopThread;
        private List<Client> clientList = new List<Client>();
        private MessageBuilder msgBuilder = new MessageBuilder();

        // CONNECTION STRING

        private void startServer()
        {
            this.tcpListener = new TcpListener(IPAddress.Loopback, 3000); // Change to IPAddress. Any for internet wide Communication
            // Clients can connect on the localhost with 3000 port.

            this.shopThread = new Thread(new ThreadStart(ListenForClients)); // wait for client connections
            this.shopThread.Start();
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
                    //a socket error has occured
                    Console.WriteLine(ex.Message);
                    break;
                }

                if (bytesRead == 0)
                {
                    connectedClients--;
                    kliens.clientThread.Abort();
                    clientList.Remove(kliens);
                    Console.WriteLine("Client disconnected!");
                    break;
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
                                SendMessage(msgBuilder.performanceList(), kliens);
                            }
                            else if (message.body.MESSAGE.Equals("2"))
                            {
                                SendMessage(msgBuilder.ordersList(kliens), kliens);
                            }
                            else if (message.body.MESSAGE.Equals("3"))
                            {
                                SendMessage(kliens.Logout(), kliens);
                            }

                            else if (message.body.MESSAGE.Equals("4"))
                            {
                                Console.WriteLine("Client disconnected!");
                                SendMessage("QUIT", kliens);
                                kliens.socket.Client.Shutdown(SocketShutdown.Send);
                                kliens.socket.Client.Close();
                                connectedClients--;
                                kliens.clientThread.Abort();
                                clientList.Remove(kliens);
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

                            SendMessage(kliens.Login(userData[0], userData[1]), kliens);
                            SendMessage(msgBuilder.mainMenuForUser(), kliens);
                        }
                        else if (message.head.STATUS.Equals("ORDER"))
                        {
                            string[] orderString = Regex.Split(message.body.MESSAGE, ",");

                            if (kliens.TicketOrder == false)
                            {
                                kliens.AktOrder = new Order(Convert.ToInt32(orderString[0]), Convert.ToInt32(orderString[1]));

                                SendMessage(kliens.AktOrder.newOrder(orderString, kliens), kliens);
                                SendMessage(msgBuilder.seatMap(Convert.ToInt32(orderString[0])), kliens);
                            }
                            else
                            {
                                SendMessage(kliens.AktOrder.addTicket(orderString, kliens), kliens);
                            }
                        }
                    } else if (kliens.UserType == "admin")
                    {
                        if (message.head.STATUS.Equals("COMMAND") && message.head.STATUSCODE.Equals("MENU"))
                        {
                            if (message.body.MESSAGE.Equals("1"))
                            {
                                SendMessage(msgBuilder.performanceList(), kliens);
                            }
                            else if (message.body.MESSAGE.Equals("2"))
                            {
                                SendMessage("Eladó felvételéhez adja meg a # karakter után a felhasználói nevét, jelszavát, nevét és hogy mióta dolgozik a cégnél.\nPélda: #seller,seller,Nagy Péter,2015-09-30", kliens);
                            }
                            else if (message.body.MESSAGE.Equals("3"))
                            {
                                SendMessage("Előadás felvételéhez adja meg a + karakter után az előadás nevét.\nPélda: +Linkin Park", kliens);
                            }
                            else if (message.body.MESSAGE.Equals("4"))
                            {
                                SendMessage("Helyszín felvételéhez adja meg a + karakter után a helyszín nevét, az ott vásárolható jegyek árát, a nézőtér sorainak és oszlopainak számát.\nPélda: +Színpad,3000,20,18", kliens);
                            }
                            else if (message.body.MESSAGE.Equals("5"))
                            {
                                SendMessage("Esemény felvételéhez adja meg a + karakter után az előadás és helyszín nevét.\nPélda: +Linkin Park,Színpad", kliens);
                            }
                            else if (message.body.MESSAGE.Equals("6"))
                            {
                                SendMessage(kliens.Logout(), kliens);
                            }

                            else if (message.body.MESSAGE.Equals("7"))
                            {
                                Console.WriteLine("Client disconnected!");
                                SendMessage("QUIT", kliens);
                                kliens.socket.Client.Shutdown(SocketShutdown.Send);
                                kliens.socket.Client.Close();
                                connectedClients--;
                                kliens.clientThread.Abort();
                                clientList.Remove(kliens);
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

                            Database db = new Database();

                            DataTable seller = db.selectQuery("SELECT * FROM Users WHERE Username = '" + data[0] + "';");

                            if (seller.Rows.Count > 0)
                            {
                                SendMessage("A megadott felhasználói név foglalt!", kliens);
                            }
                            else
                            {
                                db.executeQuery("INSERT INTO Users VALUES(null, " + data[0] + ", " + data[1] + ", " + data[2] + ", seller);");

                                seller = db.selectQuery("SELECT * FROM Users WHERE Username = '" + data[0] + "';");
                                int userID = seller.Rows[0].Field<Int32>("UserID");

                                db.executeQuery("INSERT INTO Sellers VALUES(null, " + data[3] + ", " + userID + ");");

                                SendMessage("Eladó sikeresen felvéve!\n", kliens);
                            }
                        }
                        else if (message.head.STATUS.Equals("COMMAND") && message.head.STATUSCODE.Equals("NEWPERFORM"))
                        {
                            Database db = new Database();

                            DataTable perform = db.selectQuery("SELECT * FROM Performances WHERE PerformName = '" + message.body.MESSAGE + "';");

                            if (perform.Rows.Count > 0)
                            {
                                SendMessage("Az előadás már létezik!", kliens);
                            }
                            else
                            {
                                db.executeQuery("INSERT INTO Performances VALUES(null, " + message.body.MESSAGE + ");");

                                SendMessage("Előadás sikeresen felvéve!\n", kliens);
                            }
                        }
                        else if (message.head.STATUS.Equals("COMMAND") && message.head.STATUSCODE.Equals("NEWLOCATION"))
                        {
                            string[] data = Regex.Split(message.body.MESSAGE, ",");

                            Database db = new Database();

                            DataTable loc = db.selectQuery("SELECT * FROM Locations WHERE LocationName = '" + data[0] + "';");

                            if (loc.Rows.Count > 0)
                            {
                                SendMessage("Ez a helyszín már szerepel!", kliens);
                            }
                            else
                            {
                                db.executeQuery("INSERT INTO Locations VALUES(null, " + data[0] + ", " + data[1] + ", " + data[2] + ", " + data[3] + ");");

                                SendMessage("Helyszín sikeresen hozzáadva!\n", kliens);
                            }
                        }
                        else if (message.head.STATUS.Equals("COMMAND") && message.head.STATUSCODE.Equals("NEWEVENT"))
                        {
                            string[] data = Regex.Split(message.body.MESSAGE, ",");

                            Database db = new Database();

                            DataTable ev = db.selectQuery("SELECT * FROM Events INNER JOIN Performances On Perform_ID = PerformID INNER JOIN Locations On Location_ID = LocationID WHERE PerformName = '" + data[0] + "'" + "AND LocationName = '" + data[1] + "';");

                            if (ev.Rows.Count > 0)
                            {
                                SendMessage("Ez az esemény már szerepel!", kliens);
                            }
                            else
                            {
                                DataTable perform = db.selectQuery("SELECT * FROM Performances WHERE PerformName = '" + data[0] + "';");

                                if (perform.Rows.Count == 0)
                                {
                                    SendMessage("Nem létező előadás!", kliens);
                                }
                                else
                                {
                                    DataTable loc = db.selectQuery("SELECT * FROM Locations WHERE LocationName = '" + data[1] + "';");

                                    if (loc.Rows.Count == 0)
                                    {
                                        SendMessage("Nem létező helyszín!", kliens);
                                    }
                                    else
                                    {
                                        db.executeQuery("INSERT INTO Events VALUES(null, " + perform.Rows[0].Field<Int64>("PerformID") + ", " + loc.Rows[0].Field<Int64>("LocationID") + ", " + data[3] + ", " + (loc.Rows[0].Field<Int64>("seatRow") * loc.Rows[0].Field<Int64>("seatColumn")) + ");");
                                        SendMessage("Esemény sikeresen hozzáadva!", kliens);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (message.head.STATUS.Equals("COMMAND") && message.head.STATUSCODE.Equals("MENU"))
                        {
                            if (message.body.MESSAGE.Equals("1"))
                            {
                                SendMessage(msgBuilder.performanceList(), kliens);
                            }
                            else if (message.body.MESSAGE.Equals("2"))
                            {
                                Console.WriteLine("Client disconnected!");
                                SendMessage("QUIT", kliens);
                                kliens.socket.Client.Shutdown(SocketShutdown.Send);
                                kliens.socket.Client.Close();
                                connectedClients--;
                                kliens.clientThread.Abort();
                                clientList.Remove(kliens);
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

                            SendMessage(kliens.Login(userData[0], userData[1]), kliens);
                            SendMessage(kliens.showMenu(), kliens);
                        }
                        else if (message.head.Equals("REGISTER"))
                        {
                            string[] userData = Regex.Split(message.body.MESSAGE, ",");

                            SendMessage(kliens.Register(userData[0], userData[1], userData[2]), kliens);
                        }
                    }
                }
            }
        }

        private void SendMessage(string msg, Client consignee)
        {
            foreach (Client kliens in clientList)
            {
                if (kliens.socket == consignee.socket)
                {
                    NetworkStream clientStream = consignee.socket.GetStream();

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
                if (client == kliens.socket)
                {
                    return kliens;
                }
            }
            return null;
        }


        static void Main(string[] args)
        {
            Server rf = new Server();

            rf.startServer();
        }
    }
}
