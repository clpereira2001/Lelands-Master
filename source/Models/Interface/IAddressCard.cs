using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public interface IAddressCard
  {
    string FirstName { get; set; }
    string LastName { get; set; }
    string MiddleName { get; set; }
    Int64 ID { get; set; }
    Int64 Country_ID { get; set; }
    string Address1 { get; set; }
    string Address2 { get; set; }
    string City { get; set; }
    string State { get; set; }
    string Zip { get; set; }
    string HomePhone { get; set; }
    string WorkPhone { get; set; }
    string Company { get; set; }
    string InternationalState { get; set; }
    Int64? State_ID { get; set; }
  }
}
