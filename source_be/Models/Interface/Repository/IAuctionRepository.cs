using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Vauction.Models.CustomClasses;

namespace Vauction.Models
{
  public interface IAuctionRepository
  {
    Consignment GetConsignment(long consignmentID);
    ConsignmentContract GetConsignmentContract(long consignmentID);
    ConsignmentContract UpdateConsignmentContract(ConsignmentContract cc);
    Specialist GetSpecialist(long specialistID);
    long GetConsignmentStatement(long user_id, long event_id);
    object GetConsignorsList(string sidx, string sord, int page, int rows, long? consignment_id, long? user_id, string login, string flname, string email, long? event_id, DateTime? consdate, string specialist);    
    object UpdateConsignorStatementForm(string cons, ModelStateDictionary ModelState);
    JsonExecuteResult DeleteConsignorStatement(long cons_id);
    JsonExecuteResult AddConsignment(long user_id, long event_id);
    object GetAuctionsListByConsignor(string sidx, string sord, int page, int rows, long cons_id);
    List<IdTitleValue> GetAuctionsListByConsignor(long consignmentID);
    object GetConsingorStatementDataJSON(long cons_id);    
    object GetConsingorStatementByAyctionDataJSON(long auction_id);
    object GetAuctionsListByEvent(string sidx, string sord, int page, int rows, long event_id, long? auction_id, string title, byte? priority);
    object UpdateDescription(long auction_id, string descr, string cnotes, bool iswritten);
    object GetAuctionsList(string sidx, string sord, int page, int rows, long? auction_id, long? oldinventory, short? lot, string title, string Event, long? owner_id);
    object GetConsignorStatementUnsoldItems(string sidx, string sord, int page, int rows, long cons_id);
    void UpdateAuctionBiddingResult(long auction_id, long? user_id, decimal? currentbid, decimal? maxbid);
    object GetDOWList(string sidx, string sord, int page, int rows, long? auction_id, int? status_id, string category, string title, decimal? price, decimal? shipping, string consignor, byte? commrate);
    object GetDOWDataJSON(long auction_id);
    object GetItemsForSale(string sidx, string sord, int page, int rows, long? auction_id, int? lot, string title, decimal? price, long owner_id);
    JsonExecuteResult UpdateDOWForm(string auction, ModelStateDictionary ModelState);
    JsonExecuteResult UpdateLotNumber(long auction_id, short? lot);
    JsonExecuteResult UpdateShipping(long auction_id, decimal shipping);



    // NOT DONE

    Auction GetAuction(long auction_id);
    object GetBidLogList(string sidx, string sord, int page, int rows, long event_id);
    object GetBidLogListEventManagment(string sidx, string sord, int page, int rows, long event_id);
    object GetShortDetailedList(string sidx, string sord, int page, int rows, bool _search, long? lot, string title, decimal? currentbid, int? bidnumber, long event_id);
    
    object GetAuctionsList(string sidx, string sord, int page, int rows, long? auction_id, long? evnt, string lot, byte? status, byte? maincatory, string category, string title, decimal? price, decimal? reserve, byte? commrate, string seller, decimal? shipping, byte? prioritydescription, long? oldauction_id, string eneteredby, string specialist, string highbidder, string currentbid, long? tagId);
    
    object GetAuctionsListByConsignorForPrint(long cons_id);
    object UpdateItemForConsignorStatementForm(string item, ModelStateDictionary ModelState);
    bool UpdateItemForConsignorStatement(NewAuctionForConsignorForm info);
    JsonExecuteResult DeleteAuction(long auction_id);
    object GetAuctionDataJSON(long auctionID);
    JsonExecuteResult UpdateAuctionForm(string auction, ModelStateDictionary ModelState);
    void ClearImagesForNewAuction(long user_id);
    object GetAuctionImages(long auction_id, long user_id);
    JsonExecuteResult UploadImage(long auction_id, HttpPostedFileBase file);
    JsonExecuteResult UploadImage(long auction_id, long? user_id, HttpPostedFileBase file);
    long AddAuctionImage(long auction_id, string image, string uploadedfilename);
    Image AddAuctionImg(long auction_id, string image, string uploadedfilename);
    JsonExecuteResult ResortImages(long auction_id);
    JsonExecuteResult CropImage(long auction_id, string smallimage, string mediumimage, int X, int Y, int W, int H);
    JsonExecuteResult DeleteImage(long auction_id, long image_id);
    JsonExecuteResult MoveImage(long auction_id, long image_id, bool isup);
    JsonExecuteResult SetImageAsDefault(long auction_id, long image_id);
    object GetAuctionImages(string sidx, string sord, int page, int rows);
    JsonExecuteResult UploadImage(HttpPostedFileBase file);
    JsonExecuteResult DeleteUploadedImages(string filenames);
    JsonExecuteResult EditUploadedImages(string oldfile, string newfile);
    JsonExecuteResult AsignImages(string[] filenames, long? event_id);
    object GetImagesByAuction(long auction_id);
    List<Image> GetAuctionImages(long auction_id);
    JsonExecuteResult AddWinnerToAuction(long auction_id, long user_id);
    JsonExecuteResult AddRightsToAuction(long auction_id, long user_id);
    object GetBidsByAuctionID(string sidx, string sord, int page, int rows, long auction_id);
    object GetBidsLogByAuctionID(string sidx, string sord, int page, int rows, long auction_id);
    //object GetContractItems(string sidx, string sord, int page, int rows, long cons_id);
    //JsonExecuteResult UpdateContractItem(int? id, string title, int commissionRate_ID, long cons_id);
  }
}
