using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using Vauction.Utils;
using Vauction.Utils.Helper;
using Vauction.Models.CustomClasses;
using Vauction.Models.CustomClasses.JsTree;
using System.Transactions;

namespace Vauction.Models
{
  public class CategoryRepository : ICategoryRepository
  {
    private VauctionDataContext dataContext;
        
    public CategoryRepository(VauctionDataContext dataContext)
    {
      this.dataContext = dataContext;
    }

    //GetEventCategories
    public object GetEventCategories(string sidx, string sord, int page, int rows, long event_id, int maincat_id, long? cat_id, string title)
    {
      int? totalrecords = 0;
      int pageindex = (page > 0) ? page - 1 : 0;
      dataContext.CommandTimeout = 600000;
      var res = dataContext.spCategory_View_CategoryByMCandE(event_id, maincat_id, cat_id, String.IsNullOrEmpty(title) ? String.Empty : title.Replace(" ", "%"), sidx.ToLower(), sord == "desc", pageindex, rows, ref totalrecords);      
      if (totalrecords.GetValueOrDefault(0) == 0) return new { total = 0, page = page, records = 0 };
      return new
      {
        total = (int)Math.Ceiling((float)totalrecords.GetValueOrDefault(0) / (float)rows),
        page = page,
        records = totalrecords.GetValueOrDefault(0),
        rows = (
            from query in res
            select new
            {
              i = query.Category_ID,
              cell = new string[] {
                query.Category_ID.ToString(),
                query.CategoryTitle                
              }
            }).ToArray()
      };
    }





    // NOT DONE





    //GetCategory
    public Category GetCategory(long cat_id)
    {
      return dataContext.Categories.SingleOrDefault(C => C.ID == cat_id);
    }

    //GetCategory
    public MainCategory GetMainCategory(long maincat_id)
    {
      return dataContext.MainCategories.SingleOrDefault(C => C.ID == maincat_id);
    }

    //GetCategoryMap
    public CategoriesMap GetCategoryMap(long cat_id)
    {
      return dataContext.CategoriesMaps.SingleOrDefault(C => C.ID == cat_id);
    }

    //GetMainCategoryJSON
    public object GetMainCategoryJSON()
    {      
      var jsonData = new
      {
        rows = (
            from E in dataContext.MainCategories
            where E.IsActive
            orderby E.Priority
            select new
            {
              val = E.ID,
              txt = E.Title
            }).ToArray()
      };
      return jsonData;
    }

    //GetCategoryJSON
    public object GetCategoryJSON()
    {
      var jsonData = new
      {
        rows = (
            from E in dataContext.Categories
            where E.IsActive
            orderby E.Title
            select new
            {
              val = E.ID,
              txt = E.Title
            }).ToArray()
      };
      return jsonData;
    }

    //GetCategoryList
    public object GetCategoryList(string sidx, string sord, int page, int rows, bool _search, long? cat_id, string title, string description, bool? isactive)
    {
      if (String.Compare(sidx, "Cat_ID", true) == 0) sidx = "ID";
      List<Category> categories = ((from C in dataContext.Categories                                    
                                    select C).OrderBy(sidx + " "+sord)).ToList();

      if (_search)
      {
        if (categories.Count > 0 && cat_id.HasValue)
          categories = categories.Where(C => C.ID == cat_id.Value).ToList();
        if (categories.Count > 0 && !String.IsNullOrEmpty(title))
          categories = categories.Where(C => C.Title.ToLower().Contains(title.ToLower())).ToList();
        if (categories.Count > 0 && !String.IsNullOrEmpty(description))
          categories = categories.Where(C => C.Description.ToLower().Contains(description.ToLower())).ToList();        
      }

      int pageIndex = Convert.ToInt32(page) - 1;
      int pageSize = rows;
      int totalRecords = categories.Count();
      int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

      categories = categories.Skip(pageIndex * pageSize).Take(pageSize).ToList();

