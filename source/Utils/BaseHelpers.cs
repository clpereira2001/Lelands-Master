using System.Web.Mvc;
using Vauction.Utils;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Web;

namespace Vauction.Utils
{
  public static class BaseHelpers
  {
    public static SelectList GetAuctionViewModeList(Consts.AuctonViewMode selectedValue)
    {
      List<EnumHelper<string, int>> listAuctionViewMode = new List<EnumHelper<string, int>>();

      foreach (int value in Enum.GetValues(typeof(Consts.AuctonViewMode)))
      {
        listAuctionViewMode.Add(new EnumHelper<string, int>(Enum.GetName(typeof(Consts.AuctonViewMode), value), value));
      }

      return new SelectList(listAuctionViewMode, "Value", "Text", (int)selectedValue);
    }

    public static SelectList GetDateList(DateTime startDate, int daySpan)
    {
      List<EnumHelper<string, string>> dateList = new List<EnumHelper<string, string>>();
      for (int i = 0; i <= daySpan; i++)
      {
        dateList.Add(new EnumHelper<string, string>(startDate.AddDays(i).ToShortDateString(), startDate.AddDays(i).ToShortDateString()));
      }
      return new SelectList(dateList, "Value", "Text");
    }

    public static int GetViewMode()
    {
      //Get ViewMode from Cookie or QueryString
      int ViewMode = 0;

      if (HttpContext.Current.Request.Cookies["ViewMode"] == null)
      {
        if (String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ViewMode"]))
        {
          ViewMode = Convert.ToInt32(HttpContext.Current.Request.QueryString["ViewMode"]);
        }
      }
      else
      {
        ViewMode = Convert.ToInt32(HttpContext.Current.Request.Cookies["ViewMode"].Value);
      }

      return ViewMode;
    }

    public static int GetImageViewMode()
    {
      int ImageViewMode = 1;

      if (HttpContext.Current.Request.Cookies["ImageViewMode"] == null)
      {        
        ImageViewMode = (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ImageViewMode"]))?Convert.ToInt32(HttpContext.Current.Request.QueryString["ImageViewMode"]):(byte)Consts.AuctionImageViewMode.With;
      }
      else
      {
        ImageViewMode = Convert.ToInt32(HttpContext.Current.Request.Cookies["ImageViewMode"].Value);
      }
      return ImageViewMode;
    }

  }
}