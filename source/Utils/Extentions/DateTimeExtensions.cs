using System;
using System.Configuration;
using Vauction.Configuration;

namespace Vauction.Utils
{
    public static class DateTimeExtensions
    {
        public static string Text(this DateTime values, string Format)
        {
			System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");

			if ( values == DateTime.MinValue )
			{
				return string.Empty;
			}
			else
			{
				return values.ToString(Format, new System.Globalization.CultureInfo("ru-RU"));
			}
        }

		#region Convert DateTime to text

		public static string Text(this DateTime values, IVauctionConfiguration config)
		{
			return Text(values, config.GetProperty("DateFormat") );
		}

		public static string Text(this DateTime values)
		{
			IVauctionConfiguration config = (IVauctionConfiguration)ConfigurationManager.GetSection("Vauction");

			return Text(values, config.GetProperty("DateFormat"));
		}

		#endregion

        #region Convert text To Date

        //This will return True or Flase based on date format supported. And give converted Date from String.
        public static DateTime ToDate(this string dateString)
        {
            DateTime resultDate;
            
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-CA");
                
                resultDate = DateTime.ParseExact(dateString, SupportedDateFormats(), new System.Globalization.CultureInfo("en-CA"), System.Globalization.DateTimeStyles.None );
            }
            catch
            {
                resultDate = DateTime.MinValue;
            }

            return resultDate;
        }

        private static string[] SupportedDateFormats()
        {
            string[] allFormats = new string[] {
                    "dd/MM/yyyy",
                    "dd.MM.yyyy",
                    "dd-MM-yyyy"
                    //,
                    //"MM/dd/yy",
                    //"M/d/yy",
                    //"MM/d/yy",
                    //"M/dd/yy",
                    //"M/d/yyyy",
                    //"MM/d/yyyy",
                    //"M/dd/yyyy"
            };

            return allFormats;
        }

        #endregion
    }
}
