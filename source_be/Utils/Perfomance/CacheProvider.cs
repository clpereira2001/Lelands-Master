using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace Vauction.Utils.Perfomance
{
  #region CacheDataKeys
  public enum CacheDataKeys { 
    
  };
  #endregion

  #region ICacheDataProvider
  public interface ICacheDataProvider
  {
    object Get(CacheDataKeys key);
    object Get(CacheDataKeys key, object[] prms);    
    void Set(CacheDataKeys key, object data, int cacheTime);
    void Set(CacheDataKeys key, object data, int cacheTime, object[] prms);
    void Update(CacheDataKeys key, object data);
    void Update(CacheDataKeys key, object data, object[] prms);
    void SetUpdate(CacheDataKeys key, object data, int cacheTime);
    void SetUpdate(CacheDataKeys key, object data, int cacheTime, object[] prms);
    bool Contains(CacheDataKeys key);
    bool Contains(CacheDataKeys key, object[] prms);    
    void Remove(CacheDataKeys key);
    void Remove(CacheDataKeys key, object[] prms);
    void Clear();
  }
  #endregion

  #region CacheDataProvider
  public class CacheDataProvider : ICacheDataProvider
  {
    private ObjectCache Cache { get { return MemoryCache.Default; } }

    public string GetKeyByParams(object[] param)
    {
      if (param == null || param.Length == 0) return String.Empty;
      System.Text.StringBuilder sb = new System.Text.StringBuilder();      
      foreach (object obj in param)
      {
        sb.Append("_");
        if (obj != null) sb.Append(obj.GetHashCode());
        else sb.Append("nullparam");
      }      
      return sb.ToString();
    }

    #region Get
    public object Get(CacheDataKeys key)
    {
      return Get(key, null);
    }
    public object Get(CacheDataKeys key, object[] prms)
    {
      return Cache.Get(key.ToString() + GetKeyByParams(prms));
    }    
    #endregion

    #region Set
    public void Set(CacheDataKeys key, object data, int cacheTime)
    {
      Set(key, data, cacheTime, null);
    }
    public void Set(CacheDataKeys key, object data, int cacheTime, object[] prms)
    {
      if (Contains(key, prms)) Remove(key, prms);
      CacheItemPolicy policy = new CacheItemPolicy();
      DateTime dt = DateTime.Now;
      policy.AbsoluteExpiration = dt + TimeSpan.FromSeconds(cacheTime);
      lock (Cache)
      {
        Cache.Add(new CacheItem(key.ToString() + GetKeyByParams(prms), data), policy);
      }
    }
    #endregion

    #region Update
    public void Update(CacheDataKeys key, object data)
    {
      Update(key, data, null);
    }
    public void Update(CacheDataKeys key, object data, object[] prms)
    {
      if (!Contains(key, prms)) return;
      lock (Cache)
      {
        Cache[key.ToString() + GetKeyByParams(prms)] = data;
      }
    }
    #endregion

    #region SetUpdate
    public void SetUpdate(CacheDataKeys key, object data, int cacheTime)
    {
      SetUpdate(key, data, cacheTime, null);
    }
    public void SetUpdate(CacheDataKeys key, object data, int cacheTime, object[] prms)
    {
      if (!Contains(key, prms))
        Set(key, data, cacheTime, prms);
      else
        lock (Cache)
        {
          Cache[key.ToString() + GetKeyByParams(prms)] = data;
        }
    }
    #endregion

    #region Contains
    public bool Contains(CacheDataKeys key)
    {
      return Contains(key, null);
    }
    public bool Contains(CacheDataKeys key, object[] prms)
    {
      return Cache.Contains(key.ToString()+GetKeyByParams(prms));
    }
    #endregion

    #region Remove
    public void Remove(CacheDataKeys key)
    {
      Remove(key, null);
    }    
    public void Remove(CacheDataKeys key, object[] prms)
    {
      lock (Cache)
      {
        Cache.Remove(key.ToString() + GetKeyByParams(prms));
      }
    }
    #endregion

    #region Clear
    public void Clear()
    {
      lock (Cache)
      {
        List<string> keys = Cache.Select(c => c.Key).ToList();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("[ALL CACHE CLEARED: " + System.DateTime.Now.ToString() + "]");
        foreach (string key in keys)
        {
          sb.AppendLine(key);
          Cache.Remove(key);
        }
        Vauction.Utils.Lib.Logger.LogInfo(sb.ToString());
      }
    }
    #endregion
  }
  #endregion
}