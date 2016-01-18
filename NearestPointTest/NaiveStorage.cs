using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NearestPointTest
{
    class NaiveStorage
    {
        List<StoragePoint> list = new List<StoragePoint>();

        public List<StoragePoint> PointsNear(StoragePoint point, double distance)
        {
            List<StoragePoint> list2 = new List<StoragePoint>();
            foreach (StoragePoint sp in list)
            {
                if (sp.DistanceSquared(point) < distance * distance)
                {
                    list2.Add(sp);
                }
            }

            return list2;
        }

        public void AddPoint(StoragePoint point)
        {
            list.Add(point);
        }

    }
}
