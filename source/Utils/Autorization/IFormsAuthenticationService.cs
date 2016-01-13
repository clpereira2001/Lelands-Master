
using Vauction.Models;

namespace Vauction.Utils.Autorization
{
  public interface IFormsAuthenticationService
  {
    void SignIn(string userName, bool createPersistentCookie, User user);
    void SignOut();
  }
}