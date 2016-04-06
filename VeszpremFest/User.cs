using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server
{
    class User : Client {
        private string _username;
        private string _password;
        private Order myOrder;

        public string Username
        {
            get
            {
                return _username;
            }

            set
            {
                _username = value;
            }
        }

        public string Password
        {
            set
            {
                _password = value;
            }
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

        public User(string username, string password, TcpClient socket, Thread clientThread) : base(socket, clientThread)
        {
            this.Username = username;
            this._password = password;
        }

        public bool passwordCheck(string pass)
        {
            if (pass == this._password)
            {
                return true;
            }

            return false;
        }
    }
}
