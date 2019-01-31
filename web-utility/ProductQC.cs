using System.Windows.Forms;
using System.Collections.Generic;

namespace web_utility
{
    public partial class ProductQC : Form
    {
        public ProductQC()
        {
            InitializeComponent();
        }

        public void SendEmail(string url, string from, string recipients)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            EmailHandling emailHandle = new EmailHandling();
            string token = emailHandle.GetEncryptMsg(from, recipients);

            if (string.IsNullOrWhiteSpace(token))
                return;

            webBrowser_qc.Navigate(url + emailHandle.PARA_ARG + emailHandle.ASSIGNER + token);
        }

    }
}
