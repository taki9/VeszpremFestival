using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class Location
    {
        private int _id;
        private string _name;
        private int _price;
        private int _seatRow;
        private int _seatColumn;

        public Location(int ID, string name, int price, int seatRow, int seatColumn)
        {
            Id = ID;
            Name = name;
            Price = price;
            SeatRow = seatRow;
            SeatColumn = seatColumn;
        }

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

        public int Price
        {
            get
            {
                return _price;
            }

            set
            {
                _price = value;
            }
        }

        public int SeatColumn
        {
            get
            {
                return _seatColumn;
            }

            set
            {
                _seatColumn = value;
            }
        }

        public int SeatRow
        {
            get
            {
                return _seatRow;
            }

            set
            {
                _seatRow = value;
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
    }
}
