using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Web.Mvc;
using Vauction.Utils;
using Vauction.Utils.Perfomance;

namespace Vauction.Models
{
  public class CategoryRepository : ICategoryRepository
  {
    private VauctionDataContext dataContext;
    private ICacheDataProvider CacheRepository;

    public CategoryRepository(VauctionDataContext dataContext, ICacheDataProvider CacheRepository)
    {
      this.dataContext = dataContext;
      this.CacheRepository = CacheRepository;
    }

    //GetCategoriesForCategoriesPage
    public string GetCategoriesForCategoriesPage(long eventID, bool onlyLeafs)
    {
      List<IGrouping<int, spCategory_View_CategoriesForEventResult>> rootCategories = dataContext.spCategory_View_CategoriesForEvent(eventID, null, false).GroupBy(Q => Q.MainCategory_ID).ToList();
      if (!rootCategories.Any()) return String.Empty;
      List<IdTitleCount> tags = dataContext.spGetTagsForEvent(eventID, false).Select(t => new IdTitleCount { ID = t.ID, Title = t.Title, Count = t.AuctionCount.GetValueOrDefault(0) }).ToList();
      StringBuilder sb = new StringBuilder();

      int A;
      List<spCategory_View_CategoriesForEventResult> list;
      if (onlyLeafs)
      {
        sb.Append("<div><table style=\"table-layout:fixed\"><colgroup><col width=\"375px\" /><col width=\"375px\" /></colgroup>");
      }
      foreach (IGrouping<int, spCategory_View_CategoriesForEventResult> category in rootCategories)
      {
        list = category.ToList();
        A = (int)Math.Ceiling(list.Count * 0.5);
        if (!onlyLeafs)
        {
          sb.AppendFormat("<div><u><a href='/Auction/CategoryView/{0}/{1}/{2}' title='' class = \"RootCatLink\">{3}</a></u>", list[0].EventCategory_ID, UrlParser.TitleToUrl(list[0].MainCategoryTitle), UrlParser.TitleToUrl(list[0].CategoryTitle), list[0].MainCategoryTitle);
          sb.Append("<table style=\"table-layout:fixed\"><colgroup><col width=\"375px\" /><col width=\"375px\" /></colgroup>");
        }
        for (int i = 0; i < A; i++)
        {
          sb.Append("<tr><td>");
          sb.AppendFormat("<a href='/Auction/CategoryView/{0}/{1}/{2}/{3}' title='' class=\"SubCatLink\" >{4}</a>", list[i].EventCategory_ID, UrlParser.TitleToUrl(list[i].EventTitle), UrlParser.TitleToUrl(list[i].MainCategoryTitle), UrlParser.TitleToUrl(list[i].CategoryTitle), list[i].CategoryTitle + " <span class=\"auctionCount\">(" + list[i].AuctionCount + ")</span>");
          sb.Append("</td><td>");
          if (i + A < list.Count)
          {
            sb.AppendFormat("<a href='/Auction/CategoryView/{0}/{1}/{2}/{3}' title='' class=\"SubCatLink\" >{4}</a>", list[i + A].EventCategory_ID, UrlParser.TitleToUrl(list[i + A].EventTitle), UrlParser.TitleToUrl(list[i + A].MainCategoryTitle), UrlParser.TitleToUrl(list[i + A].CategoryTitle), list[i + A].CategoryTitle + " <span class=\"auctionCount\">(" + list[i + A].AuctionCount + ")</span>");
          }
          else sb.Append("&nbsp;");
          sb.Append("</td></tr>");
        }
        if (!onlyLeafs)
        {
          sb.Append("</table></div>");
        }
      }
      if (!onlyLeafs)
      {
        foreach (var tag in tags)
        {
          sb.AppendFormat("<a class='SubCatLink' href='/Auction/Tcategory/{0}/{2}'>{1}</a>", tag.ID, tag.Title, UrlParser.TitleToUrl(tag.Title));
        }
      }
      else
      {
        foreach (var tag in tags)
        {
          sb.Append("<tr><td>");
          sb.AppendFormat("<a class='SubCatLink' href='/Auction/Tcategory/{0}/{3}'>{1} ({2})</a>", tag.ID, tag.Title, tag.Count, UrlParser.TitleToUrl(tag.Title));
          sb.Append("</td></tr>");
        }
      }
      if (onlyLeafs)
      {
        sb.Append("</table></div>");
      }
      return sb.ToString();
    }

    //GetListForCategory
    public IEnumerable<IdTitle> GetListForCategory(long eventID)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES, "GETLISTFORCATEGORY",
                                                new object[] { eventID }, CachingExpirationTime.Hours_01);
      List<IdTitle> result = CacheRepository.Get(dco) as List<IdTitle>;
      if (result != null && result.Any()) return result.AsEnumerable();
      result = (from p in dataContext.spCategory_View_MainAndCategories(eventID)
                select new IdTitle
                {
                  ID = p.EventCategory_ID,
                  Title = String.Format("{0} > {1}", p.MainCategoryTitle, p.CategoryTitle)
                }).ToList();
      if (result.Any())
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result.AsEnumerable();
    }

    //GetCategoriesForEvent
    public List<CategoryMenuItem> GetCategoriesForEvent(long eventID, bool ispastevent, out IEnumerable<IdTitleCount> tags)
    {
      tags = dataContext.spGetTagsForEvent(eventID, false).Select(t => new IdTitleCount { ID = t.ID, Title = t.Title, Count = t.AuctionCount.GetValueOrDefault(0) }).ToList();
      dataContext.CommandTimeout = 600000;
      return (from p in dataContext.spCategory_View_CategoriesForEvent(eventID, null, ispastevent)
              select new CategoryMenuItem
              {
                EventCategory_ID = p.EventCategory_ID,
                MainCategory_ID = p.MainCategory_ID,
                Category_ID = p.Category_ID,
                AuctionCount = p.AuctionCount,
                LinkParams = new LinkParams { ID = p.EventCategory_ID, EventTitle = p.EventTitle, MainCategoryTitle = p.MainCategoryTitle, CategoryTitle = p.CategoryTitle }
              }).ToList();
    }

    //GetCategoriesForCategoriesViewPage
    public string GetCategoriesForCategoriesByMainCategory(long event_id, int maincategory_id)
    {
      List<spCategory_View_CategoriesForEventResult> list = dataContext.spCategory_View_CategoriesForEvent(event_id, maincategory_id, false).ToList();
      if (list.Count() == 0) return String.Empty;

      StringBuilder sb = new StringBuilder();
      int A;
      A = (int)Math.Ceiling(list.Count * 0.5);
      sb.Append("<table style=\"table-layout:fixed\"><colgroup><col width=\"375px\" /><col width=\"375px\" /></colgroup>");
      for (int i = 0; i < A; i++)
      {
        sb.Append("<tr><td>");
        sb.AppendFormat("<a href='/Auction/CategoryView/{0}/{1}/{2}/{3}' title='' class=\"SubCatLink\" >{4}</a>", list[i].EventCategory_ID, UrlParser.TitleToUrl(list[i].EventTitle), UrlParser.TitleToUrl(list[i].MainCategoryTitle), UrlParser.TitleToUrl(list[i].CategoryTitle), list[i].CategoryTitle + " <span class=\"auctionCount\">(" + list[i].AuctionCount.ToString() + ")</span>");
        sb.Append("</td><td>");
        if (i + A < list.Count)
        {
          sb.AppendFormat("<a href='/Auction/CategoryView/{0}/{1}/{2}/{3}' title='' class=\"SubCatLink\">{4}</a>", list[i + A].EventCategory_ID, UrlParser.TitleToUrl(list[i + A].EventTitle), UrlParser.TitleToUrl(list[i + A].MainCategoryTitle), UrlParser.TitleToUrl(list[i + A].CategoryTitle), list[i + A].CategoryTitle + " <span class=\"auctionCount\">(" + list[i + A].AuctionCount.ToString() + ")</span>");
        }
        else sb.Append("&nbsp;");
        sb.Append("</td><td>");
      }
      sb.Append("</table>");
      return sb.ToString();
    }

    #region GetByID (MainCategory, Category, EventCategory)
    //GetEventCategoryById
    public EventCategory GetEventCategoryById(long eventcategory_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES, "GETEVENTCATEGORYBYID",
                                                new object[] { eventcategory_id }, CachingExpirationTime.Hours_01);
      EventCategory ec = CacheRepository.Get(dco) as EventCategory;
      if (ec != null) return ec;
      ec = dataContext.spSelect_EventCategory(eventcategory_id).FirstOrDefault();
      if (ec != null)
      {
        dco.Data = ec;
        CacheRepository.Add(dco);
      }
      return ec;
    }

    //GetMainCategoryById
    public MainCategory GetMainCategoryById(long maincategory_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES, "GETMAINCATEGORYBYID",
                                                new object[] { maincategory_id }, CachingExpirationTime.Hours_01);
      MainCategory mc = CacheRepository.Get(dco) as MainCategory;
      if (mc != null) return mc;
      mc = dataContext.spSelect_MainCategory(maincategory_id).FirstOrDefault();
      if (mc != null)
      {
        dco.Data = mc;
        CacheRepository.Add(dco);
      }
      return mc;
    }

    //GetEventCategoryByMainCategory
    public EventCategory GetEventCategoryByMainCategory(long maincategory_id, long event_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES, "GETEVENTCATEGORYBYMAINCATEGORY",
                                                new object[] { maincategory_id, event_id }, CachingExpirationTime.Hours_01);
      EventCategory ec = CacheRepository.Get(dco) as EventCategory;
      if (ec != null) return ec;
      ec = dataContext.spEventCategory_ByMainCategory(event_id, maincategory_id).FirstOrDefault();
      if (ec != null)
      {
        dco.Data = ec;
        CacheRepository.Add(dco);
      }
      return ec;
    }
    #endregion

    #region GetCategoriesTree
    //GetCategoriesTreeByEvent
    public string GetCategoriesTreeByEvent(long? event_id)
    {
      if (!event_id.HasValue)
      {
        EventRepository er = new EventRepository(dataContext, CacheRepository);
        event_id = er.GetCurrent().ID;
      }
      List<IGrouping<int, spCategory_View_CategoriesForEventResult>> rootCategories = dataContext.spCategory_View_CategoriesForEvent(event_id, null, false).GroupBy(Q => Q.MainCategory_ID).ToList();
      if (rootCategories.Count() == 0) return String.Empty;
      StringBuilder sb = new StringBuilder();

      sb.AppendLine("<ol class='category_list_main'><li class='category_list_header_container'><div class='category_list_header'>" + rootCategories[0].FirstOrDefault().EventTitle + "</div></li>");

      spCategory_View_CategoriesForEventResult item;
      List<spCategory_View_CategoriesForEventResult> list;
      foreach (IGrouping<int, spCategory_View_CategoriesForEventResult> category in rootCategories)
      {
        item = category.FirstOrDefault();
        sb.AppendFormat("<li><div class='category_list_head' id='dv{0}'><img id='imgCollapse' src='{2}' /><img id='imgExpand' src='{3}' style='display:none' />&nbsp;{1}</div>", item.MainCategory_ID, item.MainCategoryTitle, AppHelper.CompressImage("arrow_collapse.png"), AppHelper.CompressImage("arrow_expand.png"));
        list = category.ToList();
        sb.AppendLine("<div class='category_list_body'><ul>");
        foreach (spCategory_View_CategoriesForEventResult ec in list)
        {
          sb.AppendLine("<li>");
          sb.AppendFormat("<a href='/Auction/CategoryView/{0}/{1}/{2}/{3}'>{4}</a> <font>[{5}]</font>", ec.EventCategory_ID, UrlParser.TitleToUrl(ec.EventTitle), UrlParser.TitleToUrl(ec.MainCategoryTitle), UrlParser.TitleToUrl(ec.CategoryTitle), ec.CategoryTitle, ec.AuctionCount);
          sb.AppendLine("</li>");
        }
        sb.AppendLine("</ul></div>");
        sb.AppendLine("</li>");
      }
      sb.AppendLine("<li class='category_list_footer'>&nbsp;</li>");
      sb.AppendLine("</ol>");
      return sb.ToString();
    }
    //GetCategoriesTree (isdynamic, event_id)
    public string GetCategoriesTree(long? event_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES, "GETCATEGORIESTREE",
                                                new object[] { event_id.GetValueOrDefault(0) }, CachingExpirationTime.Hours_01);
      string res = CacheRepository.Get(dco) as string;
      if (!String.IsNullOrEmpty(res)) return res;
      StringBuilder sb = new StringBuilder();
      FileInfo fi = new FileInfo(System.Web.HttpContext.Current.Server.MapPath(@"~\Templates\Different\CategoriesTree.txt"));
      if (!fi.Exists)
        res = GetCategoriesTreeByEvent(event_id);
      else
      {
        TextReader fileReader = new StreamReader(fi.FullName);
        sb.Append(fileReader.ReadToEnd());
        fileReader.Close();
        res = sb.Length > 0 ? sb.ToString() : GetCategoriesTreeByEvent(event_id);
      }
      if (!String.IsNullOrEmpty(res))
      {
        dco.Data = res;
        CacheRepository.Add(dco);
      }
      return res;
    }
    #endregion

    #region GetCategoriesMenu
    //GetCategoriesMenuByEvent
    public string GetCategoriesMenuByEvent(long? eventID, bool onlyLeafs)
    {
      Event evnt;
      EventRepository er = new EventRepository(dataContext, CacheRepository);
      if (!eventID.HasValue)
      {
        evnt = er.GetCurrent();
        eventID = evnt.ID;
      }
      else
      {
        evnt = er.GetEventByID(eventID.Value);
      }
      if (evnt.Type_ID == (int)Consts.EventTypes.Sales)
      {
        return string.Format("<a id='aCurrentSales' href='/Sales' title=''>CURRENT SALES</a>");
      }
      List<IGrouping<int, spCategory_View_CategoriesForEventResult>> rootCategories = dataContext.spCategory_View_CategoriesForEvent(eventID, null, false).GroupBy(Q => Q.MainCategory_ID).ToList();
      List<IdTitle> tags = dataContext.spGetTagsForEvent(eventID, false).Select(t => new IdTitle { ID = t.ID, Title = t.Title }).ToList();
      if (!rootCategories.Any()) return String.Empty;
      StringBuilder sb = new StringBuilder();

      List<spCategory_View_CategoriesForEventResult> list;

      int A, count = 0;

      bool oneMainCategory = rootCategories.Count == 1;
      sb.AppendFormat("<a id='aCurrentAuction' {0} href='/Auction/Category' title=''><div class='drop_down'>CURRENT AUCTION</div></a><ul id='categories_menu'>", oneMainCategory || onlyLeafs ? "class='oneMainCategory'" : string.Empty);
      if (onlyLeafs)
      {
        sb.Append("<li>");
        sb.Append("<table cellpadding=\"0\" cellspacing=\"0\"><colgroup><col width=\"400px\" /><col width=\"400px\" /></colgroup>");
      }
      foreach (IGrouping<int, spCategory_View_CategoriesForEventResult> category in rootCategories)
      {
        list = category.ToList();
        A = (int)Math.Ceiling(list.Count * 0.5);
        if (!onlyLeafs)
        {
          sb.Append("<li>");
          if (!oneMainCategory) sb.AppendFormat("<a href='/Auction/CategoryView/{0}/{1}/{2}' title=''>{3} <em>&nbsp;</em></a>", list[0].EventCategory_ID, UrlParser.TitleToUrl(list[0].EventTitle), UrlParser.TitleToUrl(list[0].MainCategoryTitle), list[0].MainCategoryTitle);
          sb.AppendFormat("<table cellpadding=\"0\" cellspacing=\"0\"><colgroup><col width=\"400px\" />{0}</colgroup>", (list.Count > 2) ? "<col width=\"400px\" />" : "<col width=\"1px\" />");
        }
        for (int i = 0; i < A; i++)
        {
          sb.Append("<tr><td>");
          sb.AppendFormat("<a href='/Auction/CategoryView/{0}/{1}/{2}/{3}' title=''>{4}</a>", list[i].EventCategory_ID, UrlParser.TitleToUrl(list[i].EventTitle), UrlParser.TitleToUrl(list[i].MainCategoryTitle), UrlParser.TitleToUrl(list[i].CategoryTitle), list[i].CategoryTitle + " [" + list[i].AuctionCount + "]");
          sb.Append("</td><td>");
          if (i + A < list.Count)
          {
            sb.AppendFormat("<a href='/Auction/CategoryView/{0}/{1}/{2}/{3}' title=''>{4}</a>", list[i + A].EventCategory_ID, UrlParser.TitleToUrl(list[i + A].EventTitle), UrlParser.TitleToUrl(list[i + A].MainCategoryTitle), UrlParser.TitleToUrl(list[i + A].CategoryTitle), list[i + A].CategoryTitle + " [" + list[i + A].AuctionCount + "]");
          }
          else sb.Append("&nbsp;");
          sb.Append("</td></tr>");
        }
        if (!onlyLeafs)
        {
          sb.Append("</table></li>");
        }
        count++;
        if (!onlyLeafs && count < rootCategories.Count()) sb.Append("<li><hr /></li>");
      }

      if (!onlyLeafs)
      {
        foreach (var tag in tags)
        {
          sb.AppendFormat("<li><a href='/Auction/Tcategory/{0}/{2}'>{1}</a></li>", tag.ID, tag.Title, UrlParser.TitleToUrl(tag.Title));
        }
      }
      else
      {
        foreach (var tag in tags)
        {
          sb.Append("<tr><td>");
          sb.AppendFormat("<li><a href='/Auction/Tcategory/{0}/{2}'>{1}</a></li>", tag.ID, tag.Title, UrlParser.TitleToUrl(tag.Title));
          sb.Append("</td></tr>");
        }
      }
      if (onlyLeafs)
      {
        sb.Append("</table></li>");
      }
      sb.Append("</ul>");
      return sb.ToString();
    }

    //GetCategoriesMenu
    public string GetCategoriesMenu(long? event_id, bool onlyLeafs)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES,
                                                "GETCATEGORIESMENU",
                                                new object[] { event_id.GetValueOrDefault(0) },
                                                CachingExpirationTime.Hours_01);
      string res = CacheRepository.Get(dco) as string;
      if (!String.IsNullOrEmpty(res)) return res;
      StringBuilder sb = new StringBuilder();
      FileInfo fi =
        new FileInfo(System.Web.HttpContext.Current.Server.MapPath(@"~\Templates\Different\CategoriesMenu.txt"));
      if (!fi.Exists)
        res = GetCategoriesMenuByEvent(event_id, onlyLeafs);
      else
      {
        TextReader fileReader =
          new StreamReader(System.Web.HttpContext.Current.Server.MapPath(@"~\Templates\Different\CategoriesMenu.txt"));
        sb.Append(fileReader.ReadToEnd());
        fileReader.Close();
        res = (sb.Length > 0) ? sb.ToString() : GetCategoriesMenuByEvent(event_id, onlyLeafs);
      }
      if (!String.IsNullOrEmpty(res))
      {
        dco.Data = res;
        CacheRepository.Add(dco);
      }
      return res;
    }
    #endregion

    //GetEventCategoryDetailById
    public EventCategoryDetail GetEventCategoryDetail(long eventcategory_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES, "GETEVENTCATEGORYDETAIL",
                                                new object[] { eventcategory_id }, CachingExpirationTime.Hours_01);
      EventCategoryDetail ecd = CacheRepository.Get(dco) as EventCategoryDetail;
      if (ecd != null) return ecd;
      ecd = (from p in dataContext.spCategory_View_EventCategoriesDetail(eventcategory_id)
             select new EventCategoryDetail
             {
               DateEnd = p.DateEnd,
               IsCurrent = p.IsCurrent,
               IsClickable = p.IsClickable,
               Step = p.CloseStep,
               LinkParams = new LinkParams { EventCategory_ID = p.EventCategory_ID, Event_ID = p.Event_ID, MainCategory_ID = p.MainCategory_ID, Category_ID = p.Category_ID, EventTitle = p.EventTitle, MainCategoryTitle = p.MainCategoryTitle, CategoryTitle = p.CategoryTitle },
             }).SingleOrDefault();
      if (ecd != null)
      {
        dco.Data = ecd;
        CacheRepository.Add(dco);
      }
      return ecd;
    }
  }
}
