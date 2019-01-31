using System.Collections.Generic;
using System.Linq;

namespace web_utility
{
    public class EmailHandling
    {
        public string PARA_ARG = "encryptMsg";
        public string PARA_FROM = "from";
        public string PARA_RECIPIENT = "recipients";
        public string ASSIGNER = "=";
        private char SEPARATOR = '&';

        public string GetEncryptMsg(string from, string recipients)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(recipients))
                return null;

            Encryption encrypt = new Encryption();
            string encryptMsg = encrypt.AesEncrypt(PARA_FROM + ASSIGNER + from + SEPARATOR + PARA_RECIPIENT + ASSIGNER + recipients);

            return encryptMsg;
        }

        public Dictionary<string,string> GetDecryptMsg(string encryptMsg)
        {
            if (string.IsNullOrWhiteSpace(encryptMsg))
                return null;

            Encryption encrypt = new Encryption();
            string decryptMsg  = encrypt.AesDecrypt(encryptMsg);

            List<string> paraList = decryptMsg.Split(SEPARATOR).ToList();

            Dictionary<string, string> para = new Dictionary<string, string>();

            foreach (string item in paraList)
            {
                string paraName = item.Substring(0, item.IndexOf(ASSIGNER));
                string paraVal = item.Substring(item.IndexOf(ASSIGNER) + 1);

                if(!para.ContainsKey(paraName))
                    para.Add(paraName, paraVal);
            }

            return para;
        }
    }
}
