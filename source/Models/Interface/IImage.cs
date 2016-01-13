using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public interface IImage
  {
    Int64 ID { get; set; }
    Int64 Auction_ID { get; set; }
    string PicturePath { get; set; }
    string ThumbNailPath { get; set; }
    string LargePath { get; set; }
    bool Default { get; set; }
    int Order { get; set; }

    string DefaultString { get; }
    string FullThumbNailPath(Int64 Id, string folder);
    string FullPicturePath(Int64 Id, string folder);
  }
}