using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Vauction.Models
{
  [Serializable]
  partial class Image : IImage
  {
    public string DefaultString
    {
      get
      {
        if (Default)
          return "Default";
        return "thumb";
      }
    }
    public string FullThumbNailPath(Int64 Id, string folder)
    {
      if (this.ThumbNailPath != null && ThumbNailPath != string.Empty)
      {
        string path = string.Format("../../public/{2}/{0}/{1}", (Id / 1000000).ToString() + "/" + (Id / 1000).ToString() + "/" + Id.ToString(), ThumbNailPath, folder);
        return path;
      }
      return "";
    }
    public string FullPicturePath(Int64 Id, string folder)
    {
      if (this.PicturePath != null && PicturePath != string.Empty)
      {
        string path = string.Format("../../public/{2}/{0}/{1}", (Id / 1000000).ToString() + "/" + (Id / 1000).ToString() + "/" + Id.ToString(), PicturePath, folder);
        return path;
      }
      return "";
    }
  }
}
