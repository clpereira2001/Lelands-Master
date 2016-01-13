using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public interface IHomepageImage
  {
    long ID { get; set; }
    string ImgFileName { get; set; }
    int ImgOrder { get; set; }
    string Link { get; set; }
    string LinkTitle { get; set;}
    bool IsEnabled { get; set; }
    int ImgType { get; set; }
  }
}