using System;
using System.Collections.Generic;
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
            return "1) Előadások listázása\n" +
                "2) Foglalás\n" +
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

        public string performanceList(PerformanceStorage performs)
        {
            string tmp = "";

            for (int i = 0; i < performs.getNumOfPerformances(); i++)
            {
                Performance performance = performs.getPerformance(i);

                tmp += "Előadás: " + performance.Name + "\n";
                tmp += "Helye: " + performance.Location + "\n";
                tmp += "Férőhely: " + performance.Seats + "\n";
                tmp += "Ideje: " + performance.Date.ToString() + "\n";
                tmp += "Jegyek:\n";
                for (int j = 0; j < performance.getNumOfTickets(); j++)
                {
                    tmp += "\t" + performance.getTicket(j).Name + "\n";
                    tmp += "\tÁr: " + performance.getTicket(j).Price + " " + performance.getTicket(j).Currency + "\n";
                }
            }

            return tmp;
        }

        public string ordersList(Client client)
        {
            string tmp = "";

            foreach (OrderedItem item in client.MyOrder.OrderedItems)
            {
                tmp += item.Performance.Name + "\n" +
                    item.Performance.Location + "\n" +
                    item.Performance.Date.ToString() + "\n" +
                    "jegyek száma: " + item.Quantity + "\n";
            }

            tmp += "\nÖsszesen: " + client.MyOrder.CountPayment() + " Ft\n";
            return tmp;
        }
    }

}
