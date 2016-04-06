using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class Performance
    {
        private string _location;
        private string _name;
        private DateTime _date;
        private int _seats;
        private List<Ticket> tickets;

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public DateTime Date
        {
            get
            {
                return _date;
            }

            set
            {
                _date = value;
            }
        }

        public string Location
        {
            get
            {
                return _location;
            }

            set
            {
                _location = value;
            }
        }

        public int Seats
        {
            get
            {
                return _seats;
            }

            set
            {
                _seats = value;
            }
        }

        public Performance(string name, string location, int seats, DateTime date)
        {
            this._name = name;
            this._location = location;
            this._seats = seats;
            this._date = date;
            tickets = new List<Ticket>();
        }

        public Ticket getTicket(int index)
        {
            return tickets[index];
        }

        public int getNumOfTickets()
        {
            return tickets.Count;
        }

        public Ticket searchTicketByName(string name)
        {
            int i = 0;

            while (i < tickets.Count)
            {
                if (tickets[i].Name.Contains(name))
                {
                    return tickets[i];
                }

                i++;
            }

            return null;
        }

        public void newTicket(Ticket ticket)
        {
            tickets.Add(ticket);
        }
    }
}
