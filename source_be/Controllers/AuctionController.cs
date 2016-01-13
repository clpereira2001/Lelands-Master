using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Vauction.Models;
using Vauction.Models.CustomClasses;
using Vauction.Utils;
using Vauction.Utils.Autorization;
using Vauction.Utils.Perfomance;
using Vauction.Utils.Reports;

namespace Vauction.Controllers
{
    [Compress]
    public class AuctionController : BaseController
    {
        #region init

        private IAuctionRepository AuctionRepository;
        private IBidRepository BidRepository;
        private IEventRepository EventRepository;
        private IGeneralRepository GeneralRepository;

        public AuctionController()
        {
            AuctionRepository = dataProvider.AuctionRepository;
            BidRepository = dataProvider.BidRepository;
            EventRepository = dataProvider.EventRepository;
            GeneralRepository = dataProvider.GeneralRepository;
        }

        #endregion

        #region consignor auction listing

        //Consignors
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet]
        public ActionResult Consignors()
        {
            ViewData["PaymentTypes"] = GeneralRepository.GetPaymentMethodsForSelect();
            ViewData["EventsList"] = GeneralRepository.GetEventsListForSearchSelect();
            //ViewData["CommissionRateTypes"] = GeneralRepository.GetCommissionRatesForSearchSelect();
            return View();
        }

