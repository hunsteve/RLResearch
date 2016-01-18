using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace ContinuousRLTest
{
    public partial class ESIGMNViewer : Control
    {
        public ESIGMNViewer()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        public InvertedPendulumEnvironment Workspace
        {
            get;
            set;
        }

        public InvertedPendulumESIGMNQStore QStore
        {
            get;
            set;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Graphics g = pe.Graphics;
            g.Clear(Color.Black);

            int w = 50;
            int h = 50;

            if (QStore != null)
            {

                Bitmap bm = new Bitmap(w, h);
                BitmapData bmd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                unsafe
                {
                    int* ptr = (int*)bmd.Scan0.ToPointer();

                    

                    for (int x = 0; x < w; ++x)
                    {
                        for (int y = 0; y < h; ++y)
                        {
                            double aa = (float)(2 * ((double)x / w) - 1) * Math.PI;
                            double ww = (2 * ((double)y / h) - 1) * 3;
                            
                            float value;
                            ReinforcementLearningAction action;
                            QStore.GetBestActionAndUtilityForState(new InvertedPendulumState(aa, ww), out action, out value);
                            float act = ((InvertedPendulumAction2)action).action;


                            Color c = Color.Black;                            

                            if (act < 0)
                            {
                                c = Color.FromArgb(0, (int)Math.Min((1 + act) / 2 * 128, 128), (int)Math.Min((1 + act) / 2 * 255, 255));
                            }
                            else
                            {
                                c = Color.FromArgb((int)Math.Min((1 + act) / 2 * 255, 255), (int)Math.Min((1 + act) / 2 * 192, 192), 0);
                            }                            

                            ptr[y * bm.Width + x] = c.ToArgb();
                        }
                    }
                }

                bm.UnlockBits(bmd);

                g.InterpolationMode = InterpolationMode.Bicubic;
                g.DrawImage(bm, new Rectangle(0, 0, bm.Width * 6, bm.Height * 6));

                //int realx = ((int)((Workspace.a / Math.PI + 1) * InvertedPendulumQStore.LEN / 2) + InvertedPendulumQStore.LEN * 1000) % InvertedPendulumQStore.LEN;
                //int realy = (int)((Workspace.w / 3 + 1) * InvertedPendulumQStore.LEN / 2); if (realy > InvertedPendulumQStore.LEN - 1) realy = InvertedPendulumQStore.LEN - 1; if (realy < 0) realy = 0;

                //g.DrawLine(new Pen(Color.White, 2), 2 * realx - 5, 2 * realy - 5, 2 * realx + 5, 2 * realy + 5);
                //g.DrawLine(new Pen(Color.White, 2), 2 * realx - 5, 2 * realy + 5, 2 * realx + 5, 2 * realy - 5);
            }


        }
    }
}
