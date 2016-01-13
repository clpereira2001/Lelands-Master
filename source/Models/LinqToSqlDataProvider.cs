using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Models;
using Vauction.Utils;
using Vauction.Utils.Perfomance;

namespace Vauction.Models
{
  public class LinqToSqlDataProvider : IVauctionDataProvider
  {
    private string connectionString;
    private VauctionDataContext dataContext;

    private ICacheDataProvider CacheDataProvider;

    public LinqToSqlDataProvider()
    {
      dataContext = new VauctionDataContext();
      CacheDataProvider = AppHelper.CacheDataProvider;

      //if (Consts.DataCachingTechnology == DataCacheTechnology.MEMORYOBJECT) CacheDataProvider = new CacheDataProvider();
      //else
      //{
      //  try
      //  {
      //    CacheDataProvider = new AFCDataProvider(Consts.ProductName);
      //  } catch
      //  {
      //    CacheDataProvider = new CacheDataProvider();
      //  }
      //}
    }

    public LinqToSqlDataProvider(string connectionString) :this()
    {
      this.connectionString = connectionString;
      dataContext = new VauctionDataContext(connectionString);
    }

    public VauctionDataContext DataContext
    {
      get { return dataContext ?? new VauctionDataContext(connectionString); }
    }

    public virtual IUserRepository UserRepository
    {
      get { return new UserRepository(DataContext, CacheDataProvider); }
    }
    public virtual IDifferentRepository DifferentRepository
    {
      get { return new DifferentRepository(DataContext, CacheDataProvider); }
    }
    public virtual IBidRepository BidRepository
    {
      get { return new BidRepository(DataContext, CacheDataProvider); }
    }
    public virtual IAuctionRepository AuctionRepository
    {
      get { return new AuctionRepository(DataContext, CacheDataProvider); }
    }
    public virtual IEventRepository EventRepository
    {
      get { return new EventRepository(DataContext, CacheDataProvider); }
    }
    public virtual ICategoryRepository CategoryRepository
    {
      get { return new CategoryRepository(DataContext, CacheDataProvider); }
    }
    public virtual IInvoiceRepository InvoiceRepository
    {
      get { return new InvoiceRepository(DataContext, CacheDataProvider); }
    }
    public virtual ICacheDataProvider CacheRepository
    {
      get { return CacheDataProvider; }
    }
  }
}