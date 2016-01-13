using System;
using System.Reflection;
using System.Collections.Generic;
using System.Web.Mvc;
using Vauction.Utils.Validation;
using Vauction.Configuration;

namespace Vauction.Models
{
  [Serializable]
  public class RegisterInfo
  {
    public Int64 ID = 0;

    [FieldTitle("Create Bidder ID")]
    [FieldRequired]    
    [FieldCheckMaxLength(20)]
    public string Login { get; set; }

    [FieldTitle("Date Of Birth")]
    public DateTime DateOfBirth { get; set; }

    [FieldTitle("E-mail")]
    [FieldRequired]
    [FieldCheckMaxLength(255)]
    [FieldCheckEmail]
    public string Email { get; set; }

    [FieldTitle("Confirm E-mail")]
    [FieldRequired]
    [FieldCheckMaxLength(255)]
    [FieldCheckEmail]
    public string ConfirmEmail { get; set; }

    [FieldTitle("Create Password")]
    [FieldRequired]
    [FieldCheckMinLength(6)]
    [FieldCheckMaxLength(30)]
    public string Password { get; set; }

    [FieldTitle("Confirm passowrd")]
    [FieldRequired]
    [FieldCheckMinLength(6)]
    [FieldCheckMaxLength(30)]
    public string ConfirmPassword { get; set; }

    public string ConfirmCode { get; set; }

    [FieldCheckMaxLength(20)]
    [FieldCheckAmericanOrNotPhone]
    public string Fax { get; set; }

    [FieldTitle("First Name")]
    [FieldRequired]
    [FieldCheckAlphanumericExt]
    [FieldCheckMaxLength(50)]
    public string BillingFirstName { get; set; }

    [FieldCheckMaxLength(50)]
    public string BillingMIName { get; set; }

    [FieldTitle("Last Name")]
    [FieldRequired]
    [FieldCheckAlphanumericExt]
    [FieldCheckMaxLength(50)]
    public string BillingLastName { get; set; }
    
    [FieldTitle("Company")]
    [FieldCheckMaxLength(60)]
    public string BillingCompany { get; set; }

    [FieldTitle("InternationalState")]
    [FieldCheckMaxLength(30)]
    public string BillingInternationalState { get; set; }

    [FieldTitle("InternationalState")]
    [FieldCheckMaxLength(30)]
    public string ShippingInternationalState { get; set; }

    [FieldTitle("Address (1)")]
    [FieldRequired]
    [FieldCheckMaxLength(65)]
    public string BillingAddress1 { get; set; }
    [FieldCheckMaxLength(65)]
    public string BillingAddress2 { get; set; }

    [FieldTitle("City")]
    [FieldCheckMaxLength(30)]
    [FieldRequired]
    public string BillingCity { get; set; }

    [FieldTitle("State")]
    [FieldCheckMaxLength(20)]
    public string BillingState { get; set; }

    [FieldTitle("Zip")]
    [FieldRequired]
    [FieldCheckAlphanumeric]    
    [FieldCheckMaxLength(10)]
    [FieldCheckMinLength(4)]
    public string BillingZip { get; set; }

    [FieldTitle("Phone")]
    [FieldRequired]
    [FieldCheckMaxLength(20)]
    [FieldCheckAmericanPhone] //xxx-xxx-xxxx extxxxx
    public string BillingPhone { get; set; }

    public string ShippingFirstName { get; set; }

    public string ShippingMIName { get; set; }

    public string ShippingLastName { get; set; }

    [FieldTitle("Address (1)")]
    [FieldRequired()]
    [FieldCheckMaxLength(65)]
    public string ShippingAddress1 { get; set; }
    [FieldCheckMaxLength(65)]
    public string ShippingAddress2 { get; set; }

    [FieldTitle("City")]
    [FieldRequired]
    [FieldCheckMaxLength(30)]
    public string ShippingCity { get; set; }

    [FieldTitle("State")]
    [FieldCheckMaxLength(20)]
    public string ShippingState { get; set; }

    [FieldTitle("Zip")]
    [FieldRequired()]
    [FieldCheckMaxLength(10)]
    [FieldCheckMinLength(5)]
    [FieldCheckAlphanumeric]
    public string ShippingZip { get; set; }

    [FieldTitle("Phone")]
    [FieldRequired]
    [FieldCheckMaxLength(20)]
    [FieldCheckAmericanPhone]
    public string ShippingPhone { get; set; }
    [FieldCheckMaxLength(20)]
    [FieldCheckAmericanPhone]
    public string ShippingWorkPhone { get; set; }

    [FieldTitle("Country")]
    [FieldRequired]
    [FieldCheckMaxLength(30)]
    public long ShippingCountry { get; set; }

