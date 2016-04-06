using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class OrderedItem
    {
        Performance performance;
        Ticket ticket;
        int _quantity;

        public OrderedItem(Performance performance, Ticket ticket, int quantity)
        {
            this.Performance = performance;
            this.Ticket = ticket;
            this.Quantity = quantity;
        }

        public int Quantity
        {
            get
            {
                return _quantity;
            }

            set
            {
                _quantity = value;
            }
        }

        internal Performance Performance
        {
            get
            {
                return performance;
            }

            set
            {
                performance = value;
            }
        }

        internal Ticket Ticket
        {
            get
            {
                return ticket;
            }

            set
            {
                ticket = value;
            }
        }
    }

}
