using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  [Serializable]
  public class Address
  {
    public string Address_1 { get; set; }
    public string Address_2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string Zip { get; set; }
    public long? State_ID { get; set; }
    public long Country_ID { get; set; }
    public string HomePhone { get; set; }
    public string WorkPhone { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }

    public string GetAddressText(string separator)
    {
      return String.Format("Addr1: {0}{6}Addr2: {1}{6}City: {2}{6}State: {3}{6}Country: {4}{6}Zip: {5}", Address_1, Address_2, City, State, (Country == null) ? String.Empty : Country, Zip, separator);
    }
    public string GetAddressText2(string separator)
    {
      return String.Format("{0} {1}{6} {2}{6} {3}{6} {4}{6} {5}", Address_1, Address_2, City, State, (Country == null) ? String.Empty : Country, Zip, separator);
    }

    public string FullAddress
    {
      get { return GetAddressText("|"); }
    }
    public string FullAddress2
    {
      get { return GetAddressText2("|"); }
    }

    public string FullName
    {
      get { return String.Format("{0} {1}{2}", FirstName, String.IsNullOrEmpty(MiddleName) ? "" : MiddleName + " ", LastName); }
    }

    public static Address GetAddress(AddressCard ac)
    {
      Address addr = new Address();
      addr.Address_1 = ac.Address1;
      addr.Address_2 = ac.Address2;
      addr.City = ac.City;
      addr.State = ac.State;
      addr.State_ID = ac.State_ID;
      addr.Country = (ac.Country != null) ? ac.Country.Title : String.Empty;
      addr.Country_ID = ac.Country_ID;
      addr.Zip = ac.Zip;
      addr.FirstName = ac.FirstName;
      addr.LastName = ac.LastName;
      addr.MiddleName = ac.MiddleName;
      addr.HomePhone = ac.HomePhone;
      addr.WorkPhone = ac.WorkPhone;      
      return addr;
    }
  }
}