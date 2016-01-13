using System.Collections.Generic;

namespace Vauction.Models
{
  public interface ICategoryRepository
  {
    string GetCategoriesForCategoriesPage(long eventID, bool onlyLeafs);
    IEnumerable<IdTitle> GetListForCategory(long eventID);
    List<CategoryMenuItem> GetCategoriesForEvent(long eventID, bool ispastevent, out IEnumerable<IdTitleCount> tags);
    string GetCategoriesForCategoriesByMainCategory(long event_id, int maincategory_id);
    EventCategory GetEventCategoryById(long eventcategory_id);
    MainCategory GetMainCategoryById(long maincategory_id);
    EventCategory GetEventCategoryByMainCategory(long maincategory_id, long event_id);
    string GetCategoriesTreeByEvent(long? event_id);
    string GetCategoriesTree(long? event_id);
    string GetCategoriesMenuByEvent(long? eventID, bool onlyLeafs);
    string GetCategoriesMenu(long? event_id, bool onlyLeafs);
    EventCategoryDetail GetEventCategoryDetail(long eventcategory_id);
  }
}
