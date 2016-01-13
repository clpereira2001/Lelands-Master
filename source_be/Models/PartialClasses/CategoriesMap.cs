using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  partial class CategoriesMap : ICategoriesMap
  {
    public string FullCategory
    {
      get { return String.Format("{0} > {1}", (MainCategory != null) ? MainCategory.Title : String.Empty, (Category != null) ? Category.Title : String.Empty); }
    }
  }
}
