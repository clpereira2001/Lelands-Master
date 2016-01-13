using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Caching;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Security.Cryptography;
using System.Drawing;
using Vauction.Utils.Perfomance;
using System.Text.RegularExpressions;
using Vauction.Utils.Lib;

namespace Vauction.Controllers
{
  public class ZipController : Controller
  {
    IFileBase file;
    const string PermittedUrls = "|ajax.googleapis.com|s7.addthis.com|www.google-analytics.com";

    public ZipController() : this(null) { }
    public ZipController(IFileBase fileBase)
    {
      this.file = fileBase ?? new FileBase();
    }

    byte[] GetBytesFromCache(string path)
    {
      object fromCache = HttpRuntime.Cache.Get(path);
      if (fromCache == null) return null;
      try
      {
        return fromCache as byte[];
      }
      catch
      {
        //the object in the cache wasn't a byte array that's sad and perplexing... 
        return null;
      }
    }

    #region image
    //Image > using <img src="/Zip/Image?Path=/Public/Images/bird.png" alt="bird" /> 
    public virtual void Image(string path)
    {
      try
      {
        var server = HttpContext.Server;
        string decodedPath = server.UrlDecode(path);
        string mappedPath = server.MapPath(decodedPath);
        FileInfo fi = new FileInfo(mappedPath);
        if (!fi.Exists || System.Drawing.Image.FromFile(fi.FullName) == null) return;
        byte[] imageBytes = GetBytesFromCache(mappedPath);
        if (imageBytes == null)
        {
          imageBytes = file.ReadAllBytes(mappedPath);
          HttpRuntime.Cache.Insert(path, imageBytes, null, /*or: new CacheDependency(mappedPath), */Cache.NoAbsoluteExpiration, TimeSpan.FromDays(60));
        }
        Response.Cache.SetCacheability(HttpCacheability.Public);
        Response.Cache.SetExpires(Cache.NoAbsoluteExpiration);
        Response.AddFileDependency(mappedPath);
        Response.Cache.SetLastModifiedFromFileDependencies();
        Response.AppendHeader("Content-Length", imageBytes.Length.ToString());
        Response.ContentType = fi.Extension == "gif" ? "image/gif" : "image/jpeg";
        Response.OutputStream.Write(imageBytes, 0, imageBytes.Length);
        Response.Flush();
      }
      catch (HttpException) //he
      {
        //Logger.LogException("[just_for_check] "+path, he);
      }
      catch (Exception ex)
      {
        Logger.LogException(path, ex);
      }
    }

    //CropImage > using <img src="/Zip/CropImage?Path=/Public/Images/bird.png&x=5&y=5&width=20&height=20" alt="bird"> 
    public virtual void CropImage(string path, int? x, int? y, int? width, int? height)
    {
      try
      {
        var server = HttpContext.Server;
        string decodedPath = server.UrlDecode(path);
        string mappedPath = server.MapPath(path);
        FileInfo fi = new FileInfo(mappedPath);
        if (!fi.Exists || System.Drawing.Image.FromFile(fi.FullName) == null) return;
        Bitmap src = System.Drawing.Image.FromFile(mappedPath) as Bitmap;
        //get dimensions 
        int posX = x ?? 0;
        int posY = y ?? 0;
        int newWidth = width ?? src.Width - posX;
        int newHeight = height ?? src.Height - posY;
        Rectangle rect = new Rectangle(posX, posY, newWidth, newHeight);
        Bitmap cropped = src.Clone(rect, src.PixelFormat);
        //convert image to byte array 
        MemoryStream ms = new MemoryStream();
        cropped.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        byte[] imageBytes = ms.ToArray();
        //write image to response 
        Response.AddFileDependency(mappedPath);
        Response.Cache.SetLastModifiedFromFileDependencies();
        Response.AppendHeader("Content-Length", imageBytes.Length.ToString());
        Response.ContentType = fi.Extension == "gif" ? "image/gif" : "image/jpeg";
        Response.OutputStream.Write(imageBytes, 0, imageBytes.Length);
        Response.Flush();
      }
      catch (HttpException)
      {
      }
      catch (Exception ex)
      {
        Logger.LogException(path, ex);
      }
    }
    #endregion

    #region css & js
    //MD5Fingerprint
    string MD5Fingerprint(string s)
    {
      var bytes = Encoding.Unicode.GetBytes(s.ToCharArray());
      var hash = new MD5CryptoServiceProvider().ComputeHash(bytes);
      return hash.Aggregate(new StringBuilder(32), (sb, b) => sb.Append(b.ToString("X2"))).ToString();
    }

    //Text
    void Text(string path, bool isJS)
    {
      try
      {
        var server = HttpContext.Server;
        string decodedPath = server.UrlDecode(path);
        if (String.IsNullOrEmpty(decodedPath)) return;
        List<string> urls = new List<string>();
        List<string> paths = new List<string>();
        Regex r = new Regex(@"://(?<host>([a-z\d][-a-z\d]*[a-z\d]\.)*[a-z][-a-z\d]+[a-z])");
        FileInfo fi;
        string[] array = decodedPath.Split('|');
        if (array == null || array.Length == 0) return;
        foreach (string s in array)
        {
          if (!r.IsMatch(s))
          {
            fi = new FileInfo(server.MapPath(server.UrlDecode(s)));
            if (!fi.Exists || ((fi.Extension != ".js" && isJS) || (fi.Extension != ".css" && !isJS))) continue;
            paths.Add(fi.FullName);
          }
          else
          {
            if (PermittedUrls.IndexOf(r.Match(s).Result("${host}")) <= 0) continue;
            urls.Add(s);
          }
        }
        string encodingHeader = Request.Headers["Accept-Encoding"];
        bool gzip = !String.IsNullOrEmpty(encodingHeader) && (encodingHeader.Contains("gzip") || encodingHeader.Contains("deflate"));
        string encoding = gzip ? "gzip" : "utf-8";
        string key = MD5Fingerprint(decodedPath + encoding);
        byte[] bytes = GetBytesFromCache(key);
        if (bytes == null)
        {
          using (var stream = new MemoryStream())
          using (var wstream = gzip ? (Stream)new GZipStream(stream, CompressionMode.Compress) : stream)
          {
            var allFileText = new StringBuilder();
            foreach (string filePath in paths)
            {
              allFileText.Append(file.ReadAllText(filePath));
              allFileText.Append(Environment.NewLine);
            }
            foreach (string url in urls)
            {
              allFileText.Append(file.ReadAllTextFromUrl(url));
              allFileText.Append(Environment.NewLine);
            }
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(allFileText.ToString());
            wstream.Write(utf8Bytes, 0, utf8Bytes.Length);
            wstream.Close();
            bytes = stream.ToArray();
            HttpContext.Cache.Insert(key, bytes, null, /*or: new CacheDependency(paths), */ Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60));
          }
        }
        Response.AddFileDependencies(paths.ToArray());
        Response.ContentType = isJS ? "application/x-javascript" : "text/css";
        Response.AppendHeader("Content-Encoding", encoding);
        Response.Cache.SetCacheability(HttpCacheability.Public);
        Response.Cache.SetExpires(Cache.NoAbsoluteExpiration);
        Response.Cache.SetLastModifiedFromFileDependencies();
        Response.OutputStream.Write(bytes, 0, bytes.Length);
        Response.Flush();
      }
      catch (HttpException)
      {
      }
      catch (Exception ex)
      {
        Logger.LogException(path, ex);
      }
    }

    public void Script(string path)
    {
      Text(path, true);
    }

    public void Style(string path)
    {
      Text(path, false);
    }
    #endregion
  }
}
