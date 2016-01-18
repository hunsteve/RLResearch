using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContinuousRLTest
{
    public class InvertedPendulumAction : ReinforcementLearningAction
    {
        public int action = 0;
        public InvertedPendulumAction(int action)
        {
            this.action = action;
        }


        #region ReinforcementLearningAction Members

        public bool Equals(ReinforcementLearningAction obj)
        {
            return (((InvertedPendulumAction)obj).action == action);
        }

        #endregion
    }

    public class InvertedPendulumAction2 : ReinforcementLearningAction
    {
        public float action = 0;
        public InvertedPendulumAction2(float action)
        {
            this.action = action;
        }


        #region ReinforcementLearningAction Members

        public bool Equals(ReinforcementLearningAction obj)
        {
            return (((InvertedPendulumAction2)obj).action == action);
        }

        #endregion
    }

    public class InvertedPendulumState : ReinforcementLearningState
    {
        public double a, w;
        public InvertedPendulumState(double a, double w)
        {
            this.a = a;
            this.w = w;
        }

        #region ReinforcementLearningState Members

        private static ReinforcementLearningAction[] allActions;
        public ReinforcementLearningAction[] GetActions()
        {
            if (allActions == null)
            {
                List<InvertedPendulumAction> list = new List<InvertedPendulumAction>();
                for (int i = -1; i <= 1; ++i)
                {
                    list.Add(new InvertedPendulumAction(i));
                }
                allActions = list.ToArray();
            }
            return allActions;
        }

        #endregion
    }

    public class InvertedPendulumQStore : ReinforcementLearningQStore
    {

        public const int LEN = 200;
        public const int DD = 1;

        public float[, ,] value = new float[3, LEN, LEN];//-1,0,1;-pi..pi;-10..10

        public static void GetIndices(InvertedPendulumState state, InvertedPendulumAction action, out int i, out int j, out int k)
        {
            GetStateIndices(state, out j, out k);
            GetActionIndices(action, out i);
        }

        public static void GetStateIndices(InvertedPendulumState state, out int j, out int k)
        {
            j = ((int)((state.a / Math.PI + 1) * LEN / 2) + LEN * 1000) % LEN;
            k = (int)((state.w / 3 + 1) * LEN / 2); if (k > LEN - 1) k = LEN - 1; if (k < 0) k = 0;
        }

        public static void GetActionIndices(InvertedPendulumAction action, out int i)
        {
            i = action.action + 1;
        }

        public static void GetStateAndAction(int i, int j, int k, out InvertedPendulumState state, out InvertedPendulumAction action)
        {
            GetState(j, k, out state);
            GetAction(i, out action);
        }

        public static void GetState(int j, int k, out InvertedPendulumState state)
        {
            double a = (j / (float)(LEN / 2) - 1) * Math.PI;
            double w = (k / (float)(LEN / 2) - 1) * 3;
            state = new InvertedPendulumState(a, w);
        }

        public static void GetAction(int i, out InvertedPendulumAction action)
        {
            action = new InvertedPendulumAction(i - 1);
        }

        #region ReinforcementLearningQStore Members

        public float Evaluate(ReinforcementLearningState state, ReinforcementLearningAction action)
        {
            int i;
            int j;
            int k;
            GetIndices((InvertedPendulumState)state, (InvertedPendulumAction)action, out i, out j, out k);    
   
            return value[i,j,k];
        }

        public void ModifyValue(ReinforcementLearningState state, ReinforcementLearningAction action, float delta)
        {
            int i;
            int j;
            int k;
            GetIndices((InvertedPendulumState)state, (InvertedPendulumAction)action, out i, out j, out k);      
            
            value[i, j, k] += delta;


            //for (int jj = j - DD; jj <= j + DD; ++jj)
            //{
            //    for (int kk = Math.Max(k - DD, 0); kk <= Math.Min(k + DD, LEN - 1); ++kk)
            //    {
            //        value[i, (jj + LEN) % LEN, kk] += (float)Math.Exp(-((j - jj) * (j - jj) + (k - kk) * (k - kk)) / DD / DD) / DD / DD * delta;
            //    }
            //}
        }

        public void GetBestActionAndUtilityForState(ReinforcementLearningState state, out ReinforcementLearningAction action, out float retval)
        {
            float max = float.MinValue;
            int maxact = 0;

            int i;
            int j;
            int k;
            GetStateIndices((InvertedPendulumState)state, out j, out k);      

            for (int act = -1; act <= 1; ++act)
            {
                GetActionIndices(new InvertedPendulumAction(act), out i);              
                if (value[i, j, k] > max)
                {
                    max = value[i, j, k];
                    maxact = act;
                }
            }
            if (value[1, j, k] == max) maxact = 0;


            action = new InvertedPendulumAction(maxact);
            retval = max;
        }        

        #endregion
    }

    public class InvertedPendulumEpsilonGreedyPolicy : EpsilonGreedyPolicy
    {
        public override float Epsilon
        {
            get { return 0.5f; }
        }        
    }

    public class InvertedPendulumSoftmaxPolicy : SoftmaxPolicy
    {
        public override float Temperature
        {
            get { return 10f; }
        }
    }

    public class InvertedPendulumRLController : ReinforcementLearningController
    {
        protected override float Discount
        {
            get { return 0.99f; }
        }

        double alpha = 1f;

        protected override float Alpha
        {
            get { alpha *= 0.9999999; return (float)alpha; }
        }
        
        protected override ReinforcementLearningQStore CreateQStore(ReinforcementLearningEnvironment ws)
        {
            return new InvertedPendulumQStore(); 
        }
    }
}
