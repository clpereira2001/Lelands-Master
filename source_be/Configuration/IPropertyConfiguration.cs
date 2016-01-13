using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Configuration
{
	public interface IPropertyConfiguration
	{
		string Name { get; set; }
		string Value { get; set; }
	}
}
