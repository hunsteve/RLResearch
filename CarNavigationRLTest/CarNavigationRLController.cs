using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContinuousRLTest;
using OnlabNeuralis;
using System.IO;
using System.IO.Compression;

namespace CarNavigationRLTest
{
    public class CarNavigationAction : ReinforcementLearningAction
    {
        public double ang = 0;
        public CarNavigationAction(double ang)
        {
            this.ang = ang;
        }


        #region ReinforcementLearningAction Members

        public bool Equals(ReinforcementLearningAction obj)
        {
            return (((CarNavigationAction)obj).ang == ang);
        }

        #endregion
    }

    public class CarNavigationState : ReinforcementLearningState
    {
        public double x, y, alpha;
        public CarNavigationState(double x, double y, double alpha)
        {
            this.x = x;
            this.y = y;
            this.alpha = alpha;
        }

        #region ReinforcementLearningState Members

        static ReinforcementLearningAction[] allActions;
        public ReinforcementLearningAction[] GetActions()
        {
            if (allActions == null)
            {
                List<CarNavigationAction> list = new List<CarNavigationAction>();
                for (int i = 0; i < CarNavigationQStore.LENACTION; ++i)
                {
                    CarNavigationAction action;
                    CarNavigationQStore.GetAction(i, out action);
                    list.Add(action);
                }
                allActions = list.ToArray();
            }
            return allActions;
        }

        #endregion
    }

    public class CarNavigationQStore : ReinforcementLearningQStore
    {

        public const int LENXY = 20;
        public const int LENANG = 12;
        public const int LENACTION = 11;

        public const double MINXY = -5;
        public const double MAXXY = 5;

        public const double MAXABSANG = Math.PI/3;

        public const int DD = 1;

        public float[, , ,] value = new float[LENACTION, LENXY, LENXY, LENANG];

        public static void GetIndices(CarNavigationState state, CarNavigationAction action, out int i, out int j, out int k, out int l)
        {
            GetStateIndices(state, out j, out k, out l);
            GetActionIndices(action, out i);
        }

        public static void GetStateIndices(CarNavigationState state, out int j, out int k, out int l)
        {
            j = (int)ComMath.NormalLim(state.x, MINXY, MAXXY, 0, LENXY - 1);
            k = (int)ComMath.NormalLim(state.y, MINXY, MAXXY, 0, LENXY - 1);
            l = (int)ComMath.NormalLim(state.alpha, -Math.PI, Math.PI, 0, LENANG - 1);
        }

        public static void GetActionIndices(CarNavigationAction action, out int i)
        {
            i = (int)ComMath.NormalLim(action.ang, -MAXABSANG, MAXABSANG, 0, LENACTION - 1);            
        }

        public static void GetStateAndAction(int i, int j, int k, int l, out CarNavigationState state, out CarNavigationAction action)
        {
            GetState(j, k, l, out state);
            GetAction(i, out action);
        }

        public static void GetState(int j, int k, int l, out CarNavigationState state)
        {
            double x = ComMath.Normal(j, 0, LENXY - 1, MINXY, MAXXY);
            double y = ComMath.Normal(k, 0, LENXY - 1 , MINXY, MAXXY);
            double alpha = ComMath.Normal(l, 0, LENANG - 1, -Math.PI, Math.PI);
            state = new CarNavigationState(x, y, alpha);
        }

        public static void GetAction(int i, out CarNavigationAction action)
        {
            double ang = ComMath.Normal(i, 0, LENACTION - 1, -MAXABSANG, MAXABSANG);
            action = new CarNavigationAction(ang);
        }

        #region ReinforcementLearningQStore Members

        public float Evaluate(ReinforcementLearningState state, ReinforcementLearningAction action)
        {
            int i;
            int j;
            int k;
            int l;
            GetIndices((CarNavigationState)state, (CarNavigationAction)action, out i, out j, out k, out l);

            return value[i, j, k, l];
        }

        public void ModifyValue(ReinforcementLearningState state, ReinforcementLearningAction action, float delta)
        {
            int i;
            int j;
            int k;
            int l;
            GetIndices((CarNavigationState)state, (CarNavigationAction)action, out i, out j, out k, out l);

            value[i, j, k, l] += delta;


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
            
            int j;
            int k;
            int l;
            GetStateIndices((CarNavigationState)state, out j, out k, out l);


            for (int i = 0; i < LENACTION; ++i)
            {                
                if (value[i, j, k, l] > max)
                {
                    max = value[i, j, k, l];
                    maxact = i;
                }
            }

            if (value[(int)Math.Ceiling(LENACTION / 2.0), j, k, l] == max) maxact = (int)Math.Ceiling(LENACTION / 2.0);

            CarNavigationAction retaction;
            GetAction(maxact, out retaction);
            action = retaction;
            retval = max;
        }

        ReinforcementLearningAction[] allActions;
        public ReinforcementLearningAction[] GetAllActionsInState(ReinforcementLearningState state)
        {
            if (allActions == null)
            {
                List<CarNavigationAction> list = new List<CarNavigationAction>();
                for (int i = 0; i < LENACTION; ++i)
                {
                    CarNavigationAction action;
                    GetAction(i, out action);
                    list.Add(action);
                }
                allActions = list.ToArray();
            }
            return allActions;
        }

        #endregion

        public void Save(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                using (GZipStream stream = new GZipStream(fs, CompressionMode.Compress))
                {
                    using (BinaryWriter bw = new BinaryWriter(stream))
                    {
                        for (int i = 0; i < LENACTION; ++i)
                        {
                            for (int j = 0; j < LENXY; ++j)
                            {
                                for (int k = 0; k < LENXY; ++k)
                                {
                                    for (int l = 0; l < LENANG; ++l)
                                    {
                                        
                                        bw.Write(value[i,j,k,l]);                                        
                                    }
                                }
                            }
                        }
                    }
                }
            }
    
        }

        public static CarNavigationQStore Load(string filename)
        {
            CarNavigationQStore ret = new CarNavigationQStore();
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    using (GZipStream stream = new GZipStream(fs, CompressionMode.Decompress))
                    {
                        using (BinaryReader br = new BinaryReader(stream))
                        {
                            for (int i = 0; i < LENACTION; ++i)
                            {
                                for (int j = 0; j < LENXY; ++j)
                                {
                                    for (int k = 0; k < LENXY; ++k)
                                    {
                                        for (int l = 0; l < LENANG; ++l)
                                        {
                                            ret.value[i, j, k, l] = br.ReadSingle();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (IOException ex)
            {
            }
            return ret;
        }
    }

    public class CarNavigationEpsilonGreedyPolicy : EpsilonGreedyPolicy
    {
        public override float Epsilon
        {
            get { return 0.5f; }
        }
    }

    public class CarNavigationSoftmaxPolicy : SoftmaxPolicy
    {
        public override float Temperature
        {
            get { return 10f; }
        }
    }

    class CarNavigationRLController : ReinforcementLearningController
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
            return new CarNavigationQStore(); 
        }    
    }
}
