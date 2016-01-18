using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Matrix;

namespace ContinuousRLTest
{
    public partial class Form2 : Form
    {
        ReinforcementLearningEnvironment ws = new InvertedPendulumEnvironment();
        ReinforcementLearningController controller = new InvertedPendulumRLController();
        ReinforcementLearningPolicy policy = new InvertedPendulumEpsilonGreedyPolicy();
        //ReinforcementLearningPolicy policy = new InvertedPendulumSoftmaxPolicy();             

        ReinforcementLearningPolicy policy2 = new BestActionPolicy();

        InvertedPendulumADPController adp = new InvertedPendulumADPController();
        //InvertedPendulumIADPController iadp = new InvertedPendulumIADPController();

        InvertedPendulumESIGMNController esigmn = new InvertedPendulumESIGMNController();
        InvertedPendulumESIGMNPolicy esigmnpolicy = new InvertedPendulumESIGMNPolicy();


        int stepcount;

        public Form2()
        {
            InitializeComponent();
            controller.Initialize(ws);

            customControl11.Workspace = (InvertedPendulumEnvironment)ws;
            customControl21.Workspace = (InvertedPendulumEnvironment)ws;
            customControl21.QStore = (InvertedPendulumQStore)controller.qFunction;

            adp.qstore = (InvertedPendulumQStore)controller.qFunction;
            //customControl31.IADP = iadp;

            esigmn.Initialize(ws);

            esigmnViewer1.QStore = (InvertedPendulumESIGMNQStore)esigmn.qFunction;


        }

        private void button1_Click(object sender, EventArgs e)
        {
            ((InvertedPendulumEnvironment)ws).Reset();
            controller.EpisodeBegin();
            timer1.Enabled = !timer1.Enabled;
            button2.Enabled = !timer1.Enabled;
            button3.Enabled = !timer1.Enabled;
            button4.Enabled = !timer1.Enabled;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            float avgreward = 0;
            for (int j = 0; j < 10; ++j)
            {
                ((InvertedPendulumEnvironment)ws).Reset();            
                //controller.EpisodeBegin();
                esigmn.EpisodeBegin();
                float sumreward = 0;
                for (int i = 0; i < 200; ++i)
                {
                    //InvertedPendulumAction action = (InvertedPendulumAction)controller.Step(ws, policy);
                    InvertedPendulumAction2 action = (InvertedPendulumAction2)esigmn.Step(ws, esigmnpolicy);
                    sumreward += ws.Reward();
                    ((InvertedPendulumEnvironment)ws).Step(action.action);
                    esigmnViewer1.Invalidate();
                }
                avgreward += sumreward;
            }
            avgreward /= 30;
            Console.Out.WriteLine("Reward: " + avgreward);

            customControl11.Invalidate();
            //customControl21.Invalidate();            
            esigmnViewer1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ((InvertedPendulumEnvironment)ws).Reset();
            stepcount = 0;
            controller.EpisodeBegin();
            timer2.Enabled = !timer2.Enabled;
            button1.Enabled = !timer2.Enabled;
            button3.Enabled = !timer1.Enabled;
            button4.Enabled = !timer1.Enabled;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            InvertedPendulumAction action = (InvertedPendulumAction)controller.Step(ws, policy2);

            ((InvertedPendulumEnvironment)ws).Step(action.action);

            if (stepcount > 1000)
            {
                ((InvertedPendulumEnvironment)ws).Reset();
                stepcount = 0;
                controller.EpisodeBegin();
                customControl11.Invalidate();
                customControl21.Invalidate();
                return;
            }

            ++stepcount;

            customControl11.Invalidate();
            customControl21.Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            adp.GenerateMMatrices();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer3.Enabled = !timer3.Enabled;
            button1.Enabled = !timer3.Enabled;
            button2.Enabled = !timer3.Enabled;
            button3.Enabled = !timer1.Enabled;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            //iadp.Train();
            //customControl31.Invalidate();


            adp.Train();
            customControl21.Invalidate();

            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            adp.Train();
            customControl21.Invalidate();

            Random r = new Random();

            InvertedPendulumQStore QStore =  (InvertedPendulumQStore)adp.qstore;

            for (int aa = 0; aa < 100000; ++aa)
            {
                InvertedPendulumState state = new InvertedPendulumState((float)(2 * r.NextDouble() - 1) * Math.PI, (2 * r.NextDouble() - 1) * 3);
                InvertedPendulumAction action = new InvertedPendulumAction(r.Next(3) - 1);

                ((InvertedPendulumESIGMNQStore)esigmn.qFunction).igmn.Train(new Vector(new double[] { state.a, state.w, action.action, QStore.Evaluate(state, action) }));
            }

            //for (int j = 0; j < QStore.value.GetLength(1); ++j)
            //{
            //    for (int k = 0; k < QStore.value.GetLength(2); ++k)
            //    {
            //        for (int i = 0; i < QStore.value.GetLength(0); ++i)
            //        {
            //            InvertedPendulumState state;
            //            InvertedPendulumAction action;
            //            InvertedPendulumQStore.GetStateAndAction(i, j, k, out state, out action);
            //            ((InvertedPendulumESIGMNQStore)esigmn.qFunction).igmn.Train(new Vector(new double[] { state.a, state.w, action.action, QStore.value[i, j, k] }));
            //        }
            //    }
            //}
            
            
            esigmnViewer1.Invalidate();
        }
    }
}
