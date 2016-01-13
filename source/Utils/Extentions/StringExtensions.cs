using System;
using System.Configuration;
using Vauction.Configuration;

namespace Vauction.Utils
{
	public static class StringExtensions
	{
		#region get news teaser
		public static string Teaser(this string s, IVauctionConfiguration config)
		{
			char[] anyOf = config.GetProperty("Punctuation").ToCharArray();

			int TeaserLenght = Convert.ToInt32( config.GetProperty("TeaserLenght") );

			return s.Substring(0, s.IndexOfAny(anyOf, TeaserLenght) + 1);
		}

		public static string Teaser(this string s)
		{
			IVauctionConfiguration config = (IVauctionConfiguration)ConfigurationManager.GetSection("Vauction");
			
			char[] anyOf = config.GetProperty("Punctuation").ToCharArray();

			int TeaserLenght = Convert.ToInt32(config.GetProperty("TeaserLenght"));

			return s.Substring(0, s.IndexOfAny(anyOf, TeaserLenght) + 1);
		}
		#endregion
	}
}