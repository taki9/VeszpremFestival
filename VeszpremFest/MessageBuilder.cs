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
    }

}
