using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;
using solve;

namespace Matrix
{    
    public class SparseMatrix
    {
        public Dictionary<long, double> values;

        HashSet<int>[] rowElements;
        HashSet<int>[] columnElements;

        int size;

        public SparseMatrix(int n)
        {
            values = new Dictionary<long, double>();
            rowElements = new HashSet<int>[n];
            columnElements = new HashSet<int>[n];

            size = n;

            for (int i = 0; i < n; ++i)
            {
                rowElements[i] = new HashSet<int>();
                columnElements[i] = new HashSet<int>();
            }
        }

        public SparseMatrix(SparseMatrix copy)
        {
            size = copy.size;
            values = new Dictionary<long, double>();
            foreach (long i in copy.values.Keys)
            {
                values.Add(i, copy.values[i]);
            }

            rowElements = new HashSet<int>[size];
            columnElements = new HashSet<int>[size];

            for (int i = 0; i < size; ++i)
            {
                rowElements[i] = new HashSet<int>();
                rowElements[i].UnionWith(copy.rowElements[i]);

                columnElements[i] = new HashSet<int>();
                columnElements[i].UnionWith(copy.columnElements[i]);
            }
        }

        public SparseMatrix(MWNumericArray matlabArray)
        {
            double[,] data = (double[,])matlabArray.ToArray(MWArrayComponent.Real);
            int n = data.GetLength(0);

            values = new Dictionary<long, double>();
            rowElements = new HashSet<int>[n];
            columnElements = new HashSet<int>[n];

            size = n;

            for (int i = 0; i < n; ++i)
            {
                rowElements[i] = new HashSet<int>();
                columnElements[i] = new HashSet<int>();
            }

            for (int row = 0; row < n; ++row)
            {
                for (int col = 0; col < n; ++col)
                {
                    if (data[row, col] != 0) this[row, col] = (double)data[row, col];
                }
            }
        }

        public static SparseMatrix Identity(int n)
        {
            SparseMatrix ret = new SparseMatrix(n);
            for (int i = 0; i < n; ++i)
            {
                ret[i, i] = 1;
            }
            return ret;
        }


        private long Index(int row, int col)
        {
            return (long)row * size + col;
        }

        private int Row(long index)
        {
            return (int)(index / size);
        }

        private int Column(long index)
        {
            return (int)(index % size);
        }

        public double this[int row, int col]
        {
            get
            {
                long index = Index(row, col);
                if (values.ContainsKey(index))
                {
                    return values[index];
                }
                else return 0;
            }

            set
            {
                ChangeValue(row, col, value);
            }
        }

        public double this[long index] 
        {
            get
            {
                if (values.ContainsKey(index))
                {
                    return values[index];
                }
                else return 0;
            }

            set
            {
                ChangeValue(index, value);
            }
        }

        private void ChangeValue(long index, double f)
        {
            int row = Row(index);
            int col = Column(index);
            ChangeValue(index, row, col, f);
        }

        private void ChangeValue(int row, int col, double f)
        {
            long index = Index(row, col);
            ChangeValue(index, row, col, f);
        }

        private void ChangeValue(long index, int row, int col, double f)
        {
            if (f != 0)
            {
                if (!values.ContainsKey(index))
                {
                    rowElements[row].Add(col);
                    columnElements[col].Add(row);
                }

                values[index] = f;                                   
            }
            else
            {
                if (values.ContainsKey(index))
                {
                    values.Remove(index);
                    rowElements[row].Remove(col);
                    columnElements[col].Remove(row);
                }
            }
        }


        public void Add(SparseMatrix a, double factor) {
            if (a.size != size)
            {
                return;
            }

            foreach (long i in a.values.Keys)
            {                
                ChangeValue(i, this[i] + factor * a[i]);
            }
        }

        public void Add(SparseMatrix a)
        {
            Add(a, 1);
        }

        public void Multiply(double v)
        {
            if (v != 0)
            {
                foreach (long i in values.Keys.ToArray())
                {
                    ChangeValue(i, this[i] * v);
                }
            }            
        }

        public void Multiply(Vector v)
        {
            if (v.Elements.Length != size)
            {
                return;
            }

            foreach (long i in values.Keys.ToArray())
            {
                ChangeValue(i, this[i] * v.Elements[Row(i)]);
            }

        }

        public Vector MatrixMultiplyRight(Vector v) // return M * v
        {
            if (v.Elements.Length != size)
            {
                return null;
            }

            Vector ret = new Vector(size);

            for (int row = 0; row < size; ++row)
            {
                double sum = 0;
                foreach (int col in rowElements[row])
                {
                    sum += this[row, col] * v.Elements[col];
                }
                ret.Elements[row] = sum;
            }

            return ret;
        }

        public static SparseMatrix operator +(SparseMatrix a, SparseMatrix b)
        {
            if (a.size != b.size)
            {
                return null;
            }
            SparseMatrix ret = new SparseMatrix(a);
            ret.Add(b,1);
            return ret;
        }

        public static SparseMatrix operator -(SparseMatrix a, SparseMatrix b)
        {
            if (a.size != b.size)
            {
                return null;
            }
            SparseMatrix ret = new SparseMatrix(a);
            ret.Add(b, -1);
            return ret;
        }





        private void SubtractRowFromRow(int row1, int row2, double factor) //row1 = row1 - factor * row2
        {
            foreach (int col in rowElements[row2])
            {
                this[row1, col] -= this[row2, col] * factor;
            }
        }

        private void MultiplyRow(int row, double factor) //row1 = row1 * factor
        {
            foreach (int col in rowElements[row])
            {
                this[row, col] *= factor;
            }
        }


        public Vector SolveLinearEquation(Vector y) // this * result = y
        {
            Vector ret = new Vector(y.Elements);
            for (int row = 0; row < size; ++row)
            {
                double z = 1 / this[row, row];
                MultiplyRow(row, z);
                ret.Elements[row] *= z;

                List<int> temp = new List<int>(columnElements[row]);                             
                foreach (int row2 in temp)
                {
                    if (row2 > row)
                    {
                        double f = this[row2, row];
                        SubtractRowFromRow(row2, row, f);
                        ret.Elements[row2] -= ret.Elements[row] * f;
                    }
                }
                Console.Out.WriteLine(row);
            }

            for (int row = size-1; row >= 0; --row)
            {
                List<int> temp = new List<int>(columnElements[row]);
                foreach (int row2 in temp)
                {
                    if (row2 < row)
                    {
                        double f = this[row2, row];
                        SubtractRowFromRow(row2, row, f);
                        ret.Elements[row2] -= ret.Elements[row] * f;
                    }
                }
                Console.Out.WriteLine(row);
            }

            return ret;
        }

        public Vector SolveLinearEquation2(Vector y) // this * result = y
        {
            List<int> rows = new List<int>();
            List<int> cols = new List<int>();
            List<double> fvalues = new List<double>();

            foreach (long i in values.Keys)
            {
                rows.Add(Row(i)+1);
                cols.Add(Column(i)+1);
                fvalues.Add(this[i]);
            }

            MWNumericArray _rows = new MWNumericArray(rows.Count, 1, rows.ToArray());
            MWNumericArray _cols = new MWNumericArray(cols.Count, 1, cols.ToArray());
            MWNumericArray _values = new MWNumericArray(fvalues.Count, 1, fvalues.ToArray());
            MWNumericArray _y = new MWNumericArray(y.Elements.Length, 1, y.Elements);            

            solveclass solve = new solveclass();
            MWNumericArray ret = (MWNumericArray)solve.solve(_rows, _cols, _values, _y);
            double[,] fret = (double[,])ret.ToArray(MWArrayComponent.Real);
            double[] fret2 = new double[fret.GetLength(0)];
            for (int i = 0; i < fret2.Length; ++i)
            {
                fret2[i] = (double)fret[i, 0];
            }
            return new Vector(fret2);            
        }

        public void WriteToFile(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            foreach (long i in values.Keys)
            {
                sw.WriteLine(i % size + ", " + i / size + ", " + this[i]);
            }
            sw.Close();
        }


        public void Save(BinaryWriter bw)
        {                     
            bw.Write(size);
            bw.Write(values.Count);
            foreach (long i in values.Keys)
            {
                bw.Write(i);
                bw.Write(this[i]);
            }           
        }

        public static SparseMatrix Load(BinaryReader br)
        {
            SparseMatrix ret = null;
            
            int size = br.ReadInt32();
            ret = new SparseMatrix(size);

            int count = br.ReadInt32();
            for(int i=0; i<count; ++i)
            {
                long index = br.ReadInt64();
                double value = br.ReadSingle();
                if (index < size * size && index >= 0)
                {                                     
                    ret[index] = value;
                }
            }
            
            return ret;
        }
    }
}
