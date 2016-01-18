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
    public partial class CustomControl2 : Control
    {
        public CustomControl2()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        public InvertedPendulumEnvironment Workspace
        {
            get;
            set;
        }

        public InvertedPendulumQStore QStore
        {
            get;
            set;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Graphics g = pe.Graphics;
            g.Clear(Color.Black);

            if (QStore != null)
            {
                Bitmap bm = new Bitmap(QStore.value.GetLength(1), QStore.value.GetLength(2));
                BitmapData bmd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                unsafe
                {
                    int* ptr = (int*)bmd.Scan0.ToPointer();

                    for (int j = 0; j < QStore.value.GetLength(1); ++j)
                    {
                        for (int k = 0; k < QStore.value.GetLength(2); ++k)
                        {
                            float max = float.MinValue;
                            int maxi = 0;

                            for (int i = 0; i < QStore.value.GetLength(0); ++i)
                            {
                                if (max < QStore.value[i, j, k])
                                {
                                    max = QStore.value[i, j, k];
                                    maxi = i;
                                }
                            }

                            if (QStore.value[1, j, k] == max) maxi = 1;

                            Color c = Color.Black;
                            if (maxi == 0) c = Color.FromArgb(0, 128, 255);
                            if (maxi == 1) c = Color.FromArgb(32, 32, 32);
                            if (maxi == 2) c = Color.FromArgb(255, 192, 0);
                           
                            ptr[k * bm.Width + j] = c.ToArgb();
                        }
                    }
                }

                bm.UnlockBits(bmd);

                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(bm,new Rectangle(0,0,bm.Width*2, bm.Height*2));

                int realx = ((int)((Workspace.a / Math.PI + 1) * InvertedPendulumQStore.LEN / 2) + InvertedPendulumQStore.LEN * 1000) % InvertedPendulumQStore.LEN;
                int realy = (int)((Workspace.w / 3 + 1) * InvertedPendulumQStore.LEN / 2); if (realy > InvertedPendulumQStore.LEN - 1) realy = InvertedPendulumQStore.LEN - 1; if (realy < 0) realy = 0;

                g.DrawLine(new Pen(Color.White, 2), 2 * realx - 5, 2 * realy - 5, 2 * realx + 5, 2 * realy + 5);
                g.DrawLine(new Pen(Color.White, 2), 2 * realx - 5, 2 * realy + 5, 2 * realx + 5, 2 * realy - 5);
            }


        }
    }
}
