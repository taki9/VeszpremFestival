using System;
using System.Data;

namespace server
{
    class Order
    {
        public string newOrder(string[] orderString, Client kliens)
        {
            Database db = new Database();

            DataTable order = db.selectQuery("SELECT * FROM Events INNER JOIN Performances On Perform_ID = PerformID WHERE EventID = " + orderString[0] + ";");

            if (order.Rows.Count > 0)
            {
                db.executeQuery("INSERT INTO Tickets VALUES(null, " + order.Rows[0].Field<Int64>("EventID") + ", " + orderString[1] + ", " + kliens.UserID + ");");

                db.executeQuery("UPDATE Events SET AvailSeats = AvailSeats - " + orderString[1] + " WHERE EventID = " + order.Rows[0].Field<Int64>("EventID") + ";");

                return "Sikeresen foglalt " + orderString[1] + " darab jegyet a " + order.Rows[0].Field<string>("PerformName") + " rendezvényre!";
            }
            return "Nincs ilyen rendezvény!";
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
