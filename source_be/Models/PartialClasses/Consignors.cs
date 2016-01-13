using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Vauction.Models
{
  [Serializable, DataContract]
  partial class ConsignorsDd : IConsignors
  {
    public long ID { get; set; }
    public string Name { get; set; }
  }
}
