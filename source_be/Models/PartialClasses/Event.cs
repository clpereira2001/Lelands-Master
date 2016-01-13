using System;
using System.Linq;
using System.Text;
using Vauction.Utils;
using Vauction.Utils.Lib;

namespace Vauction.Models
{
  [Serializable]
  partial class Event : IEvent
  {
    public string UrlTitle
    {
      get { return UrlParser.TitleToUrl(Title); }
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

    public string ActiveCategoriesList
    {
      get
      {
        var sb = new StringBuilder();
        foreach (var ec in EventCategories.Where(EC => EC.IsActive))
          if (ec.CategoriesMap != null)
            sb.AppendFormat("{0},", ec.CategoriesMap.ID);
          else Logger.LogInfo("ActiveCategoriesList -> EC.ID=" + ec.ID);
        if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
      }
    }
  }
}