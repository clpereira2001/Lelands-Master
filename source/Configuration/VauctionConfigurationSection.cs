using System.Configuration;
using Vauction.Configuration;

namespace Vauction.Configuration
{
	public class VauctionConfigurationSection : ConfigurationSection, IVauctionConfiguration
    {
        [ConfigurationProperty("dataProvider")]
        public DataProviderConfigurationElement DataProviderInternal
        {
            get
            {
                return (DataProviderConfigurationElement)this["dataProvider"];
            }
            set
            {
                this["dataProvider"] = value;
            }
        }

		[ConfigurationProperty("properties")]
		public PropertyConfigurationElementCollection PropertyInternal
		{
			get
			{
				return (PropertyConfigurationElementCollection)this["properties"];
			}
			set
			{
				this["properties"] = value;
			}
		}

        #region IVauctionConfiguration Members

		public IPropertyConfigurationCollection Property
		{
			get
			{
				return PropertyInternal;
			}
		}

        public IDataProviderConfiguration DataProvider
        {
            get
            {
                return DataProviderInternal;
            }
        }

        #endregion
    }
}