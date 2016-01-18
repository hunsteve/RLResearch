using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContinuousRLTest
{
    public interface ReinforcementLearningEnvironment
    {
        ReinforcementLearningState State();
        float Reward();        
    }

    public interface ReinforcementLearningState
    {
        ReinforcementLearningAction[] GetActions();
    }

    public interface ReinforcementLearningAction
    {
        bool Equals(ReinforcementLearningAction obj);
    }

    public interface ReinforcementLearningQStore
    {
        float Evaluate(ReinforcementLearningState state, ReinforcementLearningAction action);
        void ModifyValue(ReinforcementLearningState state, ReinforcementLearningAction action, float delta);
        void GetBestActionAndUtilityForState(ReinforcementLearningState state, out ReinforcementLearningAction action, out float value);        
    }

    public abstract class ReinforcementLearningPolicy
    {
        protected Random r = new Random();

        public virtual ReinforcementLearningAction ActionForState(ReinforcementLearningState state, ReinforcementLearningQStore qFunction)
        {
            ReinforcementLearningAction[] actions;
            float[] probs;
            ActionProbabilities(state, qFunction, out actions, out probs);

            float rand = (float)r.NextDouble();

            int i = 0;
            float sum = 0;
            do
            {
                sum += probs[i]; ++i;
            }
            while ((i < actions.Length) && (sum < rand));

            return actions[i - 1];
        }

        public abstract void ActionProbabilities(ReinforcementLearningState state, ReinforcementLearningQStore qFunction, out ReinforcementLearningAction[] actions, out float[] probabilities);

    }
   
    public abstract class EpsilonGreedyPolicy : ReinforcementLearningPolicy
    {
        public abstract float Epsilon
        {
            get;
        }

        #region ReinforcementLearningPolicy Members

        public override void ActionProbabilities(ReinforcementLearningState state, ReinforcementLearningQStore qFunction, out ReinforcementLearningAction[] actions, out float[] probabilities)
        {
            ReinforcementLearningAction action;
            float utility;

            actions = state.GetActions();
            probabilities = new float[actions.Length];

            qFunction.GetBestActionAndUtilityForState(state, out action, out utility);

            for (int i = 0; i < actions.Length; ++i)
            {
                if (actions[i].Equals(action))
                {
                    probabilities[i] = Epsilon;
                }
                else probabilities[i] = (1 - Epsilon) / (actions.Length - 1);
            }            
        }

        #endregion
    }

    public class BestActionPolicy : EpsilonGreedyPolicy
    {
        #region ReinforcementLearningPolicy Members

        public new ReinforcementLearningAction ActionForState(ReinforcementLearningState state, ReinforcementLearningQStore qFunction)
        {
            ReinforcementLearningAction action;
            float utility;
            qFunction.GetBestActionAndUtilityForState(state, out action, out utility);
            return action;
        }

        #endregion

        public override float Epsilon
        {
            get { return 1; }
        }
    }


    public abstract class SoftmaxPolicy : ReinforcementLearningPolicy
    {
        public abstract float Temperature
        {
            get;
        }

        #region ReinforcementLearningPolicy Members

        public override void ActionProbabilities(ReinforcementLearningState state, ReinforcementLearningQStore qFunction, out ReinforcementLearningAction[] actions, out float[] probabilities)
        {
            actions = state.GetActions();
            probabilities = new float[actions.Length];
            float maxq = float.MinValue;
            for(int i=0; i<actions.Length; ++i)
            {
                float q = qFunction.Evaluate(state, actions[i]);
                probabilities[i] = q;
                if (q > maxq) maxq = q;
            }

            float sum = 0;

            for (int i = 0; i < actions.Length; ++i)
            {
                probabilities[i] = (float)Math.Exp((probabilities[i] - maxq) / Temperature);
                sum += probabilities[i];                
            }

            for (int i = 0; i < actions.Length; ++i)
            {
                probabilities[i] /= sum;                
            }
        }
        
        #endregion
    }
 

    public abstract class ReinforcementLearningController
    {

        protected abstract float Discount
        {
            get;
        }

        protected abstract float Alpha
        {
            get;
        }
        
        protected Random r = new Random();

        protected ReinforcementLearningAction prevAction;
        protected ReinforcementLearningState prevState;

        public ReinforcementLearningQStore qFunction;

        public void Initialize(ReinforcementLearningEnvironment env)
        {
            qFunction = CreateQStore(env);
        }

        public virtual ReinforcementLearningAction Step(ReinforcementLearningEnvironment env, ReinforcementLearningPolicy policy)
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

                float deltaQ = Alpha * (reward + Discount * Qtp1max - Qt);

                if (float.IsNaN(deltaQ)) throw new Exception();

                qFunction.ModifyValue(prevState, prevAction, deltaQ);
            }

            prevAction = action;
            prevState = state;
           
            return action;
        }        


        public void EpisodeBegin()
        {
            prevAction = null;
            prevState = null;            
        }

        public void EpisodeEnd()
        {                    
        }

        protected abstract ReinforcementLearningQStore CreateQStore(ReinforcementLearningEnvironment ws);
    }
}
