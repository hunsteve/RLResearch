using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleRLTest
{
    public partial class Form1 : Form
    {
        Workspace ws = new Workspace();
        RLRobotController rc = new RLRobotController();


        int stepcount = 0;

        public Form1()
        {
            InitializeComponent();
            customControl11.ws = ws;
            rc.Initialize(ws);
        }


        protected override void OnPaint(PaintEventArgs e)
        {            
            
            base.OnPaint(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int[] action = rc.Step(ws);
            int x = ws.pos.X;
            int y = ws.pos.Y;
            switch (action[0])
            {
                case 0:
                    x++;
                    break;
                case 1:
                    y--;
                    break;
                case 2:
                    x--;
                    break;
                case 3:
                    y++;
                    break;
            }
            if (y > ws.map.GetLength(0) - 1) y = ws.map.GetLength(0) - 1;
            if (x > ws.map.GetLength(1) - 1) x = ws.map.GetLength(1) - 1;
            if (y < 0) y = 0;
            if (x < 0) x = 0;
            

            if ((stepcount > 50) || (ws.map[ws.pos.Y, ws.pos.X] == 3))
            {
                ws.pos = new Point(0, 0);
                stepcount = 0;
                rc.EpisodeBegin();
                return;
            }


            if (ws.map[y, x] != 1)
            {
                ws.pos = new Point(x, y);
            }
            ++stepcount;


            customControl11.Invalidate();
        }
    }
}
