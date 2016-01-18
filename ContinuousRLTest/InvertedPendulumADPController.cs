using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using Matrix;

namespace ContinuousRLTest
{    
    class InvertedPendulumADPController
    {
        public InvertedPendulumQStore qstore;

        SparseMatrix[] Ma;
        Vector R;

        Random r;
        int statenum;

        public InvertedPendulumADPController()
        {
            GenerateRVector();
            LoadMMatrices("mmatrices.dat");
            r = new Random();
            qstore = new InvertedPendulumQStore();
            statenum = InvertedPendulumQStore.LEN * InvertedPendulumQStore.LEN;            
        }


        public void Train()
        {
            Vector[] Q = new Vector[3];
            for (int i = 0; i < 3; ++i)
            {
                Q[i] = new Vector(statenum);
            }

            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < InvertedPendulumQStore.LEN; ++j)
                {
                    for (int k = 0; k < InvertedPendulumQStore.LEN; ++k)
                    {
                        Q[i].Elements[k * InvertedPendulumQStore.LEN + j] = qstore.value[i, j, k];
                    }
                }
            }
                 
            Vector[] p_ = new Vector[3];
            float max_ = float.MinValue;
            for (int i = 0; i < 3; ++i)
            {
                p_[i] = new Vector(Q[i]);
                for (int j = 0; j < statenum; ++j)
                {
                    if (max_ < p_[i].Elements[j]) max_ = (float)p_[i].Elements[j];
                }
            }

            for (int i = 0; i < 3; ++i)
            {                
                p_[i].Exp(-max_);
            }
            Vector sum = p_[0] + p_[1] + p_[2];
            SparseMatrix[] Ma_ = new SparseMatrix[3];
            for (int i = 0; i < 3; ++i)
            {
                p_[i].Div(sum);
                Ma_[i] = new SparseMatrix(Ma[i]);
                Ma_[i].Multiply(p_[i]);
            }

            //M matrix kiszamitasa
            SparseMatrix M = (Ma_[0] + Ma_[1] + Ma_[2]);
            M.Multiply(0.99f);

            SparseMatrix IM = SparseMatrix.Identity(statenum) - M;
            Vector utility = IM.SolveLinearEquation2(R);

            for (int i = 0; i < 3; ++i)
            {
                Q[i] = Ma[i].MatrixMultiplyRight(utility);
            }

            Vector QMax = new Vector(statenum);
            for (int i = 0; i < statenum; ++i)
            {
                float max = float.MinValue;
                int best = 0;
                for (int j = 0; j < 3; ++j)
                {
                    if (Q[j].Elements[i] > max)
                    {
                        max = (float)Q[j].Elements[i];
                        best = j;
                    }
                }
                QMax.Elements[i] = best;
            }

               
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < InvertedPendulumQStore.LEN; ++j)
                {
                    for (int k = 0; k < InvertedPendulumQStore.LEN; ++k)
                    {
                        qstore.value[i, j, k] = (float)Q[i].Elements[k * InvertedPendulumQStore.LEN + j];
                    }
                }                               
            }

            utility.WriteToFile("u.txt");

            Q[0].WriteToFile("q1.txt");
            Q[1].WriteToFile("q2.txt");
            Q[2].WriteToFile("q3.txt");            
        }

        public void GenerateMMatrices()
        {
            InvertedPendulumEnvironment testws = new InvertedPendulumEnvironment();
            int statenum = InvertedPendulumQStore.LEN * InvertedPendulumQStore.LEN;
            
            Ma = new SparseMatrix[3];
            for (int i = 0; i < 3; ++i)
            {
                Ma[i] = new SparseMatrix(statenum);
            }

            for (int ii = 0; ii < statenum; ++ii)
            {
                int j = ii % InvertedPendulumQStore.LEN;
                int k = ii / InvertedPendulumQStore.LEN;

                InvertedPendulumState state;
                InvertedPendulumQStore.GetState(j, k, out state);

                for (int i = 0; i < 3; ++i)
                {
                    int action = i - 1;
                    for (int i1 = 0; i1 < 1000; ++i1)
                    {
                        testws.a = state.a + r.NextDouble() / InvertedPendulumQStore.LEN * 2 * Math.PI;
                        testws.w = state.w + r.NextDouble() / InvertedPendulumQStore.LEN * 2 * 3;

                        testws.Step(action);

                        int j2, k2;
                        InvertedPendulumState state2 = new InvertedPendulumState(testws.a, testws.w);
                        InvertedPendulumQStore.GetStateIndices(state2, out j2, out k2);

                        int ii2 = k2 * InvertedPendulumQStore.LEN + j2;

                        Ma[i][ii, ii2] += 0.001f;
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
            int statenum = InvertedPendulumQStore.LEN * InvertedPendulumQStore.LEN;

            InvertedPendulumEnvironment testws = new InvertedPendulumEnvironment();
            R = new Vector(statenum);
            for (int ii = 0; ii < statenum; ++ii)
            {
                int j = ii % InvertedPendulumQStore.LEN;
                int k = ii / InvertedPendulumQStore.LEN;

                InvertedPendulumState state;
                InvertedPendulumQStore.GetState(j, k, out state);

                testws.a = state.a;
                testws.w = state.w;
                R.Elements[ii] = testws.Reward();
            }
        }
    }
}
