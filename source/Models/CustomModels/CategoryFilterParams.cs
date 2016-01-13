using System;

namespace Vauction.Models
{
  /// <summary>
  ///   filter parametrs for categories
  /// </summary>
  public class CategoryFilterParams : GeneralFilterParams
  {
    public int Id { get; set; }
    public string Name { get; set; }

    public override int GetHashCode()
    {
      return ViewMode.GetHashCode() + ImageViewMode.GetHashCode() + Id.GetHashCode() +
             (!String.IsNullOrEmpty(Name) ? Name.GetHashCode() : 0) + Sortby.GetHashCode() + Orderby.GetHashCode();
    }
  }
}