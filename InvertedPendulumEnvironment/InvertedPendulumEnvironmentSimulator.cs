using System;
using System.Collections.Generic;
using System.Text;
using ReinforcementLearning;

namespace InvertedPendulumEnvironment
{
    public class InvertedPendulumEnvironmentSimulator : RLEnvironmentSimulator
    {
        double l = 1;
        double m = 1;
        double g = 1;               
        double mu = 0.03;

        double w;
        double a;

        public double Length
        {
            get
            {
                return l;
            }
        }

        public double Mass
        {
            get
            {
                return m;
            }
        }

        public double Angle
        {
            get
            {
                return a;
            }
        }

        public double AngularVelocity
        {
            get
            {
                return w;
            }
        }

        public InvertedPendulumEnvironmentSimulator()
        {
        }

        public InvertedPendulumEnvironmentSimulator(double l, double m, double g, double mu)
        {
            this.l = l;
            this.g = g;
            this.m = m;
            this.mu = mu;
        }

        public void Simulate(double[] states, double[] actions, double dt, out double[] newStates, out double reward)
        {            
            a = states[0];
            w = states[1];
            double M = actions[0];

            w += (l * m * g * Math.Sin(a) + M - mu * w) / (l * l * m) * dt;
            a += w * dt;

            if (a < -Math.PI) a += 2 * Math.PI;
            if (a > Math.PI) a -= 2 * Math.PI;

            newStates = new double[] { a, w };
            reward = Math.Cos(a);

            //if ((Math.Abs(a) < 0.1) && (Math.Abs(w) < 0.1)) reward = 1;
            //else reward = 0;
        }
    }
}
