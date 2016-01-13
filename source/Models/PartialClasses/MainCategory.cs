using System;
using System.Runtime.Serialization;

namespace Vauction.Models
{
  [Serializable]
  partial class MainCategory : IMainCategory
  {
    public string UrlTitle
    {
      get { return Vauction.Utils.UrlParser.TitleToUrl(Title); }
    }
  }
}
