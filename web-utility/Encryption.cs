using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace web_utility
{
    public class Encryption
    {
        private readonly string CRYPTOKEY = DateTime.UtcNow.Date.ToString("yyyyMMdd") + "開鎖鑰匙";

        public string AesEncrypt(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                return null;

            string encrypt = string.Empty;

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(CRYPTOKEY));
            byte[] iv = md5.ComputeHash(Encoding.UTF8.GetBytes(CRYPTOKEY));
            aes.Key = key;
            aes.IV = iv;

            byte[] dataByteArray = Encoding.UTF8.GetBytes(msg);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();
                    encrypt = Base64ForUrlEncode(Convert.ToBase64String(ms.ToArray()));
                }
            }
            return encrypt;
        }

        public string AesDecrypt(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                return null;

            string decrypt = string.Empty;
            string urlDecrypt = Base64ForUrlDecode(msg);

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(CRYPTOKEY));
            byte[] iv = md5.ComputeHash(Encoding.UTF8.GetBytes(CRYPTOKEY));
            aes.Key = key;
            aes.IV = iv;

            byte[] dataByteArray = Convert.FromBase64String(urlDecrypt);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();
                    decrypt = Encoding.UTF8.GetString(ms.ToArray());
                }
            }
          
            return decrypt;
        }

        private string Base64ForUrlEncode(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            byte[] encbuff = Encoding.UTF8.GetBytes(str);
            return HttpUtility.UrlEncode(encbuff);
        }

        private string Base64ForUrlDecode(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            byte[] decbuff = HttpUtility.UrlDecodeToBytes(str);
            return Encoding.UTF8.GetString(decbuff);
        }
    }
}
