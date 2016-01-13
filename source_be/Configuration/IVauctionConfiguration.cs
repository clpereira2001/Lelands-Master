namespace Vauction.Configuration
{
	public interface IVauctionConfiguration
	{
		IDataProviderConfiguration DataProvider { get; }
		IPropertyConfigurationCollection Property { get; }
	}
}
