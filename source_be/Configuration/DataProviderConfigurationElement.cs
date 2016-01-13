using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Vauction.Configuration
{
	public class DataProviderConfigurationElement : ConfigurationElement, IDataProviderConfiguration
	{
		#region IDataProviderConfiguration Members

		[ConfigurationProperty("name", IsRequired = true)]
		public string Name
		{
			get
			{
				return (string)this["name"];
			}
			set
			{
				this["name"] = value;
			}
		}

		[ConfigurationProperty("type", IsRequired = true)]
		public string Type
		{
			get
			{
				return (string)this["type"];
			}
			set
			{
				this["type"] = value;
			}
		}

		[ConfigurationProperty("connectionStringName", IsRequired = true)]
		public string ConnectionStringName
		{
			get
			{
				return (string)this["connectionStringName"];
			}
			set
			{
				this["connectionStringName"] = value;
			}
		}

		#endregion
	}
}

