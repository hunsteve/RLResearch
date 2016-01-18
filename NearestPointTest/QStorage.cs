using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NearestPointTest
{
    class QStorage
    {
        Storage s;
        double distance;

        public QStorage(int maxdimension, double distance) 
        {
            s = new Storage(maxdimension);
            this.distance = distance;
        }

        public double InferUtility(StoragePoint sp)
        {
            double sum = 0;
            double sum2 = 0;
            List<StoragePoint> list = s.PointsNear(sp, distance * 2);
            foreach (StoragePoint spp in list)
            {
                double q = spp.value;
                double sigma = 3 * distance;
                double relevance = 1;
                //double relevance = 1 / Math.Pow(2*Math.PI*sigma, spp.coord.Length/2) * Math.Exp(-0.5 * spp.DistanceSquared(sp) / sigma); //egyseg szorasu gauss
                sum += q * relevance;
                sum2 += relevance;
            }

            return (sum2 == 0) ? 0 : sum / sum2;
        }
    }
}
