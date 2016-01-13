using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Configuration
{
	public interface IDataProviderConfiguration
	{
		string Name { get; set; }
		string Type { get; set; }
		string ConnectionStringName { get; set; }
	}
}