using System.Configuration;
using Vauction.Configuration;
using Vauction.Models;

namespace Vauction.Configuration
{
    public static class ProjectConfig
	{
        public static IVauctionConfiguration Config
		{
			get
			{                
                return (IVauctionConfiguration)ConfigurationManager.GetSection("Vauction");
			}
		}

        public static IVauctionDataProvider DataProvider
        {
            get
            {
                return Config.DataProvider.GetInstance();
            }
        }
	}
}