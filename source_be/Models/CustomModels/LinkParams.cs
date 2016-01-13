using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils;

namespace Vauction.Models
{
  [Serializable]
  public class LinkParams
  {
    public long ID { get; set; }
    public short Lot { get; set; }
    public string Title { get; set; }    
    public string EventTitle { get; set; }
    public string MainCategoryTitle { get; set; }
    public string CategoryTitle { get; set; }
    public long Event_ID { get; set; }
    public long EventCategory_ID { get; set; }
    public long MainCategory_ID { get; set; }
    public long Category_ID { get; set; }

    public string EventUrl
    {
      get { return String.IsNullOrEmpty(EventTitle) ? String.Empty : UrlParser.TitleToUrl(EventTitle); }
    }

    public string MainCategoryUrl
    {
      get { return String.IsNullOrEmpty(MainCategoryTitle) ? String.Empty : UrlParser.TitleToUrl(MainCategoryTitle); }
    }

    public string CategoryUrl
    {
      get { return String.IsNullOrEmpty(CategoryTitle) ? String.Empty : UrlParser.TitleToUrl(CategoryTitle); }
    }

    public string GetLotTitleUrl(short lot, string title)
    {
      return UrlParser.TitleToUrl(String.Format("Lot{0}~{1}", lot, title));
    }

    public string LotTitleUrl
    {
      get { return GetLotTitleUrl(Lot, Title); }
    }

    public string AuctionDetailUrl
    {
      get { return AppHelper.GetSiteUrl(String.Format("/Auction/AuctionDetail/{0}/{1}/{2}/{3}/{4}", ID, EventUrl, MainCategoryUrl, CategoryUrl, LotTitleUrl)); }
    }
  }
}
