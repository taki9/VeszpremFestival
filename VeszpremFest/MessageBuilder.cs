using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
            return "Bejelentkezéshez a # jel után adja meg a felhasználói nevét és jelszavát vesszővel elválasztva.\n\n" +
                "Ha még nem regisztrált, akkor a .Név,Felhasználói név,Jelszó parancs megadásával tud\n" + 
                "Példa: .Kiss Béla,bela,bela\n\n" +
                "Menü:\n" +
                "1) Előadások listázása\n" +
                "2) Kilépés\n";
        }

        public string mainMenuForAdmin()
        {
            return "1) Előadások listázása\n" +
                "2) Eladó felvétele\n" +
                "3) Helyszín hozzáadása\n" +
                "4) Esemény hozzáadása\n" +
                "5) Kijelentkezés\n" +
                "6) Kilépés\n";
        }

        public string mainMenuForUser()
        {
            return "1) Előadások listázása\n" +
                "2) Új foglalás\n" +
                "3) Foglalásaim\n" +
                "4) Kijelentkezés\n" +
                "5) Kilépés\n";
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

        public string eventList(EventController evc)
        {
            string tmp = "";

            if (evc.numOfEvents() > 0)
            {
                tmp += "Rendezvények:\n";
                tmp += "-------------\n\n";
            } else
            {
                tmp += "Jelenleg nincs újabb koncert!";
            }

            for (int i = 0; i < evc.numOfEvents(); i++)
            {
                Event ev = evc.getEventByIndex(i);

                tmp += "Sorszám: " + ev.Id + "\n";
                tmp += "Előadás: " + ev.PerformName + "\n";
                tmp += "Ideje: " + ev.Start.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) + "\n";
                tmp += "Helye: " + ev.Location.Name + "\n";
                tmp += "Szabad helyek: " + ev.AvailSeats + "\n";
                tmp += "\n";
            }

            return tmp;
        }

        public string newOrder()
        {
            return "Új rendelés leadásához először az esemény azonosítója segítségével le kell kérned az ülőhelyek listáját az alábbi módon: ?azonosító.\n" +
                "Válassza ki a kívánt szabad helyet, majd az alábbi paranccsal lefoglalhatja az: ?azonosító,sor,oszlop\n" +
                "Példa: ?2,5,7 - 2-es azonosítójú eseményre foglal jegyet az 5. sor 7. székére";
        }

        
        public string ordersList(Client kliens)
        {
            if (kliens.UserID != 0)
            {
                string orderString = "";

                if (kliens.MyOrder.numOfOrders() > 0)
                {
                    orderString += "Az eddig leadott foglalásai, és azokhoz tartozó székrendelések:\n";
                    orderString += "---------------------------------------------------------------\n\n";

                    for(int i = 0; i < kliens.MyOrder.numOfOrders(); i++)
                    {
                        Order ord = kliens.MyOrder.getOrder(i);

                        orderString += "Előadás: " + ord.Perform + "\n";
                        orderString += "Ideje: " + ord.Start + "\n";
                        orderString += "Helye: " + ord.Loc + "\n";
                        orderString += "Székek:\n";

                        for (int j = 0; j < ord.numOfSeats(); j++)
                        {
                            Seat seat = ord.getSeat(j);
                            orderString += "\t" + seat.RowNumber + ". sor " + seat.ColumnNumber + ". oszlop: " + seat.SeatStatus + "\n";
                        }

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
