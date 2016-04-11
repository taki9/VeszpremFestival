using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class Ticket
    {
        int event_id;
        int _price;
        string _currency;

        public int Price
        {
            get
            {
                return _price;
            }

            set
            {
                _price = value;
            }
        }

        public string Currency
        {
            get
            {
                return _currency;
            }

            set
            {
                _currency = value;
            }
        }

        public Ticket(int price, string currency = "Ft")
        {
            this.Price = price;
            this.Currency = currency;
        }
    }
}
