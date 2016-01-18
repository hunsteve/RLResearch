namespace ContinuousRLTest
{
    partial class Form3
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.customControl11 = new ContinuousRLTest.CustomControl1();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // customControl11
            // 
            this.customControl11.Location = new System.Drawing.Point(12, 24);
            this.customControl11.Name = "customControl11";
            this.customControl11.Size = new System.Drawing.Size(351, 339);
            this.customControl11.TabIndex = 1;
            this.customControl11.Text = "customControl11";
            this.customControl11.Workspace = null;
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 394);
            this.Controls.Add(this.customControl11);
            this.Name = "Form3";
            this.Text = "Form3";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form3_KeyUp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form3_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private CustomControl1 customControl11;
        private System.Windows.Forms.Timer timer1;
    }
}