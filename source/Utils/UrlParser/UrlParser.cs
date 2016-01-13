using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace Vauction.Utils
{
  public class UrlParser
  {
    public static string TitleToUrl(string title)
    {      
      StringBuilder sb = new StringBuilder(title.Trim());

      Regex regex = new Regex(@"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", RegexOptions.Singleline);
      MatchCollection matches = regex.Matches(sb.ToString());
      foreach (Match mch in matches)
        sb.Replace(mch.Value, "");
      sb.Replace("  ", " ");
      sb.Replace(" - ", " ");
      sb.Replace(" ", "-");
      sb.Replace(",", "");
      sb.Replace("...", "");
      sb.Replace("!", "-");
      sb.Replace("#", "-");
      sb.Replace("'", "");
      sb.Replace("/", "-");
      sb.Replace("\\", "-");
      sb.Replace("\n", "-");
      sb.Replace("\"", "-");
      sb.Replace("&", "and-");
      sb.Replace(":", "-");
      sb.Replace("+", "-");
      sb.Replace(".", "_");
      sb.Replace("<", "-");
      sb.Replace(">", "-");
      sb.Replace("?", "-");
      sb.Replace("%", "-");
      sb.Replace("*", "-");
      sb.Replace("--", "-");
      sb.Replace("__", "_");
      return sb.Length < 121 ? sb.ToString() : sb.ToString().Substring(0, 120);
    }
  }
}
