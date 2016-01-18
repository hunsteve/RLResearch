using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleRLTest
{
    class Action
    {
        public int[] Actions
        {
            get;
            set;
        }

        public Action(int[] data)
        {
            Actions = data;
        }            
    }

    class State
    {
        public int[] States
        {
            get;
            set;
        }

        public State(int[] data)
        {
            States = data;
        }
    }

    class StateActionReward
    {
        public State state;
        public Action action;
        public float reward;

        public StateActionReward(State state, float reward, Action action)
        {
            this.state = state;
            this.reward = reward;
            this.action = action;
        }
    }




    class RLRobotController
    {
        const float discount = 1f;
        float epsilon = 0.75f;
        const float alpha = 1f;      
        Random r;
                   
        int actionDimension;
        bool[] fixArray;
        Action prevAction;
        State prevState;

        public RLRobotController()
        {            
            r = new Random();
        }


        #region RobotController Members

        public void Initialize(Workspace ws)
        {            
            actionDimension = 1;            

            State testState = CalcStateFromWorkspace(ws);
            fixArray = new bool[testState.States.Length + actionDimension];
            for (int i = 0; i < fixArray.Length; ++i)
            {
                if (i < testState.States.Length) fixArray[i] = true;
                else fixArray[i] = false;
            }           
        }

        public int[] Step(Workspace ws)
        {
            Action action;
            State state = CalcStateFromWorkspace(ws);

           
            float reward = GetRewardFromWorkspace(ws);
            action = EpsilonGreedy(state, epsilon, ws);

            if ((prevState != null) && (prevAction != null))
            {
                int[] maxp;                    
                float Qtp1max = float.MinValue;


                //GetSubspaceGlobalMax
                for (int i = 0; i < 4; ++i)
                {
                    if (Qtp1max < ws.uti[i,state.States[1],state.States[0]]) {
                        Qtp1max = ws.uti[i,state.States[1],state.States[0]];
                        maxp = new int[] {i};
                    }
                }

                float Qt = ws.uti[prevAction.Actions[0], prevState.States[1], prevState.States[0]];

                float deltaQ = alpha * (reward + discount * Qtp1max - Qt);

                ws.uti[prevAction.Actions[0], prevState.States[1], prevState.States[0]] += deltaQ;
            }

            prevAction = action;
            prevState = state;
            //sarList.Add(new StateActionReward(state, reward, action));
           

            return action.Actions;
        }        

    
        public void EpisodeBegin()
        {
            prevAction = null;
            prevState = null;
            //sarList = new List<StateActionReward>();
        }

        public void EpisodeEnd()
        {                    
        }

        #endregion


        private Action EpsilonGreedy(State state, float epsilon, Workspace ws)
        {
            if (r.NextDouble() < epsilon)
            {
                return GetBestAction(state,ws);
            }
            else
            {
                int[] data = new int[actionDimension];
                for (int i = 0; i < actionDimension; ++i)
                {
                    data[i] = r.Next(4);
                }                
                return new Action(data);
            }
        }

        private Action GetBestAction(State state, Workspace ws)
        {
            int[] maxp = new int[] { 0 };
            float Qtp1max = float.MinValue;


            //GetSubspaceGlobalMax
            for (int i = 0; i < 4; ++i)
            {
                if (Qtp1max < ws.uti[i, state.States[1], state.States[0]])
                {
                    Qtp1max = ws.uti[i, state.States[1], state.States[0]];
                    maxp = new int[] { i };
                }
            }

            return ActionFromInput(maxp);
        }

        private State CalcStateFromWorkspace(Workspace ws)
        {            
            List<int> data = new List<int>();

            data.Add(ws.pos.X);
            data.Add(ws.pos.Y);           
           
            return new State(data.ToArray());
        }

        private float GetRewardFromWorkspace(Workspace ws)
        {
            if (ws.map[ws.pos.Y, ws.pos.X] == 3)
            {
                return 10;
            }
            else
            {
                return -0.1f;
            }
            
        }


        private float[] InputFromStateAction(State state, Action action)
        {            
            float[] input = new float[state.States.Length + actionDimension];
            for (int i = 0; i < fixArray.Length; ++i)
            {
                if (i < state.States.Length) input[i] = state.States[i];
                else
                {
                    if (action != null)
                    {
                        input[i] = action.Actions[i - state.States.Length];
                    }                    
                }
            }
            return input;
        }


        private Action ActionFromInput(int[] maxp)
        {           
            int[] act = new int[actionDimension];
            int start = maxp.Length - actionDimension;
            for (int i = start; i < maxp.Length; ++i)
            {
                act[i - start] = maxp[i];
            }
            return new Action(act);
        }

    }
}
