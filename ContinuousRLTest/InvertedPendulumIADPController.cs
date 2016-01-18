using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Matrix;

namespace ContinuousRLTest
{
    public class InvertedPendulumIADPController
    {
        public Vector[] Q;
        public List<InvertedPendulumState> states;

        public InvertedPendulumIADPController()
        {
            Q = new Vector[3];
        }

        private int GetClosestStateIndex(List<InvertedPendulumState> states, InvertedPendulumState curstate)
        {
            double mindist = float.MaxValue;
            int closeststate = 0;
            for (int i = 0; i < states.Count; ++i)
            {
                InvertedPendulumState state = states[i];
                double dist = (state.a - curstate.a) * (state.a - curstate.a) + (state.w - curstate.w) * (state.w - curstate.w);
                if (mindist > dist)
                {
                    mindist = dist;
                    closeststate = i;
                }
            }
            return closeststate;
        }

        public void Train()
        {
            InvertedPendulumEnvironment testenv = new InvertedPendulumEnvironment();
            int mincount = 100;


            states = new List<InvertedPendulumState>();            
            for(int i=0; i<mincount; ++i) {
                testenv.Reset();
                InvertedPendulumState newstate = (InvertedPendulumState)testenv.State();                
                states.Add(newstate);
            }            
            
            int statenum = states.Count;

           


            //allapot-atmenet valsegek es varhato rewardok szamitasa
            Vector R = new Vector(statenum);

            SparseMatrix[] Ma = new SparseMatrix[3];
            for (int i = 0; i < 3; ++i)
            {
                Ma[i] = new SparseMatrix(statenum);
            }

            Vector counts = new Vector(statenum);            

            for (int ii = 0; ii < statenum*10000; ++ii)
            {
                testenv.Reset();
                InvertedPendulumState curstate = (InvertedPendulumState)testenv.State();
                int from = GetClosestStateIndex(states, curstate);

                R.Elements[from] += testenv.Reward();

                for (int i = 0; i < 3; ++i)
                {
                    int action = i - 1;
                    testenv.SetState(curstate);
                    testenv.Step(action);
                    InvertedPendulumState tostate = (InvertedPendulumState)testenv.State();
                    int to = GetClosestStateIndex(states, tostate);

                    Ma[i][from, to] += 1;                    
                }

                float temp = (float)counts.Elements[from];
                if (temp != 0) temp = 1 / (1 / temp + 1);
                else temp = 1;
                counts.Elements[from] = temp;
            }

            for (int i = 0; i < 3; ++i)
            {
                Ma[i].Multiply(counts);
            }

            R.Multiply(counts);


            SparseMatrix M = (Ma[0] + Ma[1] + Ma[2]);
            M.Multiply(0.333333f);
            M.Multiply(0.99f);

            SparseMatrix IM = SparseMatrix.Identity(statenum) - M;
            Vector utility = IM.SolveLinearEquation2(R);

            for (int i = 0; i < 3; ++i)
            {
                Q[i] = Ma[i].MatrixMultiplyRight(utility);
            }
        }

    }
}
