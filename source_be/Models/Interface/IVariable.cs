using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
    public interface IVariable
    {
    	Int64 ID { get; set; }        
	    string Name { get; set; }
	    decimal Value { get; set; }
	    

        
    }
}
