using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Runtime.Serialization;

namespace Vauction.Models
{
  [Serializable]
  partial class Event : IEvent
  {
    public string UrlTitle
    {
      get { return Vauction.Utils.UrlParser.TitleToUrl(Title); }
    }

    public string StartEndTime
    {
      get { return String.Format("({0} EST to {1} EST)", DateStart, DateEnd); }
    }
    public string StartTime
    {
      get { return String.Format("{0} EST", DateStart); }
    }
    public string EndTime
    {
      get { return String.Format("{0} EST", DateEnd); }
    }
  }
}
