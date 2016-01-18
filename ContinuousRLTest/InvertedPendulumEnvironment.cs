using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ContinuousRLTest
{
    public class InvertedPendulumEnvironment : ReinforcementLearningEnvironment
    {
        public const double l = 1;
        public const double m = 1;
        public const double g = 1;
        public const double dT = 0.2;
        public const double M_abs = 0.5;
        public const double mu = 0.03;
        public double a;
        public double w;

        private Random r = new Random();

        public void Reset()
        {
            a = (2 * r.NextDouble() - 1) * Math.PI;
            w = (2 * r.NextDouble() - 1) * Math.PI * 2;
        }

        public void Step(double action)
        {
            double M = action * M_abs;

            w += (l*m*g*Math.Sin(a) + M - mu * w)/(l*l*m) * dT;            
            a += w * dT;

            if (a < -Math.PI) a += 2 * Math.PI;
            if (a > Math.PI) a -= 2 * Math.PI;

            //if (a > Math.PI / 2) { a = Math.PI / 2; w = 0; }
            //if (a > 2 * Math.PI) { a = 2 * Math.PI; w = 0; }
        }

        public ReinforcementLearningState State()
        {
            InvertedPendulumState state = new InvertedPendulumState(a, w);

            return state;
        }

        public float Reward()
        {
            //float reward = -0.00001f;
            //if ((Math.Cos(a) < -0.99) && (Math.Abs(w) < 0.01f))
            //{
            //    reward = 1;
            //}
            //return reward;

            return (float)Math.Cos(a);
        }

        internal void SetState(InvertedPendulumState state)
        {
            this.a = state.a;
            this.w = state.w;
        }
    }
}
