using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vauction.Utils.Perfomance
{ 
  public class DataCacheObject
  {
    public DataCacheType Type { get; set; }
    public string Region { get; set; }
    public string Method { get; set; }
    public object Data { get; set; }
    public int CacheTime { get; set; }
    public object[] Params { get; set; }

    public DataCacheObject()
    {
      Type = DataCacheType.RESOURCE;
      Region = "REGION";
      Method = "METHOD";
      Data = null;
      Params = null;
      CacheTime = 0;
    }

    public DataCacheObject(string method) : this()
    {      
      Method = method;
    }

    public DataCacheObject(DataCacheType type, string region, string method)
    {
      Type = type;
      Region = region;
      Method = method;
    }

    public DataCacheObject(DataCacheType type, string region, string method, object[] prms) : this(type, region, method)
    {
      Params = (prms != null) ? prms.ToArray() : null;
    }

    public DataCacheObject(DataCacheType type, string region, string method, object[] prms, int timetolive)
      : this(type, region, method, prms)
    {
      CacheTime = timetolive;
    }

    public DataCacheObject(DataCacheType type, string region, string method, object[] prms, int timetolive, object data)
      : this(type, region, method, prms, timetolive)
    {
      Data = data;
    }

  }
}