      var jsonData = new
      {
        total = totalPages,
        page = page,
        records = totalRecords,
        rows = (
            from query in categories
            select new
            {
              i = query.ID,
              cell = new string[] {
                query.ID.ToString(),
                query.Title,
                query.Description,
                query.IsActive.ToString()                
              }
            }).ToArray()
      };

      categories = null;

      return jsonData;
    }

    //UpdateCategory
    public JsonExecuteResult UpdateCategory(long cat_id, string title, string description, bool isactive)
    {
      try
      {
        //bool newCategory = false;
        Category cat = (cat_id == -1) ? null : GetCategory(cat_id);
        if (cat == null)
        {
          cat = new Category();
          dataContext.Categories.InsertOnSubmit(cat);
          //newCategory = true;
        }
        cat.Title = title;
        cat.Description = description;        
        cat.IsActive = isactive;
        cat.LastUpdate = DateTime.Now;
        cat.DateEntered = DateTime.Now;
        cat.Priority = 1;
        //if (!newCategory && !cat.IsActive)
        //{
        //  List<CategoriesMap> evCat = dataContext.CategoriesMaps.Where(C => C.Category_ID == cat.ID).ToList();
        //  List<CategoriesMap> lst = new List<CategoriesMap>();
        //  foreach (CategoriesMap ec in evCat)
        //    AddSubCategoriesMap(lst, ec);
        //  foreach (CategoriesMap ec in lst)
        //    ec.IsDefault = false;
        //  List<EventCategory> ev = (from E in dataContext.EventCategories
        //                            where lst.Contains(E.CategoriesMap)
        //                            select E).ToList();
        //  foreach (EventCategory e in ev)
        //    e.IsActive = cat.IsActive;
        //  ev = null;
        //  lst = null;
        //  evCat = null;
        //  if (dataContext.Auctions.Where(A => A.EventCategory.CategoriesMap.Category_ID == cat.ID).Count() > 0)
        //  {
        //    List<string> str = new List<string>();
        //    var res = from A in dataContext.Auctions
        //              where A.EventCategory.CategoriesMap.Category_ID == cat.ID && A.Status_ID != (byte)Consts.AuctionStatus.Locked
        //              orderby A.Status_ID
        //              group A.Status_ID by A.Status_ID;
        //    foreach (var r in res)
        //    {
        //      if (r.Count() > 0)
        //        str.Add(String.Format("{0} {1} auction{2}", r.Count(), ((Consts.AuctionStatus)r.Key).ToString().ToLower(), (r.Count() > 1) ? "s" : String.Empty));
        //    }
        //    if (str.Count() > 0)
        //      return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't set this category as unactive, because there are auctions in the database which are using this category.", str.ToArray());
        //  }
        //}
        GeneralRepository.SubmitChanges(dataContext);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //DeleteCategory
    public JsonExecuteResult DeleteCategory(long cat_id)
    {
      try
      {
        Category cat = (cat_id == -1) ? null : GetCategory(cat_id);
        if (cat == null) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "This category doesn't exists");
        int count = dataContext.Auctions.Where(A => A.EventCategory.Category_ID == cat_id).Count();
        if (count > 0)
          return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't delete this category, because there are auctions in the database which are using this category.");        
        dataContext.Categories.DeleteOnSubmit(cat);
        GeneralRepository.SubmitChanges(dataContext);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    

    //GetCategoriesList
    public object GetCategoriesList(string sidx, string sord, int page, int rows, bool _search, long? cat_id, string title, string description, long? priority, DateTime? dateentered, string owner, DateTime? lastupdate, bool? isactive)
    {
      if (string.Compare(sidx, "Cat_ID", true) == 0) sidx = "ID";
      List<Category> categories = new List<Category>((from C in dataContext.Categories
                                                      select C).OrderBy(sidx + " " + sord));
      if (_search)
      {
        if (categories.Count() > 0 && cat_id.HasValue)
          categories = categories.Where(C => C.ID.ToString().StartsWith(cat_id.Value.ToString())).ToList();
        if (categories.Count() > 0 && !String.IsNullOrEmpty(title))
          categories = categories.Where(C => C.Title.ToLower().Contains(title.ToLower())).ToList();
        if (categories.Count() > 0 && !String.IsNullOrEmpty(description))
          categories = categories.Where(C => C.Description.ToLower().Contains(description.ToLower())).ToList();
        if (categories.Count() > 0 && lastupdate.HasValue)
          categories = categories.Where(C => C.LastUpdate.Date.CompareTo(lastupdate.Value) == 0).ToList();
        if (categories.Count() > 0 && dateentered.HasValue)
          categories = categories.Where(C => C.DateEntered.Date.CompareTo(dateentered.Value) == 0).ToList();
        if (categories.Count() > 0 && priority.HasValue)
          categories = categories.Where(C => C.Priority == priority.Value).ToList();
        if (categories.Count() > 0 && !String.IsNullOrEmpty(owner))
          categories = categories.Where(C => C.User.Login.ToLower().Contains(owner.ToLower())).ToList();
      }
      if (categories.Count() > 0 && isactive.HasValue && isactive.Value)
        categories = categories.Where(C => C.IsActive).ToList();

      int pageIndex = Convert.ToInt32(page) - 1;
      int pageSize = rows;
      int totalRecords = categories.Count();
      int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

      categories = new List<Category>(categories.Skip(pageIndex * pageSize).Take(pageSize));

      var jsonData = new
      {
        total = totalPages,
        page = page,
        records = totalRecords,
        rows = (
            from query in categories
            select new
            {
              i = query.ID,
              cell = new string[] { 
                query.ID.ToString(),               
                query.Title,
                query.Description,
                query.IsActive.ToString(),
                query.Priority.ToString(),
                query.DateEntered.ToString(),
                query.User.Login,
                query.LastUpdate.ToString()
              }
            }).ToArray()
      };

      categories = null;

      return jsonData;
    }

    //UpdateCategory
    public JsonExecuteResult UpdateCategory(long cat_id, string title, string description, bool isactive, int priority)
    {
      try
      {
        bool newCategory = false;
        Category cat = (cat_id == -1) ? null : GetCategory(cat_id);
        if (cat == null)
        {         
          cat = new Category();          
          newCategory = true;
        }
        Category tmp;
        if ((tmp=dataContext.Categories.SingleOrDefault(C => C.Title.ToLower() == title.ToLower() && C.ID != cat.ID)) != null)
          return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't add this category, because the category with the same name is already exist (Category#" + tmp.ID.ToString() + ")");
        cat.Title = title;
        cat.Description = description;
        cat.IsActive = isactive;
        cat.Priority = priority;
        cat.DateEntered = DateTime.Now; 
        cat.Owner_ID = AppHelper.CurrentUser.ID; 
        cat.LastUpdate = DateTime.Now;
        if (newCategory) dataContext.Categories.InsertOnSubmit(cat);

        if (!newCategory && !cat.IsActive)
        {
          List<CategoriesMap> Cat = dataContext.CategoriesMaps.Where(C => C.Category_ID == cat.ID).ToList();
          foreach (CategoriesMap cm in Cat)          
            cm.IsActive = cat.IsActive;
          
          if (dataContext.Auctions.Where(A => A.EventCategory.Category_ID == cat.ID).Count() > 0)
          {
            List<string> str = new List<string>();
            var res = from A in dataContext.Auctions
                      where A.EventCategory.Category_ID == cat.ID && A.Status != (byte)Consts.AuctionStatus.Locked
                      orderby A.Status
                      group A.Status by A.Status;
            foreach (var r in res)
            {
              if (r.Count() > 0)
                str.Add(String.Format("{0} {1} auction{2}", r.Count(), ((Consts.AuctionStatus)r.Key).ToString().ToLower(), (r.Count() > 1) ? "s" : String.Empty));
            }
            if (str.Count() > 0)
              return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't set this category as unactive, because there are auctions in the database which are using this category.", str.ToArray());
          }
        }
        GeneralRepository.SubmitChanges(dataContext);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //GetMainCategoriesList
    public object GetMainCategoriesList(string sidx, string sord, int page, int rows, bool _search, long? maincat_id, string title, string description, long? priority, DateTime? dateentered, string owner_id, DateTime? lastupdate, bool? isactive)
    {
      if (string.Compare(sidx, "MainCat_ID", true) == 0) sidx = "ID";
      List<MainCategory> categories = new List<MainCategory>((from C in dataContext.MainCategories
                                                      select C).OrderBy(sidx + " " + sord));
      if (_search)
      {
        if (categories.Count() > 0 && maincat_id.HasValue)
          categories = categories.Where(C => C.ID.ToString().StartsWith(maincat_id.Value.ToString())).ToList();
        if (categories.Count() > 0 && !String.IsNullOrEmpty(title))
          categories = categories.Where(C => C.Title.ToLower().Contains(title.ToLower())).ToList();
        if (categories.Count() > 0 && !String.IsNullOrEmpty(description))
          categories = categories.Where(C => C.Descr.ToLower().Contains(description.ToLower())).ToList();
        if (categories.Count() > 0 && lastupdate.HasValue)
          categories = categories.Where(C => C.LastUpdate.Date.CompareTo(lastupdate.Value) == 0).ToList();
        if (categories.Count() > 0 && dateentered.HasValue)
          categories = categories.Where(C => C.DateEntered.Date.CompareTo(dateentered.Value) == 0).ToList();
        if (categories.Count() > 0 && priority.HasValue)
          categories = categories.Where(C => C.Priority == priority.Value).ToList();
        if (categories.Count() > 0 && String.IsNullOrEmpty(owner_id))
          categories = categories.Where(C => C.User.Login.ToLower().Contains(owner_id.ToLower())).ToList();
        
      }
      if (categories.Count() > 0 && isactive.HasValue && isactive.Value)
        categories = categories.Where(C => C.IsActive).ToList();

      int pageIndex = Convert.ToInt32(page) - 1;
      int pageSize = rows;
      int totalRecords = categories.Count();
      int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

      categories = new List<MainCategory>(categories.Skip(pageIndex * pageSize).Take(pageSize));

      var jsonData = new
      {
        total = totalPages,
        page = page,
        records = totalRecords,
        rows = (
            from query in categories
            select new
            {
              i = query.ID,
              cell = new string[] { 
                query.ID.ToString(),               
                query.Title,
                query.Descr,
                query.IsActive.ToString(),
                query.Priority.ToString(),
                query.DateEntered.ToString(),
                query.User.Login,
                query.LastUpdate.ToString()
              }
            }).ToArray()
      };

      categories = null;

      return jsonData;
    }

    //UpdateMainCategory
    public JsonExecuteResult UpdateMainCategory(long maincat_id, string title, string description, bool isactive, int priority)
    {
      try
      {
        bool newCategory = false;
        MainCategory cat = (maincat_id == -1) ? null : GetMainCategory(maincat_id);        
        if (cat == null)
        {
          cat = new MainCategory();
          newCategory = true;
        }
        MainCategory tmp;
        if ((tmp = dataContext.MainCategories.SingleOrDefault(C => C.Title.ToLower() == title.ToLower() && C.ID != cat.ID)) != null)
          return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't add this category, because the category with the same name is already exist (MainCategory#" + tmp.ID.ToString() + ")");
        cat.Title = title;
        cat.Descr = description;
        cat.IsActive = isactive;
        cat.Priority = priority;
        cat.DateEntered = DateTime.Now;
        cat.Owner_ID = AppHelper.CurrentUser.ID; 
        cat.LastUpdate = DateTime.Now;
        if (newCategory) dataContext.MainCategories.InsertOnSubmit(cat);

        if (!newCategory && !cat.IsActive)
        {
          List<CategoriesMap> Cat = dataContext.CategoriesMaps.Where(C => C.Category_ID == cat.ID).ToList();
          foreach (CategoriesMap cm in Cat)
            cm.IsActive = cat.IsActive;

          if (dataContext.Auctions.Where(A => A.EventCategory.MainCategory_ID == cat.ID).Count() > 0)
          {
            List<string> str = new List<string>();
            var res = from A in dataContext.Auctions
                      where A.EventCategory.MainCategory_ID == cat.ID && A.Status != (byte)Consts.AuctionStatus.Locked
                      orderby A.Status
                      group A.Status by A.Status;
            foreach (var r in res)
            {
              if (r.Count() > 0)
                str.Add(String.Format("{0} {1} auction{2}", r.Count(), ((Consts.AuctionStatus)r.Key).ToString().ToLower(), (r.Count() > 1) ? "s" : String.Empty));
            }
            if (str.Count() > 0)
              return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't set this category as unactive, because there are auctions in the database which are using this category.", str.ToArray());
          }
        }
        if (newCategory) cat.ID = dataContext.MainCategories.Max(E => E.ID) + 1;
        GeneralRepository.SubmitChanges(dataContext);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //DeleteMainCategory
    public JsonExecuteResult DeleteMainCategory(long maincat_id)
    {
      try
      {
        MainCategory mc = (maincat_id == -1) ? null : GetMainCategory(maincat_id);
        if (mc == null) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "This category doesn't exists");
        int count = dataContext.Auctions.Where(A => A.EventCategory.MainCategory_ID == maincat_id).Count();
        if (count > 0)
          return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't delete this category, because there are auctions in the database which are using this category.");
        dataContext.MainCategories.DeleteOnSubmit(mc);
        GeneralRepository.SubmitChanges(dataContext);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //GetMainCategryForSelect
    public string GetMainCategryForSearchSelect()
    {
      List<MainCategory> mc = (from PT in dataContext.MainCategories
                              orderby PT.ID ascending
                              select PT).ToList<MainCategory>();
      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      sb.Append(":;");
      foreach (MainCategory m in mc)
        sb.AppendFormat("{0}:{1};", m.ID, m.Title);
      sb.Remove(sb.Length - 1, 1);
      return sb.ToString();
    }

    //GetCategryForSelect
    public string GetCategryForSearchSelect()
    {
      List<Category> mc = (from PT in dataContext.Categories
                           orderby PT.ID ascending
                           select PT).ToList<Category>();
      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      sb.Append(":;");
      foreach (Category m in mc)
        sb.AppendFormat("{0}:{1};", m.ID, m.Title.Replace("'", "\""));
      sb.Remove(sb.Length - 1, 1);
      return sb.ToString();
    }

    //GetMainCategoriesList
    public object GetCategoriesMapList(string sidx, string sord, int page, int rows, bool _search, long? catmap_id, long? maincat_id, string maincat, long? cat_id, string cat, string descr, string intro, long? priority, string owner, DateTime? lastupdate)
    {
      if (string.Compare(sidx, "CatMap_ID", true) == 0) sidx = "ID";
      if (string.Compare(sidx, "MainCat", true) == 0) sidx = "MainCategory_ID";
      if (string.Compare(sidx, "Cat", true) == 0) sidx = "Category_ID";

      List<CategoriesMap> categories = new List<CategoriesMap>((from C in dataContext.CategoriesMaps
                                                                select C).OrderBy(sidx + " " + sord));
      if (_search)
      {
        if (categories.Count() > 0 && catmap_id.HasValue)
          categories = categories.Where(C => C.ID.ToString().StartsWith(catmap_id.Value.ToString())).ToList();
        if (categories.Count() > 0 && maincat_id.HasValue)
          categories = categories.Where(C => C.MainCategory_ID.ToString().StartsWith(maincat_id.Value.ToString())).ToList();
        if (categories.Count() > 0 && !String.IsNullOrEmpty(maincat))
          categories = categories.Where(C => C.MainCategory.Title.ToLower().Contains(maincat.ToLower())).ToList();
        if (categories.Count() > 0 && cat_id.HasValue)
          categories = categories.Where(C => C.Category_ID == cat_id.Value).ToList();
        if (categories.Count() > 0 && !String.IsNullOrEmpty(cat))
          categories = categories.Where(C => C.Category.Title.ToLower().Contains(cat.ToLower())).ToList();
        if (categories.Count() > 0 && !String.IsNullOrEmpty(descr))
          categories = categories.Where(C => (String.IsNullOrEmpty(C.Descr) ? String.Empty : C.Descr).ToLower().Contains(descr.ToLower())).ToList();
        if (categories.Count() > 0 && !String.IsNullOrEmpty(intro))
          categories = categories.Where(C => (String.IsNullOrEmpty(C.Intro) ? String.Empty : C.Descr).ToLower().Contains(intro.ToLower())).ToList();
        if (categories.Count() > 0 && lastupdate.HasValue)
          categories = categories.Where(C => C.LastUpdate.Date.CompareTo(lastupdate.Value) == 0).ToList();
        if (categories.Count() > 0 && priority.HasValue)
          categories = categories.Where(C => C.Priority == priority.Value).ToList();
        if (categories.Count() > 0 && !String.IsNullOrEmpty(owner))
          categories = categories.Where(C => C.User.Login.ToLower().Contains(owner.ToLower())).ToList();
      }

      int pageIndex = Convert.ToInt32(page) - 1;
      int pageSize = rows;
      int totalRecords = categories.Count();
      int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

      categories = new List<CategoriesMap>(categories.Skip(pageIndex * pageSize).Take(pageSize));

      var jsonData = new
      {
        total = totalPages,
        page = page,
        records = totalRecords,
        rows = (
            from query in categories
            select new
            {
              i = query.ID,
              cell = new string[] { 
                query.ID.ToString(),               
                query.MainCategory_ID.ToString(),
                query.MainCategory.Title,
                query.Category_ID.ToString(),
                query.Category.Title,
                query.Descr,
                query.IsDefault.ToString(),
                query.Intro,
                query.IsActive.ToString(),
                query.Priority.ToString(),
                query.User.Login,
                query.LastUpdate.ToString()                
              }
            }).ToArray()
      };

      categories = null;

      return jsonData;
    }

    //UpdateCategoryMap
    public JsonExecuteResult UpdateCategoryMap(long catmap_id, int maincat_id, long cat_id, string descr, string intro, int priority, bool isactive, bool isdefault)
    {
      try
      {
        MainCategory mc = GetMainCategory(maincat_id);
        Category c = GetCategory(cat_id);
        if (mc == null || c == null)
          return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The main category or category doesn't exist.");

        bool newCategory = false;
        CategoriesMap cat = (catmap_id == -1) ? null : GetCategoryMap(catmap_id);
        if (cat == null)
        {
          if ((cat = dataContext.CategoriesMaps.Where(CM=>CM.MainCategory_ID==mc.ID&&CM.Category_ID==c.ID).FirstOrDefault()) != null)
            return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't add this record, because the combination of this Main Category and Category already exists - CM# "+cat.ID.ToString());
          cat = new CategoriesMap();
          dataContext.CategoriesMaps.InsertOnSubmit(cat);
          newCategory = true;
        }

        cat.MainCategory_ID = maincat_id;
        cat.Category_ID = cat_id;
        cat.Descr = String.IsNullOrEmpty(descr) ? mc.Title +" > "+c.Title : descr;
        cat.Intro = intro;
        cat.IsActive = isactive;
        cat.Priority = priority;
        cat.Owner_ID = AppHelper.CurrentUser.ID;
        cat.LastUpdate = DateTime.Now;
        cat.IsDefault = isdefault;

        if (!newCategory && !cat.IsActive)
        {
          List<CategoriesMap> Cat = dataContext.CategoriesMaps.Where(C => C.Category_ID == cat.ID).ToList();
          foreach (CategoriesMap cm in Cat)
            cm.IsActive = cat.IsActive;

          if (dataContext.Auctions.Where(A => A.EventCategory.MainCategory_ID == cat.ID).Count() > 0)
          {
            List<string> str = new List<string>();
            var res = from A in dataContext.Auctions
                      where A.EventCategory.MainCategory_ID == cat.ID && A.Status != (byte)Consts.AuctionStatus.Locked
                      orderby A.Status
                      group A.Status by A.Status;
            foreach (var r in res)
            {
              if (r.Count() > 0)
                str.Add(String.Format("{0} {1} auction{2}", r.Count(), ((Consts.AuctionStatus)r.Key).ToString().ToLower(), (r.Count() > 1) ? "s" : String.Empty));
            }
            if (str.Count() > 0)
              return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't set this category as unactive, because there are auctions in the database which are using this category.", str.ToArray());
          }
        }
        if (newCategory) cat.ID = dataContext.MainCategories.Max(E => E.ID) + 1;
        GeneralRepository.SubmitChanges(dataContext);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //DeleteCategoryMap
    public JsonExecuteResult DeleteCategoryMap(long catmap_id)
    {
      try
      {
        CategoriesMap cm = (catmap_id == -1) ? null : GetCategoryMap(catmap_id);
        if (cm == null) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "This category map doesn't exists");
        long count = dataContext.Auctions.Where(EC => EC.EventCategory.MainCategory_ID == cm.MainCategory_ID && EC.EventCategory.Category_ID == cm.Category_ID).Count();
        if (count > 0) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't delete this record because there are auctions which are using this category.");
        dataContext.CategoriesMaps.DeleteOnSubmit(cm);
        GeneralRepository.SubmitChanges(dataContext);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //GetCategoriesMapDefault
    public JsonExecuteResult GetCategoriesMapDefault()
    {
      List<long> result = new List<long>();
      try
      {
        result = dataContext.CategoriesMaps.Where(C1=>C1.IsDefault && C1.IsActive).Select(C => C.ID).ToList();
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, "", result);
    }

    //GetCategoriesMapTree
    public object GetCategoriesMapTree(bool isdefault)
    {
      List<JsTreeNode> nodesList = new List<JsTreeNode>();
      //for (int i = 1; i < 20; i++)
      //{
      //  JsTreeNode node = new JsTreeNode();
      //  node.attributes = new Attributes();
      //  node.attributes.id = "rootnod" + i;
      //  node.attributes.rel = "root" + i;
      //  node.data = new Data();
      //  node.data.title = "Root node:" + i;
      //  node.state = "open";
      //  node.children = new List<JsTreeNode>();
      //  for (int j = 1; j < 4; j++)
      //  {
      //    JsTreeNode cnode = new JsTreeNode();
      //    cnode.attributes = new Attributes();
      //    cnode.attributes.id = "childnod1" + j;
      //    node.attributes.rel = "folder";
      //    cnode.data = new Data();
      //    cnode.data.title = "child node: " + j;
      //    cnode.attributes.mdata = "{ draggable	: false }";
      //    node.children.Add(cnode);
      //  }
      //  node.attributes.mdata = "{ draggable	: false }";
      //  nodesList.Add(node);
      //}

      JsTreeNode node;
      List<MainCategory> map = dataContext.CategoriesMaps.Where(CM => CM.IsActive && CM.MainCategory.IsActive).Select(CM2 => CM2.MainCategory).Distinct().OrderBy(CM4 => CM4.Title).OrderBy(CM3 => CM3.Priority).ToList();
      JsTreeNode cnode;
      foreach (MainCategory cm in map)
      {
        node = new JsTreeNode();
        node.attributes = new Attributes();
        node.attributes.id = "mc"+cm.ID.ToString();
        node.attributes.rel = "node" + cm.ID.ToString();
        node.data = new Data();
        node.data.title = cm.Title;
        ////if (isdefault && cm.IsDefault)
        ////{
        ////  node.data.attributes = new Attributes();
        ////  node.data.attributes.rel = "checked";
        ////}

        List<CategoriesMap> catmap = cm.CategoriesMaps.Where(C => C.IsActive).OrderBy(C1 => C1.Category.Title).ToList();
        if (catmap.Count() > 0)
        {
          node.state = "open";
          node.children = new List<JsTreeNode>();
          foreach (CategoriesMap _cm in catmap)
          {
            cnode = new JsTreeNode();
            cnode.attributes = new Attributes();
            cnode.attributes.id = _cm.ID.ToString();
            cnode.attributes.rel = "node" + _cm.ID.ToString();
            cnode.data = new Data();
            cnode.data.title = _cm.Category.Title;
            node.children.Add(cnode);
          }
        }
      
        nodesList.Add(node);
      }
      return nodesList;
    }

    //AddSubCategoriesMap    
    public void AddSubCategoriesMap(List<CategoriesMap> catmap, CategoriesMap cm)
    {
      catmap.Add(cm);
     // foreach (CategoriesMap _cm in cm.CategoriesMap_Children)
      //  AddSubCategoriesMap(catmap, _cm);
    }

    //MergingCategoriesMap
    public JsonExecuteResult MergingCategoriesMap(long cm_from, long cm_to)
    {
      if (cm_from == cm_to)
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "Please select different source and destination categories.");
      try
      {
        using (TransactionScope ts = new TransactionScope())
        {
          CategoriesMap From = dataContext.CategoriesMaps.SingleOrDefault(CM => CM.ID == cm_from);
          CategoriesMap To = dataContext.CategoriesMaps.SingleOrDefault(CM => CM.ID == cm_to);
          if (From == null || To == null)
            return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, String.Format("The {0} category doesn't exists", (From == null) ? "source" : "destination"));
          List<CategoriesMap> catmaps = new List<CategoriesMap>();
          
          List<Event> events = (from A in dataContext.Auctions
                                where A.EventCategory.CategoriesMap.ID == cm_from
                                select A.Event).Distinct<Event>().ToList();
          if (events.Count() == 0)
            return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The are no auctions in the source category for moving to a destination category");
          EventCategory ec;
          List<Auction> auctions;
          foreach (Event evnt in events)
          {
            ec = evnt.EventCategories.SingleOrDefault(EC => EC.MainCategory_ID == To.MainCategory_ID && EC.Category_ID == To.Category_ID);
            if (ec == null)
            {              
              ec = new EventCategory();
              ec.Category_ID = To.Category_ID;
              ec.MainCategory_ID = To.MainCategory_ID;
              ec.IsActive = true;
              ec.LastUpdate = DateTime.Now;
              ec.Owner_ID = AppHelper.CurrentUser.ID;
              dataContext.EventCategories.InsertOnSubmit(ec);
              evnt.EventCategories.Add(ec);
            }
            auctions = (from A in dataContext.Auctions
                        where A.EventCategory.Category_ID == From.Category_ID && A.EventCategory.MainCategory_ID == From.MainCategory_ID && A.Event_ID == evnt.ID
                        select A).ToList();
            auctions.ForEach(A => A.EventCategory = ec);          }
          GeneralRepository.SubmitChanges(dataContext);
          ts.Complete();
        }
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //GetCategoryMapDescription
    public JsonExecuteResult GetCategoryMapDescription(long catmap_id)
    {
      List<string> str = new List<string>();
      CategoriesMap cm = dataContext.CategoriesMaps.SingleOrDefault(CM => CM.ID == catmap_id);
      List<CategoriesMap> catmaps = new List<CategoriesMap>();

      try
      {
        var res = from A in dataContext.Auctions
                  where A.EventCategory.CategoriesMap.ID == catmap_id
                  orderby A.Status
                  group A.Status by A.Status;
        foreach (var r in res)
        {
          if (r.Count() == 0) continue;
          str.Add(String.Format("{0} {1} auction{2}", r.Count(), ((Consts.AuctionStatus)r.Key).ToString().ToLower(), (r.Count() > 1) ? "s" : String.Empty));
        }
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return (str.Count() > 0) ? new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, "", str.ToArray()) : new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, "There are no auctions in this category.", null);
    }
    
  }
}
