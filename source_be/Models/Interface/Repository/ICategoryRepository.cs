using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils;
using Vauction.Models.CustomClasses;

namespace Vauction.Models
{
  public interface ICategoryRepository
  {
    object GetEventCategories(string sidx, string sord, int page, int rows, long event_id, int maincat_id, long? cat_id, string title);
    
    
    
    
    
    // NOT DONE



    Category GetCategory(long cat_id);
    MainCategory GetMainCategory(long maincat_id);
    CategoriesMap GetCategoryMap(long cat_id);
    object GetMainCategoryJSON();
    object GetCategoryJSON();
    object GetCategoryList(string sidx, string sord, int page, int rows, bool _search, long? cat_id, string title, string description, bool? isactive);
    JsonExecuteResult UpdateCategory(long cat_id, string title, string description, bool isactive);    
    JsonExecuteResult DeleteCategory(long cat_id);    
    object GetCategoriesList(string sidx, string sord, int page, int rows, bool _search, long? cat_id, string title, string description, long? priority, DateTime? dateentered, string owner, DateTime? lastupdate, bool? isactive);
    JsonExecuteResult UpdateCategory(long cat_id, string title, string description, bool isactive, int priority);
    object GetMainCategoriesList(string sidx, string sord, int page, int rows, bool _search, long? maincat_id, string title, string description, long? priority, DateTime? dateentered, string owner_id, DateTime? lastupdate, bool? IsActive);
    JsonExecuteResult UpdateMainCategory(long maincat_id, string title, string description, bool isactive, int priority);
    JsonExecuteResult DeleteMainCategory(long maincat_id);
    string GetMainCategryForSearchSelect();
    string GetCategryForSearchSelect();
    object GetCategoriesMapList(string sidx, string sord, int page, int rows, bool _search, long? catmap_id, long? maincat_id, string maincat, long? cat_id, string cat, string descr, string intro, long? priority, string owner, DateTime? lastupdate);
    JsonExecuteResult UpdateCategoryMap(long catmap_id, int maincat_id, long cat_id, string descr, string intro, int priority, bool isactive, bool isdefault);
    JsonExecuteResult DeleteCategoryMap(long catmap_id);
    JsonExecuteResult GetCategoriesMapDefault();
    object GetCategoriesMapTree(bool isdefault);
    void AddSubCategoriesMap(List<CategoriesMap> catmap, CategoriesMap cm);
    JsonExecuteResult MergingCategoriesMap(long cm_from, long cm_to);
    JsonExecuteResult GetCategoryMapDescription(long catmap_id);    
  }
}
