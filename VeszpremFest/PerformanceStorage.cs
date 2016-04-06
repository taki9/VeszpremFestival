using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class PerformanceStorage
    {
        private List<Performance> performs;

        public PerformanceStorage()
        {
            performs = new List<Performance>();
        }

        public void addPerformance(Performance perform)
        {
            performs.Add(perform);
        }

        public void deletePerformance(Performance perform)
        {
            performs.Remove(perform);
        }

        public Performance getPerformance(int index)
        {
            return performs[index];
        }

        public int getNumOfPerformances()
        {
            return performs.Count;
        }

        public Performance searchPerformanceByName(string name)
        {
            int i = 0;

            while (i < performs.Count)
            {
                if (performs[i].Name.Contains(name))
                {
                    return performs[i];
                }

                i++;
            }

            return null;
        }

        internal List<Performance> Performs
        {
            get
            {
                return performs;
            }
        }
    }
}
