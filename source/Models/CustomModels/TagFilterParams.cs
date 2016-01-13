using System;

namespace Vauction.Models
{
  /// <summary>
  ///   filter parametrs for tags
  /// </summary>
  public class TagFilterParams : GeneralFilterParams
  {
    public long ID { get; set; }
    public string Name { get; set; }

    public long? EventID { get; set; }

    public override int GetHashCode()
    {
      return ViewMode.GetHashCode() + ImageViewMode.GetHashCode() + ID.GetHashCode() +
             (!String.IsNullOrEmpty(Name) ? Name.GetHashCode() : 0) + Sortby.GetHashCode() + Orderby.GetHashCode();
    }
  }
}