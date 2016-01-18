using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Matrix;


namespace ContinuousRLTest
{
    public class InvertedPendulumESIGMNQStore : ReinforcementLearningQStore
    {

        public IGMN igmn;
        Random r = new Random();

        public InvertedPendulumESIGMNQStore()
        {
            igmn = new IGMN(3, 1, new Vector(new double[] {12, 6, 0.4, 100} ));
        }


        public void ReplaceValue(InvertedPendulumState state, InvertedPendulumAction2 action, float value)
        {
            //TODO! delta??*
            igmn.Train(new Vector(new double[] { state.a, state.w, action.action, value }));
        }


        #region ReinforcementLearningQStore Members
    
        public float Evaluate(ReinforcementLearningState state, ReinforcementLearningAction action)
        {
            return (float)igmn.Recall(new Vector(new double[] { ((InvertedPendulumState)state).a, ((InvertedPendulumState)state).w, ((InvertedPendulumAction2)action).action, 0 })).Elements[0];
        }

        public void GetBestActionAndUtilityForState(ReinforcementLearningState state, out ReinforcementLearningAction action, out float value)
        {
            
            InvertedPendulumAction2 bestaction = null;
            float bestq = float.MinValue;
            for (int i = 0; i < 10; ++i)
            {
                InvertedPendulumAction2 act = new InvertedPendulumAction2((float)r.NextDouble() * 2 - 1);
                float q = Evaluate(state, act);
                if (q > bestq)
                {
                    bestq = q;
                    bestaction = act;
                }
            }

            action = bestaction;
            value = bestq;   
        }

        public void ModifyValue(ReinforcementLearningState state, ReinforcementLearningAction action, float delta)
        {
            throw new NotImplementedException();
        }

        #endregion

    }


    public class InvertedPendulumESIGMNPolicy : ReinforcementLearningPolicy
    {
        const double EPSILON = 0.75;
        public override ReinforcementLearningAction ActionForState(ReinforcementLearningState state, ReinforcementLearningQStore qFunction)
        {
            Random r = new Random();
            if (r.NextDouble() > EPSILON)
            {
                return new InvertedPendulumAction2((float)r.NextDouble());
            }
            else
            {                
                ReinforcementLearningAction ret;
                float value;
                qFunction.GetBestActionAndUtilityForState(state, out ret, out value);
                return ret;
            }
            
        }


        public override void ActionProbabilities(ReinforcementLearningState state, ReinforcementLearningQStore qFunction, out ReinforcementLearningAction[] actions, out float[] probabilities)
        {
            throw new NotImplementedException();
        }
    }

    public class InvertedPendulumESIGMNController : ReinforcementLearningController
    {

        protected override float Discount
        {
            get { return 0.99f; }
        }

        double alpha = 0.75f;

        protected override float Alpha
        {
            get { return (float)alpha; }
        }

        protected override ReinforcementLearningQStore CreateQStore(ReinforcementLearningEnvironment ws)
        {
            return new InvertedPendulumESIGMNQStore();
        }

        public override ReinforcementLearningAction Step(ReinforcementLearningEnvironment env, ReinforcementLearningPolicy policy)
        {

            ReinforcementLearningState state = env.State();

            float reward = env.Reward();

            ReinforcementLearningAction action = policy.ActionForState(state, qFunction);

            if ((prevState != null) && (prevAction != null))
            {
                ReinforcementLearningAction bestAction;
                float Qtp1max;
                qFunction.GetBestActionAndUtilityForState(state, out bestAction, out Qtp1max);

                float Qt = qFunction.Evaluate(prevState, prevAction);

                float newQ = Alpha * (reward + Discount * Qtp1max - Qt) + Qt;

                InvertedPendulumESIGMNQStore qs = ((InvertedPendulumESIGMNQStore)qFunction);

                qs.ReplaceValue((InvertedPendulumState)prevState, (InvertedPendulumAction2)prevAction, newQ);
            }

            prevAction = action;
            prevState = state;

            return action;
        }     
    }
}
