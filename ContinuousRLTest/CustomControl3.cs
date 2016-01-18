using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BenTools.Mathematics;

namespace ContinuousRLTest
{
    public partial class CustomControl3 : Control
    {
        public CustomControl3()
        {
            InitializeComponent();
        }

        public InvertedPendulumIADPController IADP
        {
            get;
            set;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Graphics g = pe.Graphics;
            g.Clear(Color.Black);


           


            if ((IADP != null) && (IADP.states != null))
            {

                List<BenTools.Mathematics.Vector> voronoiList = new List<BenTools.Mathematics.Vector>();

                for (int i = 0; i < IADP.states.Count; ++i)
                {
                    voronoiList.Add(new BenTools.Mathematics.Vector(new double[] { IADP.states[i].a, IADP.states[i].w }));
                }

                VoronoiGraph vg = Fortune.ComputeVoronoiGraph(voronoiList);



                foreach (object obj in vg.Edges)
                {
                    VoronoiEdge e = (VoronoiEdge)obj;
                    g.DrawLine(new Pen(Color.White), 20 + (float)(Math.PI + e.VVertexA[0]) * 70, 20 + (float)(3 + e.VVertexA[1]) * 70, 20 + (float)(Math.PI + e.VVertexB[0]) * 70, 20 + (float)(3 + e.VVertexB[1]) * 70);
                }



                for (int i = 0; i < IADP.states.Count; ++i )
                {
                    InvertedPendulumState state = IADP.states[i];
                    float max = float.MinValue;
                    int maxi = 0;
                    for (int j = 0; j < IADP.Q.GetLength(0); ++j)
                    {
                        if (max < IADP.Q[j].Elements[i])
                        {
                            max = (float)IADP.Q[j].Elements[i];
                            maxi = j;
                        }
                    }

                    if (IADP.Q[1].Elements[i] == max) maxi = 1;

                    Color c = Color.Black;
                    if (maxi == 0) c = Color.FromArgb(0, 128, 255);
                    if (maxi == 1) c = Color.FromArgb(32, 32, 32);
                    if (maxi == 2) c = Color.FromArgb(255, 192, 0);


                    g.FillEllipse(new SolidBrush(c), 20 + (float)((Math.PI + state.a) * 70), 20 + (float)((3 + state.w) * 70), 2, 2);

                }
            }
        }
    }
}
