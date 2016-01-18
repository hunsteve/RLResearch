using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContinuousRLTest;
using System.IO;
using System.IO.Compression;
using OnlabNeuralis;
using Matrix;

namespace CarNavigationRLTest
{
    class CarNavigationADPController
    {
        public CarNavigationQStore qstore;

        SparseMatrix[] Ma;
        Vector R;

        Random r;
        int statenum;

        public CarNavigationADPController()
        {
            statenum = CarNavigationQStore.LENXY * CarNavigationQStore.LENXY * CarNavigationQStore.LENANG;                        
            r = new Random();
            qstore = CarNavigationQStore.Load("qstore.dat");      
        }


        public void Train()
        {
            GenerateRVector();
            LoadMMatrices("mmatrices.dat");

            Vector[] Q = new Vector[CarNavigationQStore.LENACTION];
            for (int i = 0; i < CarNavigationQStore.LENACTION; ++i)
            {
                Q[i] = new Vector(statenum);
            }

            for (int i = 0; i < CarNavigationQStore.LENACTION; ++i)
            {
                for (int j = 0; j < CarNavigationQStore.LENXY; ++j)
                {
                    for (int k = 0; k < CarNavigationQStore.LENXY; ++k)
                    {
                        for (int l = 0; l < CarNavigationQStore.LENANG; ++l)
                        {
                            Q[i].Elements[l * CarNavigationQStore.LENXY * CarNavigationQStore.LENXY + k * CarNavigationQStore.LENXY + j] = qstore.value[i, j, k, l];
                        }                        
                    }
                }
            }

            Vector[] p_ = new Vector[CarNavigationQStore.LENACTION];
            float max_ = float.MinValue;
            for (int i = 0; i < CarNavigationQStore.LENACTION; ++i)
            {
                p_[i] = new Vector(Q[i]);
                for (int j = 0; j < statenum; ++j)
                {
                    if (max_ < p_[i].Elements[j]) max_ = (float)p_[i].Elements[j];
                }
            }

            for (int i = 0; i < CarNavigationQStore.LENACTION; ++i)
            {                
                p_[i].Exp(-max_);
            }

            Vector sum = new Vector(statenum);
            for (int i = 0; i < CarNavigationQStore.LENACTION; ++i)
            {
                sum += p_[i];
            }

            SparseMatrix[] Ma_ = new SparseMatrix[CarNavigationQStore.LENACTION];
            for (int i = 0; i < CarNavigationQStore.LENACTION; ++i)
            {
                p_[i].Div(sum);
                Ma_[i] = new SparseMatrix(Ma[i]);
                Ma_[i].Multiply(p_[i]);
            }

            //M matrix kiszamitasa
            SparseMatrix M = new SparseMatrix(statenum);
            for (int i = 0; i < CarNavigationQStore.LENACTION; ++i)
            {
                M.Add(Ma_[i]);                
            }
            M.Multiply(0.9999f);

            SparseMatrix IM = SparseMatrix.Identity(statenum) - M;

            //IM.WriteToFile("IM.txt");

            Vector utility = IM.SolveLinearEquation2(R);

            for (int i = 0; i < CarNavigationQStore.LENACTION; ++i)
            {
                Q[i] = Ma[i].MatrixMultiplyRight(utility);
            }

            Vector QMax = new Vector(statenum);
            for (int j = 0; j < statenum; ++j)
            {
                float max = float.MinValue;
                int best = 0;
                for (int i = 0; i < CarNavigationQStore.LENACTION; ++i)
                {
                    if (Q[i].Elements[j] > max)
                    {
                        max = (float)Q[i].Elements[j];
                        best = i;
                    }
                }
                QMax.Elements[j] = best;
            }


            for (int i = 0; i < CarNavigationQStore.LENACTION; ++i)
            {
                for (int j = 0; j < CarNavigationQStore.LENXY; ++j)
                {
                    for (int k = 0; k < CarNavigationQStore.LENXY; ++k)
                    {
                        for (int l = 0; l < CarNavigationQStore.LENANG; ++l)
                        {
                            qstore.value[i, j, k, l] = (float)Q[i].Elements[l * CarNavigationQStore.LENXY * CarNavigationQStore.LENXY + k * CarNavigationQStore.LENXY + j];
                        }
                    }
                }                               
            }

            utility.WriteToFile("u.txt");

            qstore.Save("qstore.dat");

            //Q[0].WriteToFile("q1.txt");
            //Q[1].WriteToFile("q2.txt");
            //Q[2].WriteToFile("q3.txt");
            //Q[3].WriteToFile("q4.txt");
            //Q[4].WriteToFile("q5.txt");
            //Q[5].WriteToFile("q6.txt");
            //Q[6].WriteToFile("q7.txt");
            //Q[7].WriteToFile("q8.txt");
            //Q[8].WriteToFile("q9.txt");
            //Q[9].WriteToFile("q10.txt");
            //Q[10].WriteToFile("q11.txt");            
        }

