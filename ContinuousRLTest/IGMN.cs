using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IGMN;
using MathWorks.MATLAB.NET.Arrays;
using Matrix;

namespace ContinuousRLTest
{
    class CovarianceMatrix
    {
        private const double DELTA = 0.01;
        private DenseMatrix covariance;
        public DenseMatrix Covariance
        {
            get {
                return covariance;
            }
        }

        private DenseMatrix inverseCovariance;
        public DenseMatrix InverseCovariance
        {
            get {
                return inverseCovariance;
            }
        }

        private DenseMatrix inputInverseCovariance;
        public DenseMatrix InputInverseCovariance
        {
            get
            {
                return inputInverseCovariance;
            }
        }

        private double determinant;
        public double Determinant
        {
            get {
                return determinant;
            }
        }

        private double inputDeterminant;
        public double InputDeterminant
        {
            get
            {
                return inputDeterminant;
            }
        }

        public double GetMaxDiagonal()
        {
            double ret = double.MinValue;
            for (int i = 0; i < covariance.Cols; ++i)
            {
                ret = Math.Max(ret, covariance[i, i]);
            }
            return ret;
        }
      
        public CovarianceMatrix(IGMN owner, int inputLength, int outputLength)
        {
            Vector scaledMaxMin = new Vector(owner.MaxMin.Elements.Length);
            double prod = 1;
            double prod2 = 1;
            Vector invMaxMin = new Vector(owner.MaxMin.Elements.Length);
            for (int i = 0; i < owner.MaxMin.Elements.Length; ++i)
            {
                double temp = Math.Pow(DELTA * owner.MaxMin.Elements[i], 1);
                scaledMaxMin.Elements[i] = temp;
                prod *= temp;
                if (i < inputLength) prod2 *= temp;
                invMaxMin.Elements[i] = 1 / temp;
            }
            determinant = prod;
            covariance = DenseMatrix.Diag(scaledMaxMin);
            inverseCovariance = DenseMatrix.Diag(invMaxMin);

            inputDeterminant = prod2;
            inputInverseCovariance = inverseCovariance.Part(0, 0, inputLength, inputLength);

        }

        public void ModifyWithVector(Vector eps, double w)
        {
            double sci = GetScalarInverse(eps);
            double isci = GetInputScalarInverse(eps);
            ModifyWithVector(eps, w, sci, isci);
        }

        public void ModifyWithVector(Vector eps, double w, double scalarinverse, double inputscalarinverse)
        {            
            //szoroz (1-w)
            //det(s*X) = s^rank(X)*det(X)
            //(s*X)^(-1) = 1/s*X^(-1)
            covariance.Multiply(1 - w);
            inverseCovariance.Multiply(1 / (1 - w));
            inputInverseCovariance.Multiply(1 / (1 - w));
            determinant = (double)(determinant * Math.Pow(1 - w, covariance.Rows));
            inputDeterminant = (double)(determinant * Math.Pow(1 - w, inputInverseCovariance.Rows));
            double newscalarinverse = scalarinverse / (1 - w);
            double newinputscalarinverse = inputscalarinverse / (1 - w);

            //hozzaad (-w*w+w) * v
            //det(X + cr) = det(X)(1 + rX^(−1)c)
            //(A+cr)^(-1)=A^(-1) - (A^(-1)*c*r*A^(-1))/(1 + rX^(−1)c)

            double factor = -w * w + w;  
            covariance.Add(DenseMatrix.CreateDiad(eps, eps), factor);
            Vector inveps = inverseCovariance.VectorMultiplyRight(eps);
            Vector inpinveps = inputInverseCovariance.VectorMultiplyRight(eps);
            inverseCovariance.Add(DenseMatrix.CreateDiad(inveps, inveps), -factor / (1 + factor * newscalarinverse));
            inputInverseCovariance.Add(DenseMatrix.CreateDiad(inpinveps, inpinveps), -factor / (1 + factor * newinputscalarinverse));
            determinant *= (1 + factor * newscalarinverse);
            inputDeterminant *= (1 + factor * newinputscalarinverse);
        }

        public double GetScalarInverse(Vector eps) //eps^T * inv(C) * eps
        {
            double sum = 0;
            
            for (int c = 0; c < inverseCovariance.Cols; ++c)
            {
                double sum2 = 0;
                for (int r = 0; r < inverseCovariance.Rows; ++r)   
                {
                    sum2 += inverseCovariance[r, c] * eps.Elements[r];
                }
                sum += sum2 * eps.Elements[c];
            }

            return (double)sum;
        }

