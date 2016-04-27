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

        private List<Seat> _seats;

        public Order(string perform, string loc, DateTime start)
        {
            Perform = perform;
            Loc = loc;
            Start = start;
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
