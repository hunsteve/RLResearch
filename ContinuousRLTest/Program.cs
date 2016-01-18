using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.IO;

namespace ContinuousRLTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Application.Run(new Form2());
           
            
            //ESN esn = new ESN(400, 1, 1);

            //DenseMatrix input = new DenseMatrix(1000, 1);
            //for (int i = 0; i < input.Rows; ++i)
            //{
            //    //input[i, 0] = (float)Math.Sin(i * 0.1);
            //    if (i < 50) input[i, 0] = 1;
            //    else input[i, 0] = 0;
            //}

            //Console.Out.WriteLine("START");

            //DenseMatrix output = esn.Simulate(input);

            //Console.Out.WriteLine("END" + output.Rows);

            //output.WriteToFile("esnout.txt");



            //IGMN igmn = new IGMN(2, 1, new Vector(new double[] { 4, 4, 2}));
            //double a, b;
            //for (double x = -2; x <= 2; x += 0.2)
            //{
            //    if (Math.Abs(x) < 0.01) a = 1;
            //    else a = Math.Sin(x * 4) / (x * 4);

            //    for (double y = -2; y <= 2; y += 0.2)
            //    {
            //        if (Math.Abs(y) < 0.01) b = 1;
            //        else b = Math.Sin(y * 4) / (y * 4);

            //        igmn.Train(new Vector(new double[] { x, y, a*b }));
            //    }
            //}

            //StreamWriter sw = new StreamWriter("out.txt");
            //for (double x = -2; x <= 2; x += 0.05)
            //{
            //    for (double y = -2; y <= 2; y += 0.05)
            //    {
            //        Vector outp = igmn.Recall(new Vector(new double[] { x, y, 0 }));
            //        sw.Write(outp.Elements[0] + ", ");
            //    }
            //    sw.Write("\r\n");
            //}
            //sw.Close();


            //Console.Out.WriteLine("prob: " + igmn.ToString());


            //int tpcount = 0;
            //for (double x = -5*Math.PI; x < 5*Math.PI; x += 0.1)
            //{
            //    igmn.Train(new Vector(new double[] {x, Math.Sin(x)/x }));
            //    tpcount++;
            //}


            //int cnt = 0;
            //double err = 0;
            //for (double x = -5*Math.PI; x < 5*Math.PI; x += 0.05)
            //{
            //    Vector y = igmn.Recall(new Vector(new double[] { x, 0 }));
            //    err += Math.Pow(y.Elements[0] - Math.Sin(x) / x, 2);
            //    cnt++;
            //}


            //Console.Out.WriteLine("MSE: " + err/cnt + "    cortical count:" + igmn.cortical.Count + " tpcount" + tpcount);







            //Vector mean = new Vector(100);
            //Vector v = new Vector(100);
            //Random r =new Random();
            //for (int i = 0; i < 100; ++i)
            //{
            //    mean.Elements[i] = r.NextDouble();
            //    v.Elements[i] = r.NextDouble();
            //}
            //IGMNGaussian g = new IGMNGaussian(igmn, mean);

            //g.Covariance.ModifyWithVector(v, 0.1);

            //g.Covariance.Covariance.WriteToFile("covariance.txt");
            //g.Covariance.InverseCovariance.WriteToFile("invcovariance.txt");
            //Console.Out.WriteLine("det: " + g.Covariance.Determinant);

            //double s = 0;
            //DateTime begin = DateTime.Now;
            //for (int i = 0; i < 20000; ++i)
            //{
            //    double p = g.Probability(v);
            //    s += p;
            //}
            //DateTime end = DateTime.Now;

            //Console.Out.WriteLine("prob: " + s / 100000 + "    time: " + (end - begin).TotalMilliseconds);

            
        }
    }
}
