using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class OrderController
    {
        private List<Order> _orders;

        public OrderController()
        {
            Orders = new List<Order>();
        }

        internal List<Order> Orders
        {
            get
            {
                return _orders;
            }

            set
            {
                _orders = value;
            }
        }

        public void readOrdersFromDb(int userID, EventController evc, DbController dbc)
        {
            DataTable orderTable = dbc.readOrders(userID);

            foreach (DataRow row in orderTable.Rows)
            {
                Event ev = evc.getEventByID(Convert.ToInt32(row.Field<Int64>("Event_ID")));

                Order order = new Order(ev.PerformName, ev.Location.Name, ev.Start);

                DataTable seatTable = dbc.readSeatsForEvent(ev.Id, userID);

                foreach (DataRow seat in seatTable.Rows)
                {
                    order.addSeat(new Seat(Convert.ToInt32(seat.Field<Int64>("RowNumber")), Convert.ToInt32(seat.Field<Int64>("ColumnNumber"))));
                }

                Orders.Add(order);
            }
        }

        public int numOfOrders()
        {
            return Orders.Count();
        }

        public Order getOrder(int index)
        {
            return Orders[index];
        }

        public bool newOrder(Order order)
        {
            int i = 0;

            while (i < Orders.Count && Orders[i].Perform != order.Perform && Orders[i].Loc != order.Loc && !(Orders[i].Start.Equals(order.Start)))
            {
                i++;
            }

            if (i < Orders.Count)
            {
                Orders[i].addSeat(order.Seats[0]);

                return true;
            }

            Orders.Add(order);

            return false;
        }

    }
}
