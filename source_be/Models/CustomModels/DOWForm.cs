using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils.Validation;

namespace Vauction.Models
{
  [Serializable]
  public class DOWForm
  {
    #region variables
    public Int64 ID { get; set; }

    [FieldTitle("Status")]
    [FieldRequired]
    public byte Status_ID { get; set; }

    [FieldTitle("Main Category")]
    [FieldRequired]
    public int MainCategory_ID { get; set; }

    [FieldTitle("Category")]
    [FieldRequired]
    public long Category_ID { get; set; }

    [FieldTitle("Title")]
    [FieldRequired]
    public string Title { get; set; }

    public string Addendum { get; set; }
    public string Description { get; set; }

    [FieldTitle("Price")]
    [FieldRequired]
    [FieldCheckDecimal]
    [FieldCheckMinLength(1)]
    public decimal? Price { get; set; }

    [FieldTitle("Shipping")]
    [FieldRequired]
    [FieldCheckDecimal]
    [FieldCheckMinLength(0)]
    public decimal? Shipping { get; set; }

    [FieldTitle("Old Auction#")]
    public Int64? OldAuction_ID { get; set; }

    [FieldTitle("Consignor")]
    [FieldRequired]
    public Int64? Owner_ID { get; set; }

    [FieldTitle("Commission")]
    [FieldRequired]
    public Int32 CommissionRate_ID { get; set; }

    #endregion

    #region Validation
    public void Validate(System.Web.Mvc.ModelStateDictionary modelState)
    {
      ValidationCheck.CheckErrors(this, modelState, true);
    }
    #endregion
  }
}
