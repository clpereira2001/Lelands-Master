using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils;

namespace Vauction.Models
{
  public interface IDifferentRepository
  {
    List<Image> GetAuctionImages(long auctionID);
    List<Country> GetCountries();
    Country GetCountry(long countryID);
    List<State> GetStates(Int64? countryID);
    State GetStateByCode(string code);
    OuterSubscription GetOuterSubscription(long id);
    bool ValidateOuterSubscriptionEmail(string email, long outersubscriptionID);
    void TrackSPAbanner(string ip, long userID);
    void TrackEmail(string ip, string eventID, long? userID, string type);
    List<HotNew> GetHomePageBuzzList(bool isonlyhtml);
    void TrackForwardingURL(string ip, string eventID, string url, long? userID, string type);
    List<HomepageImage> GetHomePageImages(Consts.HomepageImageType imgtype);
    Tag GetTag(long tagId);
  }
}
