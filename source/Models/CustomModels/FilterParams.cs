using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.ComponentModel;
using Vauction.Configuration;
using System.Text;
using System.Collections.Generic;

namespace Vauction.Models.CustomModels
{
  /// <summary>
  /// filter parameter for auction
  /// </summary>
  public class AuctionFilterParams : GeneralFilterParams
  {
    /// <summary>
    /// create simple filter with ktitle and description
    /// </summary>
    /// <param name="keyword"></param>
    public AuctionFilterParams(string keyword)
    {
      Description = keyword;
      Title = keyword;
      int temp = 0;
      if (Int32.TryParse(keyword, out temp))
      {
        ID = temp;
      };

    }
    public AuctionFilterParams() { }

    public override int GetHashCode()
    {
      return ViewMode.GetHashCode() + (ID.HasValue ? ID.Value.GetHashCode() : 0) + ImageViewMode.GetHashCode() + (Event_ID.HasValue ? Event_ID.Value.GetHashCode() : 0) + (!String.IsNullOrEmpty(Lot) ? Lot.GetHashCode() : 0) + ShortFilterString().GetHashCode() + ShortSimpleFilterString().GetHashCode() + (int)Sortby + (int)Orderby; 
    }

    /// <summary>
    /// auction id
    /// </summary>
    public int? ID { get; set; }
    /// <summary>
    /// start price
    /// </summary>
    //public decimal? FromPrice { get; set; }
    /// <summary>
    /// finish price
    /// </summary>
    //public decimal? ToPrice { get; set; }
    /// <summary>
    /// selectded category
    /// </summary>
    public long? SelectedCategory { get; set; }

    //public long? SelectedSubCategory { get; set; }
    /// <summary>
    /// auction shood contains photo or not
    /// </summary>
    //public bool? WithPhoto { get; set; }
    /// <summary>
    /// auction description
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// auction title
    /// </summary>
    public string Title { get; set; }

    public string Lot { get; set; }

    public long? Event_ID { get; set; }

    public string Type { get; set; }

    public byte? SeachType { get; set; }
    /// <summary>
    /// title for selected category(return value for data base)
    /// </summary>
    //public string SelectedCategoryTitle
    //{
    //    get
    //    {

    //        if (string.IsNullOrEmpty(categoryTitle))
    //        {
    //            if (SelectedCategory.HasValue && SelectedCategory.Value > 0)
    //            {
    //                categoryTitle = categoryRepositary.GetCategoryDetails(SelectedCategory.Value).Title;
    //            }
    //        }
    //        return categoryTitle;
    //    }
    //}

    //public string SelectedSubCategoryTitle
    //{
    //    get
    //    {

    //        if (string.IsNullOrEmpty(subcategoryTitle))
    //        {
    //            if (SelectedSubCategory.HasValue && SelectedSubCategory.Value > 0)
    //            {
    //                subcategoryTitle = categoryRepositary.GetCategoryDetails(SelectedSubCategory.Value).Title;
    //            }
    //        }
    //        return subcategoryTitle;
    //    }
    //}

    //public IEnumerable<ICategory> CategoryList
    //{
    //    get
    //    {
    //        if (allCategory == null)
    //        {
    //            IEnumerable<IEventCategory> baseCategory = categoryRepositary.GetListForCurrentCategory();
    //            //List<IEventCategory> dummyCategory = new List<IEventCategory>();
    //            //dummyCategory.Add(new Category() { Title = "All" });
    //            //allCategory = dummyCategory;
    //            //allCategory = allCategory.Union(baseCategory);
    //        }

    //        return allCategory;
    //    }
    //}

    public static System.Web.Mvc.SelectList SearchMethodList
    {
      get
      {
        List<object> list = new List<object>();
        list.Add(new { @ID = "td", @Title = "Title and Description" });
        list.Add(new { @ID = "t", @Title = "Title" });
        list.Add(new { @ID = "d", @Title = "Description" });
        list.Add(new { @ID = "l", @Title = "Lot#" });

        return new System.Web.Mvc.SelectList(list, "ID", "Title");
      }
    }
    //public static System.Web.Mvc.SelectList ShowAtTimeList
    //{
    //  get
    //  {
    //    List<object> list = new List<object>();
    //    list.Add(new { @ID = "12", @Title = "12" });
    //    list.Add(new { @ID = "24", @Title = "24" });
    //    list.Add(new { @ID = "48", @Title = "48" });
    //    list.Add(new { @ID = "96", @Title = "96" });

    //    return new System.Web.Mvc.SelectList(list, "ID", "Title");
    //  }
    //}
    /// <summary>
    /// create string with short description of filter, separated by comma
    /// </summary>
    /// <returns></returns>
    public string ShortFilterString()
    {
      StringBuilder sb = new StringBuilder();
      if (this.ID != null)
        sb.AppendFormat("ID: {0}, ", this.ID);

      if (!string.IsNullOrEmpty(this.Lot))
        sb.AppendFormat("Lot: {0}, ", this.Lot);

      if (!string.IsNullOrEmpty(this.Title))
        sb.AppendFormat("Title: {0}, ", this.Title);

      if (!string.IsNullOrEmpty(this.Description))
        sb.AppendFormat("Description: {0}, ", this.Description);
      
      if (sb.Length>2) sb.Remove(sb.Length - 2, 2);
      return sb.ToString();
    }

    public string ShortSimpleFilterString()
    {
      StringBuilder sb = new StringBuilder();
      //if (!string.IsNullOrEmpty(this.Title))
      //    sb.AppendFormat("Title:{0} ", this.Title);
      //sb.Append(" or ");
      if (!string.IsNullOrEmpty(this.Description))
        sb.AppendFormat("ID:{0} ", this.Description);
      //if (this.ID != null)
      //    sb.AppendFormat("ID:{0}, ", this.ID);
      return sb.ToString();
    }

    public string DataForSearchBox
    {
      get
      {
        if (!String.IsNullOrEmpty(Lot)) return Lot;
        return !String.IsNullOrEmpty(Description) ? Description : Title;
      }
    }

    #region private filds
    //private string categoryTitle;
    //private string subcategoryTitle;
    //private ICategoryRepository categoryRepositary = ((IVauctionConfiguration)ConfigurationManager.GetSection("Vauction")).DataProvider.GetInstance().CategoryRepository;
    //private IEnumerable<ICategory> allCategory;
    #endregion
  }

  public class AuctionFilterParamsEx : GeneralFilterParamsEx
  {
    public AuctionFilterParamsEx(string keyword)
    {
      Description = keyword;
      Title = keyword;
      int temp = 0;
      if (Int32.TryParse(keyword, out temp))
      {
        ID = temp;
      };
    }

    public override int GetHashCode()
    {
      return ViewMode.GetHashCode() + (ID.HasValue ? ID.Value.GetHashCode() : 0) + ImageViewMode.GetHashCode() + (Event_ID.HasValue ? Event_ID.Value.GetHashCode() : 0) + (!String.IsNullOrEmpty(Lot) ? Lot.GetHashCode() : 0) + ShortFilterString().GetHashCode() + ShortSimpleFilterString().GetHashCode() + Sortby.GetHashCode() + Orderby.GetHashCode();
    }

    public AuctionFilterParamsEx() { }

    public int? ID { get; set; }
    
    public long? SelectedCategory { get; set; }

    public string Description { get; set; }
    
    public string Title { get; set; }

    public string Lot { get; set; }

    public long? Event_ID { get; set; }

    public string Type { get; set; }

    public byte? SeachType { get; set; }
   
    public static System.Web.Mvc.SelectList SearchMethodList
    {
      get
      {
        List<object> list = new List<object>();
        list.Add(new { @ID = "td", @Title = "Title and Description" });
        list.Add(new { @ID = "t", @Title = "Title" });
        list.Add(new { @ID = "d", @Title = "Description" });
        list.Add(new { @ID = "l", @Title = "Lot#" });

        return new System.Web.Mvc.SelectList(list, "ID", "Title");
      }
    }
   
    public string ShortFilterString()
    {
      StringBuilder sb = new StringBuilder();
      if (this.ID != null)
        sb.AppendFormat("ID: {0}, ", this.ID);

      if (!string.IsNullOrEmpty(this.Lot))
        sb.AppendFormat("Lot: {0}, ", this.Lot);

      if (!string.IsNullOrEmpty(this.Title))
        sb.AppendFormat("Title: {0}, ", this.Title);

      if (!string.IsNullOrEmpty(this.Description))
        sb.AppendFormat("Description: {0}, ", this.Description);

      if (sb.Length > 2) sb.Remove(sb.Length - 2, 2);
      return sb.ToString();
    }

    public string ShortSimpleFilterString()
    {
      StringBuilder sb = new StringBuilder();     
      if (!string.IsNullOrEmpty(this.Description))
        sb.AppendFormat("ID:{0} ", this.Description);      
      return sb.ToString();
    }
  }
}