//using SSB.Web.Mvc;
using Vauction.Utils.Validation;

namespace Vauction.Models.CustomModels
{
  public class ConsignForm
  {
    [FieldTitle("First Name")]
    [FieldRequired]
    public string FirstName { get; set; }

    [FieldTitle("Last Name")]
    [FieldRequired]
    public string LastName { get; set; }

    [FieldTitle("Email")]
    [FieldRequired]
    [FieldCheckEmail]
    [FieldCheckMaxLength(255)]
    public string Email { get; set; }

    [FieldTitle("Phone")]
    public string Phone { get; set; }

    public string BestTime { get; set; }

    [FieldRequired]
    public string Description { get; set; }

    //[ValidateMvcCaptcha(ErrorMessage = "The entered text is incorrect. Please try again.")]
    public string captchaValue { get; set; }

    public void Validate(System.Web.Mvc.ModelStateDictionary modelState)
    {
      ValidationCheck.CheckErrors(this, modelState);
    }
  }
}