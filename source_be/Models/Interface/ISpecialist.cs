using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public class ISpecialist
  {
    Int64 ID { get; set; }
    String FirstName { get; set; }
    String LastName { get; set; }
    bool IsActive { get; set; }
    long User_ID { get; set; }
  }
}
