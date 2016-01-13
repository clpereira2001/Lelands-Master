using System;
using System.Reflection;
using System.Collections.Generic;
using System.Web.Mvc;
using Vauction.Utils.Validation;
using Vauction.Configuration;

namespace Vauction.Models
{
  [Serializable]
  public class UserRegistration
  {
    #region General
    public Int64 ID = 0;

    [FieldTitle("Login")]
    [FieldRequired]    
    [FieldCheckMaxLength(20)]
    public string Login { get; set; }

    [FieldTitle("Password")]
    [FieldRequired]
    [FieldNonSpaced]
    [FieldCheckMinLength(6)]
    [FieldCheckMaxLength(30)]
    public string Password { get; set; }

    [FieldTitle("Confirm passowrd")]
    [FieldRequired]
    [FieldCheckMaxLength(30)]
    public string ConfirmPassword { get; set; }

    [FieldTitle("Type")]
    [FieldRequired]
    [FieldCheckNumeric]
    public byte UserType { get; set; }

    [FieldTitle("Status")]
    [FieldRequired]
    [FieldCheckNumeric]
    public byte UserStatus { get; set; }

    [FieldTitle("Commission")]
    [FieldRequired]
    [FieldCheckNumeric]
    public byte CommissionRate { get; set; }

    [FieldTitle("E-mail")]
    [FieldRequired]
    [FieldCheckEmail]
    [FieldCheckMaxLength(255)]
    public string Email { get; set; }

    [FieldTitle("Confirm E-mail")]
    [FieldRequired]
    [FieldCheckMaxLength(255)]
    [FieldCheckEmail]
    public string ConfirmEmail { get; set; }

    [FieldCheckMaxLength(255)]
    public string ConfirmCode { get; set; }

    [FieldTitle("Registration Date")]    
    public DateTime DateIN { get; set; }

    public bool IsConfirmed { get; set; }
    public bool IsModifyed { get; set; }

    [FieldTitle("Phone (1)")]
    [FieldRequired]
    [FieldCheckAmericanOrNotPhone]    
    public string DayPhone { get; set; }

    [FieldTitle("Phone (2)")]
    [FieldCheckAmericanOrNotPhone]
    public string EveningPhone { get; set; }

    [FieldTitle("Mobile Phone")]
    [FieldCheckAmericanOrNotPhone]  
    public string MobilePhone { get; set; }

    [FieldTitle("Fax")]    
    [FieldCheckMaxLength(14)]
    public string Fax { get; set; }

    [FieldTitle("TaxpayerID")]    
    [FieldCheckMaxLength(14)]
    public string Tax { get; set; }

    [FieldTitle("Failed Attempts")]
    [FieldCheckNumeric]
    public byte FaildAttempts { get; set; }
    
    public bool RecieveWeeklySpecials { get; set; }
    public bool RecieveNewsUpdates { get; set; }
    public bool RecieveBidConfirmation { get; set; }
    public bool RecieveOutBidNotice { get; set; }
    public bool RecieveLotSoldNotice { get; set; }
    public bool RecieveLotClosedNotice { get; set; }
    public string Notes { get; set; }

    public bool IsCatalog { get; set; }
    public bool IsPostCards { get; set; }
    #endregion

    #region References
    public string AuctionHouse1 { get; set; }
    public string PhoneNumber1 { get; set; }
    public string LastBidDate1 { get; set; }

    public string AuctionHouse2 { get; set; }
    public string PhoneNumber2 { get; set; }
    public string LastBidDate2 { get; set; }

    public string eBayBidderID { get; set; }
    public string eBayFeedback { get; set; }
    #endregion

    #region Billing information
    [FieldTitle("Billing - First Name")]
    [FieldRequired]
    [FieldCheckMaxLength(50)]
    public string BillingFirstName { get; set; }

    [FieldTitle("Billing - Middle Name")]
    [FieldCheckMaxLength(50)]
    public string BillingMiddleName { get; set; }

    [FieldTitle("Billing - Last Name")]
    [FieldRequired]
    [FieldCheckMaxLength(50)]
    public string BillingLastName { get; set; }
    
    [FieldTitle("Billing - Company")]
    public string BillingCompany { get; set; }

    [FieldTitle("Billing - Address 1")]
    [FieldRequired]
    [FieldCheckMaxLength(255)]
    public string BillingAddress1 { get; set; }

    [FieldTitle("Billing - Address 2")]    
    [FieldCheckMaxLength(255)]
    public string BillingAddress2 { get; set; }

    [FieldTitle("Billing - City/Town")]    
    [FieldRequired]
    [FieldCheckMaxLength(255)]
    public string BillingCity { get; set; }

