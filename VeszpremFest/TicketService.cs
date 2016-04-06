using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class Ticketservice
    {
        private PerformanceStorage performances;

        public Ticketservice()
        {
            Performances = new PerformanceStorage();
        }

        internal PerformanceStorage Performances
        {
            get
            {
                return performances;
            }

            set
            {
                performances = value;
            }
        }

        public void dataRead()
        {
            Performances.addPerformance(new Performance("Iron Maiden koncert", "Papp László Sportaréna", 5000, new DateTime(2016, 8, 1, 20, 0, 0)));
            Performances.getPerformance(0).newTicket(new Ticket("ülőhely", 10000));
            Performances.getPerformance(0).newTicket(new Ticket("állóhely", 8000));
            

            Performances.addPerformance(new Performance("Lady Gaga koncert", "Hangvilla, Veszprém", 500, new DateTime(2016, 5, 24, 19, 0, 0)));
            Performances.getPerformance(1).newTicket(new Ticket("ülőhely", 8000));
            Performances.getPerformance(1).newTicket(new Ticket("állóhely", 7000));
        }




    }
}
