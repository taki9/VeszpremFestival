using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    public class User
    {
        string _username;
        string _password;

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
            get
            {
                return _password;
            }

            set
            {
                _password = value;
            }
        }

        public User(string username, string password)
        {
            this._username = username;
            this._password = password;
        }
    }
}
