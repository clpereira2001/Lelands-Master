using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Vauction.Models
{
  [Serializable]
  partial class EventCategory : IEventCategory
  {
    public string FullCategory
    {
      get { return MainCategory.Title + " > " + Category.Title; }
    }
  }
}