        public double GetInputScalarInverse(Vector eps)
        {
            double sum = 0;

            for (int c = 0; c < inputInverseCovariance.Cols; ++c)
            {
                double sum2 = 0;
                for (int r = 0; r < inputInverseCovariance.Rows; ++r)
                {
                    sum2 += inputInverseCovariance[r, c] * eps.Elements[r];
                }
                sum += sum2 * eps.Elements[c];
            }

            return (double)sum;
        }
    }

    class IGMNGaussian
    {
        const double T_NEW = 0.01;

        public int Age
        {
            get;
            set;
        }

        private double accumlator;
        public double Accumlator
        {
            get
            {
                return accumlator;
            }
            set
            {
                owner.SumAccumlator += -accumlator + value;
                accumlator = value;
            }
        }

        public double Prior
        {
            get
            {
                return Accumlator / owner.SumAccumlator;
            }
        }

        public double Likelihood(Vector x, out bool relevant)
        {                     
            eps = x - Mean;

            double ret = 0;
            //Console.Out.WriteLine("max: " + Covariance.GetMaxDiagonal());           

            if (Covariance.GetMaxDiagonal() * (-2) * Math.Log(T_NEW) > eps.GetLengthSquared())
            {
                int dim = x.Elements.Length;
                double c = 1 / (Math.Pow(2 * Math.PI, dim / 2) * Math.Sqrt(Covariance.Determinant));
                covarianceScalarInverse = Covariance.GetScalarInverse(eps);
                covarianceInputScalarInverse = Covariance.GetInputScalarInverse(eps);
                ret = Math.Exp(-0.5 * covarianceScalarInverse);
                relevant = (ret > T_NEW);
                ret *= c;
            }
            else
            {
                ret = 0;
                relevant = false;
            }
                        
            lastLikelihood = (double)ret;//store prob for next phase
            //if (Double.IsNaN(lastInputLikelihood)) throw new Exception();
            return lastLikelihood;
        }

        public double InputLikelihood(Vector x, out bool relevant)
        {
            eps = x - Mean;

            double ret = 0;
            if (Covariance.GetMaxDiagonal() * 9 > eps.GetLengthSquared())
            {
                covarianceInputScalarInverse = Covariance.GetInputScalarInverse(eps);
                double c = 1 / (Math.Pow(2 * Math.PI, inputLength / 2) * Math.Sqrt(Covariance.InputDeterminant));
                ret = c * Math.Exp(-0.5 * covarianceInputScalarInverse);
                relevant = true;
            }
            else
            {
                ret = 0;
                relevant = false;
            }

            lastInputLikelihood = (double)ret;//store prob for next phase
            return lastInputLikelihood;
        }



        private double lastLikelihood;
        private double lastInputLikelihood;
        private Vector eps;
        private double w;
        private double posterior;
        private double inputPosterior;
        private double covarianceScalarInverse;
        private double covarianceInputScalarInverse;
        

        public Vector Mean
        {
            get;
            set;
        }

        public CovarianceMatrix Covariance
        {
            get;
            set;
        }

        private IGMN owner;
        int inputLength;
        int outputLength;

        public IGMNGaussian(IGMN owner, Vector mean, int inputLength, int outputLength)
        {
            this.owner = owner;
            this.inputLength = inputLength;
            this.outputLength = outputLength;

            Age = 1;
            Accumlator = 1;
            Mean = new Vector(mean);
            Covariance = new CovarianceMatrix(owner, inputLength, outputLength);
        }


        public double TrainPhase1(Vector x, out bool relevant)//calculate probability for input
        {
            return Likelihood(x, out relevant) * Prior;
        }

        public void TrainPhase2(double sum)//calculate posterior probability
        {
            //Prior fugg az accumlatortol, ezert kell kulon szamolni a TrainPhase3-tol
            if (sum == 0)
            {
                posterior = Prior;
            }
            else
            {
                posterior = lastLikelihood * Prior / sum;
            }
        }

        public void TrainPhase3()
        {
            Age++;
            Accumlator = Accumlator + posterior;
            w = posterior / Accumlator;
            Mean.Add(eps, w);
            Covariance.ModifyWithVector(eps, w, covarianceScalarInverse, covarianceInputScalarInverse);
        }

        //public double TrainPhase4(Vector x)
        //{
        //    bool relevant;
        //    return InputLikelihood(x, out relevant) * Prior;            
        //}

        //public void TrainPhase5(double sum)//calculate posterior probability
        //{
        //    if (sum == 0)
        //    {
        //        inputPosterior = Prior;
        //    }
        //    else
        //    {
        //        inputPosterior = lastInputLikelihood * Prior / sum;
        //    }
        //}

