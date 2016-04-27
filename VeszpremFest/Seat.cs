using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class Seat
    {
        private int _rowNumber;
        private int _columnNumber;
        private string _seatStatus;

        public Seat(int row, int column)
        {
            RowNumber = row;
            ColumnNumber = column;

            // If you pay, set it to paid
            SeatStatus = "foglalt";
        }

        public string SeatStatus
        {
            get
            {
                return _seatStatus;
            }

            set
            {
                _seatStatus = value;
            }
        }

        public int ColumnNumber
        {
            get
            {
                return _columnNumber;
            }

            set
            {
                _columnNumber = value;
            }
        }

        public int RowNumber
        {
            get
            {
                return _rowNumber;
            }

            set
            {
                _rowNumber = value;
            }
        }
    }
}
