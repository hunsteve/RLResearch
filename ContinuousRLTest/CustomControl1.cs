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
    public partial class CustomControl1 : Control
    {        
        public CustomControl1()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        public InvertedPendulumEnvironment Workspace
        {
            get;
            set;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            pe.Graphics.Clear(Color.White);
            if (Workspace != null)
            {
                float len = (float)InvertedPendulumEnvironment.l * 100;
                int x = Width / 2;
                int y = (int)(Height / 2);
                
                Graphics g = pe.Graphics;

                int x2 = (int)(x + Math.Sin(Workspace.a) * len);
                int y2 = (int)(y + Math.Cos(Workspace.a) * len);

                g.DrawLine(new Pen(Color.Black, 3), x, y, x2, y2);
                g.DrawEllipse(new Pen(Color.Black, 3), new Rectangle(x - 5, y - 5, 10, 10));
                g.FillEllipse(new SolidBrush(Color.Black), new Rectangle(x2 - 5, y2 - 5, 10, 10));
            }


        }
    }
}
