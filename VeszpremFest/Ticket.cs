using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class Ticket
    {
        string _name;
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

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public Ticket(string name,  int price, string currency = "Ft")
        {
            this.Name = name;
            this.Price = price;
            this.Currency = currency;
        }
    }
}
