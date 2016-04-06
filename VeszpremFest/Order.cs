using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class Order
    {
        private List<OrderedItem> orderedItems;
        private Boolean _verified;

        internal List<OrderedItem> OrderedItems
        {
            get
            {
                return orderedItems;
            }

            set
            {
                orderedItems = value;
            }
        }

        public Order()
        {
            OrderedItems = new List<OrderedItem>();
            this._verified = false;

        }

        public void addItem(OrderedItem ticket)
        {
            OrderedItems.Add(ticket);
        }

        public int numOfOrderedTickets()
        {
            return OrderedItems.Count;
        }

        public int CountPayment()
        {
            int sum = 0;

            foreach (OrderedItem item in OrderedItems)
            {
                sum += item.Ticket.Price * item.Quantity;
            }

            return sum;
        }

    }

}
