using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils;
using System.Runtime.Serialization;

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

    public string UrlLotTitle
    {
      get { return Vauction.Utils.UrlParser.TitleToUrl(LotTitle); }
    }

    public string LotTitle
    {
      get { return String.Format("Lot{0}~{1}", Lot, Title); }
    }

    public LinkParams LinkParams
    {
      get { return new LinkParams { ID = ID, EventTitle = Event.Title, MainCategoryTitle = EventCategory.MainCategory.Title, CategoryTitle = EventCategory.Category.Title }; }
    }

  }
}
