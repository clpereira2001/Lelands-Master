using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vauction.Models;

namespace Vauction.Utils.Autorization
{
  public interface IFormsAuthenticationService
  {
    void SignIn(string userName, bool createPersistentCookie, User user);
    void SignIn(string userName, bool createPersistentCookie, long user_id);
    void SignOut();
  }
}