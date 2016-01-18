using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AcrobotEnvironment;

namespace AcrobotTest
{
    public partial class Form1 : Form
    {
        bool leftDown, rightDown;

        double deltaT = 0.02;

        double absTorque = 15;

        double[] state;
        

        int count;

        AcrobotEnvironmentSimulator3 sim;

        public Form1()
        {
            InitializeComponent();
            state = new double[] {0, 0, 0, 0};                        
            sim = new AcrobotEnvironmentSimulator3();

            timer1.Start();
        }

        private void StepAcrobot(double torque)
        {
            count++;
            
            double reward;
            double[] newState;
            sim.Simulate(state, new double[] { torque }, deltaT, out newState, out reward);
            Console.Out.WriteLine(count + " " + state[0] + " " + state[1] + " " + state[2] + " " + state[3] + " " + reward);
            state = newState;

            Invalidate();
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            float zx = 400;
            float zy = 400;
            float z = 100;

            float e1x = zx + (float)(z * sim.Length1 * Math.Sin(sim.Angle1));
            float e1y = zy + (float)(z * sim.Length1 * Math.Cos(sim.Angle1));

            float e2x = e1x + (float)(z * sim.Length2 * Math.Sin(sim.Angle1 + sim.Angle2));
            float e2y = e1y + (float)(z * sim.Length2 * Math.Cos(sim.Angle1 + sim.Angle2));

            Graphics g = e.Graphics;
            g.Clear(Color.White);
            Pen p = new Pen(Color.Black);

            g.DrawLine(p, zx, zy, e1x, e1y);
            g.DrawLine(p, e1x, e1y, e2x, e2y);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            double torque = 0;
            if (leftDown)
            {
                torque -= absTorque;
            }
            if (rightDown)
            {
                torque += absTorque;
            }

            StepAcrobot(torque);
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

