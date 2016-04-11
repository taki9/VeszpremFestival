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
                    if (message.head.STATUS.Equals("COMMAND") && message.head.STATUSCODE.Equals("MENU"))
                    {
                        if (message.body.MESSAGE.Equals("1"))
                        {
                            SendMessage(msgBuilder.performanceList(), kliens);
                        }
                        else if (message.body.MESSAGE.Equals("3"))
                        {

                            kliens.socket.Client.Shutdown(SocketShutdown.Send);
                            kliens.socket.Client.Close();
                            connectedClients--;
                            kliens.clientThread.Abort();
                            clientList.Remove(kliens);
                            Console.WriteLine("Client disconnected!");
                            break;
                        }
                    } 
                    else if (message.head.STATUS.Equals("COMMAND") && message.head.STATUSCODE.Equals("ORDERS"))
                    {
                        SendMessage(msgBuilder.ordersList(kliens), kliens);

                    }
                    else if (message.head.STATUS.Equals("ORDER"))
                    {
                        
                        string[] orderString = Regex.Split(message.body.MESSAGE, ",");
                        
                        if (kliens.UserID != 0)
                        {

                            Order order = new Order();

                            SendMessage(order.newOrder(orderString, kliens), kliens);
                        } else
                        {
                            SendMessage("Először be kell jelentkeznie!", kliens);
                        }
                    }
                    else if (message.head.STATUS.Equals("LOGIN")) {
                        string[] userData = Regex.Split(message.body.MESSAGE, ",");

                        SendMessage(kliens.Login(userData[0], userData[1]), kliens);
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
