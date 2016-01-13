using System.Web.Routing;

namespace Vauction.Utils
{
	public static class RouteValueDictionaryExtensions
	{
		public static RouteValueDictionary ChangeOrderByDirection(this RouteValueDictionary routeValueDictionary, string orderDirection)
		{
			//if ( routeValueDictionary["orderby"] != null )
			{
				if (orderDirection == Vauction.Utils.Consts.OrderByValues.descending.ToString())
				{
					routeValueDictionary.Remove("orderby");
					routeValueDictionary.Add("orderby", Vauction.Utils.Consts.OrderByValues.ascending.ToString());
				}
				else
				{
					routeValueDictionary.Remove("orderby");
					routeValueDictionary.Add("orderby", Vauction.Utils.Consts.OrderByValues.descending.ToString());
				}
			}

			return routeValueDictionary;
		}


	}

}
