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


            // LOGIN
            if (msg.Substring(0, 1) == "#")
                {
                    msg = msg.Substring(1, msg.Length - 1);

                    message = new Message("LOGIN", msg);
            }
            else if (msg.Substring(0, 1) == "!")
            {
                msg = msg.Substring(1, msg.Length - 1);
                message = new Message("COMMAND", "MENU", msg);
            }
            else if (msg.Substring(0, 1) == "+")
            {
                msg = msg.Substring(1, msg.Length - 1);
                message = new Message("ORDER", msg);
            }
            else if (msg.Substring(0, 1) == ".")
            {
                msg = msg.Substring(1, msg.Length - 1);
                if (msg.Substring(0, 1) == "O")
                {
                    message = new Message("COMMAND", "ORDERS", msg);
                }


            }

            return message;
        }

    }
}
