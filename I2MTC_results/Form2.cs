using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IGMNLib;
using System.IO;
using Matrix;

namespace I2MTC_results
{
    public partial class Form2 : Form
    {
        double m1 = 1;
        double m2 = 0.3;
        double l1 = 1;
        double l2 = 1;
        double lc1;
        double lc2;
        double I1;
        double I2;
        double g = 10;
        double deltaT = 0.05;

        double M_abs = 3;
        double M;

        double q1;
        double q2;
        double dq1;
        double dq2;

        double q1_old;
        double q2_old;
        double dq1_old;
        double dq2_old;
        double actionq_act;
        double actionq_old;
        double M_old;


        IGMN igmn1, igmn2;
        Random r;

        int count = 0;
        int stepcount = 0;
        int traincount = 0;

        const int maxepochcount = 5000;
        const int maxtraincount = 1;
        const int maxstepcount = 200;
        double[] rewardsAvg = new double[maxepochcount];
        double[] rewardsMin = new double[maxepochcount];
        double[] rewardsMax = new double[maxepochcount];

        double sumreward = 0;


        public Form2()
        {
            InitializeComponent();
            r = new Random();
            I1 = m1 * l1 * l1 / 12;
            I2 = m1 * l2 * l2 / 12;
            lc1 = l1 / 2;
            lc2 = l2 / 2;



            for (int k = 0; k < maxepochcount; ++k)
            {
                rewardsMin[k] = double.MaxValue;
                rewardsMax[k] = double.MinValue;
                rewardsAvg[k] = 0;
            }
            stepcount = maxstepcount;
            count = maxepochcount;
            traincount = -1;

            timer1.Start();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (count >= maxepochcount)
            {
                igmn1 = new IGMN(new Vector(new double[] { 1, 1, 1, 1, 10 }));
                igmn2 = new IGMN(new Vector(new double[] { 1, 1, 1, 1, 10 }));
                count = 0;
                traincount++;
                if (traincount >= maxtraincount)
                {
                    timer1.Stop();
                    //saveBitmap();
                    using (StreamWriter sw = new StreamWriter("learningMin.txt"))
                    {
                        foreach (double d in rewardsMin)
                        {
                            sw.Write(d.ToString() + "\r\n");
                        }
                    }
                    using (StreamWriter sw = new StreamWriter("learningMax.txt"))
                    {
                        foreach (double d in rewardsMax)
                        {
                            sw.Write(d.ToString() + "\r\n");
                        }
                    }
                    using (StreamWriter sw = new StreamWriter("learningAvg.txt"))
                    {
                        foreach (double d in rewardsAvg)
                        {
                            sw.Write(d.ToString() + "\r\n");
                        }
                    }
                }

            }

            //if (stepcount >= maxstepcount)
            {
                stepcount = 0;
                Console.Out.WriteLine(sumreward);
                rewardsMin[count] = Math.Min(sumreward, rewardsMin[count]);
                rewardsMax[count] = Math.Max(sumreward, rewardsMax[count]);
                rewardsAvg[count] += sumreward / maxtraincount;
                count++;

                sumreward = 0;

                q1 = -Math.PI / 2;//r.NextDouble()*2*Math.PI-Math.PI;
                dq1 = 0;
                q2 = 0;
                dq2 = 0;            
            }

            for (int i = 0; i < 200; ++i)
            {
                //q values
                double actionq1 = igmn1.Recall(new Vector(new double[] { q1, q2, dq1, dq2 }));
                double actionq2 = igmn2.Recall(new Vector(new double[] { q1, q2, dq1, dq2 }));

                int action = 0;

                if (actionq1 > actionq2)
                {
                    action = -1;
                }
                else
                {
                    action = 1;
                }

                //e greedy
                if (r.NextDouble() > 0.9)
                {
                    action = r.Next(2) * 2 - 1;
                }

                if (action < 0) actionq_act = actionq1;
                else actionq_act = actionq2;

                //reward
                double reward = /*Math.Sin(q1) + Math.Sin(q1 + q2) + 100 **/ Math.Round(Math.Pow(Math.Max(0,0.5 * Math.Sin(q1) + 0.5 * Math.Sin(q1 + q2)),30),5);

                sumreward += reward;


                //elozo lepest tanitani 
                double delta_q = (reward + 0.99 * actionq_act); //Math.Max(q1,q2)

                if (M_old < 0) igmn1.Train(new Vector(new double[] { q1_old, q2_old, dq1_old, dq2_old, delta_q }));
                else igmn2.Train(new Vector(new double[] { q1_old, q2_old, dq1_old, dq2_old, delta_q }));

                //leptetni
                M = action * M_abs;

                q1_old = q1;
                q2_old = q2;
                dq1_old = dq1;
                dq2_old = dq2;

                M_old = M;
                actionq_old = actionq_act;

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
                ddq1 = (1 / delta) * (d22 * (-h1 - fi1) - d12 * (M - h2 - fi2));
                ddq2 = (1 / delta) * (-d21 * (-h1 - fi1) + d11 * (M - h2 - fi2));

                dq1 *= 0.97;
                dq2 *= 0.97;

                q1 += dq1 * deltaT;
                q2 += dq2 * deltaT;
                dq1 += ddq1 * deltaT;
                dq2 += ddq2 * deltaT;

                if (q1 < -Math.PI) q1 += 2 * Math.PI;
                if (q1 > Math.PI) q1 -= 2 * Math.PI;
                if (q2 < -Math.PI) q2 += 2 * Math.PI;
                if (q2 > Math.PI) q2 -= 2 * Math.PI;

            }

            //stepcount++;
            Invalidate();
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            float zx = 200;
            float zy = 200;
            float z = 100;

            float e1x = zx + (float)(z * l1 * Math.Cos(q1));
            float e1y = zy - (float)(z * l1 * Math.Sin(q1));

            float e2x = e1x + (float)(z * l2 * Math.Cos(q1 + q2));
            float e2y = e1y - (float)(z * l2 * Math.Sin(q1 + q2));

            Graphics g = e.Graphics;
            g.Clear(Color.White);
            Pen p = new Pen(Color.Black);

            g.DrawLine(p, zx, zy, e1x, e1y);
            g.DrawLine(p, e1x, e1y, e2x, e2y);

            float x=10;
            float dy = 200;
            float y=dy - (float)rewardsAvg[0];
            float d = 500.0f/count;
            
            Pen p2=  new Pen(Color.Red);
            for (int i = 0; i < count; ++i)
            {
                float x2 = x + d;
                float y2 = dy - (float)rewardsAvg[i];
                g.DrawLine(p2, x, y, x2, y2);
                x = x2;
                y = y2;
            }

        }
    }
}
