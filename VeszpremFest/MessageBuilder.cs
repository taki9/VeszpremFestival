using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class MessageBuilder
    {
        public string welcomeMessage()
        {
            return "Köszöntelek a VeszprémFeszt koncert jegyirodában!\n";
        }

        public string mainMenuForClient()
        {
            return "Bejelentkezéshez a # jel után adja meg a felhasználói nevét és jelszavát vesszővel elválasztva.";
        }

        public string mainMenuForUser()
        {
            return "1) Előadások listázása\n" +
                "2) Foglalásaim\n" +
                "3) Kilépés\n";
        }

        public string serverStart()
        {
            return ("### SERVER ###\n\n" +
                "### Server started...###\n" +
                "### Waiting for clients ###");
        }

        public string newClientConnected(int clientNum)
        {
            return (">> Client connected!\n") +
                ">> Amount of connected clients: " + clientNum;
        }

        public string performanceList()
        {
            string tmp = "";
            Database db = new Database();

            DataTable events = db.selectQuery("SELECT * FROM Events INNER JOIN Performances On Perform_ID = PerformID INNER JOIN Locations On Location_ID = LocationID;");

            if (events.Rows.Count > 0)
            {
                tmp += "Rendezvények:\n";
                tmp += "-------------\n\n";

                foreach (DataRow row in events.Rows)
                {
                    tmp += "Sorszám: " + row.Field<Int64>("EventID") + "\n";
                    tmp += "Előadás: " + row.Field<string>("PerformName") + "\n";
                    tmp += "Ideje: " + row.Field<string>("Start") + "\n";
                    tmp += "Helye: " + row.Field<string>("LocationName") + "\n";
                    tmp += "Szabad helyek: " + row.Field<Int64>("AvailSeats") + "\n";
                    tmp += "\n";
                }
            } else
            {
                tmp += "Jelenleg nincs újabb koncert!";
            }

            return tmp;
        }

        
        public string ordersList(Client kliens)
        {

            if (kliens.UserID != 0)
            {
                Database db = new Database();

                DataTable myOrder = db.selectQuery("SELECT * FROM Tickets INNER JOIN Events On Event_ID = EventID INNER JOIN Performances On Perform_ID = PerformID INNER JOIN Locations On Location_ID = LocationID WHERE User_ID =" + kliens.UserID + ";");

                string orderString = "";

                if (myOrder.Rows.Count > 0)
                {
                    orderString += "Az eddig leadott foglalásai, és azok állapota:\n";
                    orderString += "----------------------------------------------\n\n";

                    foreach (DataRow row in myOrder.Rows)
                    {
                        orderString += "Előadás: " + row.Field<string>("PerformName") + "\n";
                        orderString += "Ideje: " + row.Field<string>("Start") + "\n";
                        orderString += "Helye: " + row.Field<string>("LocationName") + "\n";
                        orderString += "Jegyek száma: " + row.Field<Int64>("Quantity") + "\n";
                        orderString += "\n";
                    }

                    return orderString;
                }

                return "Még nincs foglalása!";
            }

            return "Nem vagy bejelentkezve! Kérlek előbb lépj be!";
        }


        public string seatMap(int eventID)
        {
            Database db = new Database();

            DataTable eventData = db.selectQuery("SELECT * FROM Events INNER JOIN Locations ON LocationID = Location_ID WHERE EventID = " + eventID + ";");
            DataTable seats = db.selectQuery("SELECT * FROM Seats WHERE Event_ID = " + eventID + ";");

            int rows = Convert.ToInt32(eventData.Rows[0].Field<Int64>("seatRow"));
            int columns = Convert.ToInt32(eventData.Rows[0].Field<Int64>("seatColumn"));

            char seatStatus = 'F';
            char[,] seatMap = new char[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    seatMap[i, j] = 'F';
                }
            }

            foreach (DataRow row in seats.Rows)
            {

                if (row.Field<Int64>("SeatStatus") == 1)
                {
                    seatStatus = 'R';
                } else
                {
                    seatStatus = 'P';
                }

                seatMap[row.Field<Int64>("RowNumber") - 1, row.Field<Int64>("ColumnNumber") - 1] = seatStatus;
            }


            string seatMapString = "";


            seatMapString += "Válasszon helye(ke)t a rendeléséhez!\n\nAz előadás térképe:\n\n";

            seatMapString += "  |";

            if (columns > 9)
            {
                int i = 0;
                for (i = 0; i < 9; i++)
                {
                    seatMapString += " " + (i + 1) + " |";
                }

                for (; i < columns; i++)
                {
                    seatMapString += "" + (i + 1) + " |";
                }
            }
            else
            {
                for (int i = 0; i < columns; i++)
                {
                    seatMapString += " |" + (i + 1);
                }
            }
            seatMapString += "\n";

            for (int i = 0; i < rows; i++)
            {
                if (i < 9)
                {
                    seatMapString += " " + (i + 1) + "|";
                }
                else
                {
                    seatMapString += (i + 1) + "|";
                }

                for (int j = 0; j < columns; j++)
                {
                    seatMapString += " " + seatMap[i, j] + " |";
                }
                seatMapString += "\n";
            }

            seatMapString += "\nF - szabad\nR - foglalt\nP - fizetett\n";

            return seatMapString;
        }
    }

}
