using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace I2MTC_SpecIssue_results
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
            Application.Run(new RL_TDlambda());
            //Application.Run(new RL_TDlambda_acrobot());
        }
    }
}
