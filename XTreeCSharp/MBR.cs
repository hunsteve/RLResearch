using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTreeCSharp
{
    class MBRComparer : IComparer<MBR>
    {
        int dimIndex;
        bool sortByMin;

        public MBRComparer(int dimIndex, bool sortByMin)
        {
            this.dimIndex = dimIndex;
            this.sortByMin = sortByMin;
        }

        public int Compare(MBR x, MBR y)
        {
            if (sortByMin)
            {
                if (x.Min[dimIndex] < y.Min[dimIndex])
                {
                    return -1;
                }
                else if (x.Min[dimIndex] == y.Min[dimIndex])
                {
                    return 0;
                }
                else return 1;
            }
            else
            {
                if (x.Max[dimIndex] < y.Max[dimIndex])
                {
                    return -1;
                }
                else if (x.Max[dimIndex] == y.Max[dimIndex])
                {
                    return 0;
                }
                else return 1;
            }
        }
    }



    public class MBR
    {
        XTVector min;
        XTVector max;

        public XTVector Min { get { return min; } }
        public XTVector Max { get { return max; } }

        public MBR(XTVector aMin, XTVector aMax)
        {
            this.min = aMin;
            this.max = aMax;
        }

        public MBR(MBR copy)
        {
            this.min = new XTVector(copy.min);
            this.max = new XTVector(copy.max);
        }

        public static MBR EmptyMBR(int dimension)
        {
            return new MBR(XTVector.MaxVector(dimension), XTVector.MinVector(dimension));
        }

        public int Dimension
        {
            get
            {
                return min.Length;
            }
        }

        public bool Inside(XTVector p)
        {
            bool isInside = true;
            for (int i = 0; i < p.Length; ++i)
            {
                isInside = isInside && (min[i] < p[i]) && (max[i] > p[i]);
                if (!isInside) return false;
            }

            return true;
        }
        
        public bool Overlaps(MBR mbr)
        {
            bool overlaps = true;
            for (int i = 0; i < min.Length; ++i)
            {
                overlaps = overlaps && (min[i] < mbr.max[i]) && (max[i] > mbr.min[i]);
                if (!overlaps) return false;
            }

            return true;
        }

        public double Area()
        {
            double area = 1;
            for (int i = 0; i < Dimension; ++i)
            {
                area *= (max[i] - min[i]);
            }
            return area;
        }

        public static MBR Enlarge(MBR mbr1, MBR mbr2)
        {
            XTVector min = new XTVector(mbr1.Dimension);
            XTVector max = new XTVector(mbr1.Dimension);
            for (int i = 0; i < mbr1.Dimension; ++i)
            {
                min[i] = Math.Min(mbr1.min[i], mbr2.min[i]);
                max[i] = Math.Max(mbr1.max[i], mbr2.max[i]);
            }
            return new MBR(min, max);
        }

        public void Enlarge(MBR mbr)
        {
            for (int i = 0; i < Dimension; ++i)
            {
                min[i] = Math.Min(min[i], mbr.min[i]);
                max[i] = Math.Max(max[i], mbr.max[i]);
            }
        }

        public bool Inside(MBR mbr)
        {
            bool isMinInside = Inside(mbr.min);
            if (!isMinInside) return false;
            bool isMaxInside = Inside(mbr.max);
            return isMaxInside;           
        }

        public double Overlap(MBR mbr)
        {
            double overlap = 1;
            for (int i = 0; i < Dimension; ++i)
            {
                overlap *= (Math.Min(max[i], mbr.max[i]) - Math.Max(min[i], mbr.min[i]));
                if (overlap < 0) return 0;                
            }
            return overlap;
        }

        public double Margin()
        {
            double margin = 0;
            for (int i = 0; i < Dimension; ++i)
            {
                margin += (max[i] - min[i]);
            }
            return margin;
        }

        public bool EqualsByValue(MBR mbr)
        {
            return (min.EqualsByValue(mbr.min) && max.EqualsByValue(mbr.max));
        }
    }
}
