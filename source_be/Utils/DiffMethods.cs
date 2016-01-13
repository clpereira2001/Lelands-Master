using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Vauction.Utils
{
  public static class DiffMethods
  {
    //CheckAndCreateDirectiory (path)
    public static string CheckAndCreateDirectiory(string path)
    {
      if (!Directory.Exists(path)) Directory.CreateDirectory(path);
      return path;
    }

    //CheckAndCreateDirectiory (path, auction)
    public static string CheckAndCreateDirectiory(string path, long id)
    {
      return CheckAndCreateDirectiory(path, id, AppHelper.CurrentUser.ID);
    }

    //CheckAndCreateDirectiory (path, auction, user_id)
    public static string CheckAndCreateDirectiory(string path, long id, long user_id)
    {
      if (id <= 0)
      {
        string pathForAddNew = Path.Combine(path, "UserImages");
        pathForAddNew = Path.Combine(pathForAddNew, user_id.ToString());
        if (!Directory.Exists(pathForAddNew))
          Directory.CreateDirectory(pathForAddNew);
        return pathForAddNew;
      }

      string subPath = Path.Combine(path, (id / 1000000).ToString());
      if (!Directory.Exists(subPath))
      {
        Directory.CreateDirectory(subPath);
      }
      subPath = Path.Combine(subPath, (id / 1000).ToString());
      if (!Directory.Exists(subPath))
      {
        Directory.CreateDirectory(subPath);
      }
      subPath = Path.Combine(subPath, (id).ToString());
      if (!Directory.Exists(subPath))
      {
        Directory.CreateDirectory(subPath);
      }
      return subPath;
    }

    //CheckOrCreateDirectory (auction)
    public static string CheckOrCreateDirectory(long auction_id)
    {
      return CheckAndCreateDirectiory(Consts.GetVauctionFrontendDir + @"\public\AuctionImages", auction_id, AppHelper.CurrentUser.ID);
    }

    //CheckOrCreateDirectory (auction)
    public static string CheckOrCreateDirectory(long auction_id, long user_id)
    {
      return CheckAndCreateDirectiory(Consts.GetVauctionFrontendDir + @"\public\AuctionImages", auction_id, user_id);
    }

    //CopyImages
    public static bool CopyImages(long auctionSource_id, long auctionDestination_id)
    {
      bool res = true;
      try
      {
        string path = CheckAndCreateDirectiory(Consts.GetVauctionFrontendDir + @"\public\AuctionImages", auctionSource_id);
        if (!Directory.Exists(path)) return false;
        string destPath = CheckAndCreateDirectiory(Consts.GetVauctionFrontendDir + @"\public\AuctionImages", auctionDestination_id);
        List<string> files = Directory.GetFiles(path).ToList();
        foreach (string fi in files)
          File.Copy(fi, Path.Combine(destPath, Path.GetFileName(fi)));
      }
      catch
      {
        res = false;
      }
      return res;
    }

    //DeleteImagesFromDisk
    public static bool DeleteImagesFromDisk(long auction_id)
    {
      bool res = true;
      try
      {
        string path = CheckAndCreateDirectiory(Consts.GetVauctionFrontendDir + @"\public\AuctionImages", auction_id);
        if (!Directory.Exists(path)) return false;
        Directory.Delete(path, true);
      }
      catch
      {
        res = false;
      }
      return res;
    }

    //GetAuctionImageDirForUser
    public static string GetAuctionImageDirForUser(long user_id)
    {
      return CheckAndCreateDirectiory(Consts.GetVauctionFrontendDir + @"\public\AuctionImages\UserImages\" + user_id.ToString());
    }

    //GetUploadedImageDir
    public static string GetUploadedImageDir()
    {
      return HttpContext.Current.Server.MapPath(@"\Pool\UploadedImages");
    }

    //GetAuctionImageWebForUser
    public static string GetAuctionImageWebForUser(long user_id, string filename)
    {
      return Consts.SiteUrl + @"/public/AuctionImages/UserImages/" + user_id.ToString() + @"/" + filename;
    }

    //GetUploadedImageWeb
    public static string GetUploadedImageWeb(string filename)
    {
      return @"/Pool/UploadedImages/" + filename;
    }

    //GetDirectiory
    public static string GetWebDirectiory(string path, Int64 id)
    {
      return path + "/" + (id / 1000000).ToString() + "/" + (id / 1000).ToString() + "/" + id.ToString();
    }

    //GetAuctionImagePath
    public static string GetAuctionImageWebPath(Int64 auction_id, string filename)
    {
      return GetWebDirectiory(Consts.SiteUrl + @"/public/AuctionImages", auction_id) + "/" + filename;
    }

    //ChangeImageSize
    public static System.Drawing.Image ChangeImageSize(System.Drawing.Image imgPhoto, int? Width, int? Height, bool IsEqual)
    {
      int sourceWidth = imgPhoto.Width;
      int sourceHeight = imgPhoto.Height;
      int sourceX = 0;
      int sourceY = 0;
      int destX = 0;
      int destY = 0;

      float nPercent = 0;
      float nPercentW = 0;
      float nPercentH = 0;

      int w, h;
      if (Width.HasValue && !Height.HasValue)
      {
        w = Width.Value;
        nPercentW = ((float)w / (float)sourceWidth);
        h = (IsEqual) ? w : (int)((float)nPercentW * (float)sourceHeight);
        nPercentH = ((float)h / (float)sourceHeight);
      }
      else if (!Width.HasValue && Height.HasValue)
      {
        h = Height.Value;
        nPercentH = ((float)h / (float)sourceHeight);
        w = (IsEqual) ? h : (int)((float)nPercentH * (float)sourceWidth);
        nPercentW = ((float)w / (float)sourceWidth);
      }
      else
      {
        w = Width.Value;
        h = Height.Value;
        nPercentW = ((float)w / (float)sourceWidth);
        nPercentH = ((float)h / (float)sourceHeight);
      }

      if (nPercentH < nPercentW)
      {
        nPercent = nPercentH;
        destX = System.Convert.ToInt16((w - (sourceWidth * nPercent)) * 0.5);
      }
      else
      {
        nPercent = nPercentW;
        destY = System.Convert.ToInt16((h - (sourceHeight * nPercent)) * 0.5);
      }

      int destWidth = (int)(sourceWidth * nPercent);
      int destHeight = (int)(sourceHeight * nPercent);

      System.Drawing.Bitmap bmPhoto = new System.Drawing.Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      bmPhoto.SetResolution(imgPhoto.VerticalResolution, imgPhoto.HorizontalResolution);

      System.Drawing.Graphics grPhoto = System.Drawing.Graphics.FromImage(bmPhoto);
      grPhoto.Clear(System.Drawing.Color.White);
      grPhoto.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
      grPhoto.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      grPhoto.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
      grPhoto.DrawImage(imgPhoto, new System.Drawing.Rectangle(destX, destY, destWidth, destHeight), new System.Drawing.Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), System.Drawing.GraphicsUnit.Pixel);

      grPhoto.Dispose();
      return bmPhoto;
    }

    //GetUploadedImageDir
    public static string GetUploadedEmailImageDir()
    {
      return Consts.GetVauctionFrontendDir + @"\public\images\emails";
    }

    //GetUploadedImageWeb
    public static string GetUploadedEmailImageWeb(string filename)
    {
      return Consts.SiteUrl + @"/public/images/emails/" + filename;
    }

    //GetUploadedImageDir
    public static string GetUploadedDifferentImageDir()
    {
      return Consts.GetVauctionFrontendDir + @"\public\images\different";
    }

    //GetUploadedImageWeb
    public static string GetUploadedDifferentImageWeb(string filename)
    {
      return Consts.SiteUrl + @"/public/images/different/" + filename;
    }

    //GetUploadedImageDir
    public static string GetUploadedHomepageImageDir()
    {
      return Consts.GetVauctionFrontendDir + @"\public\images\homepage";
    }

    //GetUploadedImageWeb
    public static string GetUploadedHomepageImageWeb(string filename)
    {
      return Consts.SiteUrl + @"/public/images/homepage/" + filename;
    }

    //TemplateDifferentOnDisk
    public static string TemplateDifferentOnDisk(string filename)
    {
      return String.Format("{0}/Templates/Different/{1}", Consts.GetVauctionFrontendDir, filename);
    }

    //ConsignmentContractOnDisk
    public static string ConsignmentContractOnDisk(long id, string filename)
    {
      string path = String.Format("{0}/Public/Files/ConsignmentContract/{1}/{2}/{3}", Consts.GetVauctionFrontendDir,
        id / 1000000, (id / 1000), id);
      if (!Directory.Exists(path)) Directory.CreateDirectory(path);
      return String.Format("{0}/{1}", path, filename);
    }

    //SignatureImagesWeb
    public static string SignatureImagesWeb(string filename)
    {
      return String.Format("{0}/Public/Files/Signature/{1}", Consts.SiteUrl, filename);
    }

    //SignatureImagesOnDisk
    public static string SignatureImagesOnDisk(string filename)
    {
      return String.Format("{0}/Public/Files/Signature/{1}", Consts.GetVauctionFrontendDir, filename);
    }

    public static string PublicImages(string filename)
    {
      return String.Format("{0}/Public/Images/{1}", Consts.SiteUrl, filename);
    }

    //PublicImagesOnDisk
    public static string PublicImagesOnDisk(string filename)
    {
      return String.Format("{0}/Public/Images/{1}", Consts.GetVauctionFrontendDir, filename);
    }
  }
}