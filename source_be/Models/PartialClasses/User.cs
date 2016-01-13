using System;
using Vauction.Utils;
using System.Runtime.Serialization;

namespace Vauction.Models
{
  [Serializable]
  partial class User : IUser
  {
    public User(Int64 id)
    {
      ID = id;
    }

    public bool IsAdmin
    {
      get
      {
        return UserType_ID == (byte)Consts.UserTypes.Admin;
      }
    }
    public bool IsRoot
    {
      get
      {
        return UserType_ID == (byte)Consts.UserTypes.Root;
      }
    }
    public bool IsSeller
    {
      get
      {
        return UserType_ID == (byte)Consts.UserTypes.Seller;
      }
    }
    public bool IsSellerBuyer
    {
      get
      {
        return UserType_ID == (byte)Consts.UserTypes.SellerBuyer;
      }
    }
    public bool IsSellerType
    {
      get
      {
        return UserType_ID == (byte)Consts.UserTypes.SellerBuyer ||
            UserType_ID == (byte)Consts.UserTypes.Seller;
      }
    }
    public bool IsBuyer
    {
      get
      {
        return UserType_ID == (byte)Consts.UserTypes.Buyer;
      }
    }
    public bool IsBuyerType
    {
      get
      {
        return UserType_ID == (byte)Consts.UserTypes.SellerBuyer || UserType_ID == (byte)Consts.UserTypes.Buyer;
      }
    }
    public bool IsHouseBidder
    {
      get
      {
        return UserType_ID == (byte)Consts.UserTypes.HouseBidder;
      }
    }
    
    public bool IsSpecialist
    {
      get { return UserType_ID == (byte)Consts.UserTypes.Specialist; }
    }
    public string LoginEncrypted
    {
      get { return Login.Substring(0, 2) + "***"; }
    }

    public string PasswordEncrypted
    {
      get
      {
        return (!AppHelper.CurrentUser.IsRoot && (IsAdmin || IsRoot) && AppHelper.CurrentUser.ID != ID) ? "*****" : Password;
      }
    }
    public string UserNameShort
    {
      get { return (AddressCard_Billing == null) ? "---" : String.Format("{0} {1}.", AddressCard_Billing.LastName, AddressCard_Billing.FirstName[0]); }
    }
    public string UserLoginName
    {
      get { return String.Format("{0} ({1})", Login, UserNameFull); }
    }
    public string UserNameFull
    {
      get { return (AddressCard_Billing == null) ? "---" : String.Format("{0}{1} {2}", AddressCard_Billing.FirstName, (String.IsNullOrEmpty(AddressCard_Billing.MiddleName)) ? String.Empty : " " + AddressCard_Billing.MiddleName, AddressCard_Billing.LastName); }
    }

    private string GetFullPhone(AddressCard ac)
    {
      return (ac == null) ? "---" : String.Format("{0}{1}{2}", (!String.IsNullOrEmpty(ac.HomePhone) ? "H: " + ac.HomePhone : ""), (!String.IsNullOrEmpty(ac.HomePhone) && !String.IsNullOrEmpty(ac.WorkPhone)) ? ";  " : String.Empty, (!String.IsNullOrEmpty(ac.WorkPhone) ? "W: " + ac.WorkPhone : ""));
    }
    public string BillingPhoneFull
    {
      get { return GetFullPhone(AddressCard_Billing); }
    }
    public string ShippingPhoneFull
    {
      get { return GetFullPhone((BillingLikeShipping) ? AddressCard_Billing : AddressCard_Shipping); }
    }

    private string GetFullAddress(AddressCard ac, string separator)
    {
      return (ac == null) ? "---" : String.Format("Addr1: {0}{7}Addr2: {1}{7}City: {2}{7}State: {3}{7}Country: {4}{7}Zip: {5}{7}Company: {6}", ac.Address1, ac.Address2, ac.City, ac.State, ac.Country.Title, ac.Zip, ac.Company, separator);
    }
    public string BillingAddressFull_1
    {
      get { return GetFullAddress(AddressCard_Billing, "\n"); }
    }
    public string BillingAddressFull_2
    {
      get { return GetFullAddress(AddressCard_Billing, "; "); }
    }
    public string ShippingAddressFull_1
    {
      get { return (AddressBillingLikeShipping != null) ? GetFullAddress(AddressBillingLikeShipping, "\n") : String.Empty; }
    }
    public string ShippingAddressFull_2
    {
      get { return (AddressBillingLikeShipping != null) ? GetFullAddress(AddressBillingLikeShipping, "; ") : String.Empty; }
    }

    public AddressCard AddressBillingLikeShipping
    {
      get { return (BillingLikeShipping) ? AddressCard_Billing : AddressCard_Shipping; }
    }
  }
}
