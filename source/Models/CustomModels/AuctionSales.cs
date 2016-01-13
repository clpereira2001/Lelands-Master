using System;

namespace Vauction.Models
{
    [Serializable]
    public class AuctionSales
    {
        public short Lot { get; set; }
        public string Title { get; set; }
        public string ThumbnailPath { get; set; }
        public LinkParams LinkParams { get; set; }
        public decimal Price { get; set; }
        public string Estimate { get; set; }
    }
}