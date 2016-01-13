using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  [Serializable]
  public class IdTitle
  {
    public long ID { get; set; }
    public string Title { get; set; }
  }
}