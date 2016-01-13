using System;
using System.Configuration;
using Vauction.Configuration;
using System.Web.Script.Serialization;

namespace Vauction.Utils
{
    public static class JSONExtensions
    {
        public static string ToJSON(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            return serializer.Serialize(obj);
        }

        public static string ToJSON(this object obj, int recursionDepth)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            serializer.RecursionLimit = recursionDepth;

            return serializer.Serialize(obj);
        }
    }
}