        //GetConsignorsList
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet]
        public JsonResult GetConsignorsList(string sidx, string sord, int page, int rows, bool _search,
            bool? isfirstload, long? Consignment_ID, long? User_ID, string Login, string FLName, string Email,
            long? Event, DateTime? ConsDate, string Specialist)
        {
            return (!isfirstload.HasValue || isfirstload.Value)
                ? JSON(false)
                : JSON(AuctionRepository.GetConsignorsList(sidx, sord, page, rows, Consignment_ID, User_ID, Login,
                    FLName, Email, Event, ConsDate, Specialist));
        }

        //GetAuctionsListByConsignor    
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet]
        public JsonResult GetAuctionsListByConsignor(string sidx, string sord, int page, int rows, long? cons_id)
        {
            return (!cons_id.HasValue)
                ? JSON(false)
                : JSON(AuctionRepository.GetAuctionsListByConsignor(sidx, sord, page, rows, cons_id.Value));
        }

        //UpdateConsignorStatement    
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpPost]
        public JsonResult UpdateConsignorStatement(string cons)
        {
            return
                JSON(String.IsNullOrEmpty(cons)
                    ? false
                    : AuctionRepository.UpdateConsignorStatementForm(cons, ModelState));
        }

        //DeleteConsignorStatement    
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpPost]
        public JsonResult DeleteConsignorStatement(long cons_id)
        {
            return JSON(AuctionRepository.DeleteConsignorStatement(cons_id));
        }

        //GetConsignorStatementJSON    
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpPost]
        public JsonResult GetConsignorStatementJSON(long? cons_id)
        {
            return JSON((!cons_id.HasValue) ? false : AuctionRepository.GetConsingorStatementDataJSON(cons_id.Value));
        }

        //UpdateConsignmentContractText    
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpPost]
        public JsonResult UpdateConsignmentContractText(long consignmentID, string contractText)
        {
            try
            {
                var consignment = AuctionRepository.GetConsignment(consignmentID);
                if (consignment == null) throw new Exception("Consignment does not exist.");
                var consignmentContract = AuctionRepository.GetConsignmentContract(consignmentID);
                if (consignmentContract == null) throw new Exception("Consignment does not exist.");
                if (consignmentContract.StatusID == (int) Consts.ConsignmentContractStatus.Signed)
                {
                    throw new Exception("The contract is already signed by seller.");
                }
                consignmentContract.ContractText = contractText;
                var fileName = string.Format("CA{0}.pdf", DateTime.Now.Ticks);
                var specialist = AuctionRepository.GetSpecialist(consignment.Specialist_ID.GetValueOrDefault(-1));
                if (specialist == null)
                {
                    throw new Exception("Set specialist at first.");
                }
                var lelandsSignaturePath =
                    DiffMethods.SignatureImagesOnDisk(string.Format("userSignature{0}.png", specialist.User_ID));
                var fileInfo = new FileInfo(lelandsSignaturePath);
                if (!fileInfo.Exists)
                {
                    throw new Exception("You can't generate contract without specialist signature.");
                }
                if (!string.IsNullOrWhiteSpace(consignmentContract.FileName))
                {
                    fileInfo =
                        new FileInfo(DiffMethods.ConsignmentContractOnDisk(consignment.ID, consignmentContract.FileName));
                    if (fileInfo.Exists) fileInfo.Delete();
                }
                var lelandsAddress = new Address
                {
                    FirstName = "Lelands.com",
                    LastName = Consts.SiteEmail,
                    Address_1 = Consts.CompanyAddress,
                    City = Consts.CompanyCity,
                    State = Consts.CompanyState,
                    Zip = Consts.CompanyZip,
                    HomePhone = Consts.CompanyPhone,
                    WorkPhone = Consts.CompanyFax
                };
                var consignor = consignment.User;
                var consignorAddress = Address.GetAddress(consignor.AddressCard_Billing);
                var items = AuctionRepository.GetAuctionsListByConsignor(consignment.ID);
                PdfReports.ConsignmentContract(DiffMethods.ConsignmentContractOnDisk(consignment.ID, fileName),
                    DiffMethods.PublicImagesOnDisk("logo.png"), string.Empty, Consts.CompanyTitleName, string.Empty,
                    lelandsAddress, lelandsSignaturePath, consignorAddress, consignor.Email, string.Empty, DateTime.Now,
                    items, consignmentContract.ContractText);
                consignmentContract.UpdateFields(consignmentContract.ConsignmentID,
                    (int) Consts.ConsignmentContractStatus.Unsigned, consignmentContract.ContractText, fileName);
                AuctionRepository.UpdateConsignmentContract(consignmentContract);
                return
                    JSON(new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, "Contract was generated successfully.",
                        new
                        {
                            StatusText = Consts.ConsignmentContractStatus.Unsigned.ToString(),
                            DownloadLink =
                                Url.Action("GetConsignmentContract", "Auction", new {consignmentID = consignment.ID}),
                            ShowLink =
                                Url.Action("ShowConsignmentContract", "Auction", new {consignmentID = consignment.ID})
                        }));
            }
            catch (Exception exc)
            {
                return JSON(new JsonExecuteResult(JsonExecuteResultTypes.ERROR, exc.Message));
            }
        }

        //ResetConsignmentContract    
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpPost]
        public JsonResult ResetConsignmentContract(long consignmentID)
        {
            try
            {
                var consignmentContract = AuctionRepository.GetConsignmentContract(consignmentID);
                var consignment = AuctionRepository.GetConsignment(consignmentID);
                if (consignmentContract == null || consignment == null)
                    throw new Exception("Consignment does not exist.");
                if (consignmentContract.StatusID == (int) Consts.ConsignmentContractStatus.Signed)
                {
                    throw new Exception("The contract is already signed by seller.");
                }
                var filePath = DiffMethods.TemplateDifferentOnDisk("ConsignmentContract.txt");
                var fileInfo = new FileInfo(filePath);
                var contractTemplate = string.Empty;
                if (fileInfo.Exists)
                {
                    contractTemplate = System.IO.File.ReadAllText(filePath);
                    var buyerFee = consignment.Event.BuyerFee.GetValueOrDefault(0).GetPrice();
                    var buyerFeeStr = buyerFee.ToString();
                    if (buyerFee == (int) buyerFee)
                    {
                        buyerFeeStr = buyerFeeStr.Remove(buyerFeeStr.Length - 3);
                    }
                    contractTemplate =
                        contractTemplate.Replace("{{BuyersPremium}}", string.Format("{0}%", buyerFeeStr))
                            .Replace("{{CommissionRate}}", consignment.User.CommissionRate.LongDescription);
                }
                if (!string.IsNullOrWhiteSpace(consignmentContract.FileName))
                {
                    fileInfo =
                        new FileInfo(DiffMethods.ConsignmentContractOnDisk(consignmentID, consignmentContract.FileName));
                    if (fileInfo.Exists) fileInfo.Delete();
                }
                consignmentContract.UpdateFields(consignmentContract.ConsignmentID,
                    (int) Consts.ConsignmentContractStatus.NotGenerated, contractTemplate, string.Empty);
                AuctionRepository.UpdateConsignmentContract(consignmentContract);
                return
                    JSON(new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, "Contract was reset to default.",
                        new
                        {
                            StatusText = Consts.ConsignmentContractStatus.NotGenerated.ToString(),
                            ContractText = contractTemplate
                        }));
            }
            catch (Exception exc)
            {
                return JSON(new JsonExecuteResult(JsonExecuteResultTypes.ERROR, exc.Message));
            }
        }

        //RegenerateConsignmentContract    
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpPost]
        public JsonResult RegenerateConsignmentContract(long consignmentID)
        {
            try
            {
                var consignmentContract = AuctionRepository.GetConsignmentContract(consignmentID);
                var consignment = AuctionRepository.GetConsignment(consignmentID);
                if (consignmentContract == null || consignment == null)
                    throw new Exception("Consignment does not exist.");
                if (consignmentContract.StatusID == (int) Consts.ConsignmentContractStatus.Signed)
                {
                    throw new Exception("The contract is already signed by seller.");
                }
                var fileName = string.Format("CA{0}.pdf", DateTime.Now.Ticks);
                var specialist = AuctionRepository.GetSpecialist(consignment.Specialist_ID.GetValueOrDefault(-1));
                if (specialist == null)
                {
                    throw new Exception("Set specialist at first.");
                }
                var lelandsSignaturePath =
                    DiffMethods.SignatureImagesOnDisk(string.Format("userSignature{0}.png", specialist.User_ID));
                var fileInfo = new FileInfo(lelandsSignaturePath);
                if (!fileInfo.Exists)
                {
                    throw new Exception("You can't generate contract without specialist signature.");
                }
                if (!string.IsNullOrWhiteSpace(consignmentContract.FileName))
                {
                    fileInfo =
                        new FileInfo(DiffMethods.ConsignmentContractOnDisk(consignment.ID, consignmentContract.FileName));
                    if (fileInfo.Exists) fileInfo.Delete();
                }
                var lelandsAddress = new Address
                {
                    FirstName = "Lelands.com",
                    LastName = Consts.SiteEmail,
                    Address_1 = Consts.CompanyAddress,
                    City = Consts.CompanyCity,
                    State = Consts.CompanyState,
                    Zip = Consts.CompanyZip,
                    HomePhone = Consts.CompanyPhone,
                    WorkPhone = Consts.CompanyFax
                };
                var consignor = consignment.User;
                var consignorAddress = Address.GetAddress(consignor.AddressCard_Billing);
                var items = AuctionRepository.GetAuctionsListByConsignor(consignment.ID);
                PdfReports.ConsignmentContract(DiffMethods.ConsignmentContractOnDisk(consignment.ID, fileName),
                    DiffMethods.PublicImagesOnDisk("logo.png"), string.Empty, Consts.CompanyTitleName, string.Empty,
                    lelandsAddress, lelandsSignaturePath, consignorAddress, consignor.Email, string.Empty, DateTime.Now,
                    items, consignmentContract.ContractText);
                consignmentContract.UpdateFields(consignmentContract.ConsignmentID, consignmentContract.StatusID,
                    consignmentContract.ContractText, fileName);
                AuctionRepository.UpdateConsignmentContract(consignmentContract);
                return
                    JSON(new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, "Contract was regenerated successfully."));
            }
            catch (Exception exc)
            {
                return JSON(new JsonExecuteResult(JsonExecuteResultTypes.ERROR, exc.Message));
            }
        }

        //GetConsignmentContract
        [VauctionAuthorize]
        public FilePathResult GetConsignmentContract(long consignmentID)
        {
            try
            {
                var consignmentContract = AuctionRepository.GetConsignmentContract(consignmentID);
                if (consignmentContract == null ||
                    consignmentContract.StatusID == (int) Consts.ConsignmentContractStatus.NotGenerated ||
                    string.IsNullOrWhiteSpace(consignmentContract.FileName)) return null;
                var path = DiffMethods.ConsignmentContractOnDisk(consignmentID, consignmentContract.FileName);
                return File(path, "application/pdf", consignmentContract.FileName);
            }
            catch
            {
                return null;
            }
        }

        //ShowConsignmentContract
        [VauctionAuthorize]
        public FileStreamResult ShowConsignmentContract(long consignmentID)
        {
            var consignmentContract = AuctionRepository.GetConsignmentContract(consignmentID);
            if (consignmentContract == null ||
                consignmentContract.StatusID == (int) Consts.ConsignmentContractStatus.NotGenerated ||
                string.IsNullOrWhiteSpace(consignmentContract.FileName)) return null;
            var path = DiffMethods.ConsignmentContractOnDisk(consignmentID, consignmentContract.FileName);
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            return File(fs, "application/pdf");
        }

        //GetConsignorStatementByAuctionJSON
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpPost]
        public JsonResult GetConsignorStatementByAuctionJSON(long? auction_id)
        {
            return
                JSON((!auction_id.HasValue)
                    ? false
                    : AuctionRepository.GetConsingorStatementByAyctionDataJSON(auction_id.Value));
        }

        //GetConsignorStatementUnsoldItems
        [VauctionAuthorize(Roles = "Root,Admin"), ActionOutputCache(CachingExpirationTime.Minutes_01)]
        public JsonResult GetConsignorStatementUnsoldItems(string sidx, string sord, int page, int rows, long? cons_id)
        {
            return (!cons_id.HasValue)
                ? JSON(false)
                : JSON(AuctionRepository.GetConsignorStatementUnsoldItems(sidx, sord, page, rows, cons_id.Value));
        }

        #endregion

        #region updating auction description and copy notes

        //ItemDescription
        [VauctionAuthorize(Roles = "Writer"), HttpGet]
        public ActionResult ItemsDescription()
        {
            ViewData["PriorityList"] = GeneralRepository.GetAuctionPriorityForSearchSelect();
            return View();
        }

        //GetAuctionsByEvent
        [VauctionAuthorize(Roles = "Writer"), HttpGet]
        public JsonResult GetAuctionsListByEvent(string sidx, string sord, int page, int rows, long? event_id,
            long? Auction_ID, string Title, byte? PriorityDescription)
        {
            return (!event_id.HasValue)
                ? JSON(false)
                : JSON(AuctionRepository.GetAuctionsListByEvent(sidx, sord, page, rows, event_id.Value, Auction_ID,
                    Title, PriorityDescription));
        }

        //UpdateDescription
        [VauctionAuthorize(Roles = "Writer"), HttpPost]
        public JsonResult UpdateDescription(long auction_id, string descr, string cnotes, bool iswritten)
        {
            return JSON(AuctionRepository.UpdateDescription(auction_id, descr, cnotes, iswritten));
        }

        #endregion

        #region dialog methods

        //GetAuctionsList
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet]
        public JsonResult GetAuctionsList(string sidx, string sord, int page, int rows, bool? _firstload, bool _search,
            long? Auction_ID, long? OldInventory_ID, short? Lot, string Title, string EventTitle, long? owner_id)
        {
            return ((_firstload.HasValue && _firstload.Value) || (owner_id.HasValue && owner_id.Value == -1))
                ? JSON(false)
                : JSON(AuctionRepository.GetAuctionsList(sidx, sord, page, rows, Auction_ID, OldInventory_ID, Lot, Title,
                    EventTitle, owner_id));
        }

        #endregion

        // NOT DONE


        //Index


        //[VauctionAuthorize(Roles = "Root,Admin")]
        //public ActionResult Index()
        //{
        //  return View();
        //}

        //BidLog
        [VauctionAuthorize(Roles = "Root,Admin")]
        public ActionResult BidLog()
        {
            ViewData["Events"] = new List<IEvent>(EventRepository.GetEvents());
            return View();
        }

        //BidLogList
        [VauctionAuthorize(Roles = "Root,Admin")]
        public JsonResult BidLogList(string sidx, string sord, int page, int rows, long? event_id)
        {
            return
                JSON(AuctionRepository.GetBidLogList(sidx, sord, page, rows, (event_id.HasValue) ? event_id.Value : -1));
        }

        //BidLogListEventManagment
        [VauctionAuthorize(Roles = "Root,Admin,SpecialistViewer")]
        public JsonResult BidLogListEventManagment(string sidx, string sord, int page, int rows, bool? isfirstload,
            long? event_id)
        {
            return (!isfirstload.HasValue || isfirstload.Value)
                ? JSON(false)
                : JSON(AuctionRepository.GetBidLogListEventManagment(sidx, sord, page, rows,
                    (event_id.HasValue) ? event_id.Value : -1));
        }

        //ShortDetailedListEventManagment
        [VauctionAuthorize(Roles = "Root,Admin,SpecialistViewer")]
        public JsonResult ShortDetailedListEventManagment(string sidx, string sord, int page, int rows,
            bool? isfirstload, bool _search, long? Lot, string Title, decimal? CurrentBid, int? BidNumber,
            long? event_id)
        {
            return (!isfirstload.HasValue || isfirstload.Value)
                ? JSON(false)
                : JSON(AuctionRepository.GetShortDetailedList(sidx, sord, page, rows, _search, Lot, Title, CurrentBid,
                    BidNumber, (event_id.HasValue) ? event_id.Value : -1));
        }

        //AllBiddersForAuction
        [VauctionAuthorize(Roles = "Root,Admin,SpecialistViewer")]
        public JsonResult AllBiddersForAuction(long auction_id)
        {
            return JSON(BidRepository.GetBidsForAuction(auction_id));
        }

        //BiddingStatistic
        [VauctionAuthorize(Roles = "Root,Admin,SpecialistViewer")]
        public JsonResult BiddingStatistic(long? event_id)
        {
            return JSON(BidRepository.GetBiddingStatistic((event_id.HasValue) ? event_id.Value : -1));
        }

        //GetAuctionsListByConsignorForPrint
        [AcceptVerbs(HttpVerbs.Post)]
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
        public JsonResult GetAuctionsListByConsignorForPrint(string sidx, string sord, int page, int rows, long? cons_id)
        {
            return (!cons_id.HasValue)
                ? JSON(false)
                : JSON(AuctionRepository.GetAuctionsListByConsignorForPrint(cons_id.Value));
        }

        //AddNewItemForConsignorStatement
        [AcceptVerbs(HttpVerbs.Post)]
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
        public JsonResult AddNewItemForConsignorStatement(string item)
        {
            return
                JSON(String.IsNullOrEmpty(item)
                    ? false
                    : AuctionRepository.UpdateItemForConsignorStatementForm(item, ModelState));
        }

        //DeleteAuction
        [AcceptVerbs(HttpVerbs.Post)]
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
        public JsonResult DeleteAuction(long auction_id)
        {
            return JSON(AuctionRepository.DeleteAuction(auction_id));
        }

        //Auctions
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
        public ActionResult Auctions()
        {
            ViewData["AuctionStatusesList"] = GeneralRepository.GetAuctionStatusesForSearchSelect();
            ViewData["EventsList"] = GeneralRepository.GetEventsListForSearchSelect();
            ViewData["CommissionRateList"] = GeneralRepository.GetCommissionRatesForSearchSelect();
            ViewData["MainCategoryList"] = GeneralRepository.GetMainCategoriesForSearchSelect();
            ViewData["PriorityList"] = GeneralRepository.GetAuctionPriorityForSearchSelect();
            ViewData["PaymentTypes"] = GeneralRepository.GetPaymentMethodsForSelect();


            var tags = GeneralRepository.GetTags().ToList();
            var sb = new StringBuilder();
            sb.Append(":;");
            foreach (var tag in tags)
                sb.AppendFormat("{0}:{1};", tag.ID, tag.Title);
            sb.Remove(sb.Length - 1, 1);
            ViewData["AuctionTagsList"] = sb.ToString();
            ViewData["Tags"] = tags;
            return View();
        }

        //GetAuctionsListForAuctionsPage
        [AcceptVerbs(HttpVerbs.Post)]
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
        public JsonResult GetAuctionsListForAuctionsPage(string sidx, string sord, int page, int rows, bool? isfirstload,
            bool _search, long? Auction_ID, long? Event, string Lot, byte? Status, byte? MainCategory, string Category,
            string Title, decimal? Price, decimal? Reserve, byte? CommRate, string Seller, decimal? Shipping,
            byte? PriorityDescription, long? OldAuction_ID, string Enteredby, string Specialist, string HighBidder,
            string CurrentBid, long? tags)
        {
            return (!isfirstload.HasValue || isfirstload.Value)
                ? JSON(false)
                : JSON(AuctionRepository.GetAuctionsList(sidx, sord, page, rows, Auction_ID, Event, Lot, Status,
                    MainCategory, Category, Title, Price, Reserve, CommRate, Seller, Shipping, PriorityDescription,
                    OldAuction_ID, Enteredby, Specialist, HighBidder, CurrentBid, tags));
        }

        //GetAuctionJSON
        [AcceptVerbs(HttpVerbs.Post)]
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
        public JsonResult GetAuctionJSON(long? auction_id)
        {
            return JSON((!auction_id.HasValue) ? false : AuctionRepository.GetAuctionDataJSON(auction_id.Value));
        }

        //UpdateAuction
        [AcceptVerbs(HttpVerbs.Post)]
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
        public JsonResult UpdateAuction(string auction)
        {
            return String.IsNullOrEmpty(auction)
                ? JSON(false)
                : JSON(AuctionRepository.UpdateAuctionForm(auction, ModelState));
        }

        //UpdateLotNumber
        [HttpPost, VauctionAuthorize(Roles = "Root,Admin")]
        public JsonResult UpdateLotNumber(long id, short? Lot)
        {
            return Json(AuctionRepository.UpdateLotNumber(id, Lot));
        }

        //UpdateLotNumber
        [HttpPost, VauctionAuthorize(Roles = "Root,Admin")]
        public JsonResult UpdateShipping(long id, decimal? shipping)
        {
            return Json(AuctionRepository.UpdateShipping(id, shipping.GetValueOrDefault(0)));
        }

        //UploadBatchImage
        [HttpPost]
        public ActionResult UploadBatchImage(long? auction_id, long? user_id, HttpPostedFileBase fileData)
        {
            if (fileData == null) return Content("error");
            AuctionRepository.UploadImage(auction_id.GetValueOrDefault(0), user_id, fileData);
            return Content("ok");
        }

        //ClearImagesForNewAuction
        [AcceptVerbs(HttpVerbs.Post)]
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
        public JsonResult ClearImagesForNewAuction()
        {
            AuctionRepository.ClearImagesForNewAuction(AppHelper.CurrentUser.ID);
            return JSON(false);
        }

        //AuctionImages
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
        public ActionResult AuctionImages(long? auction_id)
        {
            ViewData["Auction_ID"] = auction_id.HasValue ? auction_id.Value : -1;
            return View();
        }

        //GetAuctionImages    
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpPost]
        public JsonResult GetAuctionImages(string sidx, string sord, int? page, int? rows, long? auction_id)
        {
            return (!auction_id.HasValue)
                ? JSON(false)
                : JSON(AuctionRepository.GetAuctionImages(auction_id.Value, AppHelper.CurrentUser.ID));
        }

        //UploadImage
        [AcceptVerbs(HttpVerbs.Post)]
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
        public ActionResult UploadImage(long? auction_id)
        {
            if (Request.Files.Count == 0 || !auction_id.HasValue) return null;
            AuctionRepository.UploadImage(auction_id.Value, Request.Files[0]);
            return RedirectToAction("AuctionImages", new {auction_id = auction_id.Value});
        }

        //DeleteImage
        [AcceptVerbs(HttpVerbs.Post)]
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
        public JsonResult DeleteImage(long? image_id, long? auction_id)
        {
            return (!image_id.HasValue || !auction_id.HasValue)
                ? JSON(false)
                : JSON(AuctionRepository.DeleteImage(auction_id.Value, image_id.Value));
        }

        //MoveImage
        [AcceptVerbs(HttpVerbs.Post)]
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
        public JsonResult MoveImage(long image_id, long auction_id, bool isup)
        {
            return JSON(AuctionRepository.MoveImage(auction_id, image_id, isup));
        }

        //SetDefaultImage
        [AcceptVerbs(HttpVerbs.Post)]
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
        public JsonResult SetDefaultImage(long? image_id, long? auction_id)
        {
            return (!image_id.HasValue || !auction_id.HasValue)
                ? JSON(false)
                : JSON(AuctionRepository.SetImageAsDefault(auction_id.Value, image_id.Value));
        }

        //ResortAuctionImages
        [VauctionAuthorize, HttpPost]
        public JsonResult ResortAuctionImages(long auction_id)
        {
            return JSON(AuctionRepository.ResortImages(auction_id));
        }

        //CroppImage
        [VauctionAuthorize, HttpPost]
        public JsonResult CropImage(long auction_id, string smallimage, string mediumimage, int X, int Y, int W, int H)
        {
            return JSON(AuctionRepository.CropImage(auction_id, smallimage, mediumimage, X, Y, W, H));
        }

        //GetImagesByAuction
        [AcceptVerbs(HttpVerbs.Post)]
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
        public JsonResult GetImagesByAuction(long? auction_id)
        {
            return (!auction_id.HasValue) ? JSON(false) : JSON(AuctionRepository.GetImagesByAuction(auction_id.Value));
        }

        //AuctionPreview
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
        public ActionResult AuctionPreview(long id)
        {
            ViewData["CurrentAuction"] = AuctionRepository.GetAuction(id);
            ViewData["CurrentAuctionImages"] = AuctionRepository.GetAuctionImages(id);
            return View();
        }

        //AddWinnerToAuction
        [VauctionAuthorize(Roles = "Root")]
        public JsonResult AddWinnerToAuction(long auction_id, long user_id)
        {
            return JSON(AuctionRepository.AddWinnerToAuction(auction_id, user_id));
        }

        //AddRightsToAuction
        [VauctionAuthorize(Roles = "Root")]
        public JsonResult AddRightsToAuction(long auction_id, long user_id)
        {
            return JSON(AuctionRepository.AddRightsToAuction(auction_id, user_id));
        }

        //GetBidsByAuctionID
        [VauctionAuthorize(Roles = "Root"), HttpPost]
        public JsonResult GetBidsByAuctionID(string sidx, string sord, int page, int rows, bool? isfirstload,
            long? auction_id)
        {
            return (!isfirstload.HasValue || isfirstload.Value || !auction_id.HasValue)
                ? Json(false)
                : Json(AuctionRepository.GetBidsByAuctionID(sidx, sord, page, rows, auction_id.Value));
        }

        //GetBidsLogByAuctionID
        [VauctionAuthorize(Roles = "Root"), HttpPost]
        public JsonResult GetBidsLogByAuctionID(string sidx, string sord, int page, int rows, bool? isfirstload,
            long? auction_id)
        {
            return (!isfirstload.HasValue || isfirstload.Value || !auction_id.HasValue)
                ? Json(false)
                : Json(AuctionRepository.GetBidsLogByAuctionID(sidx, sord, page, rows, auction_id.Value));
        }

        [VauctionAuthorize(Roles = "Root"), HttpPost]
        public object PlaceBid(long auction_id, long UserID, decimal Amount, decimal MaxBid, int? Quantity,
            DateTime DateMade, bool IsProxy, string Comments, string IP, string oper, long? id)
        {
            if (Amount > MaxBid)
                return
                    JSON(new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The Amount should be less than the MaxBid"));
            if (Amount < MaxBid) IsProxy = true;
            if (String.IsNullOrEmpty(IP)) IP = Consts.UsersIPAddress;
            //long ID = 0;
            //Int64.TryParse(id, out ID);
            return
                Json(BidRepository.PlaceBid(auction_id, UserID, Amount, MaxBid, Quantity.GetValueOrDefault(1), DateMade,
                    IsProxy, Comments, IP, id.GetValueOrDefault(0)));
        }

        //DeleteBid
        [VauctionAuthorize(Roles = "Root"), HttpPost]
        public object DeleteBid(long id, long auction_id)
        {
            return Json(BidRepository.DeleteBid(id, auction_id));
        }

        //UpdateBidLog
        [VauctionAuthorize(Roles = "Root"), HttpPost]
        public object UpdateBidLog(long auction_id, long UserID, decimal Amount, decimal MaxBid, int? Quantity,
            DateTime DateMade, bool IsProxy, bool IsProxyRaise, string Comments, string IP, string oper, string id)
        {
            if (Amount > MaxBid)
                return
                    Json(new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The Amount should be less than the MaxBid"));
            if (Amount < MaxBid) IsProxy = true;
            if (String.IsNullOrEmpty(IP)) IP = Consts.UsersIPAddress;
            long ID = 0;
            Int64.TryParse(id, out ID);
            return
                Json(BidRepository.UpdateBid(auction_id, UserID, Amount, MaxBid, Quantity.GetValueOrDefault(1), DateMade,
                    IsProxy, Comments, IP, IsProxyRaise, ID));
        }

        //DeleteBidLog
        [VauctionAuthorize(Roles = "Root"), HttpPost]
        public object DeleteBidLog(long id, long auction_id)
        {
            return Json(BidRepository.DeleteBidLog(id, auction_id));
        }

        //GetBidsAmountForAuction
        [VauctionAuthorize(Roles = "Root")]
        public ContentResult GetBidsAmountForAuction(long auction_id)
        {
            return Content(BidRepository.GetBidsAmountForAuction(auction_id));
        }

        //ForSale
        //[VauctionAuthorize(Roles = "Root")]
        //public ActionResult ForSale()
        //{
        //  ViewData["AuctionStatusesList"] = GeneralRepository.GetAuctionStatusesForSearchSelect();
        //  ViewData["CommissionRateList"] = GeneralRepository.GetCommissionRatesForSearchSelect();
        //  return View();
        //}

        //GetDOWList
        [VauctionAuthorize(Roles = "Root"), HttpPost]
        public JsonResult GetDOWList(string sidx, string sord, int page, int rows, long? Auction_ID, int? Status_ID,
            string Category, string Title, decimal? Price, decimal? Shipping, string Consignor, byte? CommRate,
            bool? isfirstload)
        {
            return (!isfirstload.HasValue || isfirstload.Value)
                ? JSON(false)
                : JSON(AuctionRepository.GetDOWList(sidx, sord, page, rows, Auction_ID, Status_ID, Category, Title,
                    Price, Shipping, Consignor, CommRate));
        }

        //GetDOWJSON
        [VauctionAuthorize(Roles = "Root"), HttpPost]
        public JsonResult GetDOWJSON(long? auction_id)
        {
            return JSON((!auction_id.HasValue) ? false : AuctionRepository.GetDOWDataJSON(auction_id.Value));
        }

        //GetItemsForSale
        [VauctionAuthorize(Roles = "Root"), HttpGet]
        public JsonResult GetItemsForSale(string sidx, string sord, int page, int rows, long? Auction_ID, int? Lot,
            string Title, decimal? Price, long? owner_id)
        {
            return (!owner_id.HasValue || owner_id.Value == -1)
                ? JSON(false)
                : JSON(AuctionRepository.GetItemsForSale(sidx, sord, page, rows, Auction_ID, Lot, Title, Price,
                    owner_id.GetValueOrDefault(-1)));
        }

        //UpdateDOW
        [VauctionAuthorize(Roles = "Root"), HttpPost]
        public JsonResult UpdateDOW(string auction)
        {
            return String.IsNullOrEmpty(auction)
                ? Json(false)
                : Json(AuctionRepository.UpdateDOWForm(auction, ModelState));
        }

        //Collections
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
        public ActionResult Collections()
        {
            return View();
        }

        //GetCollections
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
        public JsonResult GetCollections(string sidx, string sord, int page, int rows)
        {
            var collections = GeneralRepository.GetCollections().ToList();

            var pageIndex = Convert.ToInt32(page) - 1;
            var pageSize = rows;
            var totalRecords = collections.Count();
            var totalPages = (int) Math.Ceiling((float) totalRecords/(float) pageSize);

            collections = collections.Skip(pageIndex*pageSize).Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from query in collections
                    select new
                    {
                        i = query.ID,
                        cell = new string[]
                        {
                            query.ID.ToString(),
                            query.Title,
                            query.WebTitle,
                            query.Description
                        }
                    }).ToArray()
            };
            return JSON(jsonData);
        }

        //AddCollection    
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpPost]
        public JsonResult AddCollection(string title, string webTitle, string description)
        {
            try
            {
                GeneralRepository.UpdateCollection(new Collection
                {
                    Title = title,
                    WebTitle = webTitle,
                    Description = description
                });
            }
            catch (Exception ex)
            {
                return JSON(new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message));
            }
            return JSON(new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS));
        }

        //UpdateCollection    
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpPost]
        public JsonResult UpdateCollection(string id, string webTitle, string title, string description)
        {
            try
            {
                GeneralRepository.UpdateCollection(new Collection
                {
                    ID = Convert.ToInt64(id),
                    Title = title,
                    Description = description,
                    WebTitle = webTitle
                });
            }
            catch (Exception ex)
            {
                return JSON(new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message));
            }
            return JSON(new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS));
        }

        //DeleteCollection    
        [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpPost]
        public JsonResult DeleteCollection(string id)
        {
            if (string.IsNullOrEmpty(id)) return JSON(false);
            try
            {
                foreach (var s in id.Split(','))
                {
                    GeneralRepository.DeleteCollection(Convert.ToInt64(s));
                }
            }
            catch (Exception ex)
            {
                return JSON(new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message));
            }
            return JSON(new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS));
        }
    }
}