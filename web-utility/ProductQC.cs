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
            webBrowser_qc.ScriptErrorsSuppressed = true;
            webBrowser_qc.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(DocumentCompletedHandler);
        }

        public void Emailing(string url, string from, string recipients)
        {
            EmailHandling eh = new EmailHandling(webBrowser_qc);
            eh.SendEmail(url, from, recipients);
        }

        private void DocumentCompletedHandler(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string pageHtml = webBrowser_qc.DocumentText;
            this.Close();
        }
    }
}
