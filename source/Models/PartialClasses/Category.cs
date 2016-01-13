using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Vauction.Models
{
  [Serializable]
  partial class Category : ICategory
  {
    public string UrlTitle
    {
      get { return Vauction.Utils.UrlParser.TitleToUrl(Title); }
    }
  }
}
