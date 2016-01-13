using System.IO;
using System.Security.Cryptography;

namespace QControls.Security
{
	public class TripleDESECB
	{
		public static string EncryptIt(string Password)
		{

			byte[] rgbKey = System.Text.ASCIIEncoding.ASCII.GetBytes("673dt06m");
			byte[] rgbIV = System.Text.ASCIIEncoding.ASCII.GetBytes("asejdt4f");
			CryptoStream cryptoStream = null;
			MemoryStream memoryStream = null;
			string toDecrypt = string.Empty;

			byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(Password);
			//1024-bit encryption
			memoryStream = new MemoryStream(1024);
			DESCryptoServiceProvider desCryptoServiceProvider =
			new DESCryptoServiceProvider();

			cryptoStream = new CryptoStream(memoryStream,
			desCryptoServiceProvider.CreateEncryptor(rgbKey, rgbIV),
			CryptoStreamMode.Write);

			cryptoStream.Write(data, 0, data.Length);
			cryptoStream.FlushFinalBlock();

			byte[] result = new byte[(int)memoryStream.Position];
			memoryStream.Position = 0;
			memoryStream.Read(result, 0, result.Length);
			cryptoStream.Close();
			toDecrypt = System.Convert.ToBase64String(result);

			return toDecrypt;// DecryptIt(toDecrypt);
		}

		public static string DecryptIt(string toDecrypt)
		{

            byte[] rgbKey = System.Text.ASCIIEncoding.ASCII.GetBytes("673dt06m");
            byte[] rgbIV = System.Text.ASCIIEncoding.ASCII.GetBytes("asejdt4f");
			CryptoStream cryptoStream = null;
			MemoryStream memoryStream = null;
			string decrypted = string.Empty;

			byte[] data = System.Convert.FromBase64String(toDecrypt);
			memoryStream = new MemoryStream(data.Length);

			DESCryptoServiceProvider desCryptoServiceProvider =
			new DESCryptoServiceProvider();

			cryptoStream = new CryptoStream(memoryStream,
			desCryptoServiceProvider.CreateDecryptor(rgbKey, rgbIV),
			CryptoStreamMode.Read);

			memoryStream.Write(data, 0, data.Length);
			memoryStream.Position = 0;

			decrypted = new StreamReader(cryptoStream).ReadToEnd();

			cryptoStream.Close();

			return decrypted;
		}
	}
}
