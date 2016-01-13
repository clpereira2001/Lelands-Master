using System.Collections.Generic;
using System.Web;
using Vauction.Models.CustomClasses;

namespace Vauction.Models
{
  public interface IGeneralRepository
  {
    string GetPaymentMethodsForSelect();


    string GetEventsListForSearchSelect();
    string GetAuctionPriorityForSearchSelect();

    object GetUserTypesJSON();
    object GetUserStatusesJSON();
    object GetCommissionRatesJSON();
    object GetCountryStatesJSON();
    object GetSpecialistsJSON();
    object GetEventsJSON();
    object GetEventsViewableJSON();
    object GetPendingEventsJSON();
    object GetAuctionStatusesJSON();
    object GetMainCategoriesJSON();
    object GetEventsDateTimeJSON();
    object GetEventsClosedDateTimeJSON();
    object GetEventsClosedDateTimeShortJSON();
    object GetMainCategoriesAndForSaleJSON();
    object GetPurchasedTypesJSON(bool isNUll);

    //NOT DONE




    string GetCommissionRatesForSelect();

    string GetAuctionStatusesForSearchSelect();

    string GetMainCategoriesForSearchSelect();

    object GetCountryList(string sidx, string sord, int page, int rows, bool _search, string Id, string Title, string Code);
    bool AddEditCountry(long? ID, string Title, string Code, bool IsAdding);
    bool DeleteCountry(long ID);

    object GetCommissionRateList(string sidx, string sord, int page, int rows, bool _search, string RateID, string Description, string MaxPercent);
    bool AddEditCommissionRate(long id, string description, decimal maxPercent, bool isAddin, string longDescription);
    bool DeleteCommissionRate(long ID);

    object GetVariableList(string sidx, string sord, int page, int rows, bool _search, string Id, string Name, string Value);
    bool AddEditVariable(long ID, string Name, decimal Value, bool IsAddin);
    bool DeleteVariable(long ID);

    object GetStateList(string sidx, string sord, int page, int rows, bool _search, string Id, string Title, string Code, string Country_ID);
    bool AddEditState(long ID, string Title, string Code, long Country_ID, bool IsAddin);
    bool DeleteState(long ID);
    object GetCountriesJSON();

    string PrintLabelsForItems(Dictionary<long, byte> auction_count, long cons_id);

    string GetUserTypesForSearchSelect();
    string GetUserStatusesForSearchSelect();
    string GetCommissionRatesForSearchSelect();
    object GetEmailImages(string sidx, string sord, int page, int rows);
    JsonExecuteResult UploadEmailImage(HttpPostedFileBase file);
    JsonExecuteResult DeleteEmailImages(string filenames);
    object GetDifferentImages(string sidx, string sord, int page, int rows);
    JsonExecuteResult UploadDifferentImage(HttpPostedFileBase file);
    JsonExecuteResult DeleteDifferentImages(string filenames);
    object GetHomepageImages(string sidx, string sord, int page, int rows);
    JsonExecuteResult UploadHomepageImage(HttpPostedFileBase file);
    JsonExecuteResult DeleteHomepageImages(long[] homepageimage_id);
    JsonExecuteResult UpdateHomepageImage(long homepageimage_id, int order, string link, string linktitle, int type, bool isenabled);


    object GetCacheList(string sidx, string sord, int page, int rows, bool issearch, string type, string region, string method);
    JsonExecuteResult InitRegions();
    JsonExecuteResult DeleteCache(string key);
    JsonExecuteResult ClearRegion(string type);

    List<string> Temp_CreateSpreadsheets(long event_id);
    string Temp_CreateSpreadsheetW(long event_id);
    List<string> Temp_CreateSpreadsheetsID(long event_id);
    void Temp_SendUserInvoices();
    int SendInvoices(long event_id);
    int SendConsignments(long event_id);
    IEnumerable<Tag> GetTags();
    Tag UpdateTag(Tag tg);
    void DeleteTag(long tagID);
    IEnumerable<Collection> GetCollections();
    Collection UpdateCollection(Collection c);
    void DeleteCollection(long tagID);
  }
}