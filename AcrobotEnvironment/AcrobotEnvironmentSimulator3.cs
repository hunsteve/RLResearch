using System;
using System.Collections.Generic;
using System.Text;
using ReinforcementLearning;

namespace AcrobotEnvironment
{
    public class AcrobotEnvironmentSimulator3 : RLEnvironmentSimulator
    {
        double m1 = 1;
        double m2 = 1;
        double l1 = 1;
        double l2 = 1;
        double lc1 = 0.5;
        double lc2 = 0.5;
        double I1 = 1;
        double I2 = 1;
        double g = 9.8;
        double mu1 = 0.01;
        double mu2 = 0.01;


        double q1;
        double q2;
        double dq1;
        double dq2;

        public double Angle1
        {
            get
            {
                return q1;
            }
        }

        public double Angle2
        {
            get
            {
                return q2;
            }
        }

        public double AngularVelocity1
        {
            get
            {
                return dq1;
            }
        }

        public double AngularVelocity2
        {
            get
            {
                return dq2;
            }
        }

        public double Mass1
        {
            get
            {
                return m1;
            }
        }

        public double Mass2
        {
            get
            {
                return m2;
            }
        }

        public double Length1
        {
            get
            {
                return l1;
            }
        }

        public double Length2
        {
            get
            {
                return l2;
            }
        }
                       
        public AcrobotEnvironmentSimulator3()
        {
           
        }

        public AcrobotEnvironmentSimulator3(double m1, double m2, double l1, double l2, double g)
        {
            this.m1 = m1;
            this.m2 = m2;
            this.l1 = l1;
            this.l2 = l2;
            this.g = g;          
        }
       
        
        public void Simulate(double[] states, double[] actions, double dt, out double[] newStates, out double reward)
        {
            double torque = actions[0];
            q1 = states[0];
            q2 = states[1];
            dq1 = states[2];            
            dq2 = states[3];


            double fi2 = m2 * l2 * g * Math.Cos(q1 + q2 - Math.PI / 2);
            double fi1 = -m2 * l1 * lc2 * dq2 * dq2 * Math.Sin(q2) - 2 * m2 * l1 * lc2 * dq2 * dq1 * Math.Sin(q2) + (m1 * lc1 + m2 * l1) * g * Math.Cos(q1 - Math.PI / 2) + fi2;
            double d2 = m2 * (lc2 * lc2 + l1 * lc2 * Math.Cos(q2)) + I2;
            double d1 = m1 * lc1 * lc1 + m2 * (l1 * l1 + lc2 * lc2 + 2 * l1 * lc2 * Math.Cos(q2)) + I1 + I2;
            double ddq2 = (torque + d2 / d1 * fi1 - m2 * l1 * lc2 * dq1 * dq1 * Math.Sin(q2) - fi2) / (m2 * lc2 * lc2 + I2 - d2 * d2 / d1);
            double ddq1 = (d2 * ddq2 + fi1) / (-d1);

            dq1 += ddq1 * dt;
            dq2 += ddq2 * dt;

            q1 += dq1 * dt;
            q2 += dq2 * dt;
            
            newStates = new double[] { q1, q2, dq1, dq2 };
            //reward = /*Math.Sin(q1) + Math.Sin(q1 + q2) + 100 **/ Math.Round(Math.Pow(Math.Max(0,0.5 * Math.Sin(q1) + 0.5 * Math.Sin(q1 + q2)),30),5);
            //reward = Math.Sin(q1) +Math.Sin(q1 + q2);
            //reward = Math.Exp(-(1 - (Math.Cos(q1) + Math.Cos(q1 + q2) + 2) / 4)*2 - dq1 * dq1 - dq2 * dq2) - 0.9;
            reward = Math.Cos(q1) + Math.Cos(q1 + q2);
        }
    }
}
