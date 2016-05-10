using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class DbController
    {
        private Database db;

        public DbController()
        {
            db = new Database();
        }

        public DataTable readEvents()
        {
            return db.selectQuery("SELECT * FROM Events INNER JOIN Performances On Perform_ID = PerformID INNER JOIN Locations On Location_ID = LocationID;");
        }

        public DataTable readLocations()
        {
            return db.selectQuery("SELECT * FROM Locations;");
        }

        public DataTable readOrders(int userID)
        {
            return db.selectQuery("SELECT * FROM Tickets WHERE User_ID=" + userID + ";");
        }

        public DataTable readOrders()
        {
            return db.selectQuery("SELECT * FROM Tickets INNER JOIN Users ON User_ID = UserID;");
        }

        public DataTable readSeatsForEvent(int eventID)
        {
            return db.selectQuery("SELECT * FROM Seats WHERE Event_ID=" + eventID + ";");
        }

        public DataTable readSeatsForEvent(int eventID, int userID)
        {
            return db.selectQuery("SELECT * FROM Seats WHERE Event_ID=" + eventID + " AND User_ID=" + userID + ";");
        }

        public DataTable userAuth(string username, string password)
        {
            return db.selectQuery("SELECT * FROM Users WHERE Username = '" + username + "' AND Password = '" + password + "';");
        }

        public DataTable findUserByName(string username)
        {
            return db.selectQuery("SELECT * FROM Users WHERE Username = '" + username + "';");
        }

        public void registerUser(string username, string password, string name)
        {
            db.executeQuery("INSERT INTO Users VALUES(null, '" + username + "', '" + password + "', '" + name + "', 'user');");
        }

        public void registerSeller(string employeeSince, int userID)
        {
            db.executeQuery("INSERT INTO Sellers VALUES(null, '" + employeeSince + "', " + userID + ");");
        }

        public DataTable findPerformByName(string name)
        {
            return db.selectQuery("SELECT * FROM Performances WHERE PerformName = '" + name + "';");
        }

        public void newLocation(string name, string price, string seatRow, string seatColumn)
        {
            db.executeQuery("INSERT INTO Locations VALUES(null, '" + name + "', " + price + ", " + seatRow + ", " + seatColumn + ");");
        }

        public void newPerform(string name)
        {
            db.executeQuery("INSERT INTO Performances VALUES(null, '" + name + "');");
        }

        public void newEvent(int performID, int locationID, string start, int availseats)
        {
            db.executeQuery("INSERT INTO Events VALUES(null, " + performID + ", " + locationID + ", '" + start + "', " + availseats + ");");
        }

        public void newOrder(int eventID, int userID)
        {
            db.executeQuery("INSERT INTO Tickets VALUES(null, " + eventID + ", 0, " + userID + ");");
        }

        public bool reserveSeat(int eventID, int row, int column, int userID)
        {
            DataTable seat = db.selectQuery("SELECT * FROM Seats WHERE Event_ID=" + eventID + " AND RowNumber=" + row + " AND ColumnNumber=" + column + ";");

            if (seat.Rows.Count == 0)
            {
                db.executeQuery("UPDATE Events SET AvailSeats = AvailSeats - 1 WHERE EventID = " + eventID + ";");
                db.executeQuery("INSERT INTO Seats VALUES(null, " + row + ", " + column + ", " + 1 + ", " + eventID + ", " + userID + ");");

                return true;
            }
            return false;
        }

        public void payTicket(int userID, int row, int column)
        {
            db.executeQuery("UPDATE Seats SET SeatStatus = 2 WHERE User_ID = " + userID + " AND RowNumber=" + row + " AND ColumnNumber=" + column + ";");
        }

        public void verifyPayment(string username, int row, int column)
        {
            db.executeQuery("UPDATE Seats SET SeatStatus = 3 WHERE User_ID IN(SELECT UserID FROM Users WHERE Username = '" +  username + "') AND RowNumber=" + row + " AND ColumnNumber=" + column + ";");
        }
    }
}
