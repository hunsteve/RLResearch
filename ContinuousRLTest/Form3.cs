using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ContinuousRLTest
{
    public partial class Form3 : Form
    {
        InvertedPendulumEnvironment ws;

        public Form3()
        {
            InitializeComponent();
            ws = customControl11.Workspace = new InvertedPendulumEnvironment();               
            this.KeyPreview = true;
            timer1.Enabled = true;
        }


        private void timer1_Tick_1(object sender, EventArgs e)
        {
            int M = 0;
            if (leftDown) M += -1;
            if (rightDown) M += 1;
            ws.Step(M);
            customControl11.Invalidate();
        }

        bool leftDown = false;
        bool rightDown = false;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {            
            if (keyData == Keys.Left) leftDown = true;
            if (keyData == Keys.Right) rightDown = true;

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) leftDown = true;
            if (e.KeyCode == Keys.Right) rightDown = true;
        }

        private void Form3_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) leftDown = false;
            if (e.KeyCode == Keys.Right) rightDown = false;
        }
        
    }
}
