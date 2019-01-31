using System.Windows.Forms;

namespace web_utility
{
    public class EmailHandling
    {
        private static WebBrowser wb_email;

        public EmailHandling(WebBrowser wb)
        {
            wb_email = wb;
        }       

        public void SendEmail(string url, string from, string recipients)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            Encryption secret = new Encryption();
            string token = secret.GetEncryptMsg(from, recipients);

            if (string.IsNullOrWhiteSpace(token))
                return;

            wb_email.Navigate(url + secret.PARA_ARG + secret.ASSIGNER + token);                       
        }


    }
}
