using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class LocationController
    {
        List<Location> locations;

        internal List<Location> Locations
        {
            get
            {
                return locations;
            }

            set
            {
                locations = value;
            }
        }

        public LocationController()
        {
            locations = new List<Location>();
        }

        public void readLocationsFromDb(DbController dbc)
        {
            DataTable locTable = dbc.readLocations();

            foreach (DataRow row in locTable.Rows)
            {
                locations.Add(new Location(Convert.ToInt32(row.Field<Int64>("LocationID")), row.Field<string>("LocationName"), Convert.ToInt32(row.Field<Int64>("Price")), Convert.ToInt32(row.Field<Int64>("seatRow")), Convert.ToInt32(row.Field<Int64>("seatColumn"))));
            }
        }


        public void addLocation(Location loc)
        {
            locations.Add(loc);
            // DB update
        }

        public string removeLocation(int index)
        {
            int i = 0;
            while (i < locations.Count && locations[i].Id != index)
            {
                i++;
            }

            if (i < locations.Count)
            {
                locations.Remove(locations[i]);


                // remove from DB

                return "A helyszín sikeresen törölve.";
            }

            return "Nincs ilyen azonosítójú helyszín!";
        }

        public Location getLocationByIndex(int index)
        {
            return Locations[index];
        }

        public Location getLocationByID(int id)
        {
            int i = 0;

            while(i < locations.Count && locations[i].Id != id)
            {
                i++;
            }

            if (i < locations.Count)
            {
                return locations[i];
            }

            return null;
            }

        public int numOfLocations()
        {
            return locations.Count;
        }

        public Location findLocationByName(string name)
        {

            foreach(Location loc in locations)
            {
                if (loc.Name == name)
                {
                    return loc;
                }
            }

            return null;
        }
    }
}
