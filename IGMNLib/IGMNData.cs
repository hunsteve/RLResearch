using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTreeCSharp;
using Matrix;
using System.Drawing;

namespace IGMNLib
{
    public class IGMNData : XTData
    {
        Gaussian gauss;
        Gaussian inputGauss;

        IGMN owner;

        public int Age
        {
            get;
            set;
        }
        
        public double Accumlator
        {
            get;
            set;            
        }

        public Gaussian Gaussian
        {
            get
            {
                return gauss;
            }
        }

        public Gaussian InputGaussian
        {
            get
            {
                return inputGauss;
            }
        }

        private CovarianceMatrix getStarterCovariance()
        {

            return new CovarianceMatrix(owner.InitialGauss);
        }

        private CovarianceMatrix getInputStarterCovariance()
        {
            return new CovarianceMatrix(owner.InitialGauss.Part(0, owner.InitialGauss.Elements.Length - 1));
        }

        public IGMNData(IGMN owner, Vector mean)
        {
            this.owner = owner;
            this.gauss = new Gaussian(mean, getStarterCovariance());
            this.inputGauss = new Gaussian(mean.Part(0, mean.Elements.Length - 1), getInputStarterCovariance());
            Age = 1;
            Accumlator = 1;
        }

        public void RefineWithData(Vector x, double w)
        {
            Vector eps = x - gauss.Mean;
            Vector eps2 = x.Part(0, x.Elements.Length - 1) - inputGauss.Mean;

            /**/double oldQvalue = gauss.Mean.Elements[gauss.Mean.Elements.Length - 1];            
            gauss.Mean.Add(eps, w);

            /**/gauss.Mean.Elements[gauss.Mean.Elements.Length - 1] = Math.Max(oldQvalue, x.Elements[x.Elements.Length - 1]);//max Q value
            
            inputGauss.Mean.Add(eps2, w);

            gauss.Covariance.MultiplyScalar(1 - w);
            inputGauss.Covariance.MultiplyScalar(1 - w);

            gauss.Covariance.AddDiad(eps, -w * w + w);//-w * w + w
            inputGauss.Covariance.AddDiad(eps2, -w * w + w);
        }



        public void Draw(Graphics g, int dim1, int dim2, double relevance, double zoom)
        {
            int num = 30;
            double c11 = gauss.Covariance.Covariance[dim1,dim1];
            double c12 = gauss.Covariance.Covariance[dim1,dim2];
            double c21 = gauss.Covariance.Covariance[dim2,dim1];
            double c22 = gauss.Covariance.Covariance[dim2,dim2];

            double mux = gauss.Mean.Elements[dim1];
            double muy = gauss.Mean.Elements[dim2];

            double det = c11 * c22 - c21 * c12;
            double i11 = c22 / det;
            double i12 = -c12 / det;
            double i21 = -c21 / det;
            double i22 = c11 / det;

            double A = i11;
            double B = (i12 + i21);
            double C = i22;
            double D = -2*relevance;

            double xend = Math.Sqrt(-4 * C * D / (B * B - 4 * C * A))*0.999999999;
            double xstart = -xend;
            double step = (xend - xstart) / num;

            List<PointF> pointList = new List<PointF>();
            for (double x = xstart; x < xend; x += step)
            {
                double y = (-x * B + Math.Sqrt(x * x * B * B - 4 * C * (x * x * A - D))) / (2 * C);
                pointList.Add(new PointF((float)((x + mux) * zoom), (float)((y + muy) * zoom)));                
            }
            for (double x = xend; x > xstart; x -= step)
            {
                double y = (-x * B - Math.Sqrt(x * x * B * B - 4 * C * (x * x * A - D))) / (2 * C);
                pointList.Add(new PointF((float)((x + mux) * zoom), (float)((y + muy) * zoom)));                
            }

            
            g.DrawPolygon(new Pen(Color.Black), pointList.ToArray());

        }



        #region XTData Members

        public MBR GetMBR()
        {
            XTVector min = new XTVector(gauss.Mean.Elements.Length);
            XTVector max = new XTVector(gauss.Mean.Elements.Length);
            for (int i = 0; i < min.Length; ++i)
            {
                double s = Math.Sqrt(gauss.Covariance.Covariance[i, i] * Math.Log(owner.RelevanceLevel) * (-2));
                min[i] = gauss.Mean.Elements[i] - s;
                max[i] = gauss.Mean.Elements[i] + s;
            }

            return new MBR(min, max);
        }

        #endregion

        
    }
}


