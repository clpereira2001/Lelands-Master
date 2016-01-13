using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils.Validation;
using Vauction.Configuration;

namespace Vauction.Models
{
  [Serializable]
  public class NewAuctionForConsignorForm
  {
    [FieldRequired]
    public long Auction_ID { get; set; }

    [FieldRequired]
    [FieldTitle("Consignor")]
    public long User_ID { get; set; }

    [FieldTitle("Event")]
    [FieldRequired]
    public long Event_ID { get; set; }

    [FieldTitle("Commission Rate")]
    [FieldRequired]
    public int CommissionRate_ID { get; set; }

    [FieldTitle("Number Of Items")]
    [FieldRequired]
    [FieldCheckNumeric]
    [FieldCheckMinLength(1)]
    [FieldCheckMaxLength(1)]
    public int Quantity { get; set; }

    [FieldTitle("Main Category")]
    [FieldRequired]
    public byte MainCategory_ID { get; set; }

    [FieldTitle("Category")]
    [FieldRequired]
    public long? Category_ID { get; set; }

    public byte Priority { get; set; }

    public byte ListingStep { get; set; }

    public long? OldInventory { get; set; }

    [FieldTitle("Price")]
    [FieldRequired]
    [FieldCheckDecimal]
    [FieldCheckMinLength(1)]
    public decimal? Price { get; set; }

    public bool LOA { get; set; }

    [FieldTitle("Title")]
    [FieldRequired]
    public string Title { get; set; }

    public string CopyNotes { get; set; }
    public string PhotoNotes { get; set; }
    public string Description { get; set; }

    public bool IsLimitDisabled { get; set; }

    public decimal? PurchasedPrice { get; set; }
    public decimal? SoldPrice { get; set; }
    public int? PurchasedWay { get; set; }
    public int? SoldWay { get; set; }

    #region Validation
    public void Validate(System.Web.Mvc.ModelStateDictionary modelState)
    {
      ValidationCheck.CheckErrors(this, modelState, true);
    }
    #endregion
  }
}
