using System;
using System.Collections.Generic;
using Vauction.Utils;

namespace Vauction.Models
{
    public class AuctionDetail
    {
        public short Lot { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Addendum { get; set; }
        public Consts.AuctionStatus Status { get; set; }
        public string DefaultImage { get; set; }
        public bool IsPulledOut { get; set; }
        public bool IsCurrentEvent { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime EventDateStart { get; set; }
        public DateTime EventDateEnd { get; set; }
        public byte CloseStep { get; set; }
        public decimal Price { get; set; }
        public string Estimate { get; set; }
        public long Owner_ID { get; set; }
        public LinkParams LinkParams { get; set; }
        public LinkParams PrevAuction { get; set; }
        public LinkParams NextAuction { get; set; }
        public IList<IdTitleDesc> Collections { get; set; }
    }
}