using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NearestPointTest
{
    class HashmapStorage
    {
        double distance;

        Dictionary<long, List<StoragePoint> > hashmap;

        const int coordHashCount = 2;
        
        public HashmapStorage(int maxdimension, double distance)
        {
            this.distance = distance;
            hashmap = new Dictionary<long, List<StoragePoint> >();           
        }

        public void AddPoint(StoragePoint point)
        {
            long hash = 0;
            int cc = (int)Math.Ceiling(1 / distance);
            for (int i = 0; i < coordHashCount; ++i)
            {
                int c = (int)Math.Floor(point.coord[i] / distance);
                hash += c;
                hash *= cc;
            }

            List<StoragePoint> list = hashmap[hash];
            if (list == null)
            {
                list = new List<StoragePoint>();
                hashmap[hash] = list;
            }

            list.Add(point);
        }

        //public List<StoragePoint> PointsNear(StoragePoint point, double distance)
        //{

        //}



    }
}
