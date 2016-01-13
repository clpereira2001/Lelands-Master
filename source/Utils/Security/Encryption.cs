using System;
using System.Security.Cryptography;
using System.Text;

namespace Vauction.Utils.Security
{
  public static class Encryption
  {
    //GetHashedKey
    private static byte[] GetHashedKey(string key)
    {
      MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
      byte[] keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
      hashmd5.Clear();
      return keyArray;
    }

    //GetCryptoTransform
    private static ICryptoTransform GetCryptoTransform(string key, bool isencryption)
    {
      TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
      {
        Key = GetHashedKey(key),
        Mode = CipherMode.ECB,
        Padding = PaddingMode.PKCS7
      };
      ICryptoTransform result = isencryption? tdes.CreateEncryptor() : tdes.CreateDecryptor();
      tdes.Clear();
      return result;
    }

    //EncryptPassword
    public static string EncryptPassword(string password, string key)
    {
      byte[] bArray = Encoding.UTF8.GetBytes(password);
      ICryptoTransform cTransform = GetCryptoTransform(key, true);
      bArray = cTransform.TransformFinalBlock(bArray, 0, bArray.Length);
      return Convert.ToBase64String(bArray, 0, bArray.Length);
    }

    //DecryptPassword
    public static string DecryptPassword(string password, string key)
    {
      byte[] bArray = Convert.FromBase64String(password);
      ICryptoTransform cTransform = GetCryptoTransform(key, false);
      bArray = cTransform.TransformFinalBlock(bArray, 0, bArray.Length);
      return Encoding.UTF8.GetString(bArray, 0, bArray.Length);
    }
  }
}
