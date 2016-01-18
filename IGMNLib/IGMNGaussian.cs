//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Matrix;
//using XTreeCSharp;

//namespace IGMNLib
//{
//    class IGMNGaussian
//    {
//        const double T_NEW = 0.01;

//        public int Age
//        {
//            get;
//            set;
//        }

//        private double accumlator;
//        public double Accumlator
//        {
//            get
//            {
//                return accumlator;
//            }
//            set
//            {
//                owner.SumAccumlator += -accumlator + value;
//                accumlator = value;
//            }
//        }

//        public double Prior
//        {
//            get
//            {
//                return Accumlator / owner.SumAccumlator;
//            }
//        }

//        public double Likelihood(Vector x, out bool relevant)
//        {
//            eps = x - Mean;

//            double ret = 0;
//            //Console.Out.WriteLine("max: " + Covariance.GetMaxDiagonal());           

//            if (Covariance.GetMaxDiagonal() * (-2) * Math.Log(T_NEW) > eps.GetLengthSquared())
//            {
//                int dim = x.Elements.Length;
//                double c = 1 / (Math.Pow(2 * Math.PI, dim / 2) * Math.Sqrt(Covariance.Determinant));
//                covarianceScalarInverse = Covariance.GetScalarInverse(eps);
//                covarianceInputScalarInverse = Covariance.GetInputScalarInverse(eps);
//                ret = Math.Exp(-0.5 * covarianceScalarInverse);
//                relevant = (ret > T_NEW);
//                ret *= c;
//            }
//            else
//            {
//                ret = 0;
//                relevant = false;
//            }

//            lastLikelihood = (double)ret;//store prob for next phase
//            //if (Double.IsNaN(lastInputLikelihood)) throw new Exception();
//            return lastLikelihood;
//        }

//        public double InputLikelihood(Vector x, out bool relevant)
//        {
//            eps = x - Mean;

//            double ret = 0;
//            if (Covariance.GetMaxDiagonal() * 9 > eps.GetLengthSquared())
//            {
//                covarianceInputScalarInverse = Covariance.GetInputScalarInverse(eps);
//                double c = 1 / (Math.Pow(2 * Math.PI, inputLength / 2) * Math.Sqrt(Covariance.InputDeterminant));
//                ret = c * Math.Exp(-0.5 * covarianceInputScalarInverse);
//                relevant = true;
//            }
//            else
//            {
//                ret = 0;
//                relevant = false;
//            }

//            lastInputLikelihood = (double)ret;//store prob for next phase
//            return lastInputLikelihood;
//        }



//        private double lastLikelihood;
//        private double lastInputLikelihood;
//        private Vector eps;
//        private double w;
//        private double posterior;
//        private double inputPosterior;
//        private double covarianceScalarInverse;
//        private double covarianceInputScalarInverse;


//        public Vector Mean
//        {
//            get;
//            set;
//        }

//        public CovarianceMatrix Covariance
//        {
//            get;
//            set;
//        }

//        private IGMN owner;
//        int inputLength;
//        int outputLength;

//        public IGMNGaussian(IGMN owner, Vector mean, int inputLength, int outputLength)
//        {
//            this.owner = owner;
//            this.inputLength = inputLength;
//            this.outputLength = outputLength;

//            Age = 1;
//            Accumlator = 1;
//            Mean = new Vector(mean);
//            Covariance = new CovarianceMatrix(owner, inputLength, outputLength);
//        }


//        public double TrainPhase1(Vector x, out bool relevant)//calculate probability for input
//        {
//            return Likelihood(x, out relevant) * Prior;
//        }

//        public void TrainPhase2(double sum)//calculate posterior probability
//        {
//            //Prior fugg az accumlatortol, ezert kell kulon szamolni a TrainPhase3-tol
//            if (sum == 0)
//            {
//                posterior = Prior;
//            }
//            else
//            {
//                posterior = lastLikelihood * Prior / sum;
//            }
//        }

//        public void TrainPhase3()
//        {
//            Age++;
//            Accumlator = Accumlator + posterior;
//            w = posterior / Accumlator;
//            Mean.Add(eps, w);
//            Covariance.ModifyWithVector(eps, w, covarianceScalarInverse, covarianceInputScalarInverse);
//        }

//        //public double TrainPhase4(Vector x)
//        //{
//        //    bool relevant;
//        //    return InputLikelihood(x, out relevant) * Prior;            
//        //}

//        //public void TrainPhase5(double sum)//calculate posterior probability
//        //{
//        //    if (sum == 0)
//        //    {
//        //        inputPosterior = Prior;
//        //    }
//        //    else
//        //    {
//        //        inputPosterior = lastInputLikelihood * Prior / sum;
//        //    }
//        //}

//        //public Vector TrainPhase6()        
//        //{
//        //    DenseMatrix temp = Covariance.Covariance.MatrixMultiply(Covariance.InputInverseCovariance, inputLength, 0, outputLength, inputLength);
//        //    Vector v = temp.VectorMultiplyRight(eps);
//        //    v.Add(Mean.Part(inputLength, outputLength));
//        //    v.Multiply(inputPosterior);
//        //    return v;
//        //}



//        public double RecallPhase1(Vector x, out bool relevant)
//        {
//            return InputLikelihood(x, out relevant) * Prior;
//        }

//        public void RecallPhase2(double sum)//calculate posterior probability
//        {
//            if (sum == 0)
//            {
//                inputPosterior = Prior;
//            }
//            else
//            {
//                inputPosterior = lastInputLikelihood * Prior / sum;
//            }
//        }

//        public Vector RecallPhase3()
//        {
//            DenseMatrix temp = Covariance.Covariance.MatrixMultiply(Covariance.InputInverseCovariance, inputLength, 0, outputLength, inputLength);
//            Vector v = temp.VectorMultiplyRight(eps);
//            v.Add(Mean.Part(inputLength, outputLength));
//            v.Multiply(inputPosterior);
//            return v;
//        }


//    }
//}
