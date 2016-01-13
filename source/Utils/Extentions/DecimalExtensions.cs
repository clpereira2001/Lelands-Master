using System;
using System.Configuration;
using System.Globalization;
using Vauction.Configuration;
using Vauction.Utils.Helpers;

namespace Vauction.Utils
{
  public static class DecimalExtensions
  {
    public static string GetCurrency(this decimal? value)
    {
      if (!value.HasValue || value.Value == 0) return "";
      return Decimal.Round(value ?? 0, 2).ToString("C", CultureInfo.CurrentCulture);
    }

    public static string GetCurrency(this decimal? value, bool IsNullZero)
    {
      if (!value.HasValue || (IsNullZero && value.Value == 0)) return "";
      return Decimal.Round(value ?? 0, 2).ToString("C", CultureInfo.CurrentCulture);
    }

    public static string GetCurrency(this decimal value)
    {
      if (value == 0) return "";
      return Decimal.Round(value, 2).ToString("C", CultureInfo.CurrentCulture);
    }

    public static string GetCurrency(this decimal value, bool IsNullZero)
    {
      if (value == 0 && IsNullZero) return "";
      return Decimal.Round(value, 2).ToString("C", CultureInfo.CurrentCulture);
    }

    public static decimal GetDecimalCurrency(this decimal? value)
    {
      return Decimal.Round(value ?? 0, 2);
    }

    public static decimal GetDecimalCurrency(this decimal value)
    {
      return Decimal.Round(value, 2);
    }
    public static decimal GetPrice(this decimal value)
    {
      return value.GetDecimalCurrency();
    }
    public static decimal GetPrice(this decimal? value)
    {
      if (value.HasValue)
        return value.Value.GetPrice();
      else
        return 0;
    }
  }
}
