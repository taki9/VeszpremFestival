using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace server
{
    class Client
    {
        private TcpClient _socket;
        private Thread _clientThread;
        private Order myOrder;


        public Client(TcpClient socket, Thread clientThread)
        {
            this.socket = socket;
            this.clientThread = clientThread;
            this.clientThread.Start(socket);
            this.MyOrder = new Order();
        }

        public TcpClient socket
        {
            set { this._socket = value; }
            get { return this._socket; }
        }

        public Thread clientThread
        {
            set { this._clientThread = value; }
            get { return this._clientThread; }
        }

        internal Order MyOrder
        {
            get
            {
                return myOrder;
            }

            set
            {
                myOrder = value;
            }
        }
    }
}
