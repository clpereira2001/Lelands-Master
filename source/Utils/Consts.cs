using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using Vauction.Configuration;
using Vauction.Utils.Perfomance;

namespace Vauction.Utils
{
  public class EnumHelper<T, E>
  {
    public EnumHelper(T text, E value)
    {
      Text = text;
      Value = value;
    }

    public T Text { get; set; }
    public E Value { get; set; }
  }

  public class EnumFromString<T>
  {
    public static T GetValueByString(string value)
    {
      var retVal = (T) Enum.GetValues(typeof (T)).GetValue(0);
      foreach (T oneType in Enum.GetValues(typeof (T)))
      {
        if (oneType.ToString() == value)
          retVal = oneType;
      }
      return retVal;
    }
  }

  public static class Consts
  {
    public enum AuctionImageViewMode
    {
      WithOut = 0,
      With = 1
    }

    public enum AuctionStatus : byte
    {
      Pending = 1,
      Open = 2,
      Closed = 3,
      Locked = 4,
      PulledOut = 5
    }

    public enum AuctionType
    {
      Normal = 1,
      Dutch = 2,
      DealOfTheWeek = 3
    }

    public enum AuctonViewMode
    {
      Table = 0,
      Grid = 1
    }

    public enum CategorySortFields
    {
      Lot = 1,
      Title = 2,
      CurrentBid = 3,
      Bids = 4,
      Price = 5,
      ID = 6,
      Description = 7,
      Event_ID = 8,
      PriceRealized = 9
    }

    public enum CommissionLimitType
    {
      SellingPrice = 0,
      InCommission = 1
    }

    public enum ConsignmentContractStatus : int
    {
      NotGenerated = 0,
      Unsigned = 1,
      Signed = 2
    }

    public enum EventTypes : int
    {
      Auction = 1,
      Sales = 2
    }

    public enum HomepageImageType : byte
    {
      Big = 1,
      Small = 2,
      Stripe = 3
    }

    public enum OrderByValues
    {
      ascending = 1,
      descending = 2
    }

    public enum PaymentState
    {
      Declined = 1,
      Accepted = 2
    }

    public enum PaymentType
    {
      CreditCard = 1,
      Paypal = 2,
      WebMoney = 3,
      MasterCard = 4,
      WALKIN = 5,
      ManuallyAdmin = 6
    }

    public enum Tags
    {
      BestOfTheBest = 1,
      Sales = 2
    }

    public enum UserStatus : byte
    {
      Pending = 1,
      Active = 2,
      Inactive = 3,
      Locked = 4
    }

    public enum UserTypes : byte
    {
      Root = 1,
      Admin = 2,
      HouseBidder = 3,
      Buyer = 4,
      SellerBuyer = 5,
      Seller = 6,
      Specialist = 7
    }

    public const int FormsAuthenticationTicketTime = 30;
    public static TimeSpan AuthorizeStatusCheckTime = new TimeSpan(0, 15, 0);

    public static byte DefaultCommissionRate = 0;

    public static decimal IncementWithProxy = 1.21M;
    public static decimal IncementWithOutProxy = 1.1M;
    public static decimal IncementBeforeLevel = 25M;
    public static decimal IncementLevel = 250M;
    public static decimal ErrorRangeAmount = 24.9M;

    public static int MaxTryForFailedAtemps
    {
      get
      {
        var fa =
          ((IVauctionConfiguration) ConfigurationManager.GetSection("Vauction")).GetProperty("MaxTryForFailedAtemps");
        return String.IsNullOrEmpty(fa) ? 245 : Convert.ToInt32(fa);
      }
    }

    public static int DropDownSize
    {
      get
      {
        var value = ((IVauctionConfiguration) ConfigurationManager.GetSection("Vauction")).GetProperty("DropdownSize");
        return (!String.IsNullOrEmpty(value)) ? Convert.ToInt32(value) : 50;
      }
    }

    public static bool IsShownOpenBidOne
    {
      get
      {
        var value =
          ((IVauctionConfiguration) ConfigurationManager.GetSection("Vauction")).GetProperty("IsShownOpenBidOne");
        return (!String.IsNullOrEmpty(value)) ? Convert.ToBoolean(value) : false;
      }
    }

    public static bool IsAllUsersCanSeeBids
    {
      get
      {
        var value =
          ((IVauctionConfiguration) ConfigurationManager.GetSection("Vauction")).GetProperty("IsAllUsersCanSeeBids");
        return (!String.IsNullOrEmpty(value)) ? Convert.ToBoolean(value) : true;
      }
    }

    public static string CompanyTitleName
    {
      get
      {
        var value = ConfigurationManager.AppSettings["CompanyName"];
        return (!String.IsNullOrEmpty(value)) ? value : "---";
      }
    }

    public static string CompanyAddress
    {
      get
      {
        var value = ConfigurationManager.AppSettings["CompanyAddress"];
        return (!String.IsNullOrEmpty(value)) ? value : "---";
      }
    }

    public static string CompanyCity
    {
      get
      {
        var value = ConfigurationManager.AppSettings["companyCity"];
        return (!String.IsNullOrEmpty(value)) ? value : "---";
      }
    }

    public static string CompanyState
    {
      get
      {
        var value = ConfigurationManager.AppSettings["companyState"];
        return (!String.IsNullOrEmpty(value)) ? value : "---";
      }
    }

    public static string CompanyZip
    {
      get
      {
        var value = ConfigurationManager.AppSettings["companyZip"];
        return (!String.IsNullOrEmpty(value)) ? value : "---";
      }
    }

