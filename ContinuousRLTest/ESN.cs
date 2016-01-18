using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathWorks.MATLAB.NET.Arrays;
using ESN;
using Matrix;

namespace ContinuousRLTest
{
    class ESN
    {
        SparseMatrix innerConnections;
        DenseMatrix inputWeights;
        Vector biasWeights;
        Vector states;

        public ESN(int reservoirSize, int inputSize, int outputSize) 
        {
            states = new Vector(reservoirSize);
            innerConnections = GenerateInnerWeights(reservoirSize);
            inputWeights = new DenseMatrix(inputSize,reservoirSize);
            biasWeights = new Vector(reservoirSize);
            Random r = new Random();
            for (int row = 0; row < inputSize; ++row)
            {
                for (int col = 0; col < reservoirSize; ++col)
                {
                    inputWeights[row, col] = (double)r.NextDouble();
                }                
            }

            for (int i = 0; i < reservoirSize; ++i)
            {
                biasWeights.Elements[i] = (double)r.NextDouble();
            }
        }

        public DenseMatrix Simulate(DenseMatrix inputs) //rows: t, cols: inputcount
        {
            states = new Vector(states.Elements.Length);

            DenseMatrix temp = inputs.MatrixMultiply(inputWeights);

            DenseMatrix ret = new DenseMatrix(inputs.Rows, states.Elements.Length);

            for (int t = 0; t < temp.Rows; ++t)
            {
                Vector v = temp.GetRow(t);                
                Vector states2 = innerConnections.MatrixMultiplyRight(states);
                states2.Add(v);
                states2.Add(biasWeights);
                for (int i = 0; i < states2.Elements.Length; ++i)
                {
                    states2.Elements[i] = (double)Math.Tanh(states2.Elements[i]);
                }

                states = states2;
                for (int i = 0; i < states.Elements.Length; ++i)
                {
                    ret[t, i] = states.Elements[i];
                }
            }

            return ret;
        }

        public DenseMatrix GetOutputFromStates(DenseMatrix states)
        {
            return null;
        }

        public DenseMatrix GetOutputFromInput(DenseMatrix inputs)
        {
            return GetOutputFromStates(Simulate(inputs));
        }

        public void Train(DenseMatrix states, DenseMatrix targets)
        {
            //hogy lehetne ugy tanitani, LS modszerrel, hogy ne vesszen el a mar beletanitott cucc
            //hogy lehetne megjegyezni, hogy mi az amit mar megtanitottunk neki
            //valoszinusegi jelentest hogy lehetne adni ennek a dolognak... 
            //valami matrixot karbantartva, ami megmondja, hogy mekkora a szorasa/informaciotartalma az adott helyen

        }



        private SparseMatrix GenerateInnerWeights(int size)
        {
            ESNclass esn = new ESNclass();
            MWNumericArray _size = new MWNumericArray(size);
            MWNumericArray _ratio = new MWNumericArray(0.05f);
            MWArray ret = esn.ESNInnerWeights(_size, _ratio);
            return new SparseMatrix((MWNumericArray)ret);            
        }


    }
}
