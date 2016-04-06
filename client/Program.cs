using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace client
{
    class Client
    {
        private Thread commThread;

        private TcpClient client = new TcpClient();
        private IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);

        private void connectionToServer()
        {

            // Try to connect to the server
            // If fails, throw exception
            // todo: Handle the exception
            try
            {
                client.Connect(serverEndPoint);

                // connected
            }
            catch (Exception)
            {
                throw;
            }

            this.commThread = new Thread(new ThreadStart(ReceiveMessage));
            commThread.Start();
        }

        private void ReceiveMessage()
        {

            byte[] msg = new byte[4096];
            int bytesRead;
            NetworkStream clientStream = client.GetStream();

            while (true)
            {
                bytesRead = clientStream.Read(msg, 0, 4096);
                UTF8Encoding encoder = new UTF8Encoding();
                string message = encoder.GetString(msg, 0, bytesRead);

                Console.WriteLine(message);
            }
        }

        private void SendMessage(string msg)
        {
            NetworkStream clientStream = client.GetStream();

            Message message = new MessagePreprocessor().preprocessing(msg);

            string json = JsonConvert.SerializeObject(message);

            string csomag = "BEGINBEGIN¤"
                        + json.Length
                        + "¤"
                        + json
                        + "¤"
                        + "ENDEND"
                        ;

            UTF8Encoding encoder = new UTF8Encoding();
            byte[] buffer = encoder.GetBytes(csomag);

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();

            if (message != null && (message.head.STATUS.Equals("COMMAND") && message.head.STATUS.Equals("COMMAND") && message.body.MESSAGE.Equals("3")))
            {
                Environment.Exit(0);
            }
        }

        static void Main(string[] args)
        {
            Client newClient = new Client();
            newClient.connectionToServer();

            string message = "";

            while (true)
            {
                message = Console.ReadLine();

                newClient.SendMessage(message);
            }
        }


    }
}
