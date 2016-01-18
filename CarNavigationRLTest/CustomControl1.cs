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
    public partial class CustomControl1 : Control
    {
        public CustomControl1()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        public CarNavigationQStore QStore
        {
            get;
            set;
        }

        float scale = 60;

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Graphics g = pe.Graphics;
            g.Clear(Color.White);

            if (QStore != null)
            {
                CarNavigationEnvironment testenv = new CarNavigationEnvironment();

                for (int xx = 0; xx < CarNavigationQStore.LENXY; xx+=2)
                {
                    for (int yy = 0; yy < CarNavigationQStore.LENXY; yy+=2)
                    {
                        for (int ang = 0; ang < CarNavigationQStore.LENANG; ang+=2)
                        {
                            CarNavigationState state;
                            CarNavigationQStore.GetState(xx, yy, ang, out state);


                            testenv.x = state.x;
                            testenv.y = state.y;
                            testenv.alpha = state.alpha;


                
                            double ax = testenv.x;
                            double ay = testenv.y;
                            g.DrawEllipse(new Pen(Color.Red), Width / 2 + (float)ax * scale, Height / 2 + (float)ay * scale, 3, 3);
                            int len = 0;
                            do
                            {
                                ReinforcementLearningAction action;
                                float utility;
                                QStore.GetBestActionAndUtilityForState(testenv.State(), out action, out utility);
                                testenv.Step(((CarNavigationAction)action).ang);

                                double bx = testenv.x;
                                double by = testenv.y;

                                g.DrawLine(new Pen(Color.FromArgb(32, (int)((Math.Cos(testenv.alpha) + 1) * 127), (int)((Math.Sin(testenv.alpha) * Math.Cos(testenv.alpha) + 1) * 127), (int)((Math.Sin(testenv.alpha) + 1) * 127))), Width / 2 + (float)ax * scale, Height / 2 + (float)ay * scale, Width / 2 + (float)bx * scale, Height / 2 + (float)by * scale);

                                ax = bx;
                                ay = by;
                                ++len;
                            }
                            while (!((Math.Abs(ax) < 0.25) && (Math.Abs(ay) < 0.25) && (Math.Cos(testenv.alpha) < -0.9)) && (len < 100));

                        }
                    }
                }
            }


        }
    }
}
