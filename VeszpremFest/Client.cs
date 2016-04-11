﻿using System;
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
        private string _userType;

        public Client(TcpClient socket, Thread clientThread, int userid = 0, string usertype = "user")
        {
            this.socket = socket;
            this.clientThread = clientThread;
            this.clientThread.Start(socket);
            this.UserID = userid;
            this.UserType = usertype;
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

        public string Login(string username, string password)
        {
            Database db = new Database();


            if (UserID == 0)
            {
                DataTable user = db.selectQuery("SELECT * FROM Users WHERE Username = '" + username + "' AND Password = '" + password + "';");


                if (user.Rows.Count > 0)
                {
                    UserID = Convert.ToInt32(user.Rows[0].Field<Int64>("UserID"));
                    UserType = user.Rows[0].Field<string>("UserType");
                    return username + " sikeresen bejelentkezett!";
                }

                return "Sikertelen! Felhasználónév vagy jelszó nem megfelelő!";
            } else
            {
                return "Már korábban bejelentkezett!";
            }
        }
        
        public string showMenu()
        {
            MessageBuilder msgBuilder = new MessageBuilder();
            if (UserID != 0 && UserType == "user")
            {
                return msgBuilder.mainMenuForUser();
            }

            return msgBuilder.mainMenuForClient();
        }
            }
}
