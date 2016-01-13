using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vauction.Utils.Perfomance;
using Vauction.Utils.Autorization;
using Vauction.Models;

namespace Vauction.Controllers
{
  [HandleError, Compress, VauctionAuthorize(Roles = "Root")]
  public class CacheController : BaseController
  {
    #region init
    private IGeneralRepository GeneralRepository;
    public CacheController()
    {
      GeneralRepository = dataProvider.GeneralRepository;
    }
    #endregion

    [HttpGet]
    public ActionResult Index()
    { 
      return View();
    }

    [HttpPost]
    public ActionResult GetCacheList(string sidx, string sord, int page, int rows, bool? _firstload, bool _search, string Type, string Region, string Method)
    {
      return (!_firstload.HasValue || _firstload.Value) ? JSON(false) :JSON(GeneralRepository.GetCacheList(sidx, sord, page, rows, _search, Type, Region, Method));
    }

    [HttpPost]
    public JsonResult InitRegions()
    {
      return JSON(GeneralRepository.InitRegions());
    }

    [HttpPost]
    public JsonResult DeleteCache(string id)
    {
      return String.IsNullOrEmpty(id)?JSON(false):JSON(GeneralRepository.DeleteCache(id));
    }

    [HttpPost]
    public JsonResult ClearRegion(string type)
    {
      return JSON(GeneralRepository.ClearRegion(type));
    }

  }
}
