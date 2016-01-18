using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContinuousRLTest;
using System.Drawing;
using System.Drawing.Imaging;
using OnlabNeuralis;

namespace CarNavigationRLTest
{
    class CarNavigationEnvironment : ReinforcementLearningEnvironment
    {
        const double SHAFT_LENGTH = 1.5;
        const double TIMESTEP = 0.25;
        const double SPEED = 0.2;
        public double x;
        public double y;
        public double alpha;

        private Random r = new Random();

        private static bool[,] map;

        public void Reset()
        {
            x = 0;
            y = 0;
            alpha = 0;
        }

        public static void LoadImage(string filename)
        {
            Bitmap bm = (Bitmap)Bitmap.FromFile(filename);
            BitmapData bmd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                map = new bool[bm.Height, bm.Width];
                uint* ptr = (uint*)bmd.Scan0.ToPointer();
                int index = 0;
                for (int yy = 0; yy < bm.Height; ++yy)
                {
                    for (int xx = 0; xx < bm.Width; ++xx)
                    {
                        map[yy, xx] = ptr[index] == 0xFFFFFFFF;
                        ++index;
                    }
                }
            }

            bm.UnlockBits(bmd);
        }


        public void Step(double ang)
        {   
            //double leftSpeed = SPEED;
            //double rightSpeed = SPEED;
            //if (ang < 0) {
            //    rightSpeed = SPEED;
            //    leftSpeed = SPEED + ang;
            //}
            //else {
            //    leftSpeed = SPEED;
            //    rightSpeed = SPEED - ang;
            //}

            //double dAngle = (leftSpeed - rightSpeed) * TIMESTEP / SHAFT_LENGTH;
            //double lamda = 1;
            //if (dAngle != 0) lamda = 2 / dAngle * Math.Sin(dAngle / 2);
            //double vectLength = (rightSpeed + leftSpeed) / 2 * TIMESTEP * lamda;

            //alpha += dAngle;
            //x += vectLength * Math.Cos(alpha - dAngle / 2);
            //y += vectLength * Math.Sin(alpha - dAngle / 2);


            alpha += ang;
            double nx = x + SPEED * Math.Cos(alpha - ang / 2);
            double ny = y + SPEED * Math.Sin(alpha - ang / 2);

            if (alpha > Math.PI) alpha -= 2 * Math.PI;
            if (alpha < -Math.PI) alpha += 2 * Math.PI;

            if (map != null)
            {
                int ix = (int)ComMath.NormalLim(nx, CarNavigationQStore.MINXY, CarNavigationQStore.MAXXY, 0, map.GetLength(1)-1);
                int iy = (int)ComMath.NormalLim(ny, CarNavigationQStore.MINXY, CarNavigationQStore.MAXXY, 0, map.GetLength(0)-1);

                if (!map[iy, ix])
                {
                    nx = x;
                    ny = y;
                }
            }

            x = nx;
            y = ny;

        }




        #region ReinforcementLearningEnvironment Members

        public ReinforcementLearningState State()
        {
            return new CarNavigationState(x, y, alpha);
        }

        public float Reward()
        {
            float reward = 0;// -0.00001f;
            if ((Math.Cos(alpha) < -0.95) && (Math.Abs(x) < 0.2f) && (Math.Abs(y) < 0.2f))
            {
                reward = 1;
            }

            return reward;
        }
        
        ReinforcementLearningAction[] allActions;
        public ReinforcementLearningAction[] GetAllActions()
        {
            if (allActions == null)
            {
                List<CarNavigationAction> list = new List<CarNavigationAction>();
                for (int i = 0; i < CarNavigationQStore.LENACTION; ++i)
                {
                    CarNavigationAction action;
                    CarNavigationQStore.GetAction(i, out action);
                    list.Add(action);
                }
                allActions = list.ToArray();
            }
            return allActions;
        }

        #endregion
    }
}