    [FieldTitle("Country")]
    [FieldRequired]
    public long BillingCountry { get; set; }

    public bool RecieveWeeklySpecials { get; set; }
    public bool RecieveNewsUpdates { get; set; }
    public bool BillingLikeShipping { get; set; }

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

    [FieldTitle("Taxpayer ID")]
    [FieldCheckAlphanumeric]
    [FieldCheckMinLength(2)]
    [FieldCheckMaxLength(20)]
    public string TaxpayerID { get; set; }

    [FieldTitle("Reference 1 - Auction House")]
    [FieldCheckMaxLength(70)]
    public string Reference1AuctionHouse { get; set; }

    [FieldTitle("Reference 1 - Phone Number")]
    [FieldCheckAmericanOrNotPhone]
    [FieldCheckMaxLength(20)]
    public string Reference1PhoneNumber { get; set; }

    [FieldTitle("Reference 1 - Last Bid Placed")]
    public string Reference1LastBidPlaced { get; set; }

    [FieldTitle("Reference 2 - Auction House")]
    [FieldCheckMaxLength(70)]
    public string Reference2AuctionHouse { get; set; }

    [FieldTitle("Reference 2 - Phone Number")]
    [FieldCheckAmericanOrNotPhone]
    [FieldCheckMaxLength(20)]
    public string Reference2PhoneNumber { get; set; }

    [FieldTitle("Reference 2 - Last Bid Placed")]
    public string Reference2LastBidPlaced { get; set; }

    [FieldTitle("Ebay Bidder ID")]
    [FieldCheckMaxLength(30)]
    public string EbayID { get; set; }

    [FieldTitle("Ebay Feedback")]
    [FieldCheckMinLength(2)]
    [FieldCheckMaxLength(10)]
    [FieldCheckNumeric]
    public string EbayFeedback { get; set; }

    [FieldTitle("IsModifyed")]
    public bool IsModifyed { get; set; }

    [FieldTitle("RecievingOutBidNotice")]
    public bool RecievingOutBidNotice { get; set; }

    public bool NoReferencesAvailable { get; set; }

    public RegisterInfo()
    {
      RecievingOutBidNotice = true;
      BillingCountry = ShippingCountry = 1;
      BillingLikeShipping = false;
    }

    public void Validate(System.Web.Mvc.ModelStateDictionary modelState)
    {
      ValidationCheck.CheckErrors(this, modelState);

      //check Login
      if (!ValidationCheck.IsEmpty(this.Login) && !ProjectConfig.Config.DataProvider.GetInstance().UserRepository.ValidateLogin(this.Login, ID))
      {
        modelState.AddModelError("Login", "This login already present in system");
      }

      //check Email
      if (!ValidationCheck.IsEmpty(this.Email) && !ProjectConfig.Config.DataProvider.GetInstance().UserRepository.ValidateEmail(this.Email, ID))
      {
        modelState.AddModelError("Email", "This email already present in system");
      }

      if (!String.Equals(this.Password, this.ConfirmPassword, StringComparison.Ordinal))
      {
        modelState.AddModelError("Password", "The password and confirmation password do not match.");
        modelState.AddModelError("ConfirmPassword", "");
      }

      if (!String.Equals(this.Email, this.ConfirmEmail, StringComparison.Ordinal))
      {
        modelState.AddModelError("Email", "The email and confirmation email do not match.");
        modelState.AddModelError("ConfirmEmail", "");
      }

      if (this.BillingState=="--" && this.BillingCountry == 1)
      {
        modelState.AddModelError("BillingState", "'State' is required");
      }

      if (this.ShippingState=="--" && this.ShippingCountry == 1 && !this.BillingLikeShipping)
      {
        modelState.AddModelError("ShippingState", "'State' is required");
      }

      if (this.BillingState!="--" && this.BillingCountry > 1)
      {
        modelState.AddModelError("BillingState", "'State' must have value '--'");
      }

      if (this.ShippingState!="--" && this.ShippingCountry > 1 && !this.BillingLikeShipping)
      {
        modelState.AddModelError("ShippingState", "'State' must have value '--'");
      }
      if (this.BillingState=="--" && this.BillingCountry > 1 && String.IsNullOrEmpty(this.BillingInternationalState))
      {
        modelState.AddModelError("BillingInternationalState", "'International State' is required");
      }
      if (this.ShippingState=="--" && this.ShippingCountry > 1 && String.IsNullOrEmpty(this.ShippingInternationalState) && !this.BillingLikeShipping)
      {
        modelState.AddModelError("ShippingInternationalState", "'International State' is required");
      }

    }
  }
}