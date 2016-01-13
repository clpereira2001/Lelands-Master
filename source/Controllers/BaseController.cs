using System.Configuration;
using System.Text;
using System.Web.Mvc;
using Vauction.Configuration;
using Vauction.Models;
using Vauction.Utils;
using Vauction.Utils.Lib;
using Vauction.Utils.Perfomance;
using System.Collections.Generic;

namespace Vauction.Controllers
{
  [ValidateInput(false)] /*, NoCache*/ /*OutputCache(Location = OutputCacheLocation.None, NoStore = true, Duration = 0, VaryByParam = "*")*/
  public class BaseController : Controller
  {
    public IVauctionDataProvider dataProvider;

    public IVauctionConfiguration Config { get; protected set; }

    public BaseController()
    {
      Config = (IVauctionConfiguration)ConfigurationManager.GetSection("Vauction");
      dataProvider = Config.DataProvider.GetInstance();
    }

    #region utility virtual methods
    protected virtual void SetFilterParams(GeneralFilterParams param)
    {
      param.ViewMode = BaseHelpers.GetViewMode();
      param.ImageViewMode = BaseHelpers.GetImageViewMode();
      param.PageSize = Consts.PageSize;
      ViewData["filterParam"] = param;
    }

    protected virtual void SetFilterParams(GeneralFilterParamsEx param)
    {
      param.ViewMode = BaseHelpers.GetViewMode();
      param.ImageViewMode = BaseHelpers.GetImageViewMode();
      param.PageSize = Consts.PageSize;
      ViewData["filterParamEx"] = param;
    }
    #endregion

    [NonAction]
    public void InitCurrentEvent()
    {
      ViewData["CurrentEvent"] = dataProvider.EventRepository.GetCurrent();
    }

    public JsonResult JSON(object obj)
    {
      return Json(obj, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    public void ClearAllCache()
    {
      ICacheDataProvider CacheRepository;
      List<string> keys = new List<string>();
      if (Consts.DataCachingTechnology == DataCacheTechnology.MEMORYOBJECT)
      {
        CacheRepository = new CacheDataProvider();
        keys = CacheRepository.Clear();
      }
      else
      {
        CacheRepository = new AppFabricCacheProviderSystemRegions(Consts.ProductName);
        keys.AddRange((CacheRepository as AppFabricCacheProviderSystemRegions).Clear(DataCacheType.REFERENCE.ToString(), new List<string> { "CATEGORIES", "EVENTS", "TAGS" }));
        keys.AddRange((CacheRepository as AppFabricCacheProviderSystemRegions).Clear(DataCacheType.RESOURCE.ToString(), new List<string> { "EVENTS", "AUCTIONS", "IMAGES", "AUCTIONLISTS" }));
        keys.AddRange((CacheRepository as AppFabricCacheProviderSystemRegions).Clear(DataCacheType.ACTIVITY.ToString(), new List<string> { "WATCHLISTS", "EVENTREGISTRATIONS", "BIDS", "INVOICES" }));
      }
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("[ALL CACHE CLEARED: " + System.DateTime.Now + "]");
      keys.ForEach(k => sb.AppendLine(k));
      Logger.LogInfo(sb.ToString());
    }

    [HttpGet]
    public void ClearARP(int id)
    {
      ICacheDataProvider cacheRepository;
      if (Consts.DataCachingTechnology == DataCacheTechnology.MEMORYOBJECT) cacheRepository = new CacheDataProvider();
      else cacheRepository = new AppFabricCacheProviderSystemRegions(Consts.ProductName);

      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTIONDETAILRESULTPAST", new object[] { id });
      cacheRepository.Remove(dco);
      Logger.LogInfo("[Cache removed: " + DataCacheType.RESOURCE + "_" + DataCacheRegions.AUCTIONS + "_GETAUCTIONDETAILRESULTPAST" + "_" + id + "]");
    }

    [HttpGet]
    public void ClearADP(int id)
    {
      dataProvider.AuctionRepository.RemoveAuctionCache(id);
      Logger.LogInfo("[Cache removed for #" + id + "]");
    }
  }
}
