using System.Collections.Generic;

namespace Vauction.Utils.Perfomance
{
  public interface ICacheDataProvider
  {
    object Get(DataCacheObject obj);
    void Add(DataCacheObject obj);
    void Update(DataCacheObject obj);
    void Put(DataCacheObject obj);
    bool Contains(DataCacheObject obj);
    void Remove(DataCacheObject obj);
    List<string> Clear(string Type, string Region);
    List<string> Clear();
    List<DataCacheObject> AllCache();
  }
}