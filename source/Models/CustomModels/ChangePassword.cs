using System;
using System.Reflection;
using System.Collections.Generic;
using System.Web.Mvc;

using Vauction.Configuration;
using Vauction.Utils.Autorization;
using Vauction.Utils.Validation;
using Vauction.Utils;

namespace Vauction.Models
{
  public class ChangePassword
  {
    public int PasswordLength = 4;
    [FieldTitle("New password")]
    [FieldRequired]
    [FieldCheckMinLength(4)]
    public string NewPassword { get; set; }

    [FieldTitle("Confirm passowrd")]
    [FieldRequired]
    public string ConfirmPassword { get; set; }

    [FieldTitle("Confirm passowrd")]
    [FieldRequired]
    public string CurrentPassword { get; set; }

    public void Validate(System.Web.Mvc.ModelStateDictionary modelState)
    {
      ValidationCheck.CheckErrors(this, modelState);

      if (!String.Equals(this.NewPassword, this.ConfirmPassword, StringComparison.Ordinal))
      {
        modelState.AddModelError("NewPassword", "The new password and confirmation password do not match");
        modelState.AddModelError("ConfirmPassword", "");
      }

      SessionUser cuser = AppHelper.CurrentUser;
      if (!ProjectConfig.DataProvider.UserRepository.ValidatePasswordForUser(this.CurrentPassword, cuser != null ? cuser.ID : 0))
      {
        modelState.AddModelError("CurrentPassword", "Current password is invalid");
      }
    }
  }
}
