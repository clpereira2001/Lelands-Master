using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Vauction.Configuration
{
	public class PropertyConfigurationElementCollection : ConfigurationElementCollection, IPropertyConfigurationCollection
	{
		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.AddRemoveClearMap;
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new PropertyConfigurationElement();
		}

		protected override ConfigurationElement CreateNewElement(string elementName)
		{
			return new PropertyConfigurationElement(elementName);
		}

		protected new PropertyConfigurationElement BaseGet(int index)
		{
			return (PropertyConfigurationElement)base.BaseGet(index);
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((PropertyConfigurationElement)element).Name;
		}
	}
}
