using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ApplicationServer.Caching;
using Vauction.Utils.Lib;

namespace Vauction.Utils.Perfomance
{
  public class AppFabricCacheProviderSystemRegions : ICacheDataProvider
  {
    DataCacheFactory dcf;
    string ProductName;

    #region constructor
    public AppFabricCacheProviderSystemRegions()
    {
      dcf = new DataCacheFactory();
      ProductName = "DEFAULT";
    }

    public AppFabricCacheProviderSystemRegions(string productName)
      : this()
    {
      ProductName = productName;
    }
    #endregion

    #region get key, cachename
    //GetDataCache
    private DataCache GetDataCache(string cacheName)
    {
      if (dcf == null) dcf = new DataCacheFactory();
      return dcf.GetCache((ProductName + "_" + cacheName).ToUpper()) ?? dcf.GetDefaultCache();
    }

    //GetKeyByParams
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
    
    //GetKey
    private string GetKey(DataCacheObject obj)
    {
      return String.Format("{0}_{1}_{2}{3}", obj.Type, obj.Region, obj.Method, GetKeyByParams(obj.Params));
    }
    #endregion 

    //Get
    public object Get(DataCacheObject obj)
    {
      try
      {
        DataCache dc = GetDataCache(obj.Type.ToString());
        return dc.Get(GetKey(obj));
      } catch(Exception ex)
      {
        Logger.LogException("APPFABRICCACHEPROVIDERSYSTEMREGIONS_GET", ex);
        return null;
      }
    }

    //Add
    public void Add(DataCacheObject obj)
    {
      try
      {
        if (Contains(obj)) Remove(obj);
        DataCache dc = GetDataCache(obj.Type.ToString());
        dc.Put(GetKey(obj), obj.Data, TimeSpan.FromSeconds(obj.CacheTime));
      }
      catch (Exception ex)
      {
        Logger.LogException("APPFABRICCACHEPROVIDERSYSTEMREGIONS_ADD", ex);
      }
    }

    //Update
    public void Update(DataCacheObject obj)
    {
      try{
      DataCache dc = GetDataCache(obj.Type.ToString());
      if (!Contains(obj)) return;
      DataCacheItem dci = dc.GetCacheItem(GetKey(obj));
      dc.Put(GetKey(obj), obj.Data, dci.Version, dci.Timeout);
      }
      catch (Exception ex)
      {
        Logger.LogException("APPFABRICCACHEPROVIDERSYSTEMREGIONS_UPDATE", ex);
      }
    }

    //Put
    public void Put(DataCacheObject obj)
    {
      try{
      DataCache dc = GetDataCache(obj.Type.ToString());
      dc.Put(GetKey(obj), obj.Data, TimeSpan.FromSeconds(obj.CacheTime));
      }
      catch (Exception ex)
      {
        Logger.LogException("APPFABRICCACHEPROVIDERSYSTEMREGIONS_PUT", ex);
      }
    }

    //Contains
    public bool Contains(DataCacheObject obj)
    {
      return Get(obj) != null;
    }

    //Remove
    public void Remove(DataCacheObject obj)
    {
      try{
      if (!Contains(obj)) return;
      DataCache dc = GetDataCache(obj.Type.ToString());
      dc.Remove(GetKey(obj));
      }
      catch (Exception ex)
      {
        Logger.LogException("APPFABRICCACHEPROVIDERSYSTEMREGIONS_REMOVE", ex);
      }
    }

    public List<string> Clear(string Type, string Region)
    {
      try
      {
        List<string> removedItems = new List<string>();
        DataCache dc = GetDataCache(Type);
        foreach (string regionName in dc.GetSystemRegions())
          dc.GetObjectsInRegion(regionName).Where(
            o => o.Key.StartsWith(String.Format("{0}_{1}_", Type, Region))).ToList().ForEach(
              obj => dc.Remove(obj.Key));
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
        removedItems.AddRange(Clear(Type, s));
      return removedItems;
    }

    //Clear
    public void Clear(DataCacheType type)
    {
      DataCache dc = GetDataCache(type.ToString());
      foreach (string regionName in dc.GetSystemRegions())
        dc.ClearRegion(regionName);
    }

    //Clear
    public void Clear(DataCacheType type, out List<string> keys)
    {
      keys = new List<string>();
      DataCache dc = GetDataCache(type.ToString());
      foreach (string regionName in dc.GetSystemRegions())
      {
        keys.AddRange(dc.GetObjectsInRegion(regionName).ToList().Select(a => a.Key));
        dc.ClearRegion(regionName);
      }
    }
    
    //Clear
    public void Clear(out List<string> keys)
    {
      keys = new List<string>();
      foreach (DataCacheType type in Enum.GetValues(typeof(DataCacheType)))
      {
        List<string> subKeys;
        Clear(type, out subKeys);
        keys.AddRange(subKeys);
      }
    }

    //Clear
    public List<string> Clear()
    {
      List<string> result = new List<string>();
      foreach (DataCacheObject dataCacheObject in AllCache())
      {
        result.Add(GetKey(dataCacheObject));
        Remove(dataCacheObject);
      }
      return result;
    }

    private IEnumerable<DataCacheObject> GetAllCache(DataCacheType dct)
    {
      List<DataCacheObject> result = new List<DataCacheObject>();
      DataCache dc = GetDataCache(dct.ToString());
      DataCacheItem dci;
      foreach (string region in dc.GetSystemRegions())
        foreach (var obj in dc.GetObjectsInRegion(region))
        {
          dci = dc.GetCacheItem(obj.Key);
          result.Add(new DataCacheObject(dct, region, obj.Key, null, (int)dci.Timeout.TotalSeconds, dci.Value));
        }
      return result;
    }

    //AllCache
    public List<DataCacheObject> AllCache()
    {
      List<DataCacheObject> result = new List<DataCacheObject>();
      result.AddRange(GetAllCache(DataCacheType.REFERENCE));
      result.AddRange(GetAllCache(DataCacheType.ACTIVITY));
      result.AddRange(GetAllCache(DataCacheType.RESOURCE));
      return result;
    }
  }
}
