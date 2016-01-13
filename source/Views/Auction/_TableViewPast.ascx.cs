using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Web.Mvc;
using Vauction.Models;
using System.Collections.Generic;
using System.Web.Routing;
using Vauction.Models.CustomModels;
using Vauction.Utils;
using Vauction.Utils.Helpers;

namespace Vauction.Views.Auction
{
  public partial class TableViewPast : ViewUserControl<IEnumerable<Vauction.Models.AuctionShort>>
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      generalParams = (GeneralFilterParams)ViewData["filterParam"];
      generalRouteParameters = CollectParameters.CollectAll();
    }
    protected RouteValueDictionary GetSpesificParams(Consts.CategorySortFields title)
    {
      RouteValueDictionary routeParameters = generalRouteParameters;
      if (!routeParameters.ContainsKey("sortby"))
        routeParameters.Add("sortby", title);
      else
        routeParameters["sortby"] = title;

      if (generalParams.Sortby.ToString() == routeParameters["sortby"].ToString())
      {
        routeParameters.ChangeOrderByDirection(generalParams.Orderby.ToString());
      }
      else
        routeParameters["orderby"] = Consts.OrderByValues.ascending;
      return routeParameters;
    }
    private RouteValueDictionary generalRouteParameters;
    private GeneralFilterParams generalParams;
  }
}