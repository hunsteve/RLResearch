using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IGMNLib;
using Matrix;
using System.Drawing.Imaging;
using System.IO;

namespace I2MTC_results
{
    public partial class Form1 : Form
    {
        const double l = 1;
        const double m = 1;
        const double g = 1;
        const double dT = 0.2;
        const double M_abs = 0.5;
        const double mu = 0.03;        

        IGMN igmn1, igmn2;
        Random r;


        double w = 0;
        double a = 0;
        double q_act = 0;
        double M = 0;

        double a_old = 0;
        double w_old = 0;
        double M_old = 0;
        double q_old = 0;
        int count = 0;
        int traincount = 0;

        const int maxepochcount = 500;
        const int maxtraincount = 20;
        double[] rewardsAvg = new double[maxepochcount];
        double[] rewardsMin = new double[maxepochcount];
        double[] rewardsMax = new double[maxepochcount];



        public Form1()
        {
            InitializeComponent();
            r = new Random();

            for (int k = 0; k < maxepochcount; ++k)
            {
                rewardsMin[k] = double.MaxValue;
                rewardsMax[k] = double.MinValue;
                rewardsAvg[k] = 0;
            }

            igmn1 = new IGMN(new Vector(new double[] { 1, 1, 1 }));
            igmn2 = new IGMN(new Vector(new double[] { 1, 1, 1 }));
            timer1.Start();
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            double zoom = 50;

            Graphics g = e.Graphics;
            g.TranslateTransform((float)(4 * zoom), (float)(4 * zoom));
            g.Clear(Color.White);

            igmn1.DrawGaussians(g, 0, 1, zoom);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (count >= maxepochcount)
            {
                igmn1 = new IGMN(new Vector(new double[] { 1, 1, 1 }));
                igmn2 = new IGMN(new Vector(new double[] { 1, 1, 1 }));
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

            double sumreward = 0;

            a = (2 * r.NextDouble() - 1) * Math.PI;
            w = (2 * r.NextDouble() - 1) * 2 * Math.PI;
            a_old = a;
            w_old = w;

            for (int i = 0; i < 200; ++i)
            {
                //q values
                double q1 = igmn1.Recall(new Vector(new double[] { a, w }));
                double q2 = igmn2.Recall(new Vector(new double[] { a, w }));

                int action = 0;

                if (q1 > q2)
                {
                    action = -1;
                }
                else
                {
                    action = 1;
                }

                //e greedy
                if (r.NextDouble() > 0.7)
                {
                    action = r.Next(2) * 2 - 1;
                }

                if (action < 0) q_act = q1;
                else q_act = q2;

                //reward
                double reward = Math.Cos(a);

                sumreward += reward;


                //elozo lepest tanitani 
                double delta_q = (reward + 0.99 * q_act); //Math.Max(q1,q2)

                if (M_old < 0) igmn1.Train(new Vector(new double[] { a_old, w_old, delta_q }));
                else igmn2.Train(new Vector(new double[] { a_old, w_old, delta_q }));

                //leptetni
                M = action * M_abs;

                a_old = a;
                w_old = w;
                M_old = M;
                q_old = q_act;

                w += (l * m * g * Math.Sin(a) + M - mu * w) / (l * l * m) * dT;
                a += w * dT;

                if (a < -Math.PI) a += 2 * Math.PI;
                if (a > Math.PI) a -= 2 * Math.PI;
            }

            Console.Out.WriteLine(sumreward);            
            rewardsMin[count] = Math.Min(sumreward, rewardsMin[count]);
            rewardsMax[count] = Math.Max(sumreward, rewardsMax[count]);
            rewardsAvg[count] += sumreward / maxtraincount;
            count++;
            //Invalidate();
        }


        private void saveBitmap()
        {
            int pix = 150;

            Bitmap bm = new Bitmap(pix, pix);
            BitmapData bmd = bm.LockBits(new Rectangle(0, 0, pix, pix), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();

            unsafe
            {
                uint* p = (uint*)bmd.Scan0.ToPointer();

                for (int x = 0; x < pix; ++x)
                {
                    for (int y = 0; y < pix; ++y)
                    {
                        double q1 = igmn1.Recall(new Vector(new double[] { (x - pix / 2) / (double)pix * 3.1416, (y - pix / 2) / (double)pix * 6.28 }));
                        double q2 = igmn2.Recall(new Vector(new double[] { (x - pix / 2) / (double)pix * 3.1416, (y - pix / 2) / (double)pix * 6.28 }));

                        //sb1.Append(q1.ToString());
                        //sb1.Append(", ");

                        //sb2.Append(q2.ToString());
                        //sb2.Append(", ");

                        if (q1 > q2) p[pix * y + x] = 0xFFFF0000;
                        else p[pix * y + x] = 0xFF0000FF;
                    }
                    sb1.AppendLine("");
                    sb2.AppendLine("");
                }
            }

            bm.UnlockBits(bmd);

            bm.Save("bm.png");

            using (StreamWriter sw = new StreamWriter("q1.txt"))
            {
                sw.Write(sb1.ToString());
            }

            using (StreamWriter sw = new StreamWriter("q2.txt"))
            {
                sw.Write(sb2.ToString());
            }
        }
    }
}