        public void GenerateMMatrices()
        {
            CarNavigationEnvironment testws = new CarNavigationEnvironment();          

            Ma = new SparseMatrix[CarNavigationQStore.LENACTION];
            for (int i = 0; i < CarNavigationQStore.LENACTION; ++i)
            {
                Ma[i] = new SparseMatrix(statenum);
            }

            for (int ii = 0; ii < statenum; ++ii)
            {                
                int l = ii / (CarNavigationQStore.LENXY * CarNavigationQStore.LENXY);
                int iil = ii - l * (CarNavigationQStore.LENXY * CarNavigationQStore.LENXY);
                int k = iil / CarNavigationQStore.LENXY;
                int j = iil % CarNavigationQStore.LENXY;               

                CarNavigationState state;
                CarNavigationQStore.GetState(j, k, l, out state);

                for (int i = 0; i < CarNavigationQStore.LENACTION; ++i)
                {
                    CarNavigationAction action;
                    CarNavigationQStore.GetAction(i, out action);                    
                    for (int i1 = 0; i1 < 100; ++i1)
                    {
                        testws.x = state.x + r.NextDouble() * (CarNavigationQStore.MAXXY - CarNavigationQStore.MINXY) / CarNavigationQStore.LENXY;
                        testws.y = state.y + r.NextDouble() * (CarNavigationQStore.MAXXY - CarNavigationQStore.MINXY) / CarNavigationQStore.LENXY;
                        testws.alpha = state.alpha + r.NextDouble() * (2 * Math.PI) / CarNavigationQStore.LENANG;

                        testws.Step(action.ang);

                        int j2, k2, l2;
                        CarNavigationState state2 = new CarNavigationState(testws.x, testws.y, testws.alpha);
                        CarNavigationQStore.GetStateIndices(state2, out j2, out k2, out l2);

                        int ii2 = l2 * CarNavigationQStore.LENXY * CarNavigationQStore.LENXY + k2 * CarNavigationQStore.LENXY + j2;
                        if (ii2 > statenum - 1) throw new Exception();
                        Ma[i][ii, ii2] += 0.01f;                        
                    }
                }               
            }
            SaveMMatrices("mmatrices.dat");
        }

        private void SaveMMatrices(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                using(GZipStream stream = new GZipStream(fs,CompressionMode.Compress)) {
                    using (BinaryWriter bw = new BinaryWriter(stream))
                    {
                        for (int i = 0; i < Ma.Length; ++i)
                        {
                            Ma[i].Save(bw);
                        }
                    }
                }            
            }
        }

        private void LoadMMatrices(string filename)
        {
            List<SparseMatrix> MaTemp = new List<SparseMatrix>();
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    using (GZipStream stream = new GZipStream(fs, CompressionMode.Decompress))
                    {
                        using (BinaryReader br = new BinaryReader(stream))
                        {
                            while (fs.Position < fs.Length)
                            {
                                MaTemp.Add(SparseMatrix.Load(br));
                            }
                        }
                    }

                }
            }
            catch (IOException ex)
            {
            }

            Ma = MaTemp.ToArray();
        }

        private void GenerateRVector()
        {
            CarNavigationEnvironment testws = new CarNavigationEnvironment();
            R = new Vector(statenum);
            float sum = 0;
            for (int ii = 0; ii < statenum; ++ii)
            {
                int l = ii / (CarNavigationQStore.LENXY * CarNavigationQStore.LENXY);
                int iil = ii - l * (CarNavigationQStore.LENXY * CarNavigationQStore.LENXY);
                int k = iil / CarNavigationQStore.LENXY;
                int j = iil % CarNavigationQStore.LENXY;

                CarNavigationState state;
                CarNavigationQStore.GetState(j, k, l, out state);

                testws.x = state.x;
                testws.y = state.y;
                testws.alpha = state.alpha;

                R.Elements[ii] = testws.Reward();
                sum += (float)R.Elements[ii];
            }
            Console.Out.Write(sum);
        }
    }
}
