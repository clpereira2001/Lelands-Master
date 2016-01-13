using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Models;
using Vauction.Utils;
using System.Data.Linq;
using Vauction.Utils.Perfomance;

namespace Vauction.Models
{
  public class LinqToSqlDataProvider : IVauctionDataProvider
  {
    private string connectionString;
    private VauctionDataContext dataContext;
    private ICacheDataProvider cacheDataProvider;
    //private IUserRepository userRepository;
    //private IAuctionRepository auctionRepository;
    //private IEventRepository eventRepository;
    //private IBidRepository bidRepository;
    //private IInvoiceRepository invoiceRepository;
    //private IGeneralRepository generalRepository;
    //private ICategoryRepository categoryRepository;
    //private IReportRepository reportRepository;

    public LinqToSqlDataProvider()
    {
      dataContext = new VauctionDataContext();
      if (Consts.DataCachingTechnology == DataCacheTechnology.MEMORYOBJECT) cacheDataProvider = new CacheDataProvider();
      else cacheDataProvider = new AppFabricCacheProviderSystemRegions(Consts.ProductName);
    }

    public LinqToSqlDataProvider(string connectionString) : this()
    {
      this.connectionString = connectionString;
      dataContext = new VauctionDataContext(connectionString);
    }

    private VauctionDataContext DataContext
    {
      get { return dataContext ?? new VauctionDataContext(connectionString); }
    }

    public virtual IUserRepository UserRepository
    {
      get { return new UserRepository(DataContext); }
    }

    public virtual IAuctionRepository AuctionRepository
    {
      get { return new AuctionRepository(DataContext, cacheDataProvider); }
    }

    public virtual IEventRepository EventRepository
    {
      get { return new EventRepository(DataContext); }
    }
    public virtual IBidRepository BidRepository
    {
      get { return new BidRepository(DataContext, cacheDataProvider); }
    }
    public IInvoiceRepository InvoiceRepository
    {
      get { return new InvoiceRepository(DataContext); }
    }
    public IGeneralRepository GeneralRepository
    {
      get { return new GeneralRepository(DataContext, cacheDataProvider); }
    }
    public ICategoryRepository CategoryRepository
    {
      get { return new CategoryRepository(DataContext); }
    }

    public IReportRepository ReportRepository
    {
      get { return new ReportRepository(DataContext); }
    }
  }
}