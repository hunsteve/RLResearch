using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

namespace ContinuousRLTest
{
    public partial class Form1 : Form
    {
        InvertedPendulumEnvironment ws;
        InvertedPendulumEnvironment oppWs;

        TcpClient net;
        bool isServer = false;


        public Form1()
        {
            InitializeComponent();
            ws = customControl11.Workspace = new InvertedPendulumEnvironment();
            oppWs = customControl12.Workspace = new InvertedPendulumEnvironment();            
            this.KeyPreview = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (isServer)
            {                
                net.Client.Send(BitConverter.GetBytes(ws.a));

                byte[] buf = new byte[4];
                net.Client.Receive(buf);
                oppWs.a = BitConverter.ToSingle(buf, 0);
            }
            else
            {
                byte[] buf = new byte[4];
                net.Client.Receive(buf);
                oppWs.a = BitConverter.ToSingle(buf, 0);               

                net.Client.Send(BitConverter.GetBytes(ws.a));
            }

            int M = 0;
            if (leftDown) M += -1;
            if (rightDown) M += 1;
            ws.Step(M);
            customControl11.Invalidate();
            customControl12.Invalidate();

        }

        bool leftDown = false;
        bool rightDown = false;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {            
            if (keyData == Keys.Left) leftDown = true;
            if (keyData == Keys.Right) rightDown = true;

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) leftDown = true;
            if (e.KeyCode == Keys.Right) rightDown = true;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) leftDown = false;
            if (e.KeyCode == Keys.Right) rightDown = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            net = new TcpClient(textBox1.Text, 32123);
            isServer = false;
            timer1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            TcpListener server = new TcpListener(IPAddress.Any, 32123);
            server.Start();
            net = server.AcceptTcpClient();
            isServer = true;
            timer1.Enabled = true;
        }


    }
}
