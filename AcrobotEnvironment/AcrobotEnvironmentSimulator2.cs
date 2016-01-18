using System;
using System.Collections.Generic;
using System.Text;
using ReinforcementLearning;

namespace AcrobotEnvironment
{
    public class AcrobotEnvironmentSimulator2 : RLEnvironmentSimulator
    {
        double m1 = 1;
        double m2 = 1;
        double l1 = 1;
        double l2 = 1;
        double g = 9.81;
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
                       
        public AcrobotEnvironmentSimulator2()
        {
           
        }

        public AcrobotEnvironmentSimulator2(double m1, double m2, double l1, double l2, double g)
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

            double d11, d22, d12;            
            double ddq1;
            double ddq2;
            double fi1, fi2;
            double c1, c2;


            d11 = m1 * l1 * l1 + m2 * (l1 * l1 + l2 * l2 + 2 * l1 * l2 * Math.Cos(q2));
            d12 = m2 * (l2 * l2 + l1 * l2 * Math.Cos(q2));            
            d22 = m2 * l2 * l2;

            c1 = -m2 * l1 * l2 * dq2 * (2 * dq1 + dq2) * Math.Sin(q2);
            c2 = m2 * l1 * l2 * dq1 * dq1 * Math.Sin(q2);

            fi1 = -(m1 * l1 + m2 * l1) * g * Math.Sin(q1) - m2 * l2 * g * Math.Sin(q1 + q2);
            fi2 = -m2 * l2 * g * Math.Sin(q1 + q2);

            double delta = d11 * d22 - d12 * d12;
            ddq1 = (1 / delta) * (-d22 * (c1 + fi1) + d12 * (c2 + fi2) - d22 * (mu1 * dq1) - d12 * (torque + mu2 * dq2));
            ddq2 = (1 / delta) * (-d11 * (c2 + fi2) + d12 * (c1 + fi1) + d11 * (torque - mu2 * dq2) + d12 * (mu1 * dq1));

            
            dq1 += ddq1 * dt;
            dq2 += ddq2 * dt;
            q1 += dq1 * dt;
            q2 += dq2 * dt;

            //if (q2 > Math.PI * 0.99)
            //{
            //    q2 = Math.PI * 0.99;
            //    dq2 = 0;
            //}
            //if (q2 < -Math.PI * 0.99)
            //{
            //    q2 = -Math.PI * 0.99;
            //    dq2 = 0;
            //}

            if (q1 < -Math.PI) q1 += 2 * Math.PI;
            if (q1 > Math.PI) q1 -= 2 * Math.PI;

            if (q2 < -Math.PI) q2 += 2 * Math.PI;
            if (q2 > Math.PI) q2 -= 2 * Math.PI;

            if (dq1 > 500) dq1 = 500;
            if (dq1 < -500) dq1 = -500;
            if (dq2 > 500) dq2 = 500;
            if (dq2 < -500) dq2 = -500;

            newStates = new double[] { q1, q2, dq1, dq2 };
            //reward = /*Math.Sin(q1) + Math.Sin(q1 + q2) + 100 **/ Math.Round(Math.Pow(Math.Max(0,0.5 * Math.Sin(q1) + 0.5 * Math.Sin(q1 + q2)),30),5);
            //reward = Math.Sin(q1) +Math.Sin(q1 + q2);
            //reward = Math.Exp(-(1 - (Math.Cos(q1) + Math.Cos(q1 + q2) + 2) / 4)*2 - dq1 * dq1 - dq2 * dq2) - 0.9;
            reward = Math.Cos(q1) + Math.Cos(q1 + q2);
        }
    }
}
