namespace Vauction.Models
{
  public interface IConsignmentContract
  {
    long ConsignmentID { get; set; }
    int StatusID { get; set; }
    string ContractText { get; set; }
    string FileName { get; set; }
  }
}