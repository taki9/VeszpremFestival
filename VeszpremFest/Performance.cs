﻿using System;

namespace server
{
    class Performance
    {
        private string _name;
        private DateTime _start;

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
        
        public Performance(string name, DateTime start)
        {
            this._name = name;
            this._start = start;
        }
    }
}
