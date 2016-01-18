using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Matrix
{
    public class Vector
    {
        public double[] Elements
        {
            set;
            get;
        }

        public Vector(double[] elements)
        {
            Elements = (double[])elements.Clone();

        }

        public Vector(int elementnum)
        {
            Elements = new double[elementnum];
        }

        public Vector(Vector copy)
        {
            Elements = (double[])copy.Elements.Clone();
        }

        public void WriteToFile(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            for (int i = 0; i < Elements.Length; ++i)
            {
                sw.WriteLine(Elements[i]);
            }
            sw.Close();
        }

        public void Exp(double offs)
        {
            for (int i = 0; i < Elements.Length; ++i)
            {
                Elements[i] = (double)Math.Pow(2, Elements[i]);
            }
        }

        public void Div(Vector v)
        {
            if (Elements.Length != v.Elements.Length)
            {
                return;
            }

            for (int i = 0; i < Elements.Length; ++i)
            {
                Elements[i] /= v.Elements[i];
            }
        }

        public void Add(Vector v, double factor)
        {
            if (Elements.Length != v.Elements.Length)
            {
                return;
            }

            for (int i = 0; i < Elements.Length; ++i)
            {
                Elements[i] += factor * v.Elements[i];
            }
        }

        public void Add(Vector v)
        {
            Add(v, 1);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            if (a.Elements.Length != b.Elements.Length)
            {
                return null;
            }
            Vector ret = new Vector(a);
            ret.Add(b, 1);
            return ret;
        }

        public static Vector operator -(Vector a, Vector b)
        {
            if (a.Elements.Length != b.Elements.Length)
            {
                return null;
            }
            Vector ret = new Vector(a);
            ret.Add(b, -1);
            return ret;
        }


        public void Multiply(double scale)
        {
            for (int i = 0; i < Elements.Length; ++i)
            {
                Elements[i] *= scale;
            }
        }


        public void Multiply(Vector v)
        {
            if (Elements.Length != v.Elements.Length)
            {
                return;
            }

            for (int i = 0; i < Elements.Length; ++i)
            {
                Elements[i] *= v.Elements[i];
            }
        }

        public Vector Part(int from, int length)
        {
            Vector v = new Vector(length);
            for (int i = 0; i < length; ++i)
            {
                v.Elements[i] = Elements[i + from];
            }
            return v;
        }

        public double GetLengthSquared()
        {
            double sum = 0;
            for (int i = 0; i < Elements.Length; ++i)
            {
                sum += Elements[i] * Elements[i];
            }
            return sum;
        }
    }
}
