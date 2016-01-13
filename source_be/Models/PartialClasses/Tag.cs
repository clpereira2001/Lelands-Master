using System;

namespace Vauction.Models
{
  [Serializable]
  partial class Tag : ITag
  {
    public void UpdateFields(string title, bool isSystem, bool isViewable)
    {
      Title = title;
      IsSystem = isSystem;
      IsViewable = isViewable;
    }
  }
}