using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public struct DataModel
{
    public bool isValid { get; set; }
    public string from { get; set; }
    public string recipient { get; set; }
}

public class Encryption
{
    private readonly string CRYPTOKEY = DateTime.UtcNow.Date.ToString("yyyyMMdd") + "開鎖鑰匙";
    public string PARA_ARG = "encryptMsg";
    public string PARA_FROM = "from";
    public string PARA_RECIPIENT = "recipients";
    public string ASSIGNER = "=";
    private char SEPARATOR = '&';

    public string GetEncryptMsg(string from, string recipients)
    {
        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(recipients))
            return null;

        string encryptMsg = aesEncrypt(PARA_FROM + ASSIGNER + from + SEPARATOR + PARA_RECIPIENT + ASSIGNER + recipients);

        return encryptMsg;
    }

    private Dictionary<string, string> getDecryptMsg(string encryptMsg)
    {
        if (string.IsNullOrWhiteSpace(encryptMsg))
            return null;

        string decryptMsg = aesDecrypt(encryptMsg);

        List<string> paraList = decryptMsg.Split(SEPARATOR).ToList();

        Dictionary<string, string> para = new Dictionary<string, string>();

        foreach (string item in paraList)
        {
            string paraName = item.Substring(0, item.IndexOf(ASSIGNER));
            string paraVal = item.Substring(item.IndexOf(ASSIGNER) + 1);

            if (!para.ContainsKey(paraName))
                para.Add(paraName, paraVal);
        }

        return para;
    }

    private string aesEncrypt(string msg)
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
                encrypt = Convert.ToBase64String(ms.ToArray());
            }
        }
        return encrypt;
    }

    private string aesDecrypt(string msg)
    {
        if (string.IsNullOrEmpty(msg))
            return null;

        string decrypt = string.Empty;

        AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
        byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(CRYPTOKEY));
        byte[] iv = md5.ComputeHash(Encoding.UTF8.GetBytes(CRYPTOKEY));
        aes.Key = key;
        aes.IV = iv;

        byte[] dataByteArray = Convert.FromBase64String(msg);
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

    public DataModel StartToDecryptMsg(string encryptMsg)
    {
        Dictionary<string, string>  decryptedMsg = getDecryptMsg(encryptMsg);
        if (decryptedMsg.Count() < 1 || !decryptedMsg.ContainsKey(PARA_FROM) || !decryptedMsg.ContainsKey(PARA_RECIPIENT))
            return new DataModel();

        DataModel decryptData = new DataModel();
        decryptData.isValid = true;
        decryptData.from = decryptedMsg[PARA_FROM];
        decryptData.recipient = decryptedMsg[PARA_RECIPIENT];
        
        return decryptData;
    }
}
