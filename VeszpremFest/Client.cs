using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Data;

namespace server
{
    class Client
    {
        private TcpClient _socket;
        private Thread _clientThread;

        private int _userID; // 0 if not logged in
        private string _username;
        private string _userType;

        private OrderController _myOrder;

        public Client(TcpClient socket, Thread clientThread, int userid = 0, string usertype = "client")
        {
            Socket = socket;
            ClientThread = clientThread;
            ClientThread.Start(socket);
            UserID = userid;
            Username = "";
            UserType = usertype;
            MyOrder = new OrderController();
        }

        public TcpClient Socket
        {
            set { this._socket = value; }
            get { return this._socket; }
        }

        public Thread ClientThread
        {
            set { this._clientThread = value; }
            get { return this._clientThread; }
        }

        public int UserID
        {
            get
            {
                return _userID;
            }

            set
            {
                _userID = value;
            }
        }

        public string UserType
        {
            get
            {
                return _userType;
            }

            set
            {
                _userType = value;
            }
        }

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

        internal OrderController MyOrder
        {
            get
            {
                return _myOrder;
            }

            set
            {
                _myOrder = value;
            }
        }

        public string Register(string name, string username, string password, DbController dbc)
        {
            if (UserID == 0)
            {
                DataTable user = dbc.findUserByName(username);

                if (user.Rows.Count > 0)
                {
                    return "A megadott felhasználói név foglalt!";
                }

                dbc.registerUser(username, password, name);

                return "Sikeresen regisztrált! Kérem lépjen be.";
            }

            return "Már bejelentkezett mint " + Username + "!";
        }

        public string Login(string username, string password, DbController dbc)
        {
            if (UserID == 0)
            {
                DataTable user = dbc.userAuth(username, password);

                if (user.Rows.Count > 0)
                {
                    UserID = Convert.ToInt32(user.Rows[0].Field<Int64>("UserID"));
                    UserType = user.Rows[0].Field<string>("UserType");
                    Username = user.Rows[0].Field<string>("Username");

                    return username + " sikeresen bejelentkezett!";
                }

                return "Sikertelen! Felhasználónév vagy jelszó nem megfelelő!";
            } else
            {
                return "Már korábban bejelentkezett!";
            }
        }

        public string Logout()
        {
            UserID = 0;
            UserType = "client";

            return "Sikeresen kijelentkezett!";
        }

        public string showMenu()
        {
            MessageBuilder msgBuilder = new MessageBuilder();
            if (UserID != 0 && UserType == "user")
            {
                return msgBuilder.mainMenuForUser();
            }
            else if (UserID != 0 && UserType == "admin")
            {
                return msgBuilder.mainMenuForAdmin();
            }
            else if (UserID != 0 && UserType == "seller")
            {
                return msgBuilder.mainMenuForSeller();
            }

            return msgBuilder.mainMenuForClient();
        }

        public void Disconnect()
        {
            Socket.Client.Shutdown(SocketShutdown.Send);
            Socket.Client.Close();
        }
    }
}
