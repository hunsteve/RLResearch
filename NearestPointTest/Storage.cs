using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NearestPointTest
{
   

    class StoragePoint
    {
        public double[] coord;

        public StoragePoint(double[] coord)
        {
            this.coord = coord;        
        }

        public double DistanceSquared(StoragePoint p)
        {
            double sum = 0;
            int min = Math.Min(coord.Length, p.coord.Length);
            for (int i = 0; i < min; ++i)
            {
                sum += (this.coord[i] - p.coord[i]) * (this.coord[i] - p.coord[i]);
            }

            return sum;
        }




        public double value = 0;
        public int count = 0;
        public void Refine(double d)
        {
            //value = value * count / (count + 1) + d / (count + 1);
            value = 0.75 * value + 0.25 * d;
            count++;
        }
    }

    class StorageLevel
    {
        int dimension;
        int maxdimension;
        double[] min;
        double[] max;
        double limit;

        StorageLevel smaller;
        StorageLevel greater;

        StoragePoint point;

        public StorageLevel(int dimension, int maxdimension, double[] min, double[] max)
        {
            this.dimension = dimension;
            this.maxdimension = maxdimension;
            this.min = min;
            this.max = max;
            limit = (max[dimension] + min[dimension]) / 2;
        }

        public void AddPoint(StoragePoint point)
        {
            if (this.point == null && this.smaller == null && this.greater == null)
            {
                this.point = point;
            }
            else
            {
                if (this.point != null)
                {
                    SortPoint(this.point);
                    this.point = null;
                }

                SortPoint(point);
            }
        }

        public List<StoragePoint> PointsNear(StoragePoint p, double distance)
        {
            if (AreaContains(p, distance) && !(this.point == null && this.smaller == null && this.greater == null))
            {
                List<StoragePoint> list = new List<StoragePoint>();
                if (this.point != null)
                {
                    if (point.DistanceSquared(p) < distance*distance) list.Add(point);
                    return list;
                }

                if (this.smaller != null)
                {
                    list.AddRange(this.smaller.PointsNear(p, distance));
                }

                if (this.greater != null)
                {
                    list.AddRange(this.greater.PointsNear(p, distance));
                }

                return list;               
            }
            else 
            {
                 return new List<StoragePoint>();
            }
        }

        private bool AreaContains(StoragePoint p, double distance)
        {
            bool b = true;
            for (int i = 0; i < maxdimension; ++i)
            {
                b = b && (p.coord[i] + distance > min[i]) && (p.coord[i] - distance < max[i]);
                if (!b) return false;
            }
            return true;
        }

        private void SortPoint(StoragePoint p)
        {
            if (p.coord[dimension] < limit)
            {
                if (smaller == null) {
                    double[] amin = (double[])min.Clone();
                    double[] amax = (double[])max.Clone();
                    amax[dimension] = limit;
                    smaller = new StorageLevel((dimension + 1) % maxdimension, maxdimension, amin, amax);
                }
                smaller.AddPoint(p);
            }
            else
            {
                if (greater == null)
                {
                    double[] amin = (double[])min.Clone();
                    double[] amax = (double[])max.Clone();
                    amin[dimension] = limit;
                    greater = new StorageLevel((dimension + 1) % maxdimension, maxdimension, amin, amax);
                }
                greater.AddPoint(p);
            }
        }
    }    

    class Storage
    {

        StorageLevel root;

        public Storage(int maxdimension) {
            double[] min = new double[maxdimension];
            double[] max = new double[maxdimension];

            for (int i = 0; i < maxdimension; ++i)
            {
                min[i] = 0;
                max[i] = 1;
            }

            root = new StorageLevel(0,maxdimension, min, max);
        }

        public void AddPoint(StoragePoint point)
        {
            root.AddPoint(point);
        }

        public List<StoragePoint> PointsNear(StoragePoint point, double distance)
        {
            return root.PointsNear(point, distance);
        }
    }
}
