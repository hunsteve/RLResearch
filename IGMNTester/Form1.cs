using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Matrix;
using IGMNLib;
using XTreeCSharp;

namespace IGMNTester
{
    public partial class Form1 : Form
    {
        Color[] colors = { Color.Red, Color.Green, Color.Blue, Color.Brown, Color.Magenta, Color.Cyan };
        IGMN igmn;
        double x = 0;
        public Form1()
        {
            InitializeComponent();
                        
            igmn = new IGMN(new Vector(new double[] {0.005, 0.005, 1 }));
            //IGMNDataTest2();

            ////for ( x < 12.5; x += 0.01)
            ////{                
            ////}


            ////for (double x = 0; x < 12.5; x += 0.1)
            ////{
            ////    Console.Out.WriteLine(igmn.Recall(new Vector(new double[] { x })));
            ////}

            timer1.Start();



        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            x += 0.05;
            igmn.Train(new Vector(new double[] { Math.Cos(x), Math.Sin(x), 1 }));
            Invalidate();
            if (x > 2*3.14) timer1.Stop();
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            double zoom = 200;
            
            Graphics g = e.Graphics;
            g.TranslateTransform((float)(2 * zoom), (float)(2 * zoom));
            g.Clear(Color.White);

            List<DebugMBR> list = igmn.xtree.DebugListMBRs();
            foreach (DebugMBR d in list)
            {
                int i = (d.level - 3);
                g.DrawRectangle(new Pen(colors[d.level],3), (int)(d.mbr.Min[0] * zoom) + i, (int)(d.mbr.Min[1] * zoom) + i, (int)(d.mbr.Max[0] * zoom - d.mbr.Min[0] * zoom - 2 * i), (int)(d.mbr.Max[1] * zoom - d.mbr.Min[1] * zoom - 2 * i));
            }

            igmn.DrawGaussians(g, 0, 1, zoom);
        }

        public void IGMNDataTest2()
        {
            IGMNData a = new IGMNData(igmn, new Vector(new double[] { 0, 0 }));

            a.Gaussian.Covariance.Covariance.WriteToFile("orig_full_cov.txt");
            a.Gaussian.Covariance.InverseCovariance.WriteToFile("orig_full_invcov.txt");
            a.Gaussian.Mean.WriteToFile("orig_full_mean.txt");
            Console.Out.WriteLine(a.Gaussian.Covariance.Determinant);

            a.InputGaussian.Covariance.Covariance.WriteToFile("orig_inp_cov.txt");
            a.InputGaussian.Covariance.InverseCovariance.WriteToFile("orig_inp_invcov.txt");
            a.InputGaussian.Mean.WriteToFile("orig_inp_mean.txt");
            Console.Out.WriteLine(a.InputGaussian.Covariance.Determinant);


            a.RefineWithData(new Vector(new double[] { 1, 1 }), 0.5);
            a.RefineWithData(new Vector(new double[] { 2, 2 }), 0.3333333333333333333);
            a.RefineWithData(new Vector(new double[] { 3, 3 }), 0.25);

            a.Gaussian.Covariance.Covariance.WriteToFile("mod_full_cov.txt");
            a.Gaussian.Covariance.InverseCovariance.WriteToFile("mod_full_invcov.txt");
            a.Gaussian.Mean.WriteToFile("mod_full_mean.txt");
            Console.Out.WriteLine(a.Gaussian.Covariance.Determinant);

            a.InputGaussian.Covariance.Covariance.WriteToFile("mod_inp_cov.txt");
            a.InputGaussian.Covariance.InverseCovariance.WriteToFile("mod_inp_invcov.txt");
            a.InputGaussian.Mean.WriteToFile("mod_inp_mean.txt");
            Console.Out.WriteLine(a.InputGaussian.Covariance.Determinant);
        }


        public void IGMNDataTest()
        {
            IGMNData a = new IGMNData(null, new Vector(new double[] { 1, 2, 3, 4, 5, 4, 3, 2, 1 }));

            a.Gaussian.Covariance.Covariance.WriteToFile("orig_full_cov.txt");
            a.Gaussian.Covariance.InverseCovariance.WriteToFile("orig_full_invcov.txt");
            a.Gaussian.Mean.WriteToFile("orig_full_mean.txt");
            Console.Out.WriteLine(a.Gaussian.Covariance.Determinant);

            a.InputGaussian.Covariance.Covariance.WriteToFile("orig_inp_cov.txt");
            a.InputGaussian.Covariance.InverseCovariance.WriteToFile("orig_inp_invcov.txt");
            a.InputGaussian.Mean.WriteToFile("orig_inp_mean.txt");
            Console.Out.WriteLine(a.InputGaussian.Covariance.Determinant);


            a.RefineWithData(new Vector(new double[] { 4, 5, 1, 2, 8, 3, 3, 8, 1 }), 0.5);
            a.RefineWithData(new Vector(new double[] { 8, 3, 3, 1, 2, 8, 1, 4, 5 }), 0.5);

            a.Gaussian.Covariance.Covariance.WriteToFile("mod_full_cov.txt");
            a.Gaussian.Covariance.InverseCovariance.WriteToFile("mod_full_invcov.txt");
            a.Gaussian.Mean.WriteToFile("mod_full_mean.txt");
            Console.Out.WriteLine(a.Gaussian.Covariance.Determinant);

            a.InputGaussian.Covariance.Covariance.WriteToFile("mod_inp_cov.txt");
            a.InputGaussian.Covariance.InverseCovariance.WriteToFile("mod_inp_invcov.txt");
            a.InputGaussian.Mean.WriteToFile("mod_inp_mean.txt");
            Console.Out.WriteLine(a.InputGaussian.Covariance.Determinant);
        }

        public void CovarianceMatrixTest()
        {
            CovarianceMatrix m = new CovarianceMatrix(new Vector(new double[] { 1, 2, 3, 4, 5, 4, 3, 2, 1 }));
            m.AddDiad(new Vector(new double[] { 4, 5, 1, 2, 8, 3, 3, 8, 1 }), 0.25);
            m.AddDiad(new Vector(new double[] { 8, 3, 3, 1, 2, 8, 1, 4, 5 }), 0.25);
            m.MultiplyScalar(3.73871);
            m.Covariance.WriteToFile("m.txt");
            m.InverseCovariance.WriteToFile("minv.txt");
            Console.Out.WriteLine(m.Determinant);

            Gaussian g = new Gaussian(new Vector(9), m);
            double likelihood = g.Likelihood(new Vector(new double[] { 1, 10, 0, 0, 0, 0, 0, 0, 10 }));
            Console.Out.WriteLine(likelihood);
        }

    }
}
