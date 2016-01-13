using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Configuration;
using Vauction.Utils.Lib;

namespace Vauction
{
	public static class IVauctionConfigurationExtensions
	{
		private static Dictionary<string, string> dictionaryProperties;

		public static string GetProperty(this IVauctionConfiguration config, string name)
		{
			string property_value = String.Empty;

			if (dictionaryProperties == null)
			{
				dictionaryProperties = new Dictionary<string, string>();
			}

			if (dictionaryProperties.ContainsKey(name))
			{
				property_value = dictionaryProperties[name];
			}
			else
			{
				if (config != null)
				{
					foreach (IPropertyConfiguration property in config.Property)
					{
						if (property.Name == name)
						{
							property_value = property.Value;
							break;
						}
					}

					if ( ! String.IsNullOrEmpty(property_value))
					{
                        try
                        {
                            if (!dictionaryProperties.ContainsKey(name))
                                dictionaryProperties.Add(name, property_value);
                        }
                        catch (Exception e)
                        {
                            Logger.LogException(e);

                        }
					}
					else
					{
						throw new Exception(string.Format("Could not find property: '{0}'", name));
					}
				}
			}

			return property_value;
		}

		public static int GetIntProperty(this IVauctionConfiguration config, string name)
		{
			return Convert.ToInt32( GetProperty(config, name) );
		}
	}
}
