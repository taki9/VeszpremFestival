using System;
using System.Collections.Generic;
using System.Data;

namespace server
{
    class Order
    {
        private string _perform;
        private string _loc;
        private DateTime _start;
        private string _buyer;

        private List<Seat> _seats;

        public Order(string perform, string loc, DateTime start, string buyer)
        {
            Perform = perform;
            Loc = loc;
            Start = start;
            Buyer = buyer;
            Seats = new List<Seat>();

        }
        
        public string Perform
        {
            get
            {
                return _perform;
            }

            set
            {
                _perform = value;
            }
        }

        public string Loc
        {
            get
            {
                return _loc;
            }

            set
            {
                _loc = value;
            }
        }

        public DateTime Start
        {
            get
            {
                return _start;
            }

            set
            {
                _start = value;
            }
        }

        internal List<Seat> Seats
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

        public string Buyer
        {
            get
            {
                return _buyer;
            }

            set
            {
                _buyer = value;
            }
        }

        public void addSeat(Seat seat)
        {
            Seats.Add(seat);
        }

        public int numOfSeats()
        {
            return Seats.Count;
        }

        public Seat getSeat(int index)
        {
            return Seats[index];
        }

        public Seat getSeat(int row, int column)
        {
            int i = 0;

            while (i < Seats.Count && !(Seats[i].RowNumber == row && Seats[i].ColumnNumber == column))
            {
                i++;
            }

            if (i < Seats.Count)
            {
                return Seats[i];
            }

            return null;
        }
    }

}
