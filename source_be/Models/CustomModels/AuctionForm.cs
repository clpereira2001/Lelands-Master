using System;
using System.Collections.Generic;
using Vauction.Utils.Validation;

namespace Vauction.Models
{
  [Serializable]
  public class AuctionForm
  {
    #region variables
    public Int64 ID { get; set; }

    [FieldTitle("Event")]
    [FieldRequired]
    public Int64 Event_ID { get; set; }

    [FieldTitle("Consignor")]
    [FieldRequired]
    public Int64? Owner_ID { get; set; }

    [FieldTitle("Commission")]
    [FieldRequired]
    public Int32 CommissionRate_ID { get; set; }

    [FieldTitle("Status")]
    [FieldRequired]
    public byte Status_ID { get; set; }

    [FieldRequired]
    public string Title { get; set; }

    public short? Lot { get; set; }

    [FieldTitle("Main Category")]
    [FieldRequired]
    public Int32? MainCategory_ID { get; set; }

    [FieldTitle("Category")]
    [FieldRequired]
    public Int64? Category_ID { get; set; }

    [FieldRequired]
    public Int32? Quantity { get; set; }

    //[FieldRequired]
    //[FieldCheckDecimal]
    //[FieldCheckMinLength(1)]
    public decimal? Price { get; set; }

    [FieldRequired]
    [FieldCheckDecimal]
    [FieldCheckMinLength(1)]
    public decimal? Reserve { get; set; }

    public string Estimate { get; set; }

    [FieldRequired]
    [FieldCheckDecimal]
    [FieldCheckMinLength(0)]
    public decimal? Shipping { get; set; }

    [FieldTitle("Listing Step")]
    [FieldRequired]
    [FieldCheckNumeric]
    public byte ListingStep { get; set; }

    [FieldTitle("Priority")]
    [FieldRequired]
    [FieldCheckNumeric]
    public byte? Priority { get; set; }

    public bool LOA { get; set; }

    public string Addendum { get; set; }

    [FieldTitle("Old Auction#")]
    public Int64? OldAuction_ID { get; set; }

    public long? CollectionID { get; set; }

    public bool PulledOut { get; set; }
    public bool IsUnsold { get; set; }

 
    public bool IsPrinted { get; set; }
    public bool IsInLayout { get; set; }
    public bool IsPhotographed { get; set; }

    public string Description { get; set; }
    public string CopyNotes { get; set; }
    public string PhotoNotes { get; set; }

    public bool IsCatalog { get; set; }
    public bool IsChecked { get; set; }
    public bool IsLimitDisabled { get; set; }

    [FieldTitle("Start Date")]
    [FieldCheckDateTime]
    public DateTime? StartDate { get; set; }

    [FieldTitle("End Date")]
    [FieldCheckDateTime]
    public DateTime? EndDate { get; set; }


    public decimal? PurchasedPrice { get; set; }
    public decimal? SoldPrice { get; set; }
    public int? PurchasedWay { get; set; }
    public int? SoldWay { get; set; }

    public List<long> Tags { get; set; }

    public List<string> ImagesTag { get; set; }
    #endregion

    #region Validation
    public void Validate(System.Web.Mvc.ModelStateDictionary modelState)
    {
      ValidationCheck.CheckErrors(this, modelState, true);
    }
    #endregion
  }
}
