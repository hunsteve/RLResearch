using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using IGMNLib;
using Matrix;
using InvertedPendulumEnvironment;
using TablularApproximator;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace I2MTC_SpecIssue_results
{
    public partial class RL_TDlambda : Form
    {

        Thread trainingThread;

        IGMN igmn1, igmn2;
        Random r;
        bool isRunning;

        double dT = 0.1;

        TabularApproximator tab1, tab2;

        InvertedPendulumEnvironmentSimulator sim;
        Bitmap bm;

        public RL_TDlambda()
        {
            InitializeComponent();
            sim = new InvertedPendulumEnvironmentSimulator();

            igmn1 = new IGMN(new Vector(new double[] { 1, 1, 1 }));
            igmn2 = new IGMN(new Vector(new double[] { 1, 1, 1 }));
            tab1 = new TabularApproximator(new int[] { 200, 200 }, new double[] { -Math.PI, -1.5*Math.PI }, new double[] { Math.PI, 1.5 * Math.PI });
            tab2 = new TabularApproximator(new int[] { 200, 200 }, new double[] { -Math.PI, -1.5*Math.PI }, new double[] { Math.PI, 1.5 * Math.PI });
            r = new Random();
            isRunning = true;
            trainingThread = new Thread(new ThreadStart(train));
            trainingThread.Start();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {            
            //pictureBox1.Image = Visualize();
            //if (bm!= null) pictureBox1.Image = bm;
            Invalidate();
        }

        private void RL_TDlambda_Paint(object sender, PaintEventArgs e)
        {
            //double zoom = 50;

            //Graphics g = e.Graphics;
            ////g.TranslateTransform((float)(4 * zoom), (float)(4 * zoom));
            ////g.Clear(Color.White);
            
            ////igmn1.DrawGaussians(g, 0, 2, zoom);

            //Bitmap bm = Visualize();
            //g.InterpolationMode = InterpolationMode.NearestNeighbor;
            //g.DrawImage(bm, new Rectangle(0, 0, bm.Width * 2, bm.Height * 2));
        }


        class TDLambda_data {
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
            
            
            double lambda =0.9;// 0.9;
            double gamma = 0.99;
            Random r = new Random();

            while (isRunning)
            {
                igmn1 = new IGMN(new Vector(new double[] { 1, 1, 1 }));
                igmn2 = new IGMN(new Vector(new double[] { 1, 1, 1 }));
                for (int k = 0; k < 100; ++k)
                {
                    double sumreward = 0;

                    List<TDLambda_data> diffQs = new List<TDLambda_data>();
                    //state = new double[] { (2 * r.NextDouble() - 1) * Math.PI, (2 * r.NextDouble() - 1) * 1.2 * Math.PI };
                    state = new double[] { Math.PI, 0 };

                    double M;
                    double q_act;

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
                                igmn1.Train(new Vector(new double[] { diffQs[i].state[0], diffQs[i].state[1], diffQs[i].diff + diffQs[i].Q }));
                            }
                            //lock (tab1)
                            //{
                            //    tab1.Train(diffQs[i].state, diffQs[i].diff + diffQs[i].Q, 0.5);
                            //}
                        }
                        else if (diffQs[i].action > 0)
                        {
                            lock (igmn2)
                            {
                                igmn2.Train(new Vector(new double[] { diffQs[i].state[0], diffQs[i].state[1], diffQs[i].diff + diffQs[i].Q }));
                            }
                            //lock (tab2)
                            //{
                            //    tab2.Train(diffQs[i].state, diffQs[i].diff + diffQs[i].Q, 0.5);
                            //}
                        }
                    }

                    bm = VisualizeInvertedPendulum();

                    //Thread.Sleep(1000);
                }
            }
            
        }

        void GetAction(double[] state, out double act, out double q_act)
        {
            GetAction(state, out act, out q_act, false);
        }

        void GetAction(double[] state, out double act, out double q_act, bool isGreedy)
        {
            double M_abs = 0.5;
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
            //    q1 = tab1.Recall(state);
            //}

            //lock (tab2)
            //{
            //    q2 = tab2.Recall(state);
            //}


            int action = 0;
            if (q1 > q2) action = -1;
            else if (q1 < q2) action = 1;

            //e greedy
            lock(r)
            {
                if (!isGreedy && r.NextDouble() > 0.75) action = r.Next(2) * 2 - 1;
            }

            if (action < 0) q_act = q1;
            else q_act = q2;

            act = action * M_abs;                                   
        }



        double bestsumreward = 0;
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
                        double w = ((double)(k - res / 2)) / (res / 2) * 1.5 * Math.PI;

                        double act, q_act;
                        GetAction(new double[] { a, w }, out act, out  q_act, true);

                       
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


        protected Bitmap VisualizeInvertedPendulum()
        {
            int res = 1200;

            Bitmap bm = new Bitmap(res, res);
            Graphics g = Graphics.FromImage(bm);
            InvertedPendulumEnvironmentSimulator simm = new InvertedPendulumEnvironmentSimulator();

            double[] state = new double[] { Math.PI, 0 };
            double sumreward = 0;

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
                    float zy = 150 + 150 * (i / 100);
                    float z = 50;

                    float e1x = zx + (float)(z * simm.Length * Math.Sin(simm.Angle));
                    float e1y = zy - (float)(z * simm.Length * Math.Cos(simm.Angle));
                                        
                    Pen p = new Pen(Color.Black);

                    g.DrawLine(p, zx, zy, e1x, e1y);
                    g.DrawEllipse(p, zx-2, zy-2, 4, 4);
                }
            }
            if (bestsumreward < sumreward)
            {
                bestsumreward = sumreward;
                bm.Save("invertedpendulum.png");
            }

            Console.Out.WriteLine(sumreward);

            return bm;
        }
    }
}

