using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace web_utility
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();

            if ((args.Length != 4))
            {
                Environment.Exit(-1);
            }
            else
            {
                string url = args[1];
                string from = args[2];
                string recipients = args[3];

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Frm_ProductQC Frm_QC = new Frm_ProductQC();
                Frm_QC.Emailing(url, from, recipients);

                Application.Run(Frm_QC);
                Environment.Exit(0);
            }
           
        }
    }
}
