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
                "4) Fizetés\n" +
                "5) Kijelentkezés\n" +
                "6) Kilépés\n";
        }

        public string mainMenuForSeller()
        {
            return "1) Előadások listázása\n" +
                "2) Vásárlás hitelesítése\n" +
                "3) Kijelentkezés\n" +
                "4) Kilépés\n";
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

        public string payableList(Client kliens)
        {
            if (kliens.UserID != 0)
            {
                string orderString = "";

                if (kliens.MyOrder.numOfOrders() > 0)
                {
                    orderString += "Fizetéshez adja meg a fizetendő jegyeket az alábbi módon:\n";
                    orderString += "F1:3,2:4 - az első előadás 3. helyjegyét és a 2. előadás 4. helyjegyét fizetjük ebben az esetben\n";
                    orderString += "---------------------------------------------------------------\n\n";


                    for (int i = 0; i < kliens.MyOrder.numOfOrders(); i++)
                    {
                        Order ord = kliens.MyOrder.getOrder(i);

                        bool notPaidSeat = false;

                        int j = 0;
                        while (j < ord.numOfSeats() && notPaidSeat == false)
                        {
                            if (ord.getSeat(j).SeatStatus == "foglalt")
                            {
                                notPaidSeat = true;
                            }
                            j++;    
                        }

                        if (notPaidSeat)
                        {
                            orderString += "Sorszám: " + i + "\n";
                            orderString += "Előadás: " + ord.Perform + "\n";
                            orderString += "Ideje: " + ord.Start + "\n";
                            orderString += "Helye: " + ord.Loc + "\n";
                            orderString += "Székek:\n";

                            for (j = 0; j < ord.numOfSeats(); j++)
                            {
                                Seat seat = ord.getSeat(j);
                                if (seat.SeatStatus == "foglalt")
                                {
                                    orderString += "\t" + "sorszám: " + j + ", " + seat.RowNumber + ". sor " + seat.ColumnNumber + ". oszlop: " + seat.SeatStatus + "\n";
                                }
                                
                            }

                            orderString += "\n";
                        }
                    }

                    return orderString;
                }

                return "Még nincs foglalása!";
            }

            return "Nem vagy bejelentkezve! Kérlek előbb lépj be!";
        }

        public string paymentList(string[] results, DbController dbc, OrderController oc, LocationController lc, Client kliens)
        {
            int i = 0;

            string payment = "Fizetett jegyek\n";
            payment += "--------------------------------\n";
            double sum = 0;

            Dictionary<string, int> events = new Dictionary<string, int>();
            for (int j = 0; j < results.Length; j += 2)
            {
                events[results[j]] = 0;
            }

            for (int j = 0; j < results.Length; j += 2)
            {
                events[results[j]] += 1;
            }
            Order order;
            Seat seat;
            while (i < results.Length)
            {

                order = kliens.MyOrder.getOrder(Convert.ToInt32(results[i]));
                seat = order.getSeat(Convert.ToInt32(results[i + 1]));

                oc.getOrder(order.Perform, order.Loc, order.Start).getSeat(seat.RowNumber, seat.ColumnNumber).SeatStatus = "fizetett";

                payment += order.Perform + ", " + order.Loc + ", " + order.Start + ", " + seat.RowNumber + ". sor " + seat.ColumnNumber + ". oszlop\n";
                payment += "Ára: " + lc.findLocationByName(order.Loc).Price + "Ft\n\n";

                sum += lc.findLocationByName(order.Loc).Price;

                seat.SeatStatus = "fizetett";
                dbc.payTicket(kliens.UserID, seat.RowNumber, seat.ColumnNumber);

                i += 2;
            }

            foreach (KeyValuePair<string, int> entry in events)
            {
                if (entry.Value >= 4)
                {
                    payment += "Kedvezmény! A(z) " + kliens.MyOrder.getOrder(Convert.ToInt32(entry.Key)).Perform + " rendezvényre " + entry.Value + " darab jegyet vett, így 20% kedvezményt kap rájuk!\n";
                    double kedvezmeny = lc.findLocationByName(kliens.MyOrder.getOrder(Convert.ToInt32(entry.Key)).Loc).Price * entry.Value * 0.2;

                    payment += "Kedvezmény összege: " + kedvezmeny + "Ft\n";
                    sum -= kedvezmeny;
                }
            }

            payment += "Összesen: " + sum + "Ft\n";

            return payment;
        }

        public string verifyPayment(OrderController oc)
        {


            string orderList = "Hitelesítéshez a H betű után adja meg a jegy sorszámát. Például: H1\n\nHitelesítendő fizetések listája\n";
            orderList += "-----------------------------------------------\n";

            int sorszam = 1;
            foreach (Order order in oc.Orders)
            {
                foreach (Seat seat in order.Seats)
                {
                    if (seat.SeatStatus.Equals("fizetett"))
                    {
                        orderList += "Sorszám: " + sorszam + "\n";
                        orderList += "Vásárló: " + order.Buyer + "\n";
                        orderList += order.Perform + ", " + seat.RowNumber + ". sor " + seat.ColumnNumber + ". oszlop\n\n";
                        sorszam++;
                    }
                }
            }

            return orderList;
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