    public static string CompanyPhone
    {
      get
      {
        var value = ConfigurationManager.AppSettings["CompanyPhone"];
        return (!String.IsNullOrEmpty(value)) ? value : "---";
      }
    }

    public static string CompanyFax
    {
      get
      {
        var value = ConfigurationManager.AppSettings["CompanyFax"];
        return (!String.IsNullOrEmpty(value)) ? value : "---";
      }
    }

    public static string SiteEmail
    {
      get
      {
        var value = ConfigurationManager.AppSettings["siteEmail"];
        return (!String.IsNullOrEmpty(value)) ? value : "---";
      }
    }

    public static int PageSize
    {
      get
      {
        var value = ((IVauctionConfiguration) ConfigurationManager.GetSection("Vauction")).GetProperty("PageSize");
        return (!String.IsNullOrEmpty(value)) ? Convert.ToInt32(value) : 20;
      }
    }

    //DataCachingTechnology
    public static DataCacheTechnology DataCachingTechnology
    {
      get
      {
        var result =
          ((IVauctionConfiguration) ConfigurationManager.GetSection("Vauction")).GetProperty("DataCachingTechnology");
        byte res = 0;
        return (!String.IsNullOrEmpty(result) && Byte.TryParse(result, out res))
          ? (DataCacheTechnology) res
          : DataCacheTechnology.MEMORYOBJECT;
      }
    }

    //ProductName
    public static string ProductName
    {
      get
      {
        var result = ((IVauctionConfiguration) ConfigurationManager.GetSection("Vauction")).GetProperty("ProductName");
        return (!String.IsNullOrEmpty(result)) ? result : "DEFAULT";
      }
    }

    //ResourceHostName
    public static string ResourceHostName
    {
      get
      {
        var result =
          ((IVauctionConfiguration) ConfigurationManager.GetSection("Vauction")).GetProperty("ResourceHostName");
        return (!String.IsNullOrEmpty(result)) ? result : "";
      }
    }

    //UsersIPAddress
    public static string UsersIPAddress
    {
      get
      {
        //try
        //{
        //  string[] ip = (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? System.Web.HttpContext.Current.Request.UserHostAddress).Split(':');
        //  return ip[0];
        //}
        //catch (Exception ex)
        //{
        //  Lib.Logger.LogException("UsersIPAddress", ex);
        //  return System.Web.HttpContext.Current.Request.UserHostAddress;
        //}
        return HttpContext.Current.Request.UserHostAddress;
      }
    }

    public static string AntiForgeryToken
    {
      get { return "AhEm:pul@whET67"; }
    }

    public static byte GetUserStatusByType(string status)
    {
      if (status == UserStatus.Locked.ToString())
        return Convert.ToByte(UserStatus.Locked);
      if (status == UserStatus.Active.ToString())
        return Convert.ToByte(UserStatus.Active);
      if (status == UserStatus.Locked.ToString())
        return Convert.ToByte(UserStatus.Locked);
      if (status == UserStatus.Pending.ToString())
        return Convert.ToByte(UserStatus.Pending);
      return Convert.ToByte(UserStatus.Pending);
    }

    public static byte GetStatusByType(string status)
    {
      if (status == AuctionStatus.Closed.ToString())
        return Convert.ToByte(AuctionStatus.Closed);
      if (status == AuctionStatus.Open.ToString())
        return Convert.ToByte(AuctionStatus.Open);
      if (status == AuctionStatus.Locked.ToString())
        return Convert.ToByte(AuctionStatus.Locked);
      if (status == AuctionStatus.Pending.ToString())
        return Convert.ToByte(AuctionStatus.Pending);
      //if (status == AuctionStatus.PulledOut.ToString())
      //  return Convert.ToByte(AuctionStatus.PulledOut);

      return Convert.ToByte(AuctionStatus.Pending);
    }

    public static bool IsPriceBeforeLevel(decimal price)
    {
      return false; //price <= IncementLevel;
    }

    public static decimal GetIncrement(decimal price)
    {
      return GetIncrement(price, false);
    }

    public static decimal GetIncrement(decimal price, bool isproxy)
    {
      return (price < IncementLevel)
        ? (isproxy ? IncementBeforeLevel + IncementBeforeLevel : IncementBeforeLevel)
        : ((price*(isproxy ? IncementWithProxy : IncementWithOutProxy)).GetPrice() - price).GetPrice();
    }

    public static string GetStringByCommissionLimitType(CommissionLimitType type)
    {
      var retVal = string.Empty;
      switch (type)
      {
        case CommissionLimitType.SellingPrice:
          retVal = "of Selling Price";
          break;
        case CommissionLimitType.InCommission:
          retVal = "in Commission";
          break;
      }
      return retVal;
    }

    public static List<string> GetListOfCommissionLimitType()
    {
      var retVal = new List<string>();
      foreach (CommissionLimitType type in Enum.GetValues(typeof (CommissionLimitType)))
      {
        retVal.Add(GetStringByCommissionLimitType(type));
      }
      return retVal;
    }

    public static CommissionLimitType GetCommissionLimitTypeByString(string typeStr)
    {
      foreach (CommissionLimitType type in Enum.GetValues(typeof (CommissionLimitType)))
      {
        if (typeStr == GetStringByCommissionLimitType(type))
          return type;
      }
      return CommissionLimitType.SellingPrice;
    }

    //NotNeedToCheckIP
    //public static bool NeedToCheckIP
    //{
    //  get
    //  {
    //    return !(HttpContext.Current != null && HttpContext.Current.Request.Browser.Capabilities.Contains("extra") &&
    //           HttpContext.Current.Request.Browser.Capabilities["extra"].ToString().Contains("AOLBuild"));
    //  }
    //}
  }
}