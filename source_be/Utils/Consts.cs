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
    //public static int BottomDepositLimit = 2500;
    //public static int TopDepositLimit = 10000;
    //public const int CachingTime_30sec = 30;
    //public const int CachingTime_01min = 60;
    //public const int CachingTime_02min = 120;
    //public const int CachingTime_05min = 300;
    //public const int CachingTime_10min = 600;
    //public const int CachingTime_20min = 1200;
    //public const int CachingTime_30min = 1800;
    //public const int CachingTime_60min = 3600;
    //public const int CachingTime_01day = 86400;

    public enum AuctionImagesSize : byte
    {
      Small = 1,
      Medium = 2,
      Large = 3
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
      Title,
      Price,
      Description
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

    public enum EventTypes : byte
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
      ascending,
      descending
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

    public enum UserStatus : byte
    {
      Pending = 1, //-- User has not confirmed his email with us      
      Active = 2, // -- User had confirmed his email and has full access to his access level            
      Inactive = 3, // -- User is no longer in our system opted out
      Locked = 4
      //-- Set by the admin / User did not pay his bill or any reason the admin chooses to lock him from using the system 
    }

    public enum UserTypes : byte
    {
      Root = 1,
      Admin = 2,
      HouseBidder = 3,
      Buyer = 4,
      SellerBuyer = 5,
      Seller = 6,
      Specialist = 7,
      Writer = 8,
      SpecialistViewer = 9
    }

    public const int FormsAuthenticationTicketTime = 360;
    public static TimeSpan AuthorizeStatusCheckTime = new TimeSpan(3, 00, 0);

    public static decimal IncementWithProxy = 1.21M;
    public static decimal IncementWithOutProxy = 1.1M;
    public static decimal IncementBeforeLevel = 25M;
    public static decimal IncementLevel = 250M;
    public static decimal ErrorRangeAmount = 24.9M;

    public static string GetVauctionFrontendDir
    {
      get
      {
        return ((IVauctionConfiguration) ConfigurationManager.GetSection("Vauction")).GetProperty("VauctionSiteDir");
      }
    }

    public static string SiteUrl
    {
      get { return ConfigurationManager.AppSettings["siteUrl"]; }
    }

    public static int HouseBiddersAmountForEventRegstration
    {
      get
      {
        var value =
          ((IVauctionConfiguration) ConfigurationManager.GetSection("Vauction")).GetProperty(
            "HouseBiddersAmountForEventRegstration");
        return (!String.IsNullOrEmpty(value)) ? Convert.ToInt32(value) : 200;
      }
    }

    public static bool CacheModeDebuging
    {
      get
      {
        var value =
          ((IVauctionConfiguration) ConfigurationManager.GetSection("Vauction")).GetProperty("CacheModeDebuging");
        return (!String.IsNullOrEmpty(value)) ? Convert.ToBoolean(value) : false;
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

    public static string CacheClearFrontendIP
    {
      get
      {
        return ((IVauctionConfiguration) ConfigurationManager.GetSection("Vauction")).GetProperty("CacheClearFrontendIP");
      }
    }

    public static string FrontEndClearMethod
    {
      get { return "/Base/ClearAllCache"; }
    }

    public static string FrontEndClearARPMethod
    {
      get { return "/Base/ClearARP"; }
    }

    public static string FrontEndClearADPMethod
    {
      get { return "/Base/ClearADP"; }
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
      return Convert.ToByte(AuctionStatus.Pending);
    }

    public static string GetPhonePartByFull(byte part, string phone)
    {
      if (phone.Length != 10 || phone.Length != 14)
      {
        return "";
      }
      if (phone.Length == 10)
      {
        phone += "    ";
      }
      switch (part)
      {
        case (1):
          return phone.Substring(0, 3);
        case (2):
          return phone.Substring(3, 3);
        case (3):
          return phone.Substring(6, 4);
        case (4):
          return phone.Substring(10, 4);
      }
      return "";
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
        ? IncementBeforeLevel
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

    public static int GetAuctionImageSize(AuctionImagesSize ais)
    {
      var size = 0;
      switch (ais)
      {
        case AuctionImagesSize.Small:
          size = Convert.ToInt32(ConfigurationManager.AppSettings["AuctionImageThumbnailSize"]);
          break;
        case AuctionImagesSize.Medium:
          size = Convert.ToInt32(ConfigurationManager.AppSettings["AuctionImageSize"]);
          break;
        default:
          size = Convert.ToInt32(ConfigurationManager.AppSettings["AuctionImageLargeSize"]);
          break;
      }
      return size;
    }

    public static string GetUserTypeAbbr(UserTypes ut)
    {
      string result;
      switch (ut)
      {
        case UserTypes.Root:
          result = "R";
          break;
        case UserTypes.Admin:
          result = "A";
          break;
        case UserTypes.HouseBidder:
          result = "H";
          break;
        case UserTypes.SellerBuyer:
          result = "SB";
          break;
        case UserTypes.Seller:
          result = "S";
          break;
        case UserTypes.Specialist:
          result = "Sp";
          break;
        default:
          result = "B";
          break;
      }
      return result;
    }
  }
}