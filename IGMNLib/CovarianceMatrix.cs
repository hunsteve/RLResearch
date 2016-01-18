using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Matrix;

namespace IGMNLib
{
    public class CovarianceMatrix
    {
        private DenseMatrix covariance;
        public DenseMatrix Covariance
        {
            get
            {
                return covariance;
            }
        }

        private DenseMatrix inverseCovariance;
        public DenseMatrix InverseCovariance
        {
            get
            {
                return inverseCovariance;
            }
        }

        private double determinant;
        public double Determinant
        {
            get
            {
                return determinant;
            }
        }

        public CovarianceMatrix(int dimension)
        {
            determinant = 1;
            covariance = DenseMatrix.Identity(dimension);
            inverseCovariance = DenseMatrix.Identity(dimension);
        }

        public CovarianceMatrix(Vector diagonal)
        {
            determinant = 1;
            covariance = DenseMatrix.Diag(diagonal);
            inverseCovariance = DenseMatrix.Diag(diagonal);
            for (int i = 0; i < diagonal.Elements.Length; ++i)
            {
                double temp = covariance[i, i];
                inverseCovariance[i, i] = 1 / temp;
                determinant *= temp;
            }                        
        }

        public void AddDiad(Vector v, double factor)
        {                    
            //hozzaad (v) 
            //det(X + cr) = det(X)(1 + rX^(−1)c)
            //(A+cr)^(-1)=A^(-1) - (A^(-1)*c*r*A^(-1))/(1 + rX^(−1)c)            
            double sci = Distance(v);            
            covariance.Add(DenseMatrix.CreateDiad(v, v), factor);
            Vector inv = inverseCovariance.VectorMultiplyRight(v);
            inverseCovariance.Add(DenseMatrix.CreateDiad(inv, inv), -factor / (1 + factor * sci));
            determinant *= (1 + factor * sci);            
        }


        public void MultiplyScalar(double w)
        {
            //szoroz (w)
            //det(s*X) = s^rank(X)*det(X)
            //(s*X)^(-1) = 1/s*X^(-1)
            covariance.Multiply(w);
            inverseCovariance.Multiply(1 / w);
            determinant = (double)(determinant * Math.Pow(w, covariance.Rows));                      
        }


        public double Distance(Vector v)
        {
            //eps^T * inv(C) * eps
            double sum = 0;

            for (int c = 0; c < inverseCovariance.Cols; ++c)
            {
                double sum2 = 0;
                for (int r = 0; r < inverseCovariance.Rows; ++r)
                {
                    sum2 += inverseCovariance[r, c] * v.Elements[r];
                }
                sum += sum2 * v.Elements[c];
            }
            return sum;
        }

    }
}
