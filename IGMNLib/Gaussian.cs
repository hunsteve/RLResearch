using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Matrix;

namespace IGMNLib
{
    public class Gaussian
    {
        private Vector mean;
        public Vector Mean
        {
            get 
            {
                return mean;
            }
        }

        private CovarianceMatrix covariance;
        public CovarianceMatrix Covariance
        {
            get
            {
                return covariance;
            }

        }

        public Gaussian(int dimension)
        {
            mean = new Vector(dimension);
            covariance = new CovarianceMatrix(dimension);
        }

        public Gaussian(Vector mean)
        {
            this.mean = mean;
            covariance = new CovarianceMatrix(mean.Elements.Length);
        }

        public Gaussian(Vector mean, Vector diagCovariance)
        {
            this.mean = mean;
            covariance = new CovarianceMatrix(diagCovariance);
        }

        public Gaussian(Vector mean, CovarianceMatrix matrix)
        {
            this.mean = mean;
            this.covariance = matrix;
        }


        public double Likelihood(Vector x)
        {
            double e;
            return Likelihood(x, out e);
        }

        public double Likelihood(Vector x, out double e)
        {
            Vector vv = x - Mean;
          
            int dim = vv.Elements.Length;

            double c = 1 / Math.Pow(2 * Math.PI, dim / 2.0);
            double d = 1 / Math.Sqrt(Covariance.Determinant);
            e = Math.Exp(-0.5 * Covariance.Distance(vv));
            
            return c * d * e;
        }        
    }
}
