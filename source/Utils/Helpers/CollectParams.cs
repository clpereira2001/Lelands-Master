using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Vauction.Utils.Helpers
{
	public class CollectParameters
	{
		public string page { get; set; }
		public string id { get; set; }
    public string orderby { get; set; }
		public string sortby { get; set; }
		public string ViewMode { get; set; }

		private Dictionary<string, object> parameters = new Dictionary<string, object>();

		#region Constructors

    public CollectParameters(string controller, string action, object id, string evnt, string maincat, string cat, string lot)
    {
      parameters.Add("controller", controller);
      parameters.Add("action", action);
      parameters.Add("id", id);
      parameters.Add("evnt", evnt);
      parameters.Add("maincat", maincat);
      parameters.Add("cat", cat);
      parameters.Add("lot", lot);
    }

    public CollectParameters(string controller, string action, object id, string evnt, string maincat, string cat)
    {
      parameters.Add("controller", controller);
      parameters.Add("action", action);
      parameters.Add("id", id);
      parameters.Add("evnt", evnt);
      parameters.Add("maincat", maincat);
      parameters.Add("cat", cat);
    }

    public CollectParameters(string controller, string action, object id, string evnt, string maincat)
    {
      parameters.Add("controller", controller);
      parameters.Add("action", action);
      parameters.Add("id", id);
      parameters.Add("evnt", evnt);
      parameters.Add("maincat", maincat);
    }

    public CollectParameters(string controller, string action, object id, string evnt)
    {
      parameters.Add("controller", controller);
      parameters.Add("action", action);
      parameters.Add("id", id);
      parameters.Add("evnt", evnt);
    }

		public CollectParameters(string controller, string action, object id)
		{
			parameters.Add("controller", controller);
			parameters.Add("action", action);
			parameters.Add("id", id);
		}

		public CollectParameters(string controller, string action)
		{
			parameters.Add("controller", controller);
			parameters.Add("action", action);
		}

		public CollectParameters(string action)
		{
			parameters.Add("action", action);
		}

		public CollectParameters()
		{
		}

		#endregion

		public RouteValueDictionary Collect()
		{
			return new RouteValueDictionary(parameters);
		}

		#region Collect Parameters

		public RouteValueDictionary Collect( params string[] paramsList )
		{
			for ( int i = 0; i < paramsList.Length; i++ )
			{
				if ( HttpContext.Current.Request.QueryString[paramsList[i]] != null )
				{
					switch ( paramsList[i].ToString().ToLower() )
					{
						case "id":
							parameters.Add("id", HttpContext.Current.Request.QueryString["id"]);
							break;
						case "page":
							parameters.Add( "page", HttpContext.Current.Request.QueryString["page"] );
							break;
            case "evnt":
              parameters.Add("evnt", HttpContext.Current.Request.QueryString["evnt"]);
              break;
            case "maincat":
              parameters.Add("maincat", HttpContext.Current.Request.QueryString["maincat"]);
              break;
            case "cat":
              parameters.Add("cat", HttpContext.Current.Request.QueryString["cat"]);
              break;
            case "lot":
              parameters.Add("lot", HttpContext.Current.Request.QueryString["lot"]);
              break;
						case "viewmode":
							parameters.Add("ViewMode", HttpContext.Current.Request.QueryString["ViewMode"]);
							break;
						case "sortby":
							parameters.Add("sortby", HttpContext.Current.Request.QueryString["sortby"]);
							break;
						case "orderby":
							parameters.Add("orderby", HttpContext.Current.Request.QueryString["orderby"]);
							break;
					}
				}
			}

			return new RouteValueDictionary(parameters);
		}

    public static RouteValueDictionary CollectAll()
    {
      Dictionary<string, object> allParameters = new Dictionary<string, object>();
      foreach (string key in HttpContext.Current.Request.QueryString)
      {
        allParameters.Add(key, HttpContext.Current.Request.QueryString[key]);
      }      
      return new RouteValueDictionary(allParameters);
    }

		#endregion
	}
}
