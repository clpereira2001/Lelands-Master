﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public interface IConsignment
  {
    Int64 ID { get; set; }
    Int64 User_ID { get; set; }
    Int64 Event_ID { get; set; }
    DateTime ConsDate { get; set; }    
  }
}