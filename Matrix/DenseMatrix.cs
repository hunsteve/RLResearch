using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Matrix
{
    public class DenseMatrix
    {
        public double[] elements;

        public int Rows { get; set; }
        public int Cols { get; set; }

        public DenseMatrix(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            elements = new double[rows * cols];
        }

        public DenseMatrix(DenseMatrix copy)
        {
            Rows = copy.Rows;
            Cols = copy.Cols;
            elements = new double[Rows * Cols];
            Add(copy);
        }

        public DenseMatrix(Vector copy)
        {
            Rows = copy.Elements.Length;
            Cols = 1;
            elements = new double[Rows * Cols];
            for (int r = 0; r < copy.Elements.Length; ++r)
            {
                this[r, 0] = copy.Elements[r];
            }
        }

        public static DenseMatrix CreateDiad(Vector v1, Vector v2)
        {
            DenseMatrix ret = new DenseMatrix(v1.Elements.Length, v2.Elements.Length);

            for (int r = 0; r < ret.Rows; ++r)
            {
                for (int c = 0; c < ret.Cols; ++c)
                {
                    ret[r, c] = v1.Elements[r] * v2.Elements[c];
                }
            }

            return ret;
        }

        public static DenseMatrix Diag(Vector v)
        {
            DenseMatrix ret = new DenseMatrix(v.Elements.Length, v.Elements.Length);
            for (int i = 0; i < v.Elements.Length; ++i)
            {
                ret[i, i] = v.Elements[i];
            }
            return ret;
        }

        public double this[int row, int col]
        {
            get
            {
                return elements[row * Cols + col];
            }

            set
            {
                elements[row * Cols + col] = value;
            }
        }

        public static DenseMatrix Identity(int n)
        {
            DenseMatrix ret = new DenseMatrix(n, n);
            for (int i = 0; i < n; ++i)
            {
                ret[i, i] = 1;
            }
            return ret;
        }

        public void Add(DenseMatrix a, double factor)
        {
            if (a.Rows != this.Rows || a.Cols != this.Cols)
            {
                return;
            }

            for (int e = 0; e < elements.Length; ++e)
            {
                elements[e] += a.elements[e] * factor;
            }
        }

        public void Add(DenseMatrix a)
        {
            Add(a, 1);
        }

        public void Multiply(double v)
        {

            for (int e = 0; e < elements.Length; ++e)
            {
                elements[e] *= v;
            }
        }

        public void Transpose()
        {
            double[] temp = new double[Cols * Rows];
            for (int r = 0; r < Rows; ++r)
            {
                for (int c = 0; c < Cols; ++c)
                {
                    temp[c * Rows + r] = this[r, c];
                }
            }
            int t = Cols;
            Cols = Rows;
            Rows = t;
            elements = temp;
        }

        public DenseMatrix MatrixMultiply(DenseMatrix a)
        {
            if (Cols != a.Rows)
            {
                return null;
            }

            DenseMatrix ret = new DenseMatrix(Rows, a.Cols);
            for (int r = 0; r < ret.Rows; ++r)
            {
                for (int c = 0; c < ret.Cols; ++c)
                {
                    double sum = 0;
                    for (int i = 0; i < Cols; ++i)
                    {
                        sum += this[r, i] * a[i, c];
                    }
                    ret[r, c] = sum;
                }
            }
            return ret;
        }

        public DenseMatrix MatrixMultiply(DenseMatrix a, int rowstart, int colstart, int rowcount, int colcount)
        {
            if (colcount != a.Rows)
            {
                return null;
            }

            DenseMatrix ret = new DenseMatrix(rowcount, a.Cols);
            for (int r = 0; r < ret.Rows; ++r)
            {
                for (int c = 0; c < ret.Cols; ++c)
                {
                    double sum = 0;
                    for (int i = 0; i < colcount; ++i)
                    {
                        sum += this[rowstart + r, colstart + i] * a[i, c];
                    }
                    ret[r, c] = sum;
                }
            }

            return ret;
        }

        public Vector VectorMultiplyRight(Vector v)
        {
            Vector ret = new Vector(Rows);
            for (int r = 0; r < Rows; ++r)
            {
                double sum = 0;
                for (int c = 0; c < Cols; ++c)
                {
                    sum += this[r, c] * v.Elements[c];
                }

                ret.Elements[r] = sum;
            }

            return ret;
        }

        public Vector GetRow(int row)
        {
            Vector v = new Vector(Cols);
            for (int i = 0; i < Cols; ++i)
            {
                v.Elements[i] = this[row, i];
            }
            return v;
        }


        public void WriteToFile(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            for (int r = 0; r < Rows; ++r)
            {
                for (int c = 0; c < Cols; ++c)
                {
                    sw.Write(this[r, c] + ", ");
                }
                sw.WriteLine();
            }
            sw.Close();
        }




        public DenseMatrix Part(int rowstart, int colstart, int rowcount, int colcount)
        {
            DenseMatrix ret = new DenseMatrix(rowcount, colcount);
            for (int r = rowstart; r < rowcount + rowstart; ++r)
            {
                for (int c = colstart; c < colcount + colstart; ++c)
                {
                    ret[r - rowstart, c - colstart] = this[r, c];
                }
            }
            return ret;
        }
    }
}
