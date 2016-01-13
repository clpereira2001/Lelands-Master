using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public interface IHotNew
  {
    long ID { get; set; }
    string Title { get; set; }
    DateTime NewsDate { get; set; }
    string Description { get; set; }
    bool IsOnlyHP { get; set; }
    bool IsActive { get; set; }
    int Order { get; set; }
    string ImagePath { get; set; }
  }
}