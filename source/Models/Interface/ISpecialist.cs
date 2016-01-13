using System;

namespace Vauction.Models
{
  public interface ISpecialist
    {
      long ID { get; set; }
      string FirstName { get; set; }
      string LastName { get; set; }
      bool IsActive { get; set; }
      long User_ID { get; set; }
    }
}
