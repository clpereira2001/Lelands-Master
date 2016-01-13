using System;
using System.Collections.Generic;
using System.Linq;
using Vauction.Utils;
using Vauction.Utils.Lib;
using Vauction.Utils.Perfomance;

namespace Vauction.Models
{
  public class DifferentRepository : IDifferentRepository
  {
    private readonly ICacheDataProvider _cacheRepository;
    private readonly VauctionDataContext _dataContext;

    public DifferentRepository(VauctionDataContext dc, ICacheDataProvider cacheRepository)
    {
      _dataContext = dc;
      _cacheRepository = cacheRepository;
    }

    //GetAuctionImages
    public List<Image> GetAuctionImages(long auctionID)
    {
      var dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.IMAGES, "GETAUCTIONIMAGES",
        new object[] { auctionID }, CachingExpirationTime.Minutes_30);
      var result = _cacheRepository.Get(dco) as List<Image>;
      if (result != null && result.Any()) return result;
      _dataContext.CommandTimeout = 600000;
      result = _dataContext.spAuction_GetAuctionImages(auctionID).ToList();
      if (result.Any())
      {
        dco.Data = result;
        _cacheRepository.Add(dco);
      }
      return result;
    }

    //GetCountries
    public List<Country> GetCountries()
    {
      var dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.COUNTRIES, "GETCOUNTRIES", new object[] { },
        CachingExpirationTime.Days_01);
      var result = _cacheRepository.Get(dco) as List<Country>;
      if (result != null) return result;
      result = _dataContext.spCountry_List().ToList();
      if (result.Any())
      {
        dco.Data = result;
        _cacheRepository.Add(dco);
      }
      return result;
    }

    //GetStates
    public List<State> GetStates(long? countryID)
    {
      var dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.STATES, "GETSTATES",
        new object[] { countryID.GetValueOrDefault(0) }, CachingExpirationTime.Days_01);
      var result = _cacheRepository.Get(dco) as List<State>;
      if (result != null) return result;
      result = _dataContext.spState_List(countryID).ToList();
      if (result.Any())
      {
        dco.Data = result;
        _cacheRepository.Add(dco);
      }
      return result;
    }

    //GetCountry
    public Country GetCountry(long countryID)
    {
      var dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.COUNTRIES, "GETCOUNTRY",
        new object[] { countryID }, CachingExpirationTime.Days_01);
      var result = _cacheRepository.Get(dco) as Country;
      if (result != null) return result;
      result = _dataContext.spCountry_List().FirstOrDefault(t => t.ID == countryID);
      if (result != null)
      {
        dco.Data = result;
        _cacheRepository.Add(dco);
      }
      return result;
    }

    //GetStateByCode
    public State GetStateByCode(string code)
    {
      var dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.STATES, "GETSTATEBYCODE",
        new object[] { code }, CachingExpirationTime.Days_01);
      var result = _cacheRepository.Get(dco) as State;
      try
      {
        if (result != null) return result;
        result = _dataContext.spState_List(null).FirstOrDefault(t => t.Code.ToLower() == code.ToLower());
        if (result == null)
          result = _dataContext.States.FirstOrDefault(t => t.ID == 0);
        if (result != null)
        {
          dco.Data = result;
          _cacheRepository.Add(dco);
        }
      }
      catch (Exception ex)
      {
        Logger.LogException("[code=" + code + "]", ex);
      }
      return result;
    }

    //GetOuterSubscription
    public OuterSubscription GetOuterSubscription(long id)
    {
      return _dataContext.spSelect_OuterSubscription(id).SingleOrDefault();
    }

    //ValidateOuterSubscriptionEmail
    public bool ValidateOuterSubscriptionEmail(string email, long outersubscriptionID)
    {
      return
        _dataContext.OuterSubscriptions.Where(
          os => os.ID != outersubscriptionID && os.Email.Trim().ToLower() == email.Trim().ToLower()).Count() == 0;
    }

    //GetHomePageBuzzList
    public List<HotNew> GetHomePageBuzzList(bool isonlyhtml)
    {
      var dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.HOTNEWS, "GETHOMEPAGEBUZZLIST",
        new object[] { isonlyhtml }, CachingExpirationTime.Minutes_10);
      var result = _cacheRepository.Get(dco) as List<HotNew>;
      if (result != null && result.Any()) return result;
      result = _dataContext.spHotNews_Buzz(isonlyhtml).ToList();
      if (result.Any())
      {
        dco.Data = result;
        _cacheRepository.Add(dco);
      }
      return result;
    }

    //GetHomepageImages
    public List<HomepageImage> GetHomePageImages(Consts.HomepageImageType imgtype)
    {
      var dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.HOTNEWS, "GETHOMEPAGEIMAGES",
        new object[] { }, CachingExpirationTime.Minutes_05);
      var result = _cacheRepository.Get(dco) as List<HomepageImage>;
      if (result != null && result.Any())
        return result.Where(r => r.IsEnabled && r.ImgType == (byte)imgtype).ToList();
      result = _dataContext.spImages_Homepage().ToList();
      if (result.Any())
      {
        dco.Data = result;
        _cacheRepository.Add(dco);
      }
      return result.Where(r => r.IsEnabled && r.ImgType == (byte)imgtype).ToList();
    }

    #region tracking

    //TrackSPAbanner
    public void TrackSPAbanner(string ip, long userID)
    {
      try
      {
        _dataContext.spTracking_SPAbanner(ip, userID);
      }
      catch (Exception ex)
      {
        Logger.LogException("[SPA banner tracking: " + ip + "]", ex);
      }
    }

    //TrackEmail
    public void TrackEmail(string ip, string eventID, long? userID, string type)
    {
      try
      {
        _dataContext.spTracking_EventEmail(ip, eventID, userID, type);
      }
      catch (Exception ex)
      {
        Logger.LogException(
          String.Format("[Email tracking: event_id={0}, user_id={1}, type={2}, IP={3}]", eventID,
            userID.GetValueOrDefault(-1), type, ip), ex);
      }
    }

    //TrackForwardingURL
    public void TrackForwardingURL(string ip, string eventID, string url, long? userID, string type)
    {
      try
      {
        _dataContext.spTracking_ForwardingURL(ip, eventID, url, userID, type);
      }
      catch (Exception ex)
      {
        Logger.LogException(
          String.Format("F. URL tracking: event_id={0}, url={4}, user_id={1}, type={2}, IP={3}]", eventID,
            userID.GetValueOrDefault(-1), type, ip, url), ex);
      }
    }

    #endregion

    //GetTag
    public Tag GetTag(long tagId)
    {
      var dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.TAGS, "GETTAG",
        new object[] { tagId }, CachingExpirationTime.Days_01);
      var result = _cacheRepository.Get(dco) as Tag;
      if (result != null) return result;
      result = _dataContext.Tags.FirstOrDefault(t => t.ID == tagId);
      if (result != null)
      {
        dco.Data = result;
        _cacheRepository.Add(dco);
      }
      return result;
    }
  }
}