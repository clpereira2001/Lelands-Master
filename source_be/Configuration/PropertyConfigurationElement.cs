using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Vauction.Configuration
{
	public class PropertyConfigurationElement : ConfigurationElement, IPropertyConfiguration
    {
        public PropertyConfigurationElement() : base()
        {
        }

        public PropertyConfigurationElement(string elementName)
            : base()
        {
            Name = elementName;
        }

        #region IPropertyConfiguration Members

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
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

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get
            {
                return (string)this["value"];
            }
            set
            {
                this["value"] = value;
            }
        }

        #endregion
	}
}
