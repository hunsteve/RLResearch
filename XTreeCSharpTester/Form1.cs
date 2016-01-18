using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XTreeCSharp;

namespace XTreeCSharpTester
{
    class SimpleData : XTData
    {
        MBR mbr;        

        public SimpleData(int x, int y)
        {
            Random r = new Random();
            XTVector min = new XTVector(2);
            XTVector max = new XTVector(2);
            for (int i = 0; i < 2; ++i)
            {
                int z = r.Next(50) + 40;
                min[i] = (i==0?x:y) - z / 2;
                max[i] = min[i] + z / 2;
            }
            mbr = new MBR(min, max);
        }

        public MBR GetMBR()
        {
            return mbr;
        }

    }

    public partial class Form1 : Form
    {        
        Color[] colors = { Color.Red, Color.Green, Color.Blue, Color.Brown, Color.Magenta, Color.Cyan };
        XTree<SimpleData> tree;

        SimpleData last;


        public Form1()
        {
            InitializeComponent();
            tree = new XTree<SimpleData>();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);

            List<DebugMBR> list = tree.DebugListMBRs();           
            foreach (DebugMBR d in list)
            {
                int i = (d.level - 3);
                g.DrawRectangle(new Pen(colors[d.level]), (int)d.mbr.Min[0] + i, (int)d.mbr.Min[1] + i, (int)(d.mbr.Max[0] - d.mbr.Min[0] - 2 * i), (int)(d.mbr.Max[1] - d.mbr.Min[1] - 2 * i));
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                last = new SimpleData(e.X, e.Y);
                tree.Insert(last);
                Invalidate();
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                List<SimpleData> ret = tree.rangeQuery(new MBR(new XTVector(new double[] { double.MinValue, e.Y }), new XTVector(new double[] { double.MaxValue, e.Y })));
                Console.Out.WriteLine(ret.Count);
                foreach (SimpleData d in ret)
                {
                    tree.Delete(d);
                }
                Invalidate();
            }
        }
    }
}
