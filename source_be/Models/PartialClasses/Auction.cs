using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils;
using System.Runtime.Serialization;
using System.Text;

namespace Vauction.Models
{
  [Serializable]
  public partial class Auction : IAuction
  {
    public string DefaultThumb
    {
      get
      {
        if (this.Images.Count() != 0)
        {
          Image deg = this.Images.Where(i => i.Default).FirstOrDefault();          
          return (deg != null) ? deg.ThumbNailPath : Images.FirstOrDefault().ThumbNailPath;
        }
        else
        {
          return String.Empty;
        }
      }
    }

    public string DefaultImage
    {
      get
      {
        if (this.Images.Count() != 0)
        {
          Image deg = this.Images.Where(i => i.Default).FirstOrDefault();
          return (deg != null) ? deg.PicturePath : Images.FirstOrDefault().PicturePath;
        }
        else
        {
          return String.Empty;
        }
      }
    }
    
    public string LotForLink
    {
      get { return (!Lot.HasValue || String.IsNullOrEmpty(Lot.Value.ToString())) ? "---" :Lot.Value.ToString(); }
    }

    public bool IsUnsoldOrPulledOut
    {
      get { return IsUnsold || (PulledOut.HasValue && PulledOut.Value); }
    }

    public string UnsoldOrPulledOut
    {
      get { return IsUnsold ? "UNSOLD" : "WITHDRAWN"; }
    }

    
    public string LotTitle
    {
      get { return String.Format("Lot{0}~{1}", Lot, Title); }
    }

    public string UrlLotTitle
    {
      get {throw new NotImplementedException();} 
    }

    public string DescriptionWithoutTags
    {
      get
      {
        StringBuilder sb = new StringBuilder(Description);
        sb.Replace("<", "&lt;");
        sb.Replace(">", "&gt;");
        return sb.ToString();
      }
    }

    public string DescriptionWithoutTagsShort
    {
      get
      {
        StringBuilder sb = new StringBuilder(DescriptionWithoutTags);
        if (sb.Length > 128) { sb.Remove(100, sb.Length - 100); sb.Append(" ... "); }
        return sb.ToString();
      }
    }

    public string ShortPriorityDescription
    {
      get
      {
        string result = "SD";
        switch (Priority)
        {
          case 1: result = "FP"; break;
          case 2: result = "HP"; break;
          case 3: result = "QP"; break;
          default: break;
        }
        return result;
      }
    }

    public string UserWhoEntered
    {
      get { return (EnteredBy.HasValue) ? EnteredByUser.Login : String.Empty; }
    }
  }
}