    [FieldTitle("Billing - State")]
    [FieldRequired]
    [FieldCheckMinLength(2)]
    [FieldCheckMaxLength(2)]        
    public string BillingState { get; set; }
        
    [FieldRequired]    
    public long BillingState_ID { get; set; }

    [FieldTitle("Billing - Zip")]
    [FieldRequired]
    [FieldCheckMaxLength(10)]
    [FieldCheckMinLength(3)]
    public string BillingZip { get; set; }

    [FieldRequired]
    public long BillingCountry { get; set; }

    [FieldTitle("Billing - International State")]
    [FieldCheckAlpha]
    public string BillingInternationalState { get; set; }

    #endregion

    #region Shipping information
    
    public bool BillingLikeShipping { get; set; }

    [FieldTitle("Shipping - First Name")]
    [FieldCheckMaxLength(50)]
    public string ShippingFirstName { get; set; }

    [FieldTitle("Shipping - Middle Name")]
    [FieldCheckMaxLength(50)]
    public string ShippingMiddleName { get; set; }

    [FieldTitle("Shipping - Last Name")]
    [FieldCheckMaxLength(50)]
    public string ShippingLastName { get; set; }

    [FieldTitle("Shipping - Company")]
    public string ShippingCompany { get; set; }

    [FieldTitle("Shipping - Address 1")]
    [FieldRequired]
    [FieldCheckMaxLength(255)]
    public string ShippingAddress1 { get; set; }

    [FieldTitle("Shipping - Address 2")]
    [FieldCheckMaxLength(255)]
    public string ShippingAddress2 { get; set; }

    [FieldTitle("Shipping - City/Town")]
    [FieldRequired]
    [FieldCheckMaxLength(255)]
    public string ShippingCity { get; set; }

    [FieldTitle("Shipping - State")]
    [FieldRequired]
    [FieldCheckMinLength(2)]
    [FieldCheckMaxLength(2)]
    public string ShippingState { get; set; }

    [FieldRequired]
    public long ShippingState_ID { get; set; }

    [FieldTitle("Shipping - Zip")]
    [FieldRequired]
    [FieldCheckMaxLength(10)]
    [FieldCheckMinLength(3)]
    public string ShippingZip { get; set; }

    [FieldRequired]
    public long ShippingCountry { get; set; }

    [FieldTitle("Shipping - International State")]
    [FieldCheckAlpha]
    public string ShippingInternationalState { get; set; }
    #endregion

    #region Validation
    //ValidateWithConfim (Email, Password)
    public void ValidateWithConfim(System.Web.Mvc.ModelStateDictionary modelState)
    {
      ValidationCheck.CheckErrors(this, modelState, true);

      //check Login
      if (!ValidationCheck.IsEmpty(this.Login) && !ProjectConfig.Config.DataProvider.GetInstance().UserRepository.ValidateLogin(this.Login, ID))
      {
        modelState.AddModelError("Login", "Login already exists. Please enter a different user name.");
      }

      //check Email
      if (!ValidationCheck.IsEmpty(this.Email) && !ProjectConfig.Config.DataProvider.GetInstance().UserRepository.ValidateEmail(this.Email, ID))
      {
        modelState.AddModelError("Email", "A username for that e-mail address already exists. Please enter a different e-mail address.");
      }
      // check Email and Confirm Email
      if (!String.Equals(this.Email, this.ConfirmEmail, StringComparison.Ordinal))
      {
        modelState.AddModelError("Email", "The Email and confirmation Email do not match.");
        modelState.AddModelError("ConfirmEmail", "");
      }
      // check Password and Confirm Password
      if (!String.Equals(this.Password, this.ConfirmPassword, StringComparison.Ordinal))
      {
        modelState.AddModelError("Password", "The password and confirmation password do not match.");
        modelState.AddModelError("ConfirmPassword", "");
      }
    }

    //ValidateWithoutConfim
    public void ValidateWithoutConfim(System.Web.Mvc.ModelStateDictionary modelState)
    {
      ValidationCheck.CheckErrors(this, modelState, true);

      //check Login
      if (!ValidationCheck.IsEmpty(this.Login) && !ProjectConfig.Config.DataProvider.GetInstance().UserRepository.ValidateLogin(this.Login, ID))
      {
        modelState.AddModelError("Login", "Login already exists. Please enter a different user name.");
      }

      //check Email
      if (!ValidationCheck.IsEmpty(this.Email) && !ProjectConfig.Config.DataProvider.GetInstance().UserRepository.ValidateEmail(this.Email, ID))
      {
        modelState.AddModelError("Email", "A username for that e-mail address already exists. Please enter a different e-mail address.");
      }      
    }
    #endregion
  }
}