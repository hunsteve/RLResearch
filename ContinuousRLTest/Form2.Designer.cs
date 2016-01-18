namespace ContinuousRLTest
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.esigmnViewer1 = new ContinuousRLTest.ESIGMNViewer();
            this.customControl31 = new ContinuousRLTest.CustomControl3();
            this.customControl21 = new ContinuousRLTest.CustomControl2();
            this.customControl11 = new ContinuousRLTest.CustomControl1();
            this.button5 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(413, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(413, 51);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(121, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "RealTimeStart";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // timer2
            // 
            this.timer2.Interval = 10;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(413, 195);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(121, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "Generate M matrices";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(413, 155);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(121, 23);
            this.button4.TabIndex = 5;
            this.button4.Text = "ADP start";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // timer3
            // 
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // esigmnViewer1
            // 
            this.esigmnViewer1.Location = new System.Drawing.Point(540, 12);
            this.esigmnViewer1.Name = "esigmnViewer1";
            this.esigmnViewer1.QStore = null;
            this.esigmnViewer1.Size = new System.Drawing.Size(289, 281);
            this.esigmnViewer1.TabIndex = 7;
            this.esigmnViewer1.Text = "esigmnViewer1";
            this.esigmnViewer1.Workspace = null;
            // 
            // customControl31
            // 
            this.customControl31.IADP = null;
            this.customControl31.Location = new System.Drawing.Point(1058, 507);
            this.customControl31.Name = "customControl31";
            this.customControl31.Size = new System.Drawing.Size(82, 55);
            this.customControl31.TabIndex = 6;
            this.customControl31.Text = "customControl31";
            // 
            // customControl21
            // 
            this.customControl21.Location = new System.Drawing.Point(916, 34);
            this.customControl21.Name = "customControl21";
            this.customControl21.QStore = null;
            this.customControl21.Size = new System.Drawing.Size(413, 413);
            this.customControl21.TabIndex = 2;
            this.customControl21.Text = "customControl21";
            this.customControl21.Workspace = null;
            // 
            // customControl11
            // 
            this.customControl11.Location = new System.Drawing.Point(12, 12);
            this.customControl11.Name = "customControl11";
            this.customControl11.Size = new System.Drawing.Size(385, 413);
            this.customControl11.TabIndex = 0;
            this.customControl11.Text = "customControl11";
            this.customControl11.Workspace = null;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(261, 497);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 8;
            this.button5.Text = "button5";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1170, 607);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.esigmnViewer1);
            this.Controls.Add(this.customControl31);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.customControl21);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.customControl11);
            this.Name = "Form2";
            this.Text = "Form2";
            this.ResumeLayout(false);

        }

        #endregion

        private CustomControl1 customControl11;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer timer1;
        private CustomControl2 customControl21;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Timer timer3;
        private CustomControl3 customControl31;
        private ESIGMNViewer esigmnViewer1;
        private System.Windows.Forms.Button button5;
    }
}