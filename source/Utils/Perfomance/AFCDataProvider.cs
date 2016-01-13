using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ApplicationServer.Caching;
using Vauction.Utils.Lib;

namespace Vauction.Utils.Perfomance
{
  public class AFCDataProvider : ICacheDataProvider
  {
    DataCacheFactory dcf;
    string ProductName;

    public AFCDataProvider()
    {
      dcf = new DataCacheFactory();
      ProductName = "Default";
    }

    public AFCDataProvider(string ProductName)
      : this()
    {
      this.ProductName = ProductName;
    }

    private DataCache GetDataCache(string cacheName)
    {
      if (dcf == null) dcf = new DataCacheFactory();
      return dcf.GetCache((ProductName + "_" + cacheName).ToUpper()) ?? dcf.GetDefaultCache();
    }

    public void InitDataCacheRegions(Dictionary<string, List<string>> regionsInCache)
    {
      DataCache dc;
      foreach (KeyValuePair<string, List<string>> kvp in regionsInCache)
      {
        dc = GetDataCache(kvp.Key);
        foreach (string region in kvp.Value)
          dc.CreateRegion(region);
      }
    }

    private string GetKeyByParams(object[] param)
    {
      if (param == null || param.Length == 0) return String.Empty;
      StringBuilder sb = new StringBuilder();
      foreach (object obj in param)
      {
        sb.Append("_");
        sb.Append(obj != null ? obj.GetHashCode().ToString() : "nullparam");
      }
      return sb.ToString();
    }

    private string GetKey(DataCacheObject obj)
    {
      return String.Format("{0}{1}", obj.Method, GetKeyByParams(obj.Params));
    }

    public object Get(DataCacheObject obj)
    {
      try
      {
        DataCache dc = GetDataCache(obj.Type.ToString());
        return dc.Get(GetKey(obj), obj.Region);
      }
      catch (DataCacheException ex)
      {
        Logger.LogException(obj.Method, ex);
      }
      return null;
    }

    public void Add(DataCacheObject obj)
    {
      try
      {
        if (Contains(obj)) Remove(obj);
        DataCache dc = GetDataCache(obj.Type.ToString());
        dc.Put(GetKey(obj), obj.Data, TimeSpan.FromSeconds(obj.CacheTime), obj.Region);
      }
      catch (DataCacheException ex)
      {
        Logger.LogException(obj.Method, ex);
      }
    }

    public void Update(DataCacheObject obj)
    {
      DataCacheItem dci = new DataCacheItem();
      try
      {
        DataCache dc = GetDataCache(obj.Type.ToString());
        if (!Contains(obj)) return;
        dci = dc.GetCacheItem(GetKey(obj), obj.Region);
        dc.Put(GetKey(obj), obj.Data, dci.Version, dci.Timeout, obj.Region);
      }
      catch (DataCacheException ex)
      {
        Logger.LogException(obj.Method + " ~ " + String.Format("cached_value:{0}; new_value:{1}", dci.Value, obj.Data), ex);
      }
    }

    public void Put(DataCacheObject obj)
    {
      DataCacheItem dci = new DataCacheItem();
      try
      {
        DataCache dc = GetDataCache(obj.Type.ToString());
        dc.Put(GetKey(obj), obj.Data, TimeSpan.FromSeconds(obj.CacheTime), obj.Region);
      }
      catch (DataCacheException ex)
      {
        Logger.LogException(obj.Method + " ~ " + String.Format("cached_value:{0}; new_value:{1}", dci.Value, obj.Data), ex);
      }
    }

    public bool Contains(DataCacheObject obj)
    {
      return Get(obj) != null;
    }

    public void Remove(DataCacheObject obj)
    {
      try
      {
        if (!Contains(obj)) return;
        DataCache dc = GetDataCache(obj.Type.ToString());
        dc.Remove(GetKey(obj), obj.Region);
      }
      catch (DataCacheException ex)
      {
        Logger.LogException(obj.Method, ex);
      }
    }

    public List<string> Clear(string Type, string Region)
    {
      try
      {
        List<string> removedItems = new List<string>();
        DataCache dc = GetDataCache(Type);
        foreach (var item in dc.GetObjectsInRegion(Region))
        {
          dc.Remove(item.Key, Region);
          removedItems.Add(item.Key);
        }
        return removedItems;
      }
      catch (Exception ex)
      {
        Logger.LogException(Type + "-" + Region, ex);
      }
      return null;
    }

    public List<string> Clear(string Type, IEnumerable<string> Regions)
    {
      List<string> removedItems = new List<string>();
      foreach (string s in Regions)
      {
        removedItems.AddRange(Clear(Type, s));
      }
      return removedItems;
    }

    public List<string> Clear()
    {
      throw new NotImplementedException();
    }

    private IEnumerable<DataCacheObject> GetAllCache(DataCacheType dct)
    {
      try
      {
        List<DataCacheObject> result = new List<DataCacheObject>();
        DataCache dc = GetDataCache(dct.ToString());
        DataCacheItem dci;
        foreach (string region in dc.GetSystemRegions())
        {
          foreach (var obj in dc.GetObjectsInRegion(region))
          {
            dci = dc.GetCacheItem(obj.Key, region);
            result.Add(new DataCacheObject(dct, region, obj.Key, null, (int)dci.Timeout.TotalSeconds, dci.Value));
          }
        }
        return result;
      }
      catch (Exception ex)
      {
        Logger.LogException(dct.ToString(), ex);
      }
      return null;
    }

    public List<DataCacheObject> AllCache()
    {
      try
      {
        List<DataCacheObject> result = new List<DataCacheObject>();
        result.AddRange(GetAllCache(DataCacheType.REFERENCE));
        result.AddRange(GetAllCache(DataCacheType.ACTIVITY));
        result.AddRange(GetAllCache(DataCacheType.RESOURCE));
        return result;
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
      }
      return null;
    }

    private IEnumerable<DataCacheObject> GetAllCache(string cache, IEnumerable<string> regions)
    {
      try
      {
        List<DataCacheObject> result = new List<DataCacheObject>();
        DataCache dc = GetDataCache(cache);
        DataCacheItem dci;
        foreach (string region in regions)
        {
          foreach (var obj in dc.GetObjectsInRegion(region))
          {
            dci = dc.GetCacheItem(obj.Key, region);
            DataCacheType dct;
            switch (cache)
            {
              case "RESOURCE": dct = DataCacheType.RESOURCE; break;
              case "ACTIVITY": dct = DataCacheType.ACTIVITY; break;
              default: dct = DataCacheType.REFERENCE; break;
            }
            result.Add(new DataCacheObject(dct, region, obj.Key, null, (int)dci.Timeout.TotalSeconds, dci.Value));
          }
        }
        return result;
      }
      catch (Exception ex)
      {
        Logger.LogException(cache, ex);
      }
      return null;
    }

    public List<DataCacheObject> AllCache(Dictionary<string, List<string>> regions)
    {
      try
      {
        List<DataCacheObject> result = new List<DataCacheObject>();
        foreach (var kvp in regions)
          result.AddRange(GetAllCache(kvp.Key, kvp.Value));
        return result;
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
      }
      return null;
    }


  }
}
