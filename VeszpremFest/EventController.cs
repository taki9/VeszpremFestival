using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class EventController
    {
        List<Event> events;

        public EventController()
        {
            events = new List<Event>();
        }

        public void readEventsFromDb(DbController dbc)
        {
            DataTable eventTable = dbc.readEvents();

            foreach (DataRow row in eventTable.Rows)
            {
                DateTime start = DateTime.ParseExact(row.Field<string>("Start"), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

                events.Add(new Event(Convert.ToInt32(row.Field<Int64>("EventID")), row.Field<string>("PerformName"), new Location(Convert.ToInt32(row.Field<Int64>("LocationID")), row.Field<string>("LocationName"), Convert.ToInt32(row.Field<Int64>("Price")), Convert.ToInt32(row.Field<Int64>("seatRow")), Convert.ToInt32(row.Field<Int64>("seatColumn"))), start, Convert.ToInt32(row.Field<Int64>("AvailSeats"))));
            }
        }

        public void addEvent(Event ev)
        {
            events.Add(ev);
        }

        public string removeEvent(int index)
        {
            int i = 0;
            while (i < events.Count && events[i].Id != index)
            {
                i++;
            }

            if (i < events.Count)
            {
                events.Remove(events[i]);

                // remove from DB

                return "Esemény sikeresen törölve.";
            }

            return "Nincs ilyen azonosítójú esemény!";
        }

        public Event getEventByIndex(int index)
        {
            return events[index];
        }

        public Event getEventByID(int id)
        {
            int i = 0;

            while (i < events.Count && events[i].Id != id)
            {
                i++;
            }

            if (i < events.Count)
            {
                return events[i];
            }

            return null;
        }

        public int numOfEvents()
        {
            return events.Count;
        }

        public Event findEvent(string perform, string location, DateTime start)
        {
            foreach (Event ev in events)
            {
                if (ev.PerformName == perform && ev.Location.Name == location && ev.Start.Equals(start))
                {
                    return ev;
                }
            }

            return null;
        }
    }
}