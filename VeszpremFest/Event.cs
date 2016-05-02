using System;

namespace server
{
    class Event
    {
        private int _id;
        private string _performName;
        Location location;
        private DateTime _start;
        private int _availSeats;

        public Event(int id, string performName, Location location, DateTime start, int availSeats)
        {
            Id = id;
            PerformName = performName;
            Location = location;
            Start = start;
            AvailSeats = availSeats;
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

        public int AvailSeats
        {
            get
            {
                return _availSeats;
            }

            set
            {
                _availSeats = value;
            }
        }

        public string PerformName
        {
            get
            {
                return _performName;
            }

            set
            {
                _performName = value;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        internal Location Location
        {
            get
            {
                return location;
            }

            set
            {
                location = value;
            }
        }


    }
}
