using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ContinuousRLTest;

namespace CarNavigationRLTest
{
    public partial class Form1 : Form
    {

        ReinforcementLearningEnvironment ws = new CarNavigationEnvironment();
        ReinforcementLearningController controller = new CarNavigationRLController();
        //ReinforcementLearningPolicy policy = new CarNavigationEpsilonGreedyPolicy();
        ReinforcementLearningPolicy policy = new CarNavigationSoftmaxPolicy();
        ReinforcementLearningPolicy policy2 = new BestActionPolicy();

        CarNavigationADPController adp = new CarNavigationADPController();


        public Form1()
        {
            InitializeComponent();
            CarNavigationEnvironment.LoadImage("map.png");
            customControl11.QStore = adp.qstore;
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            adp.Train();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            adp.GenerateMMatrices();
        }
    }
}