        //public Vector TrainPhase6()        
        //{
        //    DenseMatrix temp = Covariance.Covariance.MatrixMultiply(Covariance.InputInverseCovariance, inputLength, 0, outputLength, inputLength);
        //    Vector v = temp.VectorMultiplyRight(eps);
        //    v.Add(Mean.Part(inputLength, outputLength));
        //    v.Multiply(inputPosterior);
        //    return v;
        //}



        public double RecallPhase1(Vector x, out bool relevant)
        {            
            return InputLikelihood(x, out relevant) * Prior;
        }

        public void RecallPhase2(double sum)//calculate posterior probability
        {
            if (sum == 0)
            {
                inputPosterior = Prior;
            }
            else
            {
                inputPosterior = lastInputLikelihood * Prior / sum;
            }
        }

        public Vector RecallPhase3()
        {
            DenseMatrix temp = Covariance.Covariance.MatrixMultiply(Covariance.InputInverseCovariance, inputLength, 0, outputLength, inputLength);
            Vector v = temp.VectorMultiplyRight(eps);
            v.Add(Mean.Part(inputLength, outputLength));
            v.Multiply(inputPosterior);
            return v;
        }
    }


    public class IGMN
    {
        List<IGMNGaussian> cortical;
        //const double EPSILON_MAX = 0.01;        
        const int AGE_MIN = 500;
        const double ACCUMLATOR_MIN = 5;
        public double SumAccumlator
        {
            get;
            set;
        }

        int inputLength;
        int outputLength;

        public IGMN(int inputLength, int outputLength, Vector maxMin)
        {
            this.maxMin = maxMin;
            this.inputLength = inputLength;
            this.outputLength = outputLength;
            cortical = new List<IGMNGaussian>();
            SumAccumlator = 0;
        }

        public void Train(Vector x)
        {
            double sum = 0;

            //E part of EM alg.

            List<IGMNGaussian> relevantCortical = new List<IGMNGaussian>();

            foreach (IGMNGaussian g in cortical)
            {
                bool relevant;
                sum += g.TrainPhase1(x, out relevant);
                if (relevant) relevantCortical.Add(g);
            }

            if (relevantCortical.Count == 0)
            {
                cortical.Add(new IGMNGaussian(this, x, inputLength, outputLength));
            }
            else
            {                
                foreach (IGMNGaussian g in relevantCortical)
                {
                    g.TrainPhase2(sum);
                }

                //M part of EM alg.
                foreach (IGMNGaussian g in relevantCortical)
                {
                    g.TrainPhase3();
                }

                //double sum2 = 0;
                //foreach (IGMNGaussian g in relevantCortical)
                //{
                //    sum2 += g.TrainPhase4(x);
                //}

                //foreach (IGMNGaussian g in relevantCortical)
                //{
                //    g.TrainPhase5(sum2);
                //}

                //Vector sumV = new Vector(outputLength);
                //foreach (IGMNGaussian g in relevantCortical)
                //{
                //    sumV.Add(g.TrainPhase6());
                //}

                //bool neednew = false;
                //for (int i = 0; i < outputLength; ++i)
                //{
                //    double epsilon = Math.Abs(sumV.Elements[i] - x.Elements[inputLength+i]) / (MaxMin.Elements[inputLength+i]);
                //    if (epsilon > EPSILON_MAX)
                //    {
                //        neednew = true;
                //    }
                //}

                //if (neednew)
                //{
                //    cortical.Add(new IGMNGaussian(this, x, inputLength, outputLength));
                //}


                //delete unneeded gaussians
                List<IGMNGaussian> deleteList = new List<IGMNGaussian>();
                foreach (IGMNGaussian g in cortical)
                {
                    if ((g.Age > AGE_MIN) && (g.Accumlator < ACCUMLATOR_MIN))
                    {
                        deleteList.Add(g);
                    }
                }

                foreach (IGMNGaussian g in deleteList)
                {
                    cortical.Remove(g);
                }
            }

        }

        public Vector Recall(Vector input)
        {
            double sum2 = 0;
            List<IGMNGaussian> relevantCortical = new List<IGMNGaussian>();
            foreach (IGMNGaussian g in cortical)
            {
                bool relevant;
                sum2 += g.RecallPhase1(input, out relevant);
                if (relevant)
                {
                    relevantCortical.Add(g);
                }
            }

            foreach (IGMNGaussian g in relevantCortical)
            {
                g.RecallPhase2(sum2);
            }

            Vector sumV = new Vector(outputLength);
            foreach (IGMNGaussian g in relevantCortical)
            {
                sumV.Add(g.RecallPhase3());
            }

            return sumV;
        }

        Vector maxMin;
        public Vector MaxMin 
        { 
            get {             
                return maxMin;
            }
        }
    }
}
