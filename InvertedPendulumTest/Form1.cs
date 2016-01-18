using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InvertedPendulumEnvironment;

namespace InvertedPendulumTest
{
    public partial class Form1 : Form
    {
        bool leftDown, rightDown;

        double deltaT = 0.2;

        double absTorque = 0.5;

        double[] state;


        int count;

        InvertedPendulumEnvironmentSimulator sim;

        public Form1()
        {
            InitializeComponent();
            state = new double[] { -Math.PI, 0 };
            sim = new InvertedPendulumEnvironmentSimulator();

            timer1.Start();
        }

        private void Step(double torque)
        {
            count++;
            Console.Out.WriteLine(count);
            double reward;
            double[] newState;
            sim.Simulate(state, new double[] { torque }, deltaT, out newState, out reward);
            state = newState;

            Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            double torque = 0;
            if (leftDown)
            {
                torque += absTorque;
            }
            if (rightDown)
            {
                torque -= absTorque;
            }

            Step(torque);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            float zx = 400;
            float zy = 400;
            float z = 100;

            float e1x = zx + (float)(z * sim.Length * Math.Sin(sim.Angle));
            float e1y = zy - (float)(z * sim.Length * Math.Cos(sim.Angle));            

            Graphics g = e.Graphics;
            g.Clear(Color.White);
            Pen p = new Pen(Color.Black);

            g.DrawLine(p, zx, zy, e1x, e1y);            

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left)
            {
                leftDown = true;
            }
            if (e.KeyData == Keys.Right)
            {
                rightDown = true;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left)
            {
                leftDown = false;
            }
            if (e.KeyData == Keys.Right)
            {
                rightDown = false;
            }
        }
    }
}
