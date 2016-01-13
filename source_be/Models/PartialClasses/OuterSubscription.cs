using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Vauction.Configuration;
using Vauction.Utils.Validation;
using System.Runtime.Serialization;

namespace Vauction.Models
{
  [Serializable]
  partial class OuterSubscription : IOuterSubscription
  {
    public string EmailConfirm { get; set; }
  }
}