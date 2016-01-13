using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils.Validation;
using Vauction.Configuration;

namespace Vauction.Models
{
  [Serializable]
  public class ConsignorStatementForm : IConsignment
  {
    [FieldRequired]
    public long ID { get; set; }

    [FieldRequired]
    [FieldTitle("Consignor")]
    public long User_ID { get; set; }

    [FieldRequired]
    [FieldTitle("Event")]
    public long Event_ID { get; set; }

    [FieldRequired]
    [FieldTitle("Date")]
    public DateTime ConsDate { get; set;}

    [FieldRequired]
    [FieldTitle("Specialist")]
    public long? Specialist_ID { get; set; }

    #region Validation
    //ValidateWithConfim (Email, Password)
    public void Validate(System.Web.Mvc.ModelStateDictionary modelState)
    {
      ValidationCheck.CheckErrors(this, modelState, true);
    }
    #endregion
  }
}
