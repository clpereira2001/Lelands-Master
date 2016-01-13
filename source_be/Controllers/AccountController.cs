using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Vauction.Models;
using Vauction.Utils;
using Vauction.Utils.Autorization;
using Vauction.Utils.Perfomance;

namespace Vauction.Controllers
{
  [HandleError, Compress]
  public class AccountController : BaseController
  {
    #region initialization
    public IFormsAuthenticationService FormsService { get; set; }
    protected override void Initialize(RequestContext requestContext)
    {
      if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
      base.Initialize(requestContext);
    }

    private IUserRepository UserRepository;
    public AccountController()
    {
      UserRepository = dataProvider.UserRepository;      
    }
    #endregion

    #region Log On/ Off methods
    //LogOn
    [HttpGet, VCache(CachingExpirationTime.Days_01)]
    public ActionResult LogOn()
    {
      return View();
    }

    //LogOn
    [HttpPost]    
    public ActionResult LogOn(string userName, string password, bool? rememberMe, string returnUrl)
    {
      if (!ValidateLogOn(userName, password))
      {        
        return View();
      }
      User user = UserRepository.GetUserAdministrator(userName, password);
      if (user == null)
      {
        ModelState.AddModelError("login", "Incorrect UserID or password.");
        return View();
      }
      UserRepository.UserLogInSuccessfuly(user);
      FormsService.SignIn(userName, rememberMe.HasValue && rememberMe.Value, user);
      //if (!String.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);
      if (HttpContext.Request.IsUrlLocalToHost(returnUrl)) return Redirect(returnUrl);
      return RedirectToAction("Index", "Home");
    }
    
    //LogOff
    [HttpGet]
    public ActionResult LogOff()
    {
      FormsService.SignOut();
      return RedirectToAction("Index", "Home");
    }
    #endregion

    #region Validation Methods
    //ValidateLogOn
    private bool ValidateLogOn(string userName, string password)
    {
      if (String.IsNullOrEmpty(userName))
      {
        ModelState.AddModelError("username", "You must specify a username.");
      }
      if (String.IsNullOrEmpty(password))
      {
        ModelState.AddModelError("password", "You must specify a password.");
      }      
      return ModelState.IsValid;
    }
    //ErrorCodeToString
    private static string ErrorCodeToString(MembershipCreateStatus createStatus)
    {
      // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
      // a full list of status codes.
      switch (createStatus)
      {
        case MembershipCreateStatus.DuplicateUserName:
          return "Username already exists. Please enter a different user name.";

        case MembershipCreateStatus.DuplicateEmail:
          return "A username for that e-mail address already exists. Please enter a different e-mail address.";

        case MembershipCreateStatus.InvalidPassword:
          return "The password provided is invalid. Please enter a valid password value.";

        case MembershipCreateStatus.InvalidEmail:
          return "The e-mail address provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidAnswer:
          return "The password retrieval answer provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidQuestion:
          return "The password retrieval question provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidUserName:
          return "The user name provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.ProviderError:
          return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        case MembershipCreateStatus.UserRejected:
          return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        default:
          return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
      }
    }
    #endregion
  }
}
