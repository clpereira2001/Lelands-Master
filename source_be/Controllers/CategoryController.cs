using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Vauction.Models;
using Vauction.Models.CustomClasses;
using Vauction.Utils;
using Vauction.Utils.Autorization;
using Vauction.Utils.Perfomance;

namespace Vauction.Controllers
{
  [Compress]
  public class CategoryController : BaseController
  {
    #region init
    private ICategoryRepository CategoryRepository;
    public CategoryController()
    {
      CategoryRepository = dataProvider.CategoryRepository;
    }
    #endregion

    //GetEventCategories
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet, ActionOutputCache(CachingExpirationTime.Minutes_30)]
    public JsonResult GetEventCategories(string sidx, string sord, int page, int rows, long? event_id, bool _search, long? Cat_ID, string Title, int? maincat_id)
    {
      return (!event_id.HasValue || !maincat_id.HasValue) ? JSON(false) : JSON(CategoryRepository.GetEventCategories(sidx, sord, page, rows, event_id.Value, maincat_id.Value, Cat_ID, Title));
    }

    #region json
    //GetMainCategoryJSON
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
    public JsonResult GetMainCategoryJSON()
    {
      return JSON(CategoryRepository.GetMainCategoryJSON());
    }

    #endregion

    //NOT DONE


    
    //GetCategoryJSON
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
    public JsonResult GetCategoryJSON()
    {
      return JSON(CategoryRepository.GetCategoryJSON());
    }

    //GetCategoryList
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
    public JsonResult GetCategoryList(string sidx, string sord, int page, int rows, bool _search, long? Cat_Id, string Title, string Description, bool? IsActive)
    {
      return JSON(CategoryRepository.GetCategoryList(sidx, sord, page, rows, _search, Cat_Id, Title, Description, IsActive));
    }

    //UpdateCategoryForm
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
    public JsonResult UpdateCategoryForm(string id, string oper, long? Cat_ID, string Title, string Description, bool? IsActive)
    {
      long ID = Cat_ID ?? -1;
      object result;
      if (oper != "del")
      {
        result = CategoryRepository.UpdateCategory(ID, Title, Description, (IsActive.HasValue)?IsActive.Value:false);
      }
      else
      {
        result = (!(!String.IsNullOrEmpty(id) && !Int64.TryParse(id, out ID))) ? CategoryRepository.DeleteCategory(ID) : new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "It is impossible to delete this category.");
      }
      return JSON(result);     
    }

    //Categories
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
    public ActionResult Categories()
    {
      if (AppHelper.CurrentUser.ID !=21254) return RedirectToAction("Index", "Home");
      return View();
    }

    //GetCategoriesList
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
    public JsonResult GetCategoriesList(string sidx, string sord, int page, int rows, bool? _firstload, bool _search, long? Cat_ID, string Title, string Description, long? Priority, DateTime? DateEntered, string Owner_ID, DateTime? LastUpdate, bool? IsActive)
    {
      return JSON(CategoryRepository.GetCategoriesList(sidx, sord, page, rows, _search, Cat_ID, Title, Description, Priority, DateEntered, Owner_ID,  LastUpdate, IsActive));
    }

    //UpdateCategory
    [AcceptVerbs(HttpVerbs.Post)]
    public JsonResult UpdateCategory(string Title, string Description, bool? IsActive, int? Priority, DateTime? DateEntered, long? Owner_ID, string oper, string id)
    {
      long ID = String.Compare(oper, "add") == 0 ? -1 : Convert.ToInt64(id);
      JsonExecuteResult result;
      if (oper != "del")
      {
        result = CategoryRepository.UpdateCategory(ID, Title, Description, (IsActive.HasValue) ? IsActive.Value : false, Priority.Value);
      }
      else
      {
        result = (!(!String.IsNullOrEmpty(id) && !Int64.TryParse(id, out ID))) ? CategoryRepository.DeleteCategory(ID) : new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "It is impossible to delete this category.");
      }
      return JSON(result);
    }

    //GetMainCategoriesList
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
    public JsonResult GetMainCategoriesList(string sidx, string sord, int page, int rows, bool? _firstload, bool _search, long? MainCat_ID, string Name, string Descr, long? Priority, DateTime? DateEntered, string Owner_ID, DateTime? LastUpdate, bool? IsActive)
    {
      return JSON(CategoryRepository.GetMainCategoriesList(sidx, sord, page, rows, _search, MainCat_ID, Name, Descr, Priority, DateEntered, Owner_ID, LastUpdate, IsActive));
    }

    //UpdateMainCategory
    [VauctionAuthorize(Roles = "Root,Admin")]
    [AcceptVerbs(HttpVerbs.Post)]
    public JsonResult UpdateMainCategory(long? MainCat_ID, string Name, string Descr, bool? IsActive, int? Priority, string oper, string id)
    {
      long ID = String.Compare(oper, "add") == 0 ? -1 : Convert.ToInt64(id);
      JsonExecuteResult result;
      if (oper != "del")
        result = CategoryRepository.UpdateMainCategory(ID, Name, Descr, (IsActive.HasValue) ? IsActive.Value : false, Priority.Value);
      else
        result = (!(!String.IsNullOrEmpty(id) && !Int64.TryParse(id, out ID))) ? CategoryRepository.DeleteMainCategory(ID) : new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "It is impossible to delete this category.");
      return JSON(result);
    }

    //GetCategoriesMapList
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
    public JsonResult GetCategoriesMapList(string sidx, string sord, int page, int rows, bool _search, long? CatMap_ID, long? MainCategory_ID, string MainCat, long? Category_ID, string Cat, string Descr, string Intro, long? Priority, string Owner_ID, DateTime? LastUpdate)
    {
      return JSON(CategoryRepository.GetCategoriesMapList(sidx, sord, page, rows, _search, CatMap_ID, MainCategory_ID, MainCat, Category_ID, Cat, Descr, Intro, Priority, Owner_ID, LastUpdate));
    }

    //UpdateCategoiesMap
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult UpdateCategoiesMap(int? MainCategory_ID, long? Category_ID, string Descr, string Intro, int? Priority,bool? IsActive, bool? IsDefault, string oper, string id)
    {
      long ID = String.Compare(oper, "add") == 0 ? -1 : Convert.ToInt64(id);
      JsonExecuteResult result;
      if (oper != "del")
      {
        result = CategoryRepository.UpdateCategoryMap(ID, MainCategory_ID.Value, Category_ID.Value, Descr, Intro, Priority.Value, (IsActive.HasValue) ? IsActive.Value : false, (IsDefault.HasValue) ? IsDefault.Value : false);
      }
      else
      {
        result = (!(!String.IsNullOrEmpty(id) && !Int64.TryParse(id, out ID))) ? CategoryRepository.DeleteCategoryMap(ID) : new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "It is impossible to delete this category map.");
      }
      return JSON(result);
    }

    //GetCategoriesMapTreeJson
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult GetCategoriesMapTreeJson()
    {
      return JSON(CategoryRepository.GetCategoriesMapTree(false));
    }

    //MergingCategoriesMap
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult MergingCategoriesMap(long? cm_from, long? cm_to)
    {
      return (cm_from.HasValue && cm_to.HasValue) ? JSON(CategoryRepository.MergingCategoriesMap(cm_from.Value, cm_to.Value)) : JSON(false);
    }

    //GetCategoryMapDescription
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult GetCategoryMapDescription(long? catmap_id)
    {
      return (catmap_id.HasValue) ? JSON(CategoryRepository.GetCategoryMapDescription(catmap_id.Value)) : JSON(false);
    }
  }
}
