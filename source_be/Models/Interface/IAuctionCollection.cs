namespace Vauction.Models
{
  public interface IAuctionCollection
  {
    long ID { get; set; }
    long AuctionID { get; set; }
    long CollectionID { get; set; }
  }
}