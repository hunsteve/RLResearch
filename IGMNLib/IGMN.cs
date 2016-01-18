using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Matrix;
using XTreeCSharp;
using System.Drawing;

namespace IGMNLib
{
    public class IGMN 
    {
        
        public const int AGE_MIN = 50;
        public const double ACCUMLATOR_MIN = 5;
        public const double INITIAL_VARIANCE = 0.01;//acrobot = 0.1, inverted pendulum = 0.03


        public double RelevanceLevel = 0.01;

        List<IGMNData> cortical;
        public XTree<IGMNData> xtree;

        public IGMN(Vector maxMin)
        {
            this.initialGauss = maxMin;
            this.initialGauss.Multiply(INITIAL_VARIANCE);
            cortical = new List<IGMNData>();
            xtree = new XTree<IGMNData>();
        }

        public void Train(Vector x)
        {
            double sum = 0;

            XTVector min = convertVector(x); min[min.Length - 1] = double.MinValue;
            XTVector max = convertVector(x); max[max.Length - 1] = double.MaxValue;
            List<IGMNData> possibleRelevant2 = xtree.rangeQuery(new MBR(min, max));

            for (int i = 0; i < possibleRelevant2.Count; ++i)
            {
                sum += possibleRelevant2[i].Accumlator;
                possibleRelevant2[i].Age++;
            }
            sum /= possibleRelevant2.Count;


            List<IGMNData> possibleRelevant = xtree.pointQuery(convertVector(x));
            List<double> posterior = new List<double>();
            double sum2 = 0;
            List<IGMNData> relevant = new List<IGMNData>();
            for (int i = 0; i < possibleRelevant.Count; ++i)
            {
                double e;
                double relev = possibleRelevant[i].Gaussian.Likelihood(x, out e) * (possibleRelevant[i].Accumlator / sum); // likelihood * prior
                if (e > RelevanceLevel)
                {
                    posterior.Add(relev);
                    sum2 += relev;
                    relevant.Add(possibleRelevant[i]);
                }
            }

            if (relevant.Count == 0)
            {
                IGMNData newdata = new IGMNData(this, x);
                cortical.Add(newdata);
                xtree.Insert(newdata);
            }
            else
            {
                //relevance normalization
                for (int i = 0; i < posterior.Count; ++i)
                {
                    posterior[i] /= sum2;
                }
                
                //true relevant gaussians update
                for (int i = 0; i < relevant.Count; ++i)
                {                    
                    relevant[i].Accumlator += posterior[i];
                    double w = posterior[i] / relevant[i].Accumlator;
                    
                    xtree.Delete(relevant[i]);
                    relevant[i].RefineWithData(x, w);//0.25|w
                    xtree.Insert(relevant[i]);
                }              
            }

            //delete unneeded gaussians
            List<IGMNData> deleteList = new List<IGMNData>();
            foreach (IGMNData g in possibleRelevant2)
            {
                if ((g.Age > AGE_MIN) && (g.Accumlator < ACCUMLATOR_MIN))
                {
                    deleteList.Add(g);
                }
            }

            foreach (IGMNData g in deleteList)
            {
                cortical.Remove(g);
                xtree.Delete(g);
            }
        }


        private XTVector convertVector(Vector v)
        {
            return new XTVector((double[])v.Elements.Clone());
        }

        private XTVector convertInputVector(Vector v, double lastValue)
        {
            double[] vv = new double[v.Elements.Length+1];
            for(int i=0; i<vv.Length-1; ++i) 
            {
                vv[i] = v.Elements[i];
            }
            vv[vv.Length - 1] = lastValue;
            return new XTVector(vv);
        }

        public double Recall(Vector input) {
            double e;
            return Recall(input, out e);
        }

        public double Recall(Vector input, out double variance)//input vector is 1 dimension shorter then the gaussians
        {
            variance = -1;//TODO!

            List<IGMNData> possibleRelevant = xtree.rangeQuery(new MBR(convertInputVector(input, double.MinValue), convertInputVector(input, double.MaxValue)));
            
            double sum = 0;
            for (int i = 0; i < possibleRelevant.Count; ++i)//cortical
            {
                sum += possibleRelevant[i].Accumlator;
            }
            sum /= possibleRelevant.Count;

            
            List<double> posterior = new List<double>();
            double sum2 = 0;
            List<IGMNData> relevant = new List<IGMNData>();
            for (int i = 0; i < possibleRelevant.Count; ++i)
            {
                double e;
                double relev = possibleRelevant[i].InputGaussian.Likelihood(input, out e) * (possibleRelevant[i].Accumlator / sum); // likelihood * prior
                if (e > RelevanceLevel)
                {
                    posterior.Add(relev);
                    sum2 += relev;
                    relevant.Add(possibleRelevant[i]);
                }
            }
            
            for (int i = 0; i < posterior.Count; ++i)
            {
                posterior[i] /= sum2;
            }

            double sumV = 0;
            for (int i = 0; i < relevant.Count; ++i)
            {
                int inputLength = relevant[i].InputGaussian.Mean.Elements.Length;
                DenseMatrix temp = relevant[i].Gaussian.Covariance.Covariance.MatrixMultiply(relevant[i].InputGaussian.Covariance.InverseCovariance, inputLength, 0, 1, inputLength);
                Vector vv = temp.VectorMultiplyRight(input - relevant[i].InputGaussian.Mean);
                double v = vv.Elements[0];
                v += relevant[i].Gaussian.Mean.Elements[inputLength];
                v *= posterior[i];
                sumV += v;
            }

            return sumV;
        }

        Vector initialGauss;
        public Vector InitialGauss
        {
            get
            {
                return initialGauss;
            }
        }


        public void DrawGaussians(Graphics g, int i1, int i2, double zoom)
        {
            foreach (IGMNData d in cortical)
            {
                d.Draw(g, i1, i2, Math.Log(RelevanceLevel), zoom);
            }
        }
    }
}
