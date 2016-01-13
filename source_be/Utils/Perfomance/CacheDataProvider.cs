using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;

namespace Vauction.Utils.Perfomance
{
  public class CacheDataProvider : ICacheDataProvider
  {
    private ObjectCache Cache { get { return MemoryCache.Default; } }

    public string GetKeyByParams(object[] param)
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
      return String.Format("{0}_{1}{2}", obj.Region, obj.Method, GetKeyByParams(obj.Params));
    }

    public object Get(DataCacheObject obj)
    {
      return Cache.Get(GetKey(obj));
    }

    private void AddWithoutCheck(DataCacheObject obj)
    {
      CacheItemPolicy policy = new CacheItemPolicy();
      policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromSeconds(obj.CacheTime);
      lock (Cache)
        Cache.Add(new CacheItem(GetKey(obj), obj.Data), policy);
    }

    public void Add(DataCacheObject obj)
    {
      if (Contains(obj)) Remove(obj);
      AddWithoutCheck(obj);
    }

    private void UpdateWithoutCheck(DataCacheObject obj)
    {
      lock (Cache)
        Cache[GetKey(obj)] = obj.Data;
    }

    public void Update(DataCacheObject obj)
    {
      if (!Contains(obj)) return;
      UpdateWithoutCheck(obj);
    }

    public void Put(DataCacheObject obj)
    {
      if (!Contains(obj)) AddWithoutCheck(obj);
      else UpdateWithoutCheck(obj);
    }

    public bool Contains(DataCacheObject obj)
    {
      return Cache.Contains(GetKey(obj));
    }

    public void Remove(DataCacheObject obj)
    {
      if (!Contains(obj)) return;
      Cache.Remove(GetKey(obj));
    }

    public List<string> Clear(string Type, string Region)
    {
      List<string> cacheItems = new List<string>();
      lock (Cache)
      {
        foreach (string res in Cache.Where(C => C.Key.StartsWith(Region)).Select(C1=>C1.Key).ToList())
        {
          Cache.Remove(res);
          cacheItems.Add(res);
        }
      }
      return cacheItems;
    }

    public List<string> Clear()
    {
      List<string> cacheItems = new List<string>();
      lock (Cache)
      {
        foreach (string key in Cache.Select(C => C.Key).ToList())
        {
          Cache.Remove(key);
          cacheItems.Add(key);
        }
      }
      return cacheItems;
    }

    public List<DataCacheObject> AllCache()
    {
      List<DataCacheObject> result = new List<DataCacheObject>();
      DataCacheObject dco;
      foreach (var c in Cache)
      {
        dco = new DataCacheObject();
        dco.Data = c.Value;
        dco.Method = c.Key;
        result.Add(dco);
      }
      return result;
    }
  }
}
