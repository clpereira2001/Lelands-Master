using System;
using System.Configuration;
using System.Reflection;
using Vauction.Models;

namespace Vauction.Configuration
{
	public static class IDataProviderConfigurationExtensions
	{
		public static IVauctionDataProvider GetInstance(this IDataProviderConfiguration dataProviderConfiguration)
		{
			string[] typeParts = dataProviderConfiguration.Type.Split(',');

			string typeName = Assembly.CreateQualifiedName(typeParts[1].Trim(), typeParts[0].Trim());

			Type type = Type.GetType(typeName);

			return
				(IVauctionDataProvider)
				Activator.CreateInstance(type,
										 new object[]
                                         {
                                             ConfigurationManager.ConnectionStrings[
                                                 dataProviderConfiguration.ConnectionStringName].ConnectionString
                                         });
		}
	}
}