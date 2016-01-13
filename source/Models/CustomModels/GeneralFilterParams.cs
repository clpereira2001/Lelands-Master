using System.ComponentModel;
using Vauction.Utils;

namespace Vauction.Models
{
  public abstract class GeneralFilterParams
  {
    public GeneralFilterParams() { }
    [DefaultValue(Consts.CategorySortFields.Lot)]
    public Consts.CategorySortFields Sortby { get; set; }
    [DefaultValue(Consts.OrderByValues.ascending)]
    public Consts.OrderByValues Orderby { get; set; }
    public int page { get; set; }    
    [DefaultValue(20)]
    public int PageSize { get; set; }
    [DefaultValue((int)Consts.AuctonViewMode.Table)]
    public int ViewMode { get; set; }
    [DefaultValue((int)Consts.AuctionImageViewMode.With)]
    public int ImageViewMode { get; set; }
  }

  public abstract class GeneralFilterParamsEx
  {
    public GeneralFilterParamsEx() { }
    [DefaultValue(Consts.CategorySortFields.Event_ID)]
    public Consts.CategorySortFields Sortby { get; set; }
    [DefaultValue(Consts.OrderByValues.descending)]
    public Consts.OrderByValues Orderby { get; set; }
    public int page { get; set; }
    [DefaultValue(20)]
    public int PageSize { get; set; }
    [DefaultValue((int)Consts.AuctonViewMode.Table)]
    public int ViewMode { get; set; }
    [DefaultValue((int)Consts.AuctionImageViewMode.With)]
    public int ImageViewMode { get; set; }
  }
}
