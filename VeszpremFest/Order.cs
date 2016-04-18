using System;
using System.Data;

namespace server
{
    class Order
    {
        private int eventID;
        private int reservedTickets;
        private int selectedTickets;

        public Order(int eventID, int selTickets)
        {
            this.eventID = eventID;
            this.reservedTickets = 0;
            this.selectedTickets = selTickets;
        }

        public string newOrder(string[] orderString, Client kliens)
        {
            Database db = new Database();

            DataTable order = db.selectQuery("SELECT * FROM Events INNER JOIN Performances On Perform_ID = PerformID WHERE EventID = " + orderString[0] + ";");

            if (order.Rows.Count > 0)
            {
                db.executeQuery("INSERT INTO Tickets VALUES(null, " + order.Rows[0].Field<Int64>("EventID") + ", " + orderString[1] + ", " + kliens.UserID + ");");

                db.executeQuery("UPDATE Events SET AvailSeats = AvailSeats - " + orderString[1] + " WHERE EventID = " + order.Rows[0].Field<Int64>("EventID") + ";");

                kliens.TicketOrder = true;

                return "Sikeresen foglalt " + orderString[1] + " darab jegyet a " + order.Rows[0].Field<string>("PerformName") + " rendezvényre!";
            }
            return "Nincs ilyen rendezvény!";
        }

        public string addTicket(string[] ticketString, Client kliens)
        {
            Database db = new Database();

            DataTable seat = db.selectQuery("SELECT * FROM Seats WHERE Event_ID = " + eventID + " AND RowNumber = " + ticketString[0] + " AND ColumnNumber = " + ticketString[1] + ";");


            if (selectedTickets == reservedTickets)
            {
                return "Nem választhat több jegyet a rendeléshez!\n";
            }

            else if (seat.Rows.Count == 0) {
                db.executeQuery("INSERT INTO Seats VALUES(null, " + ticketString[0] + ", " + ticketString[1] + ", " + 1 + ", " + eventID + ");");

                reservedTickets++;

                if (reservedTickets == selectedTickets)
                {
                    kliens.TicketOrder = false;

                    return ("Sikeresen lefoglalta az utolsó helyet a foglalásához!\n");
                }

                return "Sikeresen lefoglalta a helyet!\n";
            }

            return "A hely már foglalt!\n";
        }
        
        //public int CountPayment()
        //{
        //    int sum = 0;

        //    foreach (OrderedItem item in OrderedItems)
        //    {
        //        sum += item.Ticket.Price * item.Quantity;
        //    }

        //    return sum;
        //}

    }

}
