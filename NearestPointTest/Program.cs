using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Threading;
using System.Globalization;


namespace NearestPointTest
{
    static class Program
    {
        const double distance = 0.03;        
        const double l = 1;
        const double m = 1;
        const double g = 1;
        const double dT = 0.2;
        const double M_abs = 0.5;
        const double mu = 0.03;
        const int pix = 150;

        private static StoragePoint ToCoord(double a, double w, double action)
        {
            return new StoragePoint(new double[] { a / 2 / Math.PI + 0.5, w / (4 * Math.PI) + 0.5, action / 2 / M_abs + 0.5 });
        }

        public static double InferUtility(Storage s, StoragePoint sp)
        {
            double sum = 0;
            double sum2 = 0;
            List<StoragePoint> list = s.PointsNear(sp, distance * 2);
            foreach (StoragePoint spp in list)
            {
                double q = spp.value;
                double sigma = 3 * distance;
                double relevance = 1;
                //double relevance = 1 / Math.Pow(2*Math.PI*sigma, spp.coord.Length/2) * Math.Exp(-0.5 * spp.DistanceSquared(sp) / sigma); //egyseg szorasu gauss
                sum += q * relevance;
                sum2 += relevance;
            }

            return (sum2 == 0) ? 0 : sum / sum2;
        }

        
        [STAThread]
        static void Main()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());


            Random r = new Random();
            Storage s = null;

            double[] rewardMins = new double[1000];
            double[] rewardMaxs = new double[1000];
            double[] rewardAvgs = new double[1000];

            for(int k=0; k<rewardMaxs.Length; ++k)
            {
                rewardMins[k] = double.MaxValue;
                rewardMaxs[k] = double.MinValue;
                rewardAvgs[k] = 0;
            }

            for (int k = 0; k < 1; ++k)//20
            {
                s = new Storage(3);
            
                int count = 0;

                double w = 0;
                double a = 0;
                double q_act = 0;
                double M = 0;

                double a_old = 0;
                double w_old = 0;
                double M_old = 0;
                double q_old = 0;

                for (int j = 0; j < 580; ++j)
                {
                    double sumreward = 0;

                    a = (2 * r.NextDouble() - 1) * Math.PI;
                    w = (2 * r.NextDouble() - 1) * 2 * Math.PI;
                    a_old = a;
                    w_old = w;

                    for (int i = 0; i < 200; ++i)
                    {
                        //q values
                        double q1 = InferUtility(s, ToCoord(a, w, -M_abs));
                        double q2 = InferUtility(s, ToCoord(a, w, M_abs));

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
                        double reward = Math.Cos(a_old);

                        sumreward += reward;


                        //elozo lepest tanitani 
                        double delta_q = (reward + 0.99 * q_act); //Math.Max(q1,q2)


                        List<StoragePoint> list = s.PointsNear(ToCoord(a_old, w_old, M_old), distance);
                        if (list.Count == 0)
                        {
                            StoragePoint sp = ToCoord(a_old, w_old, M_old);
                            sp.Refine(delta_q);
                            s.AddPoint(sp);
                            ++count;
                        }
                        else
                        {
                            foreach (StoragePoint sp in list)
                            {
                                sp.Refine(delta_q);
                            }
                        }


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
                    rewardMins[j] = Math.Min(sumreward,rewardMins[j]);
                    rewardMaxs[j] = Math.Max(sumreward,rewardMaxs[j]);
                    rewardAvgs[j] += sumreward / 20;
                }
            }

            using (StreamWriter sw = new StreamWriter("learningMins.txt"))
            {
                foreach (double d in rewardMins)
                {
                    sw.Write(d.ToString() + "\r\n");
                }
            }
            using (StreamWriter sw = new StreamWriter("learningMaxs.txt"))
            {
                foreach (double d in rewardMaxs)
                {
                    sw.Write(d.ToString() + "\r\n");
                }
            }
            using (StreamWriter sw = new StreamWriter("learningAvgs.txt"))
            {
                foreach (double d in rewardAvgs)
                {
                    sw.Write(d.ToString() + "\r\n");
                }
            }

            //Console.Out.WriteLine(count);
            
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
                        double q1 = InferUtility(s, new StoragePoint(new double[] { x / (double)pix, y / (double)pix, 0 }));
                        double q2 = InferUtility(s, new StoragePoint(new double[] { x / (double)pix, y / (double)pix, 1 }));
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

