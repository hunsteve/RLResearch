using System;
using System.Collections.Generic;
using System.Text;
using ReinforcementLearning;

namespace AcrobotEnvironment
{
    public class AcrobotEnvironmentSimulator : RLEnvironmentSimulator
    {
        double m1 = 1;
        double m2 = 0.1;
        double l1 = 1;
        double l2 = 1;
        double lc1;
        double lc2;
        double I1;
        double I2;
        double g = 10;

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
                       
        public AcrobotEnvironmentSimulator()
        {
            calcOtherParams();
        }

        public AcrobotEnvironmentSimulator(double m1, double m2, double l1, double l2, double g)
        {
            this.m1 = m1;
            this.m2 = m2;
            this.l1 = l1;
            this.l2 = l2;
            this.g = g;
            calcOtherParams();
        }

        private void calcOtherParams()
        {
            I1 = m1 * l1 * l1 / 12;
            I2 = m1 * l2 * l2 / 12;
            lc1 = l1 / 2;
            lc2 = l2 / 2;
        }
        
        public void Simulate(double[] states, double[] actions, double dt, out double[] newStates, out double reward)
        {
            double torque = actions[0];
            q1 = states[0];
            q2 = states[1];
            dq1 = states[2];            
            dq2 = states[3];

            double d11, d22, d12, d21;
            double h1, h2, fi1, fi2;
            double ddq1;
            double ddq2;

            double c1, c2, c3, c4, c5;

            c1 = m1 * lc1 * lc1 + m2 * l1 * l1 + I1;
            c2 = m2 * lc2 * lc2 + I2;
            c3 = m2 * l1 * lc2;
            c4 = m1 * lc1 + m2 * l1;
            c5 = m2 * lc2;

            d11 = c1 + c2 + 2 * c3 * Math.Cos(q2);
            d21 = d12 = c2 + c3 * Math.Cos(q2);
            d22 = c2;

            h1 = c3 * (-2 * dq1 * dq2 - dq2 * dq2) * Math.Sin(q2);
            h2 = c3 * (dq1 * dq1) * Math.Sin(q2);

            fi1 = c4 * g * Math.Cos(q1) + c5 * g * Math.Cos(q1 + q2);
            fi2 = c5 * g * Math.Cos(q1 + q2);

            double delta = d11 * d22 - d12 * d21;
            ddq1 = (1 / delta) * (d22 * (-h1 - fi1) - d12 * (torque - h2 - fi2));
            ddq2 = (1 / delta) * (-d21 * (-h1 - fi1) + d11 * (torque - h2 - fi2));

            dq1 *= 0.98;
            dq2 *= 0.98;

            q1 += dq1 * dt;
            q2 += dq2 * dt;
            dq1 += ddq1 * dt;
            dq2 += ddq2 * dt;

            if (q2 > Math.PI * 0.8)
            {
                q2 = Math.PI * 0.8;
                dq2 = 0;
            }
            if (q2 < -Math.PI * 0.8)
            {
                q2 = -Math.PI * 0.8;
                dq2 = 0;
            }

            if (q1 < -Math.PI) q1 += 2 * Math.PI;
            if (q1 > Math.PI) q1 -= 2 * Math.PI;

            newStates = new double[] { q1, q2, dq1, dq2 };
            //reward = /*Math.Sin(q1) + Math.Sin(q1 + q2) + 100 **/ Math.Round(Math.Pow(Math.Max(0,0.5 * Math.Sin(q1) + 0.5 * Math.Sin(q1 + q2)),30),5);
            //reward = Math.Sin(q1) +Math.Sin(q1 + q2);
            //reward = Math.Exp(-(1 - (Math.Sin(q1) + Math.Sin(q1 + q2) + 2) / 4) - dq1 * dq1 * 0.001 - dq2 * dq2 * 0.001) - 1;
            reward = Math.Sin(q1);
        }
    }
}
