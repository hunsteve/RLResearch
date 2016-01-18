using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TablularApproximator
{
    public class TabularApproximator
    {
        int[] cellCounts;
        double[] dataStorage;
        double[] minVector;
        double[] maxVector;

        public TabularApproximator(int[] cellCounts, double[] minVector, double[] maxVector)
        {
            this.minVector = minVector;
            this.maxVector = maxVector;
            this.cellCounts = cellCounts;
            long c = 1;
            for(int i=0;i<cellCounts.Length;++i) 
            {
                c *= cellCounts[i];
            }
            dataStorage = new double[c];
        }

        public double Recall(double[] coordinates)
        {
            return dataStorage[calcIndex(coordinates)];
        }

        public void Train(double[] coordinates, double value, double learningRate)
        {
            int index = calcIndex(coordinates);

            double oldvalue = dataStorage[index];
            double newvalue = (1 - learningRate) * oldvalue + learningRate * value;
            if (Double.IsNaN(newvalue))
            {
                Console.Out.WriteLine();
            }

            dataStorage[index] = newvalue;
        }

        private int calcIndex(double[] coordinates)
        {
            int index = 0;
            for (int i = 0; i < cellCounts.Length; ++i)
            {                
                int dimindex = (int)(((coordinates[i] - minVector[i]) / (maxVector[i] - minVector[i])) * cellCounts[i]);
                if (dimindex < 0) dimindex = 0;
                if (dimindex > cellCounts[i] - 1) dimindex = cellCounts[i] - 1;

                index *= cellCounts[i];
                index += dimindex;
            }
            return index;
        }

    }
}
