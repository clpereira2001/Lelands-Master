using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Vauction.Utils.Validation;
using System.Text;

namespace Vauction.Utils.Html
{
  public static class HtmlExtensions
  {
    #region HtmlHelper extensions

    public static string ValidationMessageArea(this HtmlHelper htmlHelper, string name)
    {
      ModelStateDictionary state = htmlHelper.ViewData.ModelState;
      string errorMessage = string.Empty;
      if (state != null && state[name] != null && state[name].Errors != null && state[name].Errors.Count > 0)
      {
        errorMessage = state[name].Errors[0].ErrorMessage;
      }

      Dictionary<string, string> d = new Dictionary<string, string>();

      if (errorMessage == String.Empty)
        return "<span id=\"vm_" + name + "\" class=\"field-validation-error\" id=\"\"></span>";

      return "<span id=\"vm_" + name + "\" class=\"field-validation-error show\" id=\"\">" + errorMessage + "</span>";
    }

    public static string SubmitWithClientValidation(this HtmlHelper htmlHelper, string value)
    {
      string out_str = string.Empty;

      if (htmlHelper.ViewData.Model != null)
      {
        out_str += GetClientErrorData(htmlHelper.ViewData.Model.GetType());
        out_str += "<button class=\"cssbutton small white\" type=\"submit\" id=\"btSubmit\" onclick=\"return SubmitFormWithClientValidation(errorRules, this)\">";
        out_str += "    <span>" + value + "</span>";
        out_str += "</button>";
      }
      else
      {
        out_str += "<button type=\"submit\"  class=\"cssbutton submit_payment small white\"><span>" + value + "</span></button>";
      }

      return out_str;
    }

    public static string SubmitWithClientValidation(this HtmlHelper htmlHelper, string value, string classCss)
    {
      string out_str = string.Empty;

      if (htmlHelper.ViewData.Model != null)
      {
        out_str += GetClientErrorData(htmlHelper.ViewData.Model.GetType());
        out_str += "<button class=\"cssbutton small white " + classCss + "\" type=\"submit\" id=\"btSubmit\" onclick=\"return SubmitFormWithClientValidation(errorRules, this)\">";
        out_str += "    <span>" + value + "</span>";
        out_str += "</button>";
      }
      else
      {
        out_str += "<button type=\"submit\"  class=\"cssbutton submit_payment small white\"><span>" + value + "</span></button>";
      }

      return out_str;
    }

    public static string GetClientErrorData(Type modelType)
    {
      string out_str = "<script language=\"javascript\" type=\"text/javascript\">" + Environment.NewLine;

      out_str += "var errorRules = new Hashtable();" + Environment.NewLine + Environment.NewLine;

      Dictionary<string, List<ValidationRule>> errorRules = ValidationCheck.GetValidationRules(modelType);

      foreach (KeyValuePair<string, List<ValidationRule>> item in errorRules)
      {
        out_str += "var fieldErrorRules = new Hashtable();" + Environment.NewLine;

        foreach (ValidationRule rule in item.Value)
        {
          string errorMessage = rule.ErrorMessage;

          //out_str += "fieldErrorRules.put(\"" + rule.Type.ToString().ToLower() + "\", \"" + errorMessage + "\");" + Environment.NewLine;
          out_str += "fieldErrorRules.put(\"" + rule.Type.ToString().ToLower() + "\", new Array(\"" + errorMessage + "\"" + rule.IFieldValidation.GetAdditionalParams() + "));" + Environment.NewLine;
        }

        out_str += "errorRules.put(\"" + item.Key + "\", fieldErrorRules);" + Environment.NewLine + Environment.NewLine;

      }
      out_str += "SetInputsMaxLength(errorRules);" + Environment.NewLine;
      out_str += "</script>" + Environment.NewLine;
      return out_str;
    }

    #endregion

    public static string GetHtmlTableString(object data, bool printTotal)
    {
      IEnumerable enumData = data as IEnumerable;
      StringBuilder result = null;
      decimal[] total = null;
      if (enumData != null)
      {
        bool header = false;
        result = new StringBuilder("<table>\r\n");
        foreach (object item in enumData)
        {
          if (!header)
          {
            int cols = 0;
            result.Append("\t<tr>\r\n");
            foreach (PropertyInfo info in item.GetType().GetProperties())
            {
              if (info.Name[0] != '_')
              {
                result.AppendFormat("\t\t<th>{0}</th>\r\n", info.Name);
                cols++;
              }
            }
            total = new decimal[cols];
            result.Append("\t</tr>\r\n");
            header = true;
          }

          result.Append("\t<tr>\r\n");
          int index = 0;
          foreach (PropertyInfo info in item.GetType().GetProperties())
          {
            if (info.Name[0] != '_')
            {
              object val = info.GetValue(item, null);
              if (info.DeclaringType == typeof(decimal))
                total[index] += Convert.ToDecimal(val);
              result.AppendFormat("\t\t<td>{0}</td>\r\n", val);
              index++;
            }
          }
          result.Append("\t</tr>\r\n");
        }
        if (printTotal)
        {
          result.Append("\t<tr>\r\n");
          for (int ind = 0; ind < total.Length; ind++)
          {
            if (ind == 0)
              result.AppendFormat("\t\t<td>Total</td>\r\n", total[ind]);
            else if (total[ind] > 0)
              result.AppendFormat("\t\t<td>{0}</td>\r\n", total[ind]);
            else
              result.AppendFormat("\t\t<td></td>\r\n");
          }
          result.Append("\t</tr>\r\n");
        }
        result.Append("</table>");
      }
      return result == null ? null : result.ToString();
    }
  }
}