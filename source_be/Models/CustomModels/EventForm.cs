using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Vauction.Utils.Validation;

namespace Vauction.Models
{
  [Serializable]
  public class EventForm : IEvent
  {
    #region IEvent Members

    [FieldTitle("Entering Fee")]
    public decimal? EnteringFee { get; set; }

    public string ThumbNailUrl { get; set; }
    public string FullThumbNailPath { get; set; }

    public long ID { get; set; }

    [FieldRequired]
    public string Title { get; set; }

    [FieldTitle("Start Date")]
    [FieldRequired]
    public DateTime DateStart { get; set; }

    [FieldTitle("End Date")]
    [FieldRequired]
    public DateTime DateEnd { get; set; }

    [FieldTitle("Buyer Fee")]
    [FieldRequired]
    [FieldCheckDecimal]
    [FieldCheckMinLength(0)]
    [FieldCheckMaxLength(100)]
    public decimal? BuyerFee { get; set; }

    public string Description { get; set; }

    public bool IsViewable { get; set; }

    public bool IsClickable { get; set; }

    public byte CloseStep { get; set; }

    [FieldRequired]
    [FieldCheckNumeric]
    [FieldCheckMinLength(0)]
    [FieldCheckMaxLength(1000)]
    public int? Ordinary { get; set; }

    // do not used
    public bool IsCurrent { get; set; }
    public DateTime LastUpdate { get; set; }
    public int Type_ID { get; set; }

    #endregion

    public List<long> CategoriesList;

    #region Validation

    public static long StringToLong(string _id)
    {
      return Convert.ToInt64(_id);
    }

    //ValidateWithConfim (Email, Password)
    public void Validate(ModelStateDictionary modelState)
    {
      ValidationCheck.CheckErrors(this, modelState, true);

      //check start and end date
      if (DateStart.CompareTo(DateEnd) >= 0)
        modelState.AddModelError("DateEnd", "End Date must be greater than Start Date.");

      try
      {
        CategoriesList = new List<long>();
        if (!String.IsNullOrEmpty(Categories))
          CategoriesList.AddRange(Array.ConvertAll(Categories.Split(','), new Converter<string, long>(StringToLong)));
      }
      catch (Exception ex)
      {
        modelState.AddModelError("Categories", ex.Message);
      }

      if (CategoriesList == null || CategoriesList.Count == 0)
        modelState.AddModelError("CategoriesList", "Choose category(s) before updating this event.");
    }

    #endregion

    [FieldRequired]
    public string Categories { get; set; }

    public string StartEndTime
    {
      get { throw new NotImplementedException(); }
    }
  }
}