namespace Vauction.Models
{
    public interface ICollection
    {
        long ID { get; set; }
        string Title { get; set; }
        string WebTitle { get; set; }
        string Description { get; set; }
    }
}