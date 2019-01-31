using System.Windows.Forms;
using System.Collections.Generic;
using System;

namespace web_utility
{
    public partial class Frm_ProductQC : Form
    {
        public Frm_ProductQC()
        {
            InitializeComponent();
        }

        public void Emailing(string url, string from, string recipients)
        {
            EmailHandling eh = new EmailHandling(webBrowser_qc);
            eh.SendEmail(url, from, recipients);
        }

    }
}
