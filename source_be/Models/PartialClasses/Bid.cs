using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Vauction.Models
{
  [Serializable]
  public partial class Bid : IBid
  {
    public string HighBidderLogin
    {
      get
      {
        return this.User.Login;
      }
    }
    public string HighBidderName
    {
      get
      {
        if (User == null)
          return null;
        return User.AddressCard_Billing.FirstName + " " + User.AddressCard_Billing.LastName + " [" + User.Login + "]";
      }
    }    
  }
}
