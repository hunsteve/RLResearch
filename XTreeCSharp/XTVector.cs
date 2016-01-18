using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTreeCSharp
{
    public class XTVector
    {
        double[] vector;        
        public XTVector(int p)
        {
            vector = new double[p];            
        }

        public XTVector(double[] v)
        {
            vector = v;
        }

        public XTVector(XTVector copy)
        {
            vector = new double[copy.vector.Length];
            for (int i = 0; i < vector.Length; ++i)
            {
                vector[i] = copy.vector[i];
            }
        }

        public double this[int index]
        {
            get
            {
                return vector[index];
            }
            set
            {
                vector[index] = value;
            }
        }

        public static XTVector MaxVector(int dimension)
        {
            XTVector v = new XTVector(dimension);
            for (int i = 0; i < dimension; ++i)
            {
                v[i] = double.MaxValue;
            }
            return v;
        }

        public static XTVector MinVector(int dimension)
        {
            XTVector v = new XTVector(dimension);
            for (int i = 0; i < dimension; ++i)
            {
                v[i] = double.MinValue;
            }
            return v;
        }

        public int Length {
            get { return vector.Length; }
        }

        internal bool EqualsByValue(XTVector vect)
        {
            bool ret = true;
            for (int i = 0; i < vector.Length; ++i)
            {
                ret = ret && (this[i] == vect[i]);
                if (!ret) return false;
            }
            return ret;
        }
    }
}
