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
    public partial class CustomControl1 : Control
    {
        public CustomControl1()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        public Workspace ws;
        int gridsize = 60;

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (ws != null)
            {
                Graphics g = pe.Graphics;
                g.Clear(Color.DarkGray);
                Brush bw = new SolidBrush(Color.White);
                Brush bb = new SolidBrush(Color.FromArgb(64, 64, 96));
                Font f = new Font(FontFamily.GenericSansSerif, 7);
                for (int x = 0; x < ws.map.GetLength(1); ++x)
                {
                    for (int y = 0; y < ws.map.GetLength(0); ++y)
                    {
                        if (ws.map[y, x] == 0)
                        {
                            g.FillRectangle(bw, x * gridsize, y * gridsize, gridsize - 1, gridsize - 1);
                        }
                        else if (ws.map[y, x] == 1)
                        {
                            g.FillRectangle(bb, x * gridsize, y * gridsize, gridsize - 1, gridsize - 1);
                        }
                        else if (ws.map[y, x] == 2) //start
                        {
                            g.FillRectangle(new SolidBrush(Color.Green), x * gridsize, y * gridsize, gridsize - 1, gridsize - 1);
                        }
                        else if (ws.map[y, x] == 3) //finish
                        {
                            g.FillRectangle(new SolidBrush(Color.Yellow), x * gridsize, y * gridsize, gridsize - 1, gridsize - 1);
                        }
                        for (int i = 0; i < 4; ++i) //0,1,2,3 = Right, Top, Left, Bottom
                        {
                            string s = String.Format("{0:0.00}", ws.uti[i, y, x]);
                            switch (i)
                            {
                                case 0:
                                    g.DrawString(s, f, bb, new PointF(x * gridsize + gridsize / 2 + 7, y * gridsize + gridsize / 2 - 5));
                                    break;
                                case 1:
                                    g.DrawString(s, f, bb, new PointF(x * gridsize + gridsize / 2 - 12, y * gridsize + 3));
                                    break;
                                case 2:
                                    g.DrawString(s, f, bb, new PointF(x * gridsize + gridsize / 2 - 30, y * gridsize + gridsize / 2 - 5));
                                    break;
                                case 3:
                                    g.DrawString(s, f, bb, new PointF(x * gridsize + gridsize / 2 - 12, y * gridsize + gridsize - 13));
                                    break;
                            }
                        }
                    }
                }

            }

            base.OnPaint(pe);
        }
    }
}
