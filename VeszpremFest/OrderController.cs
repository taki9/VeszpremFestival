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

        public void readEveryOder(EventController evc, DbController dbc)
        {
            DataTable orderTable = dbc.readOrders();

            foreach (DataRow row in orderTable.Rows)
            {
                Event ev = evc.getEventByID(Convert.ToInt32(row.Field<Int64>("Event_ID")));

                Order order = new Order(ev.PerformName, ev.Location.Name, ev.Start, row.Field<string>("Username"));

                DataTable seatTable = dbc.readSeatsForEvent(ev.Id);

                foreach (DataRow seat in seatTable.Rows)
                {
                    string status = "";
                    int statusNumber = Convert.ToInt32(seat.Field<Int64>("SeatStatus"));

                    if (statusNumber == 1)
                    {
                        status = "foglalt";
                    }
                    else if (statusNumber == 2)
                    {
                        status = "fizetett";
                    }
                    else if (statusNumber == 3)
                    {
                        status = "hitelesitett";
                    }

                    order.addSeat(new Seat(Convert.ToInt32(seat.Field<Int64>("RowNumber")), Convert.ToInt32(seat.Field<Int64>("ColumnNumber")), status));
                }

                Orders.Add(order);
            }

        }

        public void readOrdersFromDb(int userID, string username, EventController evc, DbController dbc)
        {
            DataTable orderTable = dbc.readOrders(userID);

            foreach (DataRow row in orderTable.Rows)
            {
                Event ev = evc.getEventByID(Convert.ToInt32(row.Field<Int64>("Event_ID")));

                Order order = new Order(ev.PerformName, ev.Location.Name, ev.Start, username);

                DataTable seatTable = dbc.readSeatsForEvent(ev.Id, userID);

                foreach (DataRow seat in seatTable.Rows)
                {
                    string status = "";
                    int statusNumber = Convert.ToInt32(seat.Field<Int64>("SeatStatus"));

                    if (statusNumber == 1)
                    {
                        status = "foglalt";
                    } else if (statusNumber == 2)
                    {
                        status = "fizetett";
                    } else if (statusNumber == 3)
                    {
                        status = "hitelesitett";
                    }

                    order.addSeat(new Seat(Convert.ToInt32(seat.Field<Int64>("RowNumber")), Convert.ToInt32(seat.Field<Int64>("ColumnNumber")), status));
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

        public Order getOrder(string perform, string loc, DateTime start) 
        {
            int i = 0;

            while (i < Orders.Count && !(Orders[i].Perform.Equals(perform) && Orders[i].Loc.Equals(loc) && DateTime.Equals(Orders[i].Start, start)))
            {
                i++;
            }

            if (i < Orders.Count)
            {
                return Orders[i];
            }

            return null;
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

        public bool verifyPayment(int keresett, DbController dbc)
        {
            string orderList = "Hitelesítendő fizetések listája\n";
            orderList += "-----------------------------------------------\n";

            int sorszam = 1;
            foreach (Order order in Orders)
            {
                foreach (Seat seat in order.Seats)
                {
                    if (seat.SeatStatus.Equals("fizetett"))
                    {
                        if (sorszam == keresett)
                        {
                            seat.SeatStatus = "hitelesített";

                            dbc.verifyPayment(order.Buyer, seat.RowNumber, seat.ColumnNumber);

                            return true;
                        }

                        sorszam++;
                    }
                }
            }

            return false;
        }

    }
}
