using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SimpleRLTest
{
    public class Workspace
    {
        public int[,] map = new int[,] { { 2, 0, 0, 0, 0, 0 }, 
                                         { 0, 1, 0, 1, 1, 0 }, 
                                         { 0, 1, 0, 0, 0, 0 }, 
                                         { 0, 0, 0, 0, 1, 0 }, 
                                         { 0, 1, 1, 0, 1, 0 }, 
                                         { 0, 0, 0, 0, 0, 3 }, 
        };

        public float[, ,] uti = new float[4, 6, 6];

        public Point pos;

        public Workspace()
        {
            pos = new Point(0, 0);
        }


    }
}
