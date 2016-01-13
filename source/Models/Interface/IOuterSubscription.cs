using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils.Validation;
using System.Data;
using System.Data.SqlTypes;

namespace Vauction.Models
{
  public interface IOuterSubscription
  {
    Int64 ID { get; set; }

    [FieldTitle("First Name")]
    [FieldRequired]
    string FirstName { get; set; }

    [FieldTitle("Last Name")]
    [FieldRequired]
    string LastName { get; set; }

    [FieldTitle("Email")]
    [FieldCheckEmail]
    [FieldRequired]
    string Email { get; set; }

    bool IsRecievingWeeklySpecials { get; set; }
    bool IsRecievingUpdates { get; set; }
    bool IsActive { get; set; }
  }
}
