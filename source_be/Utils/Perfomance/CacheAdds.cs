using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vauction.Utils.Perfomance
{
  public enum DataCacheTechnology { MEMORYOBJECT = 0, APPFABRIC = 1 };

  public enum DataCacheType { REFERENCE, ACTIVITY, RESOURCE };

  public enum DataCacheRegionTypes { IMAGES, COUNTRIES, STATES, CATEGORIES, HOTNEWS, AUCTIONLISTS, AUCTIONS, WATCHLISTS, BIDS, EVENTS, EVENTREGISTRATIONS, INVOICES, USERS };

  public static class DataCacheRegions
  {
    public static string IMAGES { get { return DataCacheRegionTypes.IMAGES.ToString(); } }
    public static string COUNTRIES { get { return DataCacheRegionTypes.COUNTRIES.ToString(); } }
    public static string STATES { get { return DataCacheRegionTypes.STATES.ToString(); } }
    public static string CATEGORIES { get { return DataCacheRegionTypes.CATEGORIES.ToString(); } }
    public static string HOTNEWS { get { return DataCacheRegionTypes.HOTNEWS.ToString(); } }
    public static string AUCTIONLISTS { get { return DataCacheRegionTypes.AUCTIONLISTS.ToString(); } }
    public static string AUCTIONS { get { return DataCacheRegionTypes.AUCTIONS.ToString(); } }
    public static string WATCHLISTS { get { return DataCacheRegionTypes.WATCHLISTS.ToString(); } }
    public static string BIDS { get { return DataCacheRegionTypes.BIDS.ToString(); } }
    public static string EVENTS { get { return DataCacheRegionTypes.EVENTS.ToString(); } }
    public static string EVENTREGISTRATIONS { get { return DataCacheRegionTypes.EVENTREGISTRATIONS.ToString(); } }
    public static string INVOICES { get { return DataCacheRegionTypes.INVOICES.ToString(); } }
    public static string USERS { get { return DataCacheRegionTypes.USERS.ToString(); } }
  }
  
  public static class CachingExpirationTime
  {
    public const int Seconds_05 = 5;
    public const int Seconds_10 = 10;
    public const int Seconds_30 = 30;
    public const int Minutes_01 = 60;
    public const int Minutes_02 = 120;
    public const int Minutes_05 = 300;
    public const int Minutes_10 = 600;
    public const int Minutes_20 = 1200;
    public const int Minutes_30 = 1800;
    public const int Hours_01 = 3600;
    public const int Days_01 = 86400;

    public static int FromMinutes(int minutes)
    {
      return TimeSpan.FromMinutes(minutes).Seconds;
    }

    public static int FromHours(int hours)
    {
      return TimeSpan.FromHours(hours).Seconds;
    }

    public static int FromDays(int days)
    {
      return TimeSpan.FromDays(days).Seconds;
    }
  }
}
