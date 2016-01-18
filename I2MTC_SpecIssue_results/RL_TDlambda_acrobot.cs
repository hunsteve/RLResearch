using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IGMNLib;
using System.Threading;
using TablularApproximator;
using InvertedPendulumEnvironment;
using Matrix;
using System.Drawing.Imaging;
using AcrobotEnvironment;

namespace I2MTC_SpecIssue_results
{
    public partial class RL_TDlambda_acrobot : Form
    {
        Thread trainingThread;

        IGMN igmn1, igmn2;
        Random r;
        bool isRunning;

        TabularApproximator tab1, tab2;
        double dT = 0.02;

        AcrobotEnvironmentSimulator2 sim;

        Bitmap bm;


        public RL_TDlambda_acrobot()
        {
            InitializeComponent();
            sim = new AcrobotEnvironmentSimulator2();

            igmn1 = new IGMN(new Vector(new double[] { 1, 1, 1, 1, 1 }));
            igmn2 = new IGMN(new Vector(new double[] { 1, 1, 1, 1, 1 }));

            tab1 = new TabularApproximator(new int[] { 50, 50, 50, 50 }, new double[] { -Math.PI, -Math.PI, -3 * Math.PI, -3 * Math.PI }, new double[] { Math.PI, Math.PI, 3 * Math.PI, 3 * Math.PI });
            tab2 = new TabularApproximator(new int[] { 50, 50, 50, 50 }, new double[] { -Math.PI, -Math.PI, -3 * Math.PI, -3 * Math.PI }, new double[] { Math.PI, Math.PI, 3 * Math.PI, 3 * Math.PI });

            r = new Random();
            isRunning = true;
            trainingThread = new Thread(new ThreadStart(train));
            trainingThread.Start();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lock (this)
            {
                //pictureBox1.Image = Visualize();
                if (bm!= null) pictureBox1.Image = bm;
                Invalidate();
            }            
        }
   
 
        class TDLambda_data
        {
            public double[] state;
            public double action;
            public double Q;
            public double diff;

            public TDLambda_data(double[] state, double action, double Q, double diff)
            {
                this.state = state;
                this.action = action;
                this.Q = Q;
                this.diff = diff;
            }
        }

        void train()
        {
            double[] state;//a, w            
           
            double lambda = 0.9;// 0.9;
            double gamma = 0.99;
            Random r = new Random();

            while (isRunning)
            {
                
                igmn1 = new IGMN(new Vector(new double[] { 1, 1, 1, 1, 1 }));
                igmn2 = new IGMN(new Vector(new double[] { 1, 1, 1, 1, 1 }));
                for (int k = 0; k < 500; ++k)
                {
                    List<TDLambda_data> diffQs = new List<TDLambda_data>();

                    if (r.NextDouble() > 0.01) state = new double[] { (2 * r.NextDouble() - 1) * Math.PI, (2 * r.NextDouble() - 1) * 0.8 * Math.PI, (2 * r.NextDouble() - 1) * 0.5 * Math.PI, (2 * r.NextDouble() - 1) * 0.5 * Math.PI };
                    else state = new double[] { -Math.PI, 0, 0, 0 };
                    //state = new double[] { (2 * r.NextDouble() - 1) * Math.PI, (2 * r.NextDouble() - 1) * 0.8 * Math.PI, 0, 0 };
                    //state = new double[] { 0, 0, 0, 0 };

                    double M;
                    double q_act;
                    double sumreward = 0;
                    GetAction(state, out M, out q_act);
                    for (int i = 0; i < 300; ++i)
                    {
                        double[] state_new;
                        double reward;
                        sim.Simulate(state, new double[] { M }, dT, out state_new, out reward);

                        double M_new;
                        double q_act_new;
                        GetAction(state_new, out M_new, out q_act_new);
                        double delta_q = (reward + gamma * q_act_new - q_act);


                        diffQs.Add(new TDLambda_data(state, M, q_act, 0));
                        for (int j = i; j >= 0; --j)
                        {
                            diffQs[j].diff += Math.Pow(lambda * gamma, i - j) * delta_q;
                        }


                        M = M_new;
                        q_act = q_act_new;
                        state = state_new;

                        sumreward += reward;
                    }

                    //Console.Out.WriteLine(sumreward);


                    //td lambda tanitas
                    for (int i = 0; i < diffQs.Count; ++i)
                    {
                        if (diffQs[i].action < 0)
                        {
                            lock (igmn1)
                            {
                                igmn1.Train(new Vector(new double[] { diffQs[i].state[0], diffQs[i].state[1], diffQs[i].state[2], diffQs[i].state[3], diffQs[i].diff + diffQs[i].Q }));
                            }
                            lock (tab1)
                            {
                                //tab1.Train(new double[] { diffQs[i].state[0], diffQs[i].state[1], diffQs[i].state[2], diffQs[i].state[3] }, diffQs[i].diff + diffQs[i].Q, 0.1);
                            }
                        }
                        else if (diffQs[i].action > 0)
                        {
                            lock (igmn2)
                            {
                                igmn2.Train(new Vector(new double[] { diffQs[i].state[0], diffQs[i].state[1], diffQs[i].state[2], diffQs[i].state[3], diffQs[i].diff + diffQs[i].Q }));
                            }
                            lock (tab2)
                            {
                                //tab2.Train(new double[] { diffQs[i].state[0], diffQs[i].state[1], diffQs[i].state[2], diffQs[i].state[3] }, diffQs[i].diff + diffQs[i].Q, 0.1);
                            }
                        }
                    }

                    bm = VisualizeAcrobot();

                    lock (this)
                    {
                    }
                    //Thread.Sleep(1000);
                }
            }

        }

        int egreedyaction;
        void GetAction(double[] state, out double act, out double q_act)
        {
            GetAction(state, out act, out q_act, false);
        }
        void GetAction(double[] state, out double act, out double q_act, bool isGreedy)
        {
            double M_abs = 30;
            double q1, q2;
            //q values
            lock (igmn1)
            {
                q1 = igmn1.Recall(new Vector(state));
            }
            lock (igmn2)
            {
                q2 = igmn2.Recall(new Vector(state));
            }


            //lock (tab1)
            //{
            //    q1 = tab1.Recall(new double[] { state[0], state[1], state[2], state[3] });
            //}
            //lock (tab2)
            //{
            //    q2 = tab2.Recall(new double[] { state[0], state[1], state[2], state[3] });
            //}


            int action = 0;
            if (q1 > q2) action = -1;
            else if (q1 < q2) action = 1;

            //e greedy
            lock (r)
            {
                if (!isGreedy && r.NextDouble() > 0.75)
                {
                    if (r.NextDouble() > 0)//0.95
                    {
                        egreedyaction = r.Next(2) * 2 - 1;
                    }
                    action = egreedyaction;
                }
            }

            if (action < 0) q_act = q1;
            else q_act = q2;

            act = action * M_abs;
        }




        protected Bitmap Visualize()
        {
            int res = 200;

            Bitmap bm = new Bitmap(res, res);
            BitmapData bmd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                int* ptr = (int*)bmd.Scan0.ToPointer();

                for (int j = 0; j < res; ++j)
                {
                    for (int k = 0; k < res; ++k)
                    {
                        double a = ((double)(j - res / 2)) / (res / 2) * Math.PI;
                        double w = ((double)(k - res / 2)) / (res / 2) * 3 * Math.PI;

                        double act, q_act;
                        GetAction(new double[] { a, 0, w, 0 }, out act, out  q_act);


                        Color c = Color.Black;
                        if (act < 0) c = Color.FromArgb(0, 128, 255);
                        else if (act > 0) c = Color.FromArgb(255, 192, 0);

                        ptr[k * bm.Width + j] = c.ToArgb();
                    }
                }
            }

            bm.UnlockBits(bmd);
            return bm;
        }

        double bestsumreward = 0;
        protected Bitmap VisualizeAcrobot()
        {
            int res = 1200;

            Bitmap bm = new Bitmap(res, res);
            Graphics g = Graphics.FromImage(bm);
            AcrobotEnvironmentSimulator2 simm = new AcrobotEnvironmentSimulator2();

            double sumreward = 0;
            double[] state = new double[] { -Math.PI, 0, 0, 0};
            for (int i = 0; i < 300; ++i)
            {
                double M;
                double q_act;
                double reward;
                GetAction(state, out M, out q_act, true);
                simm.Simulate(state, new double[] { M }, dT, out state, out reward);
                sumreward += reward;
                if (i % 1 == 0)
                {
                    float zx = 75 + ((i % 100)) * 10;
                    float zy = 150 + 150 * (i/100);
                    float z = 50;

                    float e1x = zx + (float)(z * simm.Length1 * Math.Sin(simm.Angle1));
                    float e1y = zy - (float)(z * simm.Length1 * Math.Cos(simm.Angle1));

                    float e2x = e1x + (float)(z * simm.Length2 * Math.Sin(simm.Angle1 + simm.Angle2));
                    float e2y = e1y - (float)(z * simm.Length2 * Math.Cos(simm.Angle1 + simm.Angle2));

                    //float e1x = zx + (float)(z * simm.Length1 * Math.Cos(simm.Angle1));
                    //float e1y = zy - (float)(z * simm.Length1 * Math.Sin(simm.Angle1));

                    //float e2x = e1x + (float)(z * simm.Length2 * Math.Cos(simm.Angle1 + simm.Angle2));
                    //float e2y = e1y - (float)(z * simm.Length2 * Math.Sin(simm.Angle1 + simm.Angle2));
                  
                    Pen p = new Pen(Color.Black);
                    g.DrawLine(p, zx, zy, e1x, e1y);
                    p.Color = Color.Red;
                    g.DrawLine(p, e1x, e1y, e2x, e2y);
                }
            }
            g.DrawString("sum reward: " + sumreward, new Font("Arial", 20), new SolidBrush(Color.Black), 20, 20);
            Console.Out.WriteLine(sumreward);
            if (bestsumreward < sumreward)
            {
                bestsumreward = sumreward;
                bm.Save("acrobot.png");
            }
            return bm;
        }
    }
}
