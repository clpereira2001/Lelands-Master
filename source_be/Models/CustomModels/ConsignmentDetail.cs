﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public class ConsignmentDetail
  {
    public long Consignment_ID { get; set; }
    public long Invoice_ID { get; set; }
    public Int64 Auction_ID { get; set; }
    public decimal Cost { get; set; }
    public decimal Amount { get; set; }
    public string CommissionRate { get; set; }
    public LinkParams LinkParams { get; set; }
  }
}