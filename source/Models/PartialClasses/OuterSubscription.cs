using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Vauction.Configuration;
using Vauction.Utils.Validation;
using System.Runtime.Serialization;

namespace Vauction.Models
{
  [Serializable]
  partial class OuterSubscription : IOuterSubscription
  {
    public void Validate(System.Web.Mvc.ModelStateDictionary modelState)
    {
      ValidationCheck.CheckErrors(this, modelState);

      //check Email
      if (!ValidationCheck.IsEmpty(this.Email) && !ProjectConfig.DataProvider.DifferentRepository.ValidateOuterSubscriptionEmail(this.Email, ID))
      {
        modelState.AddModelError("Email", "This email already present in system");
      }

      if (!(this.Email == this.EmailConfirm))
      {
        modelState.AddModelError("Email", "Email and confirmation email should be match.");
      }

      if (!IsRecievingWeeklySpecials && !IsRecievingUpdates)
      {
        modelState.AddModelError("IsRecievingUpdates", "Select to recieve news and updates or/and weekly specials.");
      }
    }

    public string EmailConfirm { get; set; }
  }
}