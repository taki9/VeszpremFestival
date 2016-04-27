using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace client
{
    class MessagePreprocessor
    {
        public Message preprocessing(string msg)
        {
            Message message = null;


            if (msg.Substring(0, 1) == "#")
            {
                msg = msg.Substring(1, msg.Length - 1);

                string[] data = Regex.Split(msg, ",");

                if (data.Length == 2)
                {
                    message = new Message("LOGIN", msg);
                } else if (data.Length == 4)
                {
                    message = new Message("COMMAND", "NEWSELLER", msg);
                }
            }
            else if (msg.Substring(0, 1) == "!")
            {
                msg = msg.Substring(1, msg.Length - 1);
                message = new Message("COMMAND", "MENU", msg);
            }
            else if (msg.Substring(0, 1) == "+")
            {
                msg = msg.Substring(1, msg.Length - 1);

                string[] data = Regex.Split(msg, ",");

                if (data.Length == 1)
                {
                    message = new Message("COMMAND", "NEWPERFORM", msg);
                } else if (data.Length == 3)
                {
                    message = new Message("COMMAND", "NEWEVENT", msg);
                } else if (data.Length == 4)
                {
                    message = new Message("COMMAND", "NEWLOCATION", msg);
                }
            }
            else if (msg.Substring(0,1) == ".")
            {
                msg = msg.Substring(1, msg.Length - 1);
                message = new Message("REGISTER", msg);
            } else if (msg.Substring(0,1) == "?")
            {
                msg = msg.Substring(1, msg.Length - 1);

                string[] data = Regex.Split(msg, ",");

                if (data.Length == 1)
                {
                    message = new Message("SEATMAP", msg);
                }
                else if (data.Length == 3)
                {
                    message = new Message("ORDER", msg);
                }
            }

            return message;
        }

    }
}
