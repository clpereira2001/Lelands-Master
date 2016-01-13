using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace Vauction.Utils.Perfomance
{
  public static class HtmlHelpers
  {
    const string keyForScript = "__key_For_Js_StringBuilder";
    const string keyForStyle = "__key_For_Css_StringBuilder";

    // AddToStringBuilder
    static void AddToStringBuilder(this ViewDataDictionary dictionary, string key, string addString, char delimBetweenStrings)
    {
      StringBuilder str;
      object viewDataObject;
      if (!dictionary.TryGetValue(key, out viewDataObject))
      {
        str = new StringBuilder();
        str.Append(addString);
        dictionary[key] = str;
      }
      else
      {
        str = viewDataObject as StringBuilder;
        str.Append(delimBetweenStrings);
        str.Append(addString);
      }
    }

    public static void Clear(this HtmlHelper html)
    {
      object viewDataObject;
      if (html.ViewData.TryGetValue(keyForStyle, out viewDataObject))
        html.ViewData.Remove(keyForStyle);
      if (html.ViewData.TryGetValue(keyForScript, out viewDataObject))
        html.ViewData.Remove(keyForScript);
    }

    //Script
    public static void Script(this HtmlHelper html, string file)
    {
      html.ViewData.AddToStringBuilder(keyForScript, AppHelper.ScriptUrl(file), '|');
    }

    //ScriptUrl
    public static void ScriptUrl(this HtmlHelper html, string url)
    {
      html.ViewData.AddToStringBuilder(keyForScript, url, '|');
    }

    //Style
    public static void Style(this HtmlHelper html, string file)
    {
      html.ViewData.AddToStringBuilder(keyForStyle, AppHelper.CssUrl(file), '|');
    }

    //StyleUrl
    public static void StyleUrl(this HtmlHelper html, string url)
    {
      html.ViewData.AddToStringBuilder(keyForStyle, url, '|');
    }

    //CompressJs
    public static string CompressJs(this HtmlHelper html, UrlHelper url)
    {
      StringBuilder builder = html.ViewData[keyForScript] as StringBuilder;
      if (builder == null || String.IsNullOrEmpty(builder.ToString())) return String.Empty;
      return string.Format(@"<script type=""text/javascript"" src=""{0}"" ></script>", AppHelper.CompressScriptForFiles(builder.ToString()));      
    }

    //CompressCss
    public static string CompressCss(this HtmlHelper html, UrlHelper url)
    {
      StringBuilder builder = html.ViewData[keyForStyle] as StringBuilder;
      if (builder == null || String.IsNullOrEmpty(builder.ToString())) return String.Empty;
      return string.Format(@"<link rel=""stylesheet"" href=""{0}"" type=""text/css"" ></link>", AppHelper.CompressStyleForFiles(builder.ToString()));
      //return string.Format(@"<link href=""{0}"" type=""text/css"" rel=""stylesheet"" />", url.Action("Style", "Zip", new { Path = builder.ToString() }));
    }
  } 
}