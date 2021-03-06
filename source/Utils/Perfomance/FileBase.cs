﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;

namespace Vauction.Utils.Perfomance
{
  #region IFileBase
  public interface IFileBase
  {
    /// <summary> 
    /// Opens file, reads all of the bytes from the file into an array, closes file. 
    /// </summary> 
    /// <param name="path">The path of a file.</param> 
    byte[] ReadAllBytes(string path);

    /// <summary> 
    /// Reads all of the text from a file into a string. 
    /// </summary> 
    /// <param name="path">The path of the file.</param> 
    string ReadAllText(string path);

    string ReadAllTextFromUrl(string url);
  }
  #endregion

  #region FileBase
  public class FileBase : IFileBase
  {
    public byte[] ReadAllBytes(string path)
    {
      return File.ReadAllBytes(path);
      //byte[] bytes;
      //FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      //int index = 0;
      //long fileLength = fs.Length;
      //if (fileLength > Int32.MaxValue)
      //  throw new IOException("File too long");
      //int count = (int)fileLength;
      //bytes = new byte[count];
      //while (count > 0)
      //{
      //  int n = fs.Read(bytes, index, count);
      //  if (n == 0)
      //    throw new InvalidOperationException("End of file reached before expected");
      //  index += n;
      //  count -= n;
      //}
      //fs.Close();
      //return bytes;
    }

    public string ReadAllText(string path)
    {
      return File.ReadAllText(path);
    }

    public string ReadAllTextFromUrl(string url)
    {
      WebClient client = new WebClient();
      // Add a user agent header in case the requested URI contains a query.
      client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
      Stream data = client.OpenRead(url);
      StreamReader reader = new StreamReader(data);
      string result = reader.ReadToEnd();
      reader.Close();
      data.Close();
      return result;
    }
  }
  #endregion
}