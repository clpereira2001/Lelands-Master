using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  partial class Specialist : ISpecialist
  {
    public string FullName
    {
      get { return String.Format("{0}{1}", FirstName, (!String.IsNullOrEmpty(LastName))?" "+LastName:String.Empty); }
    }
  }
}
