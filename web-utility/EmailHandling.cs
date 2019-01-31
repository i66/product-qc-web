using System;
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
            {
                Console.WriteLine("Url is not good !! : " + url);
                Environment.Exit(-1);
            }
                
            Encryption secret = new Encryption();
            string token = secret.GetEncryptMsg(from, recipients);

            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("Error happened during encryption !!");
                Environment.Exit(-1);
            }
           
            wb_email.Navigate(url + secret.PARA_ARG + secret.ASSIGNER + token);
        }


    }
}
