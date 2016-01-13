using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Data.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Globalization;
using System.Transactions;
using System.Text;
using System.Text.RegularExpressions;
using Vauction.Utils;
using Vauction.Utils.Helper;
using Vauction.Utils.Lib;
using Vauction.Models.CustomClasses;
using Vauction.Utils.Perfomance;

namespace Vauction.Models
{
    public class AuctionRepository : IAuctionRepository
    {
        #region init
        private VauctionDataContext dataContext;
        private ICacheDataProvider CacheRepository;

        public AuctionRepository(VauctionDataContext dataContext, ICacheDataProvider cacheRepository)
        {
            this.dataContext = dataContext;
            CacheRepository = cacheRepository;
        }
        #endregion

        #region consignor auction listing
        //GetConsignmentStatetion
        public long GetConsignmentStatement(long user_id, long event_id)
        {
            Consignment cons = dataContext.Consignments.SingleOrDefault(C => C.User_ID == user_id && C.Event_ID == event_id);
            return (cons == null) ? -1 : cons.ID;
        }

        //GetConsignment
        public Consignment GetConsignment(long consignmentID)
        {
            return dataContext.Consignments.FirstOrDefault(t => t.ID == consignmentID);
        }

        //GetConsignmentContract
        public ConsignmentContract GetConsignmentContract(long consignmentID)
        {
            return dataContext.ConsignmentContracts.FirstOrDefault(t => t.ConsignmentID == consignmentID);
        }

        //UpdateConsignmentContract
        public ConsignmentContract UpdateConsignmentContract(ConsignmentContract cc)
        {
            ConsignmentContract consignmentContract = dataContext.ConsignmentContracts.FirstOrDefault(t => t.ConsignmentID == cc.ConsignmentID);
            if (consignmentContract == null)
            {
                consignmentContract = new ConsignmentContract { ConsignmentID = cc.ConsignmentID };
                dataContext.ConsignmentContracts.InsertOnSubmit(consignmentContract);
            }
            consignmentContract.UpdateFields(cc);
            GeneralRepository.SubmitChanges(dataContext);
            return consignmentContract;
        }

        //GetSpecialist
        public Specialist GetSpecialist(long specialistID)
        {
            return dataContext.Specialists.FirstOrDefault(t => t.ID == specialistID);
        }

        //GetConsignorsList
        public object GetConsignorsList(string sidx, string sord, int page, int rows, long? consignment_id, long? user_id, string login, string flname, string email, long? event_id, DateTime? consdate, string specialist)
        {
            int pageindex = (page > 0) ? page - 1 : 0;
            int? totalRecords = 0;
            flname = String.IsNullOrEmpty(flname) ? String.Empty : flname.Replace(" ", "%");
            specialist = String.IsNullOrEmpty(specialist) ? String.Empty : specialist.Replace(" ", "%");
            var result = dataContext.spAuction_View_Consignments(consignment_id, user_id, login, flname, email, event_id, consdate.HasValue ? consdate.Value.ToShortDateString() : String.Empty, specialist, sidx.ToLower(), sord, pageindex, rows, ref totalRecords);
            if (totalRecords.GetValueOrDefault(0) == 0) return new { total = 0, page = page, records = 0 };
            return new
            {
                total = (int)Math.Ceiling((float)totalRecords.Value / (float)rows),
                page = page,
                records = totalRecords,
                rows = (
                    from query in result
                    select new
                    {
                        i = query.Consignment_ID,
                        cell = new string[] {
                query.Consignment_ID.ToString(), 
                query.User_ID.ToString(),
                query.Login,
                query.FLName,
                query.Email,               
                query.Event,
                query.ConsDate.ToString(),
                query.Specialist
              }
                    }).ToArray()
            };
        }

        //UpdateConsignorStatementForm
        public object UpdateConsignorStatementForm(string cons, ModelStateDictionary ModelState)
        {
            long co_id = 0;
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ConsignorStatementForm c = serializer.Deserialize<ConsignorStatementForm>(cons);

                c.Validate(ModelState);

                if (ModelState.IsValid)
                {
                    if ((co_id = GetConsignmentStatement(c.User_ID, c.Event_ID)) > 0 && c.ID == 0)
                        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The consignor statement for this seller and this event already exists (Cons#" + co_id.ToString() + ")");

                    if (!UpdateConsignorStatement(c))
                        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The user's information wasn't saved.");
                    co_id = c.ID;
                }
                else
                {
                    ModelState.Remove("cons");
                    var errors = (from M in ModelState select new { field = M.Key, message = M.Value.Errors.FirstOrDefault().ErrorMessage }).ToArray();
                    return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "Please correct the errors and try again.", errors);
                }
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, "", co_id);
        }

        //UpdateConsignorStatement
        public bool UpdateConsignorStatement(ConsignorStatementForm info)
        {
            Consignment cons = (info.ID > 0) ? GetConsignment(info.ID) : null;
            bool newCons = cons == null;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    if (newCons)
                    {
                        cons = new Consignment();
                        dataContext.Consignments.InsertOnSubmit(cons);
                    }
                    cons.ConsDate = info.ConsDate;
                    cons.Event_ID = info.Event_ID;
                    cons.Specialist_ID = info.Specialist_ID;
                    cons.User_ID = info.User_ID;

                    GeneralRepository.SubmitChanges(dataContext);
                    ts.Complete();
                    info.ID = cons.ID;
                }
            }
            catch (ChangeConflictException cce)
            {
                Logger.LogException(cce);
                throw cce;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                throw ex;
            }
            return true;
        }

        //DeleteConsignorStatement
        public JsonExecuteResult DeleteConsignorStatement(long cons_id)
        {
            Consignment cons = GetConsignment(cons_id);
            if (cons == null) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The consignot statement doesn't exist. Operation failed");
            int count = dataContext.Auctions.Where(A => A.Owner_ID == cons.User_ID && A.Event_ID == cons.Event_ID).Count();
            if (count > 0)
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't delete this consignor statement because it is not empty.");
            long ID = cons.ID;
            try
            {
                dataContext.Consignments.DeleteOnSubmit(cons);
                GeneralRepository.SubmitChanges(dataContext);
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, ID);
        }

        //CheckAndAddConsignment
        public JsonExecuteResult AddConsignment(long user_id, long event_id)
        {
            try
            {
                Consignment cons = dataContext.Consignments.SingleOrDefault(C => C.User_ID == user_id && C.Event_ID == event_id);
                if (cons != null) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The consignor statement already exists.");
                cons = new Consignment();
                dataContext.Consignments.InsertOnSubmit(cons);
                cons.User_ID = user_id;
                cons.Event_ID = event_id;
                cons.ConsDate = DateTime.Now;
                Specialist sp = dataContext.Specialists.SingleOrDefault(S => S.FirstName == "Other" && S.IsActive);
                cons.Specialist_ID = (sp != null) ? sp.ID : dataContext.Specialists.FirstOrDefault(S => sp.IsActive).ID;
                GeneralRepository.SubmitChanges(dataContext);
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //GetConsingorStatementDataJSON
        public object GetConsingorStatementDataJSON(long consignmentID)
        {
            Consignment consignment = GetConsignment(consignmentID);
            if (consignment == null) return null;
            ConsignmentContract consignmentContract = GetConsignmentContract(consignmentID);
            if (consignmentContract == null)
            {
                string filePath = DiffMethods.TemplateDifferentOnDisk("ConsignmentContract.txt");
                FileInfo fileInfo = new FileInfo(filePath);
                string contractTemplate = string.Empty;
                if (fileInfo.Exists)
                {
                    contractTemplate = File.ReadAllText(filePath);
                    decimal buyerFee = consignment.Event.BuyerFee.GetValueOrDefault(0).GetPrice();
                    string buyerFeeStr = buyerFee.ToString();
                    if (buyerFee == (int)buyerFee)
                    {
                        buyerFeeStr = buyerFeeStr.Remove(buyerFeeStr.Length - 3);
                    }
                    contractTemplate = contractTemplate.Replace("{{BuyersPremium}}", string.Format("{0}%", buyerFeeStr)).Replace("{{CommissionRate}}", consignment.User.CommissionRate.LongDescription);
                }
                consignmentContract = new ConsignmentContract();
                consignmentContract.UpdateFields(consignment.ID, (int)Consts.ConsignmentContractStatus.NotGenerated, contractTemplate, string.Empty);
                consignmentContract = UpdateConsignmentContract(consignmentContract);
            }
            var jsonData = new
            {
                id = consignment.ID,
                e = consignment.Event_ID,
                s = consignment.Specialist_ID,
                u = consignment.User_ID,
                u_t = consignment.User.Login + " (" + consignment.User.AddressCard_Billing.FirstName + " " + consignment.User.AddressCard_Billing.LastName + ")",
                c = consignment.User.CommissionRate_ID,
                cd = consignment.ConsDate.ToString("MM/dd/yyyy hh:mm tt"),
                ccs = consignmentContract.StatusID,
                ccst = ((Consts.ConsignmentContractStatus)consignmentContract.StatusID).ToString(),
                cct = consignmentContract.ContractText
            };
            return jsonData;
        }

        //GetConsingorStatementDataJSON
        public object GetConsingorStatementByAyctionDataJSON(long auction_id)
        {
            Consignment cons = (from A in dataContext.Auctions
                                join C in dataContext.Consignments on A.Owner_ID equals C.User_ID
                                where C.Event_ID == A.Event_ID && A.ID == auction_id
                                select C).SingleOrDefault();
            if (cons == null) return false;
            return GetConsingorStatementDataJSON(cons.ID);
        }

        //GetUnsoldItemsList
        public object GetConsignorStatementUnsoldItems(string sidx, string sord, int page, int rows, long cons_id)
        {
            int pageindex = (page > 0) ? page - 1 : 0;
            int? totalrecords = 0;
            var result = dataContext.spAuction_View_UnsoldItems(cons_id, pageindex, rows, ref totalrecords);
            if (totalrecords.GetValueOrDefault(0) == 0) return new { total = 0, page = page, records = 0 };
            return new
            {
                total = (int)Math.Ceiling((float)totalrecords.Value / (float)rows),
                page = page,
                records = totalrecords.GetValueOrDefault(0),
                rows = (
                    from query in result
                    select new
                    {
                        i = query.Auction_ID,
                        cell = new string[] {
                query.Auction_ID.ToString(),
                (query.Lot.HasValue)?query.Lot.Value.ToString():String.Empty,
                query.Title,
                ((Consts.AuctionStatus)query.AuctionStatus).ToString(),
                query.Price.GetCurrency(false),
                query.Reserve.GetCurrency(false),
                query.Estimate,
                query.CurrentBid.HasValue?query.CurrentBid.GetCurrency(false):String.Empty,
                query.Bids.HasValue?query.Bids.Value.ToString():String.Empty
              }
                    }).ToArray()
            };
        }
        #endregion

        #region get auction list
        //GetAuctionsListByConsignor
        public object GetAuctionsListByConsignor(string sidx, string sord, int page, int rows, long cons_id)
        {
            int pageindex = (page > 0) ? page - 1 : 0;
            int? totalRecords = 0;
            var result = dataContext.spAuction_View_ConsignmentAuctions(cons_id, sidx.ToLower() == "auction_id", pageindex, rows, ref totalRecords);
            if (totalRecords.GetValueOrDefault(0) == 0) return new { total = 0, page = page, records = 0 };
            return new
            {
                total = (int)Math.Ceiling((float)totalRecords.Value / (float)rows),
                page = page,
                records = totalRecords,
                rows = (
                    from query in result
                    select new
                    {
                        i = query.Auction_ID.Value,
                        cell = new string[] {
                query.Auction_ID.Value.ToString(),
                (query.Lot.HasValue)?query.Lot.Value.ToString():String.Empty,
                query.Title,
                query.Description,
                query.FullCategoy,
                query.Price.GetCurrency(false),
                query.Quantity.HasValue?query.Quantity.Value.ToString():String.Empty,
                query.CommDescription,
                query.LOA.GetValueOrDefault(false).ToString(),
                query.CopyNotes,
                query.PhotoNotes,
                query.PriorityDescription,
                query.Old_Inventory.HasValue?query.Old_Inventory.Value.ToString():String.Empty,
                query.EnteredBy,
                query.ListingStepDescription,
                query.CommRate_ID.HasValue?query.CommRate_ID.Value.ToString():String.Empty,
                query.MainCategory_ID.HasValue?query.MainCategory_ID.Value.ToString() : String.Empty,
                query.Category_ID.HasValue?query.Category_ID.Value.ToString():String.Empty,
                query.Priority.HasValue?query.Priority.Value.ToString() : String.Empty,
                query.Category,
                query.Price.GetValueOrDefault(0).GetPrice().ToString(),
                Convert.ToByte(query.IsAuctionPrinter.GetValueOrDefault(false)).ToString(),
                query.Old_Inventory_Title,
                query.ListingStep.ToString(),
                query.IsLimitDisabled.GetValueOrDefault(false)?"1":"0",
                query.PurchasedWay.GetValueOrDefault(-1).ToString(),
                (!query.PurchasedPrice.HasValue ? String.Empty : query.PurchasedPrice.GetValueOrDefault(0).GetPrice().ToString()),
                query.SoldWay.GetValueOrDefault(-1).ToString(),
                (!query.SoldPrice.HasValue ? String.Empty : query.SoldPrice.GetValueOrDefault(0).GetPrice().ToString())
              }
                    }).ToArray()
            };
        }

        //GetAuctionsListByConsignor    
        public List<IdTitleValue> GetAuctionsListByConsignor(long consignmentID)
        {
            int? totalRecords = 0;
            return dataContext.spAuction_View_ConsignmentAuctions(consignmentID, true, 0, 999999, ref totalRecords).OrderBy(t => t.Lot.GetValueOrDefault(9999)).ThenBy(t => t.Title).Select(t => new IdTitleValue { ID = t.Auction_ID.GetValueOrDefault(0), Title = string.Format("{0}{1}", t.Lot.HasValue ? string.Format("Lot {0}. ", t.Lot.Value) : string.Empty, t.Title), Value = t.Price.GetPrice() }).ToList();
        }

        //GetAuctionsListByConsignor
        public object GetAuctionsListByEvent(string sidx, string sord, int page, int rows, long event_id, long? auction_id, string title, byte? priority)
        {
            int pageindex = (page > 0) ? page - 1 : 0;
            int? totalRecords = 0;
            var result = dataContext.spAuction_View_AuctionsForLS(event_id, auction_id, title, priority, sidx.ToLower(), sord.ToLower(), pageindex, rows, ref totalRecords);
            if (totalRecords.GetValueOrDefault(0) == 0) return new { total = 0, page = page, records = 0 };
            return new
            {
                total = (int)Math.Ceiling((float)totalRecords.Value / (float)rows),
                page = page,
                records = totalRecords,
                rows = (
                    from query in result
                    select new
                    {
                        i = query.Auction_ID.Value,
                        cell = new string[] {
                query.Auction_ID.Value.ToString(),
                query.Title,
                query.PriorityDescription,
                query.ListedStep.HasValue && query.ListedStep.Value>=1?"1":"0",
                query.Priority.GetValueOrDefault(0).ToString(),
                query.Description,
                query.CopyNotes,
                (query.ListedStep<2).ToString(),
                query.IsLimitDisabled.GetValueOrDefault(false)?"1":"0"
              }
                    }).ToArray()
            };
        }

        //GetAuctionsList
        public object GetAuctionsList(string sidx, string sord, int page, int rows, long? auction_id, long? oldinventory, short? lot, string title, string Event, long? owner_id)
        {
            int pageindex = (page > 0) ? page - 1 : 0;
            int? totalRecords = 0;
            var result = dataContext.spAuction_View_SearchList(owner_id, auction_id, oldinventory, lot, title, Event, pageindex, rows, ref totalRecords);
            if (totalRecords.GetValueOrDefault(0) == 0) return new { total = 0, page = page, records = 0 };
            return new
            {
                total = (int)Math.Ceiling((float)totalRecords.Value / (float)rows),
                page = page,
                records = totalRecords,
                rows = (
                    from query in result
                    select new
                    {
                        i = query.Auction_ID.GetValueOrDefault(0),
                        cell = new string[] {
                query.Auction_ID.GetValueOrDefault(0).ToString(),
                query.OldAuction_ID.HasValue?query.OldAuction_ID.Value.ToString():String.Empty,
                query.Lot.HasValue?query.Lot.Value.ToString():String.Empty,
                query.Title,
                query.EventTitle,
                query.MainCategory_ID.GetValueOrDefault(0).ToString(),
                query.Category_ID.GetValueOrDefault(0).ToString(),
                query.Category,
                query.CommissionRate_ID.GetValueOrDefault(0).ToString(),
                query.Quantity.GetValueOrDefault(0).ToString(),
                query.Priority.GetValueOrDefault(1).ToString(),
                Convert.ToByte(query.LOA.GetValueOrDefault(false)).ToString(),
                query.Price.GetValueOrDefault(0).GetPrice().ToString(),
                query.CopyNotes,
                query.PhotoNotes,
                query.Description,
                query.IsLimitDisabled.GetValueOrDefault(false).ToString(),
                query.Reserve.GetValueOrDefault(0).GetPrice().ToString(),
                query.Estimate,
                query.Shipping.GetValueOrDefault(0).GetPrice().ToString()
              }
                    }).ToArray()
            };
        }

        #endregion

        //UpdateDescription
        public object UpdateDescription(long auction_id, string descr, string cnotes, bool iswritten)
        {
            try
            {
                Auction auc = GetAuction(auction_id);
                if (auc == null) throw new Exception("The auction doesn't exist.");
                if (auc.Status != (byte)Consts.AuctionStatus.Pending && auc.Status != (byte)Consts.AuctionStatus.PulledOut)
                    throw new Exception("You can't update the description and copy notes for this lot.");
                auc.Description = descr;
                auc.CopyNotes = cnotes;
                if (auc.ListedStep < 2) auc.ListedStep = Convert.ToByte(iswritten);
                GeneralRepository.SubmitChanges(dataContext);
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        #region for sale (dow)
        //GetDOWList
        public object GetDOWList(string sidx, string sord, int page, int rows, long? auction_id, int? status_id, string category, string title, decimal? price, decimal? shipping, string consignor, byte? commrate)
        {
            int pageindex = (page > 0) ? page - 1 : 0;
            int? totalRecords = 0;
            title = String.IsNullOrEmpty(title) ? String.Empty : title.Replace(" ", "%");
            category = String.IsNullOrEmpty(category) ? String.Empty : category.Replace(" ", "%");
            consignor = String.IsNullOrEmpty(consignor) ? String.Empty : consignor.Replace(" ", "%");
            var result = dataContext.spAuction_ForSaleList(auction_id, status_id, category, title, price, shipping, consignor, commrate, pageindex, rows, ref totalRecords);
            if (totalRecords.GetValueOrDefault(0) == 0) return new { total = 0, page = page, records = 0 };
            return new
            {
                total = (int)Math.Ceiling((float)totalRecords.Value / (float)rows),
                page = page,
                records = totalRecords,
                rows = (
                    from query in result
                    select new
                    {
                        i = query.Auction_ID.GetValueOrDefault(0),
                        cell = new string[] {
                query.Auction_ID.GetValueOrDefault(0).ToString(),
                query.Status,
                query.Category,
                query.Title,
                query.Reserve.GetValueOrDefault(0).GetCurrency(false),                
                query.Shipping.GetValueOrDefault(0).GetCurrency(false),
                query.Consignor,
                query.CommRate                
              }
                    }).ToArray()
            };
        }

        //GetDOWDataJSON
        public object GetDOWDataJSON(long auction_id)
        {
            Auction auc = GetAuction(auction_id);
            if (auc == null) return null;
            var jsonData = new
            {
                id = auc.ID,
                s = auc.Status,
                t = auc.Title,
                d = auc.Description,
                p = auc.Price,
                sh = (auc.Shipping.HasValue) ? auc.Shipping.Value : 0,
                old = (auc.OldAuction_ID.HasValue) ? auc.Auction_Parent.ID.ToString() : String.Empty,
                old_n = (auc.OldAuction_ID.HasValue) ? auc.Auction_Parent.Title : String.Empty,
                o = auc.Owner_ID,
                o_n = auc.User.UserLoginName,
                cr = auc.CommissionRate_ID,
                add = auc.Addendum,
                cat = auc.EventCategory.Category_ID,
                cat_n = auc.EventCategory.Category.Title,
                mcat = auc.EventCategory.MainCategory_ID
            };
            return jsonData;
        }

        //GetItemsForSale
        public object GetItemsForSale(string sidx, string sord, int page, int rows, long? auction_id, int? lot, string title, decimal? price, long owner_id)
        {
            int pageindex = (page > 0) ? page - 1 : 0;
            int? totalRecords = 0;
            title = String.IsNullOrEmpty(title) ? String.Empty : title.Replace(" ", "%");
            var result = dataContext.spAuction_ItemsForSale(auction_id, lot, title, price, owner_id, pageindex, rows, ref totalRecords);
            if (totalRecords.GetValueOrDefault(0) == 0) return new { total = 0, page = page, records = 0 };
            return new
            {
                total = (int)Math.Ceiling((float)totalRecords.Value / (float)rows),
                page = page,
                records = totalRecords,
                rows = (
                    from query in result
                    select new
                    {
                        i = query.Auction_ID.GetValueOrDefault(0),
                        cell = new string[] {
                query.Auction_ID.GetValueOrDefault(0).ToString(),
                query.Lot.HasValue?query.Lot.Value.ToString():String.Empty,                
                query.Title,
                query.Price.GetValueOrDefault(0).GetPrice().ToString(),
                query.Shipping.GetValueOrDefault(0).GetPrice().ToString(),
                query.Description,
                query.MainCategory_ID.GetValueOrDefault(0).ToString(),
                query.Category_ID.GetValueOrDefault(0).ToString(),
                query.Category,
                query.Addendum,
                query.CommissionRate_ID.GetValueOrDefault(0).ToString()
              }
                    }).ToArray()
            };
        }

        //UpdateDOWForm
        public JsonExecuteResult UpdateDOWForm(string auction, ModelStateDictionary ModelState)
        {
            long auction_id = 0;
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                DOWForm auc = serializer.Deserialize<DOWForm>(auction);

                auc.Validate(ModelState);

                if (ModelState.IsValid)
                {
                    if (!UpdateDOW(auc))
                        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The data wasn't saved.");
                    auction_id = auc.ID;
                }
                else
                {
                    ModelState.Remove("auction");
                    var errors = (from M in ModelState select new { field = M.Key, message = M.Value.Errors.FirstOrDefault().ErrorMessage }).ToArray();
                    return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "Please correct the errors and try again.", errors);
                }
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, "", auction_id);
        }

        //UpdateDOW
        public bool UpdateDOW(DOWForm info)
        {
            Auction old = (info.OldAuction_ID.HasValue) ? GetAuction(info.OldAuction_ID.Value) : null;
            bool IsCopy = (info.OldAuction_ID.HasValue && old != null);
            Event evnt = dataContext.Events.Where(E => E.ID == 0).FirstOrDefault();
            bool IsNew = false;
            try
            {
                Auction auction = GetAuction(info.ID);
                if (IsNew = (auction == null))
                {
                    auction = new Auction();
                    dataContext.Auctions.InsertOnSubmit(auction);
                    auction.NotifiedOn = DateTime.Now;
                }
                auction.AuctionType_ID = (byte)Consts.AuctionType.DealOfTheWeek;
                auction.Event_ID = 0;
                auction.Owner_ID = info.Owner_ID.HasValue ? info.Owner_ID.Value : AppHelper.CurrentUser.ID;
                auction.CommissionRate_ID = info.CommissionRate_ID;
                auction.OldAuction_ID = (!IsCopy) ? info.OldAuction_ID : old.ID;
                auction.Price = info.Price.Value;
                auction.Reserve = info.Price;
                auction.Status = info.Status_ID;
                auction.Title = info.Title;
                auction.Shipping = info.Shipping;
                auction.Description = info.Description;
                auction.Addendum = info.Addendum;
                info.MainCategory_ID = IsCopy ? old.EventCategory.MainCategory_ID : 0;
                EventCategory ec = dataContext.EventCategories.Where(EC => EC.Event_ID == 0 && EC.MainCategory_ID == info.MainCategory_ID && EC.Category_ID == info.Category_ID).FirstOrDefault();
                if (ec == null)
                {
                    ec = new EventCategory();
                    ec.Event_ID = 0;
                    ec.MainCategory_ID = info.MainCategory_ID;
                    ec.IsActive = false;
                    ec.Owner_ID = AppHelper.CurrentUser.ID;
                    ec.Category_ID = info.Category_ID;
                    ec.Priority = 1;
                    ec.LastUpdate = DateTime.Now;
                    dataContext.EventCategories.InsertOnSubmit(ec);
                }
                auction.EventCategory = ec;
                auction.Cost = 0;
                auction.StartDate = evnt.DateStart;
                auction.EndDate = evnt.DateEnd;
                auction.Quantity = 1;
                auction.Estimate = String.Empty;
                auction.ListedStep = 0;
                auction.Priority = 1;
                auction.LOA = false;
                auction.PulledOut = false;
                auction.IsUnsold = false;
                auction.IsCatalog = false;

                auction.IsPrinted = false;
                auction.CopyNotes = String.Empty;
                auction.PhotoNotes = String.Empty;
                auction.LastUpdate = DateTime.Now;
                auction.IsBold = false;
                auction.IsFeatured = false;
                auction.IsPhotographed = false;
                auction.IsInLayout = false;
                auction.IsLimitDisabled = false;

                if (IsCopy && IsNew)
                {
                    List<Image> imgs = dataContext.Images.Where(I => I.Auction_ID == auction.ID).ToList();
                    if (imgs != null && imgs.Count() > 0)
                        dataContext.Images.DeleteAllOnSubmit(imgs);
                }
                GeneralRepository.SubmitChanges(dataContext);
                if (IsCopy && old.Images.Count() > 0 && IsNew)
                {
                    Image i1;
                    foreach (Image i2 in old.Images)
                    {
                        i1 = (!IsNew) ? auction.Images.FirstOrDefault(I => I.PicturePath == i2.PicturePath) : null;
                        if (i1 != null) continue;
                        i1 = i1 ?? new Image();
                        i1.Auction_ID = auction.ID;
                        i1.Default = i2.Default;
                        i1.PicturePath = i2.PicturePath;
                        i1.ThumbNailPath = i2.ThumbNailPath;
                        i1.LargePath = i2.LargePath;
                        i1.Order = i2.Order;
                        i1.UploadedFileName = i2.UploadedFileName;
                        i1.isChecked = i2.isChecked;
                        dataContext.Images.InsertOnSubmit(i1);
                        auction.Images.Add(i1);
                    }
                    GeneralRepository.SubmitChanges(dataContext);
                }
                if (IsCopy && !DiffMethods.CopyImages(old.ID, auction.ID))
                    Logger.LogException(new Exception("Images don't exist for auction " + old.ID.ToString()));
                if (IsNew)
                {
                    if (IsCopy && old.Event_ID > 0)
                        AddConsignment(auction.Owner_ID, old.Event_ID);
                    else
                        AddConsignment(auction.Owner_ID, auction.Event_ID);
                    MoveImagesToAction(auction.ID);
                }
            }
            catch (ChangeConflictException cce)
            {
                Logger.LogException(cce);
                throw cce;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                throw ex;
            }
            return true;
        }
        #endregion


        // NOT DONE

        public Auction GetAuction(long auction_id)
        {
            return dataContext.Auctions.SingleOrDefault(A => A.ID == auction_id);
        }

        //GetBidLogList
        public object GetBidLogList(string sidx, string sord, int page, int rows, long event_id)
        {
            List<BidLog> bidlogs = new List<BidLog>((from BL in dataContext.BidLogs
                                                     where (BL.Auction.Event_ID == event_id || event_id <= 0) && (BL.Auction.Status == (byte)Consts.AuctionStatus.Open || BL.Auction.Status == (byte)Consts.AuctionStatus.Closed)
                                                     select BL).OrderBy(sidx + " " + sord));

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = bidlogs.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            bidlogs = new List<BidLog>(bidlogs.Skip(pageIndex * pageSize).Take(pageSize));

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from query in bidlogs
                    select new
                    {
                        i = query.ID,
                        cell = new string[] { query.ID.ToString(), query.Auction.Event.Title, (query.Auction.Lot.HasValue) ? query.Auction.Lot.Value.ToString() : String.Empty, query.User.UserNameShort, "$ " + query.Amount.ToString("0.00"), "$ " + query.MaxBid.ToString("0.00"), query.Quantity.ToString(), query.DateMade.Date.ToString("d") + " " + query.DateMade.ToString("hh:mm:ss.fff", CultureInfo.InvariantCulture), query.IP, (query.IsProxy) ? "yes" : "no", (query.IsProxyRaise) ? "yes" : "no", "no", String.Empty, String.Empty }
                    }).ToArray()
            };

            return jsonData;
        }

        //GetBidLogListEventManagment
        public object GetBidLogListEventManagment(string sidx, string sord, int page, int rows, long event_id)
        {
            //DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.BIDS, "GETBIDLOGLISTEVENTMANAGMENT", new object[] { sidx, sord, page, rows, event_id }, CachingExpirationTime.Seconds_30);
            //var res = CacheRepository.Get(dco);
            //if (res != null) return res;

            int pageindex = (page > 0) ? page - 1 : 0;
            int? totalRecords = 0;
            var result = dataContext.spBid_View_BidLog(event_id, pageindex, rows, ref totalRecords);
            if (totalRecords.GetValueOrDefault(0) == 0) return new { total = 0, page, records = 0 };

            var statistics = dataContext.spBid_View_BidLogStatistics(event_id).FirstOrDefault() ?? new spBid_View_BidLogStatisticsResult();

            return new
            {
                total = (int)Math.Ceiling((float)totalRecords.Value / (float)rows),
                page = page,
                records = totalRecords,
                userdata = (new { totalbid = (statistics.BidsAmount.HasValue ? statistics.BidsAmount.Value.ToString() : "---"), totalbidamount = (statistics.BidAmount.HasValue ? (statistics.BidAmount.Value.GetCurrency()) : "---"), buyers = (statistics.BuyersPremium.HasValue ? (statistics.BuyersPremium.Value.GetCurrency()) : "---"), total = (statistics.BuyersPremium.HasValue ? ((statistics.BuyersPremium.Value + statistics.BidAmount.GetValueOrDefault(0)).GetCurrency()) : "---") }),
                rows = (
                    from query in result
                    select new
                    {
                        i = query.Bid_ID.GetValueOrDefault(-1),
                        cell = new[] {
                query.Bid_ID.ToString(), 
                query.Lot.GetValueOrDefault(0).ToString(),
                query.Title, 
                query.Bidder,
                query.Amount.GetValueOrDefault(0).GetCurrency(),
                query.MaxBid.GetValueOrDefault(0).GetCurrency(),
                query.DateMade.GetValueOrDefault(DateTime.Now).Date.ToString("d") + " " + query.DateMade.GetValueOrDefault(DateTime.Now).ToString("hh:mm:ss.fff", CultureInfo.InvariantCulture),
                query.IsProxy.GetValueOrDefault(false) ? "yes" : "no",
                query.IsProxyRaise.GetValueOrDefault(false) ? "yes" : "no"
              }
                    }).ToArray()
            };

            //dco.Data = res;
            //CacheRepository.Add(dco);

            //return res;

            //dataContext.CommandTimeout = 10 * 60 * 1000;
            //Event evnt = dataContext.Events.SingleOrDefault(E => E.ID == event_id);

            //IQueryable<IBidLog> bidlogs = (evnt.IsCurrent) ?
            //  ((from BL in dataContext.BidLogCurrents
            //    where (BL.Auction.Event_ID == event_id || event_id <= 0) && (BL.Auction.Status == (byte)
            //    Consts.AuctionStatus.Open || BL.Auction.Status == (byte)Consts.AuctionStatus.Closed)
            //    select BL).OrderBy(sidx + " " + sord)).Cast<IBidLog>() :
            //  ((from BL in dataContext.BidLogs
            //    where (BL.Auction.Event_ID == event_id || event_id <= 0) && (BL.Auction.Status == (byte)
            //    Consts.AuctionStatus.Open || BL.Auction.Status == (byte)Consts.AuctionStatus.Closed)
            //    select BL).OrderBy(sidx + " " + sord)).Cast<IBidLog>();

            //if (bidlogs.Count() == 0) return new { total = 0, page = 0, records = 0, userdata = (new { totalbid = "0", totalbidamount = "0.00", buyers = "0.00", total = "0.00" }), rows = new { } };

            //int pageIndex = Convert.ToInt32(page) - 1;
            //int pageSize = rows;
            //int totalRecords = bidlogs.Count();
            //int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            //int? totalbid = bidlogs.Where(bl => DateTime.Now.AddMinutes(-10).CompareTo(bl.DateMade) <= 0).Count();

            //bidlogs = bidlogs.Skip(pageIndex * pageSize).Take(pageSize);

            //decimal? totalbidamount = (evnt.IsCurrent) ?
            //  (from B in dataContext.BidCurrents
            //   join A in dataContext.Auctions on B.Auction_ID equals A.ID
            //   where (A.Event_ID == event_id || event_id <= 0) &&
            //        (A.Status == (byte)Consts.AuctionStatus.Open || A.Status == (byte)Consts.AuctionStatus.Closed)
            //   group B by B.Auction_ID into bids
            //   select new { Auction_ID = bids.Key, Amount = bids.Max(b => b.Amount) }).Sum(a => a.Amount) :
            //  (from B in dataContext.Bids
            //   join A in dataContext.Auctions on B.Auction_ID equals A.ID
            //   where (A.Event_ID == event_id || event_id <= 0) && (A.Status == (byte)Consts.AuctionStatus.Open || A.Status == (byte)Consts.AuctionStatus.Closed)
            //   group B by B.Auction_ID into bids
            //   select new { Auction_ID = bids.Key, Amount = bids.Max(b => b.Amount) }).Sum(a => a.Amount);

            //decimal buyers = (totalbidamount.HasValue) ? totalbidamount.Value * Convert.ToDecimal(evnt.BuyerFee.Value / 100) : (decimal)0;

            //decimal total = buyers + ((totalbidamount.HasValue) ? totalbidamount.Value : 0);


            //var jsonData = new
            //{
            //  total = totalPages,
            //  page = page,
            //  records = totalRecords,
            //  userdata = (new { totalbid = ((totalbid.HasValue) ? totalbid.Value.ToString() : "---"), totalbidamount = ((totalbidamount.HasValue) ? (totalbidamount.Value.GetCurrency()) : "---"), buyers = buyers.GetCurrency(), total = total.GetCurrency() }),
            //  rows = (
            //      from query in bidlogs.ToList()
            //      select new
            //      {
            //        i = query.ID,
            //        cell = new string[] { 
            //          query.ID.ToString(), 
            //          evnt.IsCurrent ? (query as BidLogCurrent).Auction.Lot.ToString() : (query as BidLog).Auction.Lot.ToString(),
            //          evnt.IsCurrent ? (query as BidLogCurrent).Auction.Title : (query as BidLog).Auction.Title, 
            //          evnt.IsCurrent ? (query as BidLogCurrent).User.UserLoginName : (query as BidLog).User.UserLoginName, 
            //          "$ " + query.Amount.ToString("0.00"), 
            //          "$ " + query.MaxBid.ToString("0.00"), 
            //          query.DateMade.Date.ToString("d") + " " + query.DateMade.ToString("hh:mm:ss.fff", CultureInfo.InvariantCulture), (query.IsProxy) ? "yes" : "no", (query.IsProxyRaise) ? "yes" : "no" }
            //      }).ToArray()
            //};

            //bidlogs = null;

            //return jsonData;
        }

        //GetShortDetailedList
        public object GetShortDetailedList(string sidx, string sord, int page, int rows, bool _search, long? lot, string title, decimal? currentbid, int? bidnumber, long event_id)
        {
            //DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.BIDS, "GETSHORTDETAILEDLIST", new object[] { sidx, sord, page, rows, _search, lot, title, currentbid, bidnumber, event_id }, CachingExpirationTime.Seconds_30);
            //var res = CacheRepository.Get(dco);
            int pageindex = (page > 0) ? page - 1 : 0;
            int? totalRecords = 0;
            title = String.IsNullOrEmpty(title) ? String.Empty : title.Replace(" ", "%");

            //if (res != null) return res;

            var result = dataContext.spBid_View_AuctionBids(event_id, lot, title, currentbid, bidnumber, sidx, sord == "desc", pageindex, rows, ref totalRecords);
            if (totalRecords.GetValueOrDefault(0) == 0) return new { total = 0, page = page, records = 0 };
            return new
            {
                total = (int)Math.Ceiling((float)totalRecords.Value / (float)rows),
                page = page,
                records = totalRecords,
                rows = (
                    from query in result
                    select new
                    {
                        i = query.Auction_ID,
                        cell = new string[] {
                query.Auction_ID.GetValueOrDefault(0).ToString(),
                query.Lot.GetValueOrDefault(0).ToString(),
                query.Title, 
                query.CurrentBid.HasValue ? query.CurrentBid.GetCurrency() : String.Empty,
                query.MaxBid.HasValue ? query.MaxBid.GetCurrency() : String.Empty,
                query.Bids.HasValue ? query.Bids.GetValueOrDefault(0).ToString() : String.Empty
              }
                    }).ToArray()
            };
            //dco.Data = res;
            //CacheRepository.Add(dco);
            //return res;

            //dataContext.CommandTimeout = 600000;
            //Event evnt = dataContext.Events.SingleOrDefault(E => E.ID == event_id);
            //IQueryable<AuctionShortDescription> auctions =
            //  (from A in dataContext.Auctions
            //   join ar in dataContext.AuctionResults on A.ID equals ar.Auction_ID into tmpAR
            //   from AR in tmpAR.DefaultIfEmpty()
            //   where (A.Event_ID == event_id || event_id <= 0)
            //      && (A.Status == (byte)Consts.AuctionStatus.Pending || A.Status == (byte)Consts.AuctionStatus.Open || A.Status == (byte)Consts.AuctionStatus.Closed)
            //   select new AuctionShortDescription
            //   {
            //     ID = A.ID,
            //     Lot = A.Lot.HasValue ? A.Lot.Value : 0,
            //     Title = A.Title,
            //     CurrentBid = AR == null ? String.Empty : AR.CurrentBid.GetCurrency(),
            //     BidNumber = AR == null ? 0 : AR.Bids,
            //     UserType = "--",
            //     DateMade = null,
            //     Auction = A
            //   }).OrderBy(sidx + " " + sord);

            //if (_search)
            //{
            //  if (!String.IsNullOrEmpty(Lot))
            //    auctions = auctions.Where(A => A.Lot.HasValue && A.Lot.Value.ToString().StartsWith(Lot));
            //  if (!String.IsNullOrEmpty(Title))
            //    auctions = auctions.Where(A => A.Title.Contains(Title));
            //  if (!String.IsNullOrEmpty(CurrentBid))
            //    auctions = auctions.Where(A => !String.IsNullOrEmpty(A.CurrentBid) && A.CurrentBid.StartsWith(CurrentBid));
            //  if (!String.IsNullOrEmpty(BidNumber))
            //    auctions = auctions.Where(A => A.BidNumber.HasValue && A.BidNumber.Value.ToString().StartsWith(BidNumber));
            //}
            //int pageIndex = Convert.ToInt32(page) - 1;
            //int pageSize = rows;
            //int totalRecords = auctions.Count();
            //int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            //auctions = auctions.Skip(pageIndex * pageSize).Take(pageSize);

            //List<AuctionShortDescription> list = auctions.ToList();
            //foreach (AuctionShortDescription asd in list)
            //{
            //  asd.DateMade = (asd.Auction.Status == (byte)Consts.AuctionStatus.Open) ? ((asd.Auction.BidCurrents.Count() > 0) ? (DateTime?)asd.Auction.BidCurrents.OrderByDescending(DD => DD.DateMade).First().DateMade : null) : ((asd.Auction.Bids.Count() > 0) ? (DateTime?)asd.Auction.Bids.OrderByDescending(DD => DD.DateMade).First().DateMade : null);
            //}

            //var jsonData = new
            //{
            //  total = totalPages,
            //  page = page,
            //  records = totalRecords,
            //  rows = (
            //      from query in auctions.ToList()
            //      select new
            //      {
            //        i = query.ID,
            //        cell = new string[] { 
            //          query.ID.ToString(),
            //          (query.Lot.HasValue) ? query.Lot.Value.ToString() : String.Empty, query.Title,
            //          (!String.IsNullOrEmpty(query.CurrentBid))? query.CurrentBid:String.Empty,
            //          String.Empty,
            //          (query.BidNumber.HasValue && query.BidNumber.Value > 0) ? query.BidNumber.Value.ToString() : String.Empty,
            //          (query.DateMade.HasValue) ? (query.DateMade.Value.Date.ToString("d") + " " + query.DateMade.Value.ToString("hh:mm:ss.fff", CultureInfo.InvariantCulture)) : String.Empty }
            //      }).ToArray()
            //};

            //auctions = null;

            //return jsonData;
        }

        public object GetAuctionsList(string sidx, string sord, int page, int rows, long? auction_id, long? evnt, string lot, byte? status, byte? maincatory, string category, string title, decimal? price, decimal? reserve, byte? commrate, string seller, decimal? shipping, byte? prioritydescription, long? oldauction_id, string eneteredby, string specialist, string highbidder, string currentbid, long? tagId)
        {
            byte orderby = 0;
            switch (sidx)
            {
                case "Event":
                    orderby = (byte)((sord == "desc") ? 2 : 3);
                    break;
                case "Lot":
                    orderby = (byte)((sord == "desc") ? 4 : 5);
                    break;
                case "Status":
                    orderby = (byte)((sord == "desc") ? 6 : 7);
                    break;
                case "MainCategory":
                    orderby = (byte)((sord == "desc") ? 8 : 9);
                    break;
                case "Category":
                    orderby = (byte)((sord == "desc") ? 10 : 11);
                    break;
                case "Title":
                    orderby = (byte)((sord == "desc") ? 12 : 13);
                    break;
                case "Reserve":
                    orderby = (byte)((sord == "desc") ? 14 : 15);
                    break;
                case "Estimate":
                    orderby = (byte)((sord == "desc") ? 16 : 17);
                    break;
                case "Quantity":
                    orderby = (byte)((sord == "desc") ? 18 : 19);
                    break;
                case "StartDate":
                    orderby = (byte)((sord == "desc") ? 20 : 21);
                    break;
                case "EndDate":
                    orderby = (byte)((sord == "desc") ? 22 : 23);
                    break;
                case "CommRate":
                    orderby = (byte)((sord == "desc") ? 24 : 25);
                    break;
                case "Seller":
                    orderby = (byte)((sord == "desc") ? 26 : 27);
                    break;
                case "Shipping":
                    orderby = (byte)((sord == "desc") ? 28 : 29);
                    break;
                case "PriorityDescription":
                    orderby = (byte)((sord == "desc") ? 30 : 31);
                    break;
                case "LOA":
                    orderby = (byte)((sord == "desc") ? 32 : 33);
                    break;
                case "PulledOut":
                    orderby = (byte)((sord == "desc") ? 34 : 35);
                    break;
                case "IsUnsold":
                    orderby = (byte)((sord == "desc") ? 36 : 37);
                    break;
                case "OldAuction_ID":
                    orderby = (byte)((sord == "desc") ? 38 : 39);
                    break;
                case "NotifiedOn":
                    orderby = (byte)((sord == "desc") ? 40 : 41);
                    break;
                case "EnteredBy":
                    orderby = (byte)((sord == "desc") ? 42 : 43);
                    break;
                case "Specialist":
                    orderby = (byte)((sord == "desc") ? 44 : 45);
                    break;
                case "HighBidder":
                    orderby = (byte)((sord == "desc") ? 46 : 47);
                    break;
                case "CurrentBid":
                    orderby = (byte)((sord == "desc") ? 48 : 49);
                    break;
                case "Price":
                    orderby = (byte)((sord == "desc") ? 50 : 51);
                    break;
                default:
                    orderby = (byte)((sord == "desc") ? 0 : 1);
                    break;
            }
            dataContext.CommandTimeout = 10 * 60 * 1000;
            List<sp_Auctions_GetAuctionListForPageResult> auctions = dataContext.sp_Auctions_GetAuctionListForPage(auction_id, evnt, lot, status, maincatory, category, title, price, reserve, commrate, seller, shipping, prioritydescription, oldauction_id, eneteredby, specialist, highbidder, currentbid, tagId, orderby).ToList();

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = auctions.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            auctions = auctions.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from query in auctions
                    select new
                    {
                        i = query.Auction_ID,
                        cell = new string[] {
                query.Auction_ID.ToString(),
                query.Event,
                query.Lot.HasValue?query.Lot.Value.ToString():String.Empty,
                query.Status,
                query.MainCategory,
                query.Category,
                !query.Title.ToLower().Contains("hidden") ? query.Title: query.Title.Replace("\"","\'"),
                query.Price.GetCurrency(false),
                query.Reserve.GetCurrency(false),
                (!query.Shipping.HasValue)?"0.00":query.Shipping.GetPrice().ToString(),
                query.Quantity.ToString(),
                query.Tags,
                query.StartDate.ToString(),
                query.EndDate.ToString(),
                query.CommRate,
                query.Seller,                
                query.PriorityDescription,
                query.LOA.ToString(),
                query.PulledOut.HasValue?query.PulledOut.Value.ToString():"False",
                query.IsUnsold.ToString(),
                query.IsCatalog.ToString(),                
                query.OldAuction_ID.HasValue?query.OldAuction_ID.Value.ToString(): String.Empty,
                query.NotifiedOn.HasValue?query.NotifiedOn.Value.ToString():String.Empty,
                query.EnteredBy,
                query.Specialist,
                query.HighBidder,
                (String.IsNullOrEmpty(query.CurrentBid)?String.Empty:"$")+query.CurrentBid
                
                
              }
                    }).ToArray()
            };

            auctions = null;

            return jsonData;
        }

        //GetAuctionsListByConsignorForPrint
        public object GetAuctionsListByConsignorForPrint(long cons_id)
        {
            List<Auction> auctions = (from C in dataContext.Consignments
                                      join A in dataContext.Auctions on C.User_ID equals A.Owner_ID
                                      where C.Event_ID == A.Event_ID && C.ID == cons_id
                                      orderby A.IsPrinted ascending, A.ID descending
                                      select A).ToList();

            if (AppHelper.CurrentUser.IsSpecialist)
                auctions = auctions.Where(A => A.EnteredBy == AppHelper.CurrentUser.ID).ToList();

            var jsonData = new
            {
                total = 1,
                page = 1,
                records = auctions.Count(),
                rows = (
                    from query in auctions
                    select new
                    {
                        i = query.ID,
                        cell = new string[] {
                query.ID.ToString(),
                (query.Lot.HasValue)?query.Lot.Value.ToString():String.Empty,
                query.Title,
                Convert.ToByte((query.IsPrinted.HasValue)?query.IsPrinted.Value:false).ToString()
              }
                    }).ToArray()
            };

            auctions = null;

            return jsonData;
        }

        //AddNewItemForConsignorStatementForm
        public object UpdateItemForConsignorStatementForm(string item, ModelStateDictionary ModelState)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                NewAuctionForConsignorForm auction = serializer.Deserialize<NewAuctionForConsignorForm>(item);

                auction.Validate(ModelState);

                if (ModelState.IsValid)
                {
                    EventCategory ec = dataContext.EventCategories.SingleOrDefault(EC => EC.MainCategory_ID == auction.MainCategory_ID && EC.Category_ID == auction.Category_ID.Value && EC.Event_ID == auction.Event_ID);
                    if (ec == null)
                        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The category doesn't exist in this event.");
                    if (!UpdateItemForConsignorStatement(auction))
                        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The new auction wasn't saved.");
                }
                else
                {
                    ModelState.Remove("item");
                    var errors = (from M in ModelState select new { field = M.Key, message = M.Value.Errors.FirstOrDefault().ErrorMessage }).ToArray();
                    return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "Please correct the errors and try again.", errors);
                }
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //AddNewItemForConsignorStatement
        public bool UpdateItemForConsignorStatement(NewAuctionForConsignorForm info)
        {
            Auction old = (info.OldInventory.HasValue) ? GetAuction(info.OldInventory.Value) : null;
            bool IsCopy = (info.OldInventory.HasValue && old != null);
            Event evnt = dataContext.Events.SingleOrDefault(E => E.ID == info.Event_ID);
            bool IsNew;
            try
            {
                Auction auction = GetAuction(info.Auction_ID);
                using (TransactionScope ts = new TransactionScope())
                {
                    IsNew = (auction == null);
                    if (IsNew)
                    {
                        auction = new Auction();
                        dataContext.Auctions.InsertOnSubmit(auction);
                        auction.EnteredBy = AppHelper.CurrentUser.ID;
                        auction.NotifiedOn = DateTime.Now;
                        auction.Event_ID = info.Event_ID;
                        auction.StartDate = evnt.DateStart;
                        auction.EndDate = evnt.DateEnd.AddDays(2);
                        auction.IsInLayout = false;
                        auction.IsPhotographed = false;
                    }
                    auction.AuctionType_ID = (byte)Consts.AuctionType.Normal; //(byte)((info.Quantity == 1) ? Consts.AuctionType.Normal : Consts.AuctionType.Dutch);
                    auction.CommissionRate_ID = info.CommissionRate_ID;
                    auction.CopyNotes = info.CopyNotes;
                    auction.IsBold = (!IsCopy) ? (false) : old.IsBold;
                    auction.IsFeatured = (!IsCopy) ? (false) : old.IsFeatured;
                    auction.LastUpdate = DateTime.Now;
                    auction.LOA = info.LOA;
                    auction.OldAuction_ID = (!IsCopy) ? (info.OldInventory) : old.ID;
                    auction.Owner_ID = info.User_ID;
                    auction.PhotoNotes = info.PhotoNotes;
                    auction.Price = info.Price.GetValueOrDefault(0);
                    auction.Reserve = info.Price.GetValueOrDefault(0);
                    auction.Priority = info.Priority;
                    auction.Quantity = info.Quantity;
                    auction.Status = (IsNew) ? ((byte)((evnt.DateStart > DateTime.Now) ? Consts.AuctionStatus.Pending : Consts.AuctionStatus.Locked)) : auction.Status;
                    auction.Title = info.Title;
                    auction.PulledOut = false;
                    auction.Description = info.Description;
                    auction.IsPrinted = false;
                    auction.ListedStep = info.ListingStep;
                    auction.IsLimitDisabled = info.IsLimitDisabled;

                    EventCategory ec = dataContext.EventCategories.SingleOrDefault(EC => EC.MainCategory_ID == info.MainCategory_ID && EC.Category_ID == info.Category_ID.Value && EC.Event_ID == info.Event_ID);
                    if (ec == null) throw new Exception("Event category doesn't exist.");
                    auction.Category_ID = ec.ID;
                    if (IsCopy)
                    {
                        auction.Cost = old.Cost;
                        auction.Estimate = old.Estimate;
                        auction.Shipping = old.Shipping;
                    }
                    GeneralRepository.SubmitChanges(dataContext);
                    if (IsCopy && old.Images.Count() > 0)
                    {
                        Image i1;
                        foreach (Image i2 in old.Images)
                        {
                            i1 = (!IsNew) ? auction.Images.FirstOrDefault(I => I.PicturePath == i2.PicturePath) : null;
                            if (i1 != null) continue;
                            i1 = i1 ?? new Image();
                            i1.Auction_ID = auction.ID;
                            i1.Default = i2.Default;
                            i1.PicturePath = i2.PicturePath;
                            i1.ThumbNailPath = i2.ThumbNailPath;
                            i1.LargePath = i2.LargePath;
                            i1.Order = i2.Order;
                            i1.UploadedFileName = i2.UploadedFileName;
                            i1.isChecked = i2.isChecked;
                            dataContext.Images.InsertOnSubmit(i1);
                            auction.Images.Add(i1);
                        }
                        GeneralRepository.SubmitChanges(dataContext);
                    }
                    if (IsCopy && !DiffMethods.CopyImages(old.ID, auction.ID))
                        Logger.LogException(new Exception("Images don't exist for auction " + old.ID.ToString()));
                    ts.Complete();
                }

                AuctionExtended ae = dataContext.AuctionExtendeds.Where(a => a.Auction_ID == auction.ID).FirstOrDefault();
                if (info.PurchasedPrice.HasValue || info.SoldPrice.HasValue || info.PurchasedWay.GetValueOrDefault(-1) > 0 || info.SoldWay.GetValueOrDefault(-1) > 0)
                {
                    if (ae == null)
                    {
                        ae = new AuctionExtended();
                        dataContext.AuctionExtendeds.InsertOnSubmit(ae);
                        ae.Auction_ID = auction.ID;
                    }
                    ae.PurchasedWay = info.PurchasedWay.HasValue && info.PurchasedWay.GetValueOrDefault(-1) > 0 ? info.PurchasedWay : null;
                    ae.SoldWay = info.SoldWay.HasValue && info.SoldWay.GetValueOrDefault(-1) > 0 ? info.SoldWay : null;
                    ae.PurchasedPrice = info.PurchasedPrice.HasValue && info.PurchasedPrice.GetValueOrDefault(-1) > 0 ? info.PurchasedPrice : null;
                    ae.SoldPrice = info.SoldPrice.HasValue && info.SoldPrice.GetValueOrDefault(-1) > 0 ? info.SoldPrice : null;
                    GeneralRepository.SubmitChanges(dataContext);
                }
                else if (ae != null)
                {
                    dataContext.AuctionExtendeds.DeleteOnSubmit(ae);
                    GeneralRepository.SubmitChanges(dataContext);
                }

                if (evnt.IsViewable)
                {
                    try
                    {
                        System.Net.WebClient client = new System.Net.WebClient();
                        client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                        client.OpenRead(Consts.CacheClearFrontendIP + Consts.FrontEndClearADPMethod + "/" + auction.ID);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException("[" + Consts.CacheClearFrontendIP + Consts.FrontEndClearADPMethod + "/" + auction.ID + "]", ex);
                    }
                }
            }
            catch (ChangeConflictException cce)
            {
                Logger.LogException(cce);
                throw cce;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                throw ex;
            }
            return true;
        }

        //DeleteAuction
        public JsonExecuteResult DeleteAuction(long auction_id)
        {
            try
            {
                //Auction auction = GetAuction(auction_id);
                //if (auction == null) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "This item doesn't exists");
                //if (auction.Status != (byte)Consts.AuctionStatus.Pending && auction.Status!=(byte)Consts.AuctionStatus.Locked)
                //  return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't delete this item, because it's status is "+((Consts.AuctionStatus)auction.Status).ToString());
                //if (auction.BidsCount > 0)
                //  return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't delete this item, because it has bids.");
                //if (auction.Invoices.Count()>0)
                //  return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't delete this item, because there are invoices in the database for this item.");
                //dataContext.Auctions.DeleteOnSubmit(auction);
                //GeneralRepository.SubmitChanges(dataContext);
                dataContext.CommandTimeout = 10 * 60 * 1000;
                dataContext.sp_Auctions_DeleteAuction(auction_id);
                DiffMethods.DeleteImagesFromDisk(auction_id);
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, "", auction_id);
        }

        //GetAuctionDataJSON
        public object GetAuctionDataJSON(long auctionID)
        {
            Auction auc = GetAuction(auctionID);
            if (auc == null) return null;
            List<IdTitle> tags = (from at in dataContext.AuctionTags
                                  join t in dataContext.Tags on at.TagID equals t.ID
                                  where at.AuctionID == auc.ID
                                  select new IdTitle { ID = t.ID, Title = t.Title }).ToList();
            IdTitle collection = (from ac in dataContext.AuctionCollections
                                  join c in dataContext.Collections on ac.CollectionID equals c.ID
                                  where ac.AuctionID == auc.ID
                                  select new IdTitle { ID = c.ID, Title = c.Title }).FirstOrDefault();
            var jsonData = new
            {
                id = auc.ID,
                ev = auc.Event_ID,
                o = auc.Owner_ID,
                o_n = auc.User.UserLoginName,
                cr = auc.CommissionRate_ID,
                sd = auc.StartDate.ToString(),
                ed = auc.EndDate.ToString(),
                s = auc.Status.Value,
                t = auc.Title,
                lot = auc.Lot.HasValue ? auc.Lot.Value.ToString() : String.Empty,
                mc = auc.EventCategory.MainCategory_ID.ToString(),
                cat = auc.EventCategory.Category_ID.ToString(),
                cat_n = auc.EventCategory.Category.Title,
                q = auc.Quantity,
                p = auc.Price,
                r = auc.Reserve.HasValue ? auc.Reserve.Value : auc.Price,
                est = auc.Estimate,
                sh = auc.Shipping.HasValue ? auc.Shipping.Value : 0,
                ls = auc.ListedStep.ToString(),
                isphot = auc.IsPhotographed,
                inlay = auc.IsInLayout,
                pr = auc.Priority,
                loa = auc.LOA,
                add = auc.Addendum,
                old = (auc.OldAuction_ID.HasValue) ? auc.Auction_Parent.ID.ToString() : String.Empty,
                old_n = (auc.OldAuction_ID.HasValue) ? auc.Auction_Parent.Title : String.Empty,
                collection = collection != null ? collection.Title : string.Empty,
                collectionID = collection != null ? collection.ID.ToString() : string.Empty,
                pul = auc.PulledOut,
                uns = auc.IsUnsold,
                catl = auc.IsCatalog,
                //chk=auc.IsC ,

                pri = auc.IsPrinted,
                d = auc.Description,
                crn = auc.CopyNotes,
                phn = auc.PhotoNotes,
                dislim = auc.IsLimitDisabled,
                purchw = auc.AuctionExtended == null ? -1 : auc.AuctionExtended.PurchasedWay.GetValueOrDefault(-1),
                purchp = auc.AuctionExtended == null || !auc.AuctionExtended.PurchasedPrice.HasValue ? String.Empty : auc.AuctionExtended.PurchasedPrice.GetValueOrDefault(0).GetPrice().ToString(),
                soldw = auc.AuctionExtended == null ? -1 : auc.AuctionExtended.SoldWay.GetValueOrDefault(-1),
                soldp = auc.AuctionExtended == null || !auc.AuctionExtended.SoldPrice.HasValue ? String.Empty : auc.AuctionExtended.SoldPrice.GetValueOrDefault(0).GetPrice().ToString(),
                tags
            };
            return jsonData;
        }

        //UpdateAuctionForm
        public JsonExecuteResult UpdateAuctionForm(string auction, ModelStateDictionary ModelState)
        {
            long auction_id = 0;
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                AuctionForm auc = serializer.Deserialize<AuctionForm>(auction);

                auc.Validate(ModelState);

                if (ModelState.IsValid)
                {
                    if (auc.Lot.HasValue)
                    {
                        Auction auc2 = dataContext.Auctions.FirstOrDefault(A => A.Event_ID == auc.Event_ID && A.Lot == auc.Lot.Value && A.ID != auc.ID);
                        if (auc2 != null)
                            return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The item with the same Lot number is already exist in this event (Auction#" + auc2.ID + ")");
                    }
                    if (!UpdateAuction(auc))
                        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The data wasn't saved.");
                    auction_id = auc.ID;
                }
                else
                {
                    ModelState.Remove("auction");
                    var errors = (from M in ModelState select new { field = M.Key, message = M.Value.Errors.FirstOrDefault().ErrorMessage }).ToArray();
                    return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "Please correct the errors and try again.", errors);
                }
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, "", auction_id);
        }

        //UpdateAuction
        public bool UpdateAuction(AuctionForm info)
        {
            Auction old = (info.OldAuction_ID.HasValue) ? GetAuction(info.OldAuction_ID.Value) : null;
            bool IsCopy = (info.OldAuction_ID.HasValue && old != null);
            Event evnt = dataContext.Events.FirstOrDefault(t => t.ID == info.Event_ID);
            bool IsNew = false;
            bool isoldpulledout = false;
            try
            {
                Auction auction = GetAuction(info.ID);
                using (TransactionScope ts = new TransactionScope())
                {
                    if (IsNew = (auction == null))
                    {
                        auction = new Auction();
                        dataContext.Auctions.InsertOnSubmit(auction);
                        auction.EnteredBy = AppHelper.CurrentUser.ID;
                        auction.NotifiedOn = DateTime.Now;
                    }
                    if (IsCopy && auction.ID == old.ID)
                    {
                        IsCopy = false;
                        old = null;
                        info.OldAuction_ID = null;
                    }
                    auction.AuctionType_ID = (byte)Consts.AuctionType.Normal; //(byte)((info.Quantity == 1) ? Consts.AuctionType.Normal : Consts.AuctionType.Dutch);          
                    if (IsNew)
                    {
                        auction.Event_ID = info.Event_ID;
                        auction.StartDate = evnt.DateStart;
                        auction.EndDate = evnt.DateEnd.AddDays(2);
                    }
                    else
                    {
                        isoldpulledout = (auction.PulledOut.HasValue && auction.PulledOut.Value) || auction.Status == (byte)Consts.AuctionStatus.PulledOut;
                    }
                    auction.Owner_ID = info.Owner_ID.Value;
                    auction.CommissionRate_ID = info.CommissionRate_ID;
                    auction.Status = info.Status_ID; //(byte)((evnt.DateStart > DateTime.Now) ? Consts.AuctionStatus.Pending : Consts.AuctionStatus.Locked);
                    auction.Title = info.Title;
                    auction.Lot = info.Lot;
                    EventCategory ec = dataContext.EventCategories.SingleOrDefault(t => t.MainCategory_ID == info.MainCategory_ID && t.Category_ID == info.Category_ID.Value && t.Event_ID == info.Event_ID);
                    if (ec == null) throw new Exception("Event category doesn't exist.");
                    auction.Category_ID = ec.ID;
                    auction.Quantity = info.Quantity.HasValue ? info.Quantity.Value : 1;
                    auction.Price = info.Reserve.Value;
                    auction.Reserve = info.Reserve.Value;
                    auction.Estimate = info.Estimate;
                    auction.Shipping = info.Shipping;
                    auction.ListedStep = info.ListingStep;
                    auction.Priority = info.Priority.Value;
                    auction.LOA = info.LOA;
                    auction.Addendum = info.Addendum;
                    auction.OldAuction_ID = (!IsCopy) ? (info.OldAuction_ID) : old.ID;
                    auction.PulledOut = info.PulledOut;
                    auction.IsUnsold = info.IsUnsold;
                    auction.IsCatalog = info.IsCatalog;
                    auction.IsPrinted = info.IsPrinted;
                    auction.Description = info.Description;
                    auction.CopyNotes = info.CopyNotes;
                    auction.PhotoNotes = info.PhotoNotes;
                    auction.LastUpdate = DateTime.Now;
                    auction.IsBold = (IsCopy) && old.IsBold;
                    auction.IsFeatured = (IsCopy) && old.IsFeatured;
                    auction.IsPhotographed = info.IsPhotographed;
                    auction.IsInLayout = info.IsInLayout;
                    auction.IsLimitDisabled = info.IsLimitDisabled;
                    auction.IsCatalog = info.IsCatalog;
                    if (auction.PulledOut.Value || auction.Status == (byte)Consts.AuctionStatus.PulledOut)
                    {
                        auction.PulledOut = true;
                        auction.Status = (byte)Consts.AuctionStatus.PulledOut;
                        if (!IsNew && !isoldpulledout)
                        {
                            if (auction.Event.IsCurrent)
                            {
                                List<BidWatch> bw = dataContext.BidWatches.Where(t => t.Auction_ID == auction.ID).ToList();
                                bw.ForEach(t => Mail.SendPulledOutLetter(t.User.AddressCard_Billing.FirstName, t.User.AddressCard_Billing.LastName, t.User.Email, auction.ID, auction.Title));
                            }
                        }
                    }
                    if (IsCopy)
                    {
                        auction.Cost = old.Cost;
                    }
                    GeneralRepository.SubmitChanges(dataContext);

                    List<long> tags = (from t in dataContext.AuctionTags where t.AuctionID == auction.ID select t.TagID).ToList();
                    foreach (long tagID in info.Tags)
                    {
                        if (tags.Contains(tagID)) tags.Remove(tagID);
                        else
                        {
                            dataContext.AuctionTags.InsertOnSubmit(new AuctionTag { AuctionID = auction.ID, TagID = tagID });
                        }
                    }
                    foreach (long tagID in tags)
                    {
                        var r = dataContext.AuctionTags.FirstOrDefault(a => a.AuctionID == auction.ID && a.TagID == tagID);
                        if (r != null)
                            dataContext.AuctionTags.DeleteOnSubmit(r);
                    }
                    
                    GeneralRepository.SubmitChanges(dataContext);
                   // List<Image> images = dataContext.Images.Where(a => a.Auction_ID == auction.ID).OrderByDescending(a => a.Default).ThenBy(a => a.Order).ToList();
                   //Alan 11/10/2015
                    if (info.ImagesTag != null)
                        foreach (string imgDesc in info.ImagesTag)
                        {
                            string[] imgArray = imgDesc.Split('|');

                            long imgId = Convert.ToInt64(imgArray[1]);
                            bool isChecked =Convert.ToBoolean( Convert.ToInt32(imgArray[0]));
                            Image r = dataContext.Images.FirstOrDefault(i => i.ID == imgId && i.Auction_ID == auction.ID);
                            if (r != null)
                            {
                            
                                r.isChecked = isChecked;
                           
                                GeneralRepository.SubmitChanges(dataContext);
                        
                            }
                            
                     
                        }
                   
                    if (IsCopy && !DiffMethods.CopyImages(old.ID, auction.ID))
                        Logger.LogException(new Exception("Images don't exist for auction " + old.ID));
                    if (IsNew)
                    {
                        AddConsignment(auction.Owner_ID, auction.Event_ID);
                        MoveImagesToAction(auction.ID);
                    }

                    if (IsNew && IsCopy && old.Images.Any())
                    {
                        //Image i1;
                        //foreach (Image i2 in old.Images)
                        //{
                        //  i1 = (!IsNew) ? auction.Images.FirstOrDefault(I => I.PicturePath == i2.PicturePath) : null;
                        //  if (i1 != null) continue;
                        //  i1 = i1 ?? new Image();
                        //  i1.Auction_ID = auction.ID;
                        //  i1.Default = i2.Default;
                        //  i1.PicturePath = i2.PicturePath;
                        //  i1.ThumbNailPath = i2.ThumbNailPath;
                        //  i1.LargePath = i2.LargePath;
                        //  i1.Order = i2.Order;
                        //  i1.UploadedFileName = i2.UploadedFileName;
                        //  dataContext.Images.InsertOnSubmit(i1);
                        //  auction.Images.Add(i1);
                        //}
                        //GeneralRepository.SubmitChanges(dataContext);

                        List<Image> imgs = dataContext.Images.Where(a => a.Auction_ID == auction.ID).OrderByDescending(a => a.Default).ThenBy(a => a.Order).ToList();
                        int imagecount = imgs.Count();
                        bool isimages = imagecount > 0; //auction.Images.Count() > 0;
                        int maxOrder = imagecount > 0 ? imgs.Max(m => m.Order) : 0;
                        Image i1;
                        foreach (Image i2 in old.Images)
                        {
                            i1 = new Image
                              {
                                  Auction_ID = auction.ID,
                                  Default = !isimages && i2.Default,
                                  PicturePath = i2.PicturePath,
                                  ThumbNailPath = i2.ThumbNailPath,
                                  LargePath = i2.LargePath,
                                  Order = ++maxOrder,
                                  UploadedFileName = i2.UploadedFileName
                              };
                            dataContext.Images.InsertOnSubmit(i1);
                            imgs.Add(i1);
                        }
                        GeneralRepository.SubmitChanges(dataContext);
                        SetImageAsDefault(auction.ID, imgs[0].ID);
                        if (DiffMethods.CopyImages(old.ID, auction.ID))
                            Logger.LogException(new Exception("Images don't exist for auction " + old.ID));
                    }
                    ts.Complete();
                }

                AuctionExtended ae = dataContext.AuctionExtendeds.FirstOrDefault(a => a.Auction_ID == auction.ID);
                if (info.PurchasedPrice.HasValue || info.SoldPrice.HasValue || info.PurchasedWay.GetValueOrDefault(-1) > 0 || info.SoldWay.GetValueOrDefault(-1) > 0)
                {
                    if (ae == null)
                    {
                        ae = new AuctionExtended();
                        dataContext.AuctionExtendeds.InsertOnSubmit(ae);
                        ae.Auction_ID = auction.ID;
                    }
                    ae.PurchasedWay = info.PurchasedWay.HasValue && info.PurchasedWay.GetValueOrDefault(-1) > 0 ? info.PurchasedWay : null;
                    ae.SoldWay = info.SoldWay.HasValue && info.SoldWay.GetValueOrDefault(-1) > 0 ? info.SoldWay : null;
                    ae.PurchasedPrice = info.PurchasedPrice.HasValue && info.PurchasedPrice.GetValueOrDefault(-1) > 0 ? info.PurchasedPrice : null;
                    ae.SoldPrice = info.SoldPrice.HasValue && info.SoldPrice.GetValueOrDefault(-1) > 0 ? info.SoldPrice : null;
                    GeneralRepository.SubmitChanges(dataContext);
                }
                else if (ae != null)
                {
                    dataContext.AuctionExtendeds.DeleteOnSubmit(ae);
                    GeneralRepository.SubmitChanges(dataContext);
                }

                AuctionCollection auctionCollection = dataContext.AuctionCollections.FirstOrDefault(t => t.AuctionID == auction.ID);
                if (info.CollectionID.HasValue)
                {
                    if (auctionCollection == null)
                    {
                        auctionCollection = new AuctionCollection { AuctionID = auction.ID };
                        dataContext.AuctionCollections.InsertOnSubmit(auctionCollection);
                    }
                    auctionCollection.CollectionID = info.CollectionID.Value;
                    GeneralRepository.SubmitChanges(dataContext);
                }
                else if (auctionCollection != null)
                {
                    dataContext.AuctionCollections.DeleteOnSubmit(auctionCollection);
                    GeneralRepository.SubmitChanges(dataContext);
                }

                if (evnt.IsViewable)
                {
                    try
                    {
                        System.Net.WebClient client = new System.Net.WebClient();
                        client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                        client.OpenRead(Consts.CacheClearFrontendIP + Consts.FrontEndClearADPMethod + "/" + auction.ID);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException("[" + Consts.CacheClearFrontendIP + Consts.FrontEndClearADPMethod + "/" + auction.ID + "]", ex);
                    }
                }
            }
            catch (ChangeConflictException cce)
            {
                Logger.LogException(cce);
                throw cce;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                throw ex;
            }
            return true;
        }

        //MoveImagesToAction
        private void MoveImagesToAction(long auction_id)
        {
            try
            {
                string sourcePath = DiffMethods.GetAuctionImageDirForUser(AppHelper.CurrentUser.ID);
                string[] files = Directory.GetFiles(sourcePath, "thmb_*.*");
                if (files.Count() == 0) return;
                int order = 0;
                FileInfo fi1, fi2, fi3;
                string fName1, fName2, fName3;
                string path = DiffMethods.CheckOrCreateDirectory(auction_id);
                long tickes = DateTime.Now.Ticks;
                Image img;
                foreach (string f in files)
                {
                    fi2 = new FileInfo(f);
                    fi1 = new FileInfo(Path.Combine(sourcePath, fi2.Name.Remove(0, 5)));
                    fi3 = new FileInfo(Path.Combine(sourcePath, "xl_" + fi2.Name.Remove(0, 5)));
                    if (!fi1.Exists || !fi2.Exists) continue;
                    fName1 = String.Format("{0}-{1}_{2}{3}", auction_id, (++order), tickes, fi1.Extension);
                    fName2 = String.Format("thmb_{0}-{1}_{2}{3}", auction_id, order, tickes, fi2.Extension);
                    fi1.MoveTo(Path.Combine(path, fName1));
                    fi2.MoveTo(Path.Combine(path, fName2));
                    img = new Image();
                    img.Auction_ID = auction_id;
                    img.Default = order == 1;
                    img.Order = order;
                    img.PicturePath = fName1;
                    img.ThumbNailPath = fName2;
                    if (fi3.Exists)
                    {
                        fName3 = String.Format("xl_{0}-{1}_{2}{3}", auction_id, order, tickes, fi3.Extension);
                        fi3.MoveTo(Path.Combine(path, fName3));
                        img.LargePath = fName3;
                    }
                    else img.LargePath = img.PicturePath;
                    dataContext.Images.InsertOnSubmit(img);
                    File.Delete(f);
                }
                GeneralRepository.SubmitChanges(dataContext);
            }
            catch (Exception ex)
            {
                Logger.LogInfo("MoveImagesToAction(" + auction_id.ToString() + ")-" + ex.Message);
            }
        }

        //ClearImagesForNewAuction
        public void ClearImagesForNewAuction(long user_id)
        {
            try
            {
                string[] files = Directory.GetFiles(DiffMethods.GetAuctionImageDirForUser(user_id));
                FileInfo fi;
                foreach (string f in files)
                {
                    fi = new FileInfo(f);
                    if (fi.Exists) fi.Delete();
                }
            }
            catch (IOException ex)
            {
                Logger.LogInfo(ex.Message + " - " + ex.StackTrace);
            }
            return;
        }

        //GetAuctionImgages
        public object GetAuctionImages(long auction_id, long user_id)
        {
            Auction auction = GetAuction(auction_id);
            int order = 0;
            Size[] sizes;
            string path = DiffMethods.CheckOrCreateDirectory(auction_id, user_id);
            FileInfo fi;
            System.Drawing.Image image;
            if (auction == null)
            {
                string[] files = Directory.GetFiles(DiffMethods.GetAuctionImageDirForUser(user_id), "thmb_*.*");
                if (files.Count() == 0) return false;
                sizes = new Size[files.Count()];
                for (int i = 0; i < files.Length; i++)
                {
                    fi = new FileInfo(Path.Combine(path, Path.GetFileName(files[i]).Remove(0, 5)));
                    if (!fi.Exists)
                    {
                        sizes[i] = new Size(0, 0);
                        continue;
                    }
                    image = System.Drawing.Image.FromFile(fi.FullName);
                    sizes[i] = new Size(image.Width, image.Height);
                    image.Dispose();
                }
                var jsonData1 = new
                {
                    total = 1,
                    page = 0,
                    records = files.Count(),
                    rows = (
                        from query in files
                        select new
                        {
                            i = (++order),
                            cell = new string[] {
                order.ToString(),
                String.Format("<img src='{0}' />", DiffMethods.GetAuctionImageWebForUser(user_id, Path.GetFileName(query))),
                String.Format("<img src='{0}' style='max-weight:120px;max-height:120px' />", DiffMethods.GetAuctionImageWebForUser(user_id, Path.GetFileName(query).Remove(0,5))),
                String.Format("<b>Image:</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0}<br/><b>Thumbnail:</b>&nbsp;&nbsp;{1}", Path.GetFileName(query).Remove(0,5), Path.GetFileName(query)),
                order.ToString(),
               // String.Format("<input type='checkbox' name='select' checked>"),
                Path.GetFileName(query),
                Path.GetFileName(query).Remove(0,5),
                DiffMethods.GetAuctionImageWebForUser(AppHelper.CurrentUser.ID, Path.GetFileName(query).Remove(0,5)),
                sizes[order-1].Width.ToString(),
                sizes[order-1].Height.ToString()
              }
                        }).ToArray()
                };
                return jsonData1;
            }

            List<Image> images = dataContext.spAuction_GetAuctionImages(auction_id).ToList();
            sizes = new Size[images.Count()];
            for (int i = 0; i < images.Count(); i++)
            {
                fi = new FileInfo(Path.Combine(path, images[i].PicturePath));
                if (!fi.Exists)
                {
                    sizes[i] = new Size(0, 0);
                    continue;
                }
                image = System.Drawing.Image.FromFile(fi.FullName);
                sizes[i] = new Size(image.Width, image.Height);
                image.Dispose();
            }
            var jsonData = new
            {
                total = 1,
                page = 0,
                records = images.Count(),
                rows = (
                    from query in images
                    select new
                    {
                        i = query.ID,
                        cell = new string[] {
                query.ID.ToString(),
                String.Format("<img src='{0}' />", DiffMethods.GetAuctionImageWebPath(query.Auction_ID, query.ThumbNailPath)),
                String.Format("<img src='{0}' style='max-weight:120px;max-height:120px' runat='server' />", DiffMethods.GetAuctionImageWebPath(query.Auction_ID, query.PicturePath)),
                String.Format("<b>Image:</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0}<br/><b>Thumbnail:</b>&nbsp;&nbsp;{1}", query.PicturePath, query.ThumbNailPath),
                query.Order.ToString(),

                
                String.Format("<input type='checkbox' name='chkIsChecked' {0} />",query.isChecked==true?"checked":""),
                query.ThumbNailPath,
                query.PicturePath,
                DiffMethods.GetAuctionImageWebPath(auction_id, query.PicturePath),
                sizes[order].Width.ToString(),
                sizes[order++].Height.ToString(),
                query.isChecked.ToString ()
                
              }
                    }).ToArray()
            };
            return jsonData;
        }

        //UploadImage
        public JsonExecuteResult UploadImage(long auction_id, HttpPostedFileBase file)
        {
            try
            {
                Auction auc = GetAuction(auction_id);

                string path = DiffMethods.CheckOrCreateDirectory(auction_id);
                System.Drawing.Image img, imgNew, imgThumb, imgLarge;

                try
                {
                    if (!file.ContentType.Contains("bmp") && !file.ContentType.Contains("jpeg") && !file.ContentType.Contains("png") && !file.ContentType.Contains("gif")) throw new Exception();
                    img = System.Drawing.Image.FromStream(file.InputStream);
                }
                catch (Exception ex)
                {
                    return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
                }

                string prefix = (auc == null) ? (Directory.GetFiles(path, "thmb_*.*").Count() + 1).ToString() + "_" + DateTime.Now.Ticks.ToString() : auction_id.ToString() + "-" + ((auc == null || auc.Images == null) ? 0 : auc.Images.Count + 1).ToString() + "_" + DateTime.Now.Ticks.ToString();

                prefix += Path.GetExtension(file.FileName);

                imgNew = DiffMethods.ChangeImageSize(img, (img.Width > img.Height) ? (int?)Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Medium) : null, (img.Width > img.Height) ? null : (int?)Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Medium), false);
                imgThumb = DiffMethods.ChangeImageSize(img, Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Small), null, false);
                imgLarge = DiffMethods.ChangeImageSize(img, (img.Width > img.Height) ? (int?)Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Large) : null, (img.Width > img.Height) ? null : (int?)Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Large), false);
                try
                {
                    imgNew.Save(Path.Combine(path, prefix), System.Drawing.Imaging.ImageFormat.Jpeg);
                    imgThumb.Save(Path.Combine(path, "thmb_" + prefix), System.Drawing.Imaging.ImageFormat.Jpeg);
                    imgLarge.Save(Path.Combine(path, "xl_" + prefix), System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch (System.Runtime.InteropServices.ExternalException ex)
                {
                    throw new System.Runtime.InteropServices.ExternalException(ex.Message);
                }
                img.Dispose();
                imgNew.Dispose();
                imgThumb.Dispose();
                imgLarge.Dispose();
                if (auction_id > 0)
                    AddAuctionImage(auction_id, prefix, file.FileName);

            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //UploadImage
        public JsonExecuteResult UploadImage(Auction auc, string file)
        {
            try
            {
                string path = DiffMethods.CheckOrCreateDirectory(auc.ID);
                System.Drawing.Image img, imgNew, imgThumb, imgLarge;

                img = System.Drawing.Image.FromFile(file);

                string prefix = (auc == null) ? (Directory.GetFiles(path, "thmb_*.*").Count() + 1).ToString() + "_" + DateTime.Now.Ticks.ToString() : auc.ID.ToString() + "-" + ((auc == null || auc.Images == null) ? 0 : auc.Images.Count + 1).ToString() + "_" + DateTime.Now.Ticks.ToString();

                prefix += Path.GetExtension(file);

                imgNew = DiffMethods.ChangeImageSize(img, (img.Width > img.Height) ? (int?)Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Medium) : null, (img.Width > img.Height) ? null : (int?)Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Medium), false);
                imgThumb = DiffMethods.ChangeImageSize(img, Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Small), null, false);
                imgLarge = (img.Width <= Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Large) || img.Height <= Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Large)) ? img : DiffMethods.ChangeImageSize(img, (img.Width > img.Height) ? (int?)Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Large) : null, (img.Width > img.Height) ? null : (int?)Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Large), false);

                imgNew.Save(Path.Combine(path, prefix), System.Drawing.Imaging.ImageFormat.Jpeg);
                imgThumb.Save(Path.Combine(path, "thmb_" + prefix), System.Drawing.Imaging.ImageFormat.Jpeg);
                imgLarge.Save(Path.Combine(path, "xl_" + prefix), System.Drawing.Imaging.ImageFormat.Jpeg);

                img.Dispose();
                imgNew.Dispose();
                imgThumb.Dispose();
                imgLarge.Dispose();

                Image imgnew = AddAuctionImg(auc.ID, prefix, Path.GetFileName(file));
                if (imgnew != null) auc.Images.Add(imgnew);
            }
            catch (System.Runtime.InteropServices.ExternalException ex)
            {
                throw new System.Runtime.InteropServices.ExternalException(ex.Message);
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //UploadImage
        public JsonExecuteResult UploadImage(long auction_id, long? user_id, HttpPostedFileBase file)
        {
            try
            {
                Auction auc = GetAuction(auction_id);
                System.Drawing.Image img, imgNew, imgThumb, imgLarge;
                string path = DiffMethods.CheckOrCreateDirectory(auction_id, user_id.GetValueOrDefault(AppHelper.CurrentUser != null ? AppHelper.CurrentUser.ID : 0));

                try
                {
                    //if (!file.ContentType.Contains("bmp") && !file.ContentType.Contains("jpeg") && !file.ContentType.Contains("png") && !file.ContentType.Contains("gif")) throw new Exception();
                    img = System.Drawing.Image.FromStream(file.InputStream);
                }
                catch (Exception ex)
                {
                    return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
                }

                string prefix = (auc == null) ? (Directory.GetFiles(path, "thmb_*.*").Count() + 1).ToString() + "_" + DateTime.Now.Ticks.ToString() : auction_id.ToString() + "-" + (auc.Images == null ? 0 : auc.Images.Count + 1).ToString() + "_" + DateTime.Now.Ticks.ToString();

                prefix += Path.GetExtension(file.FileName);

                imgNew = DiffMethods.ChangeImageSize(img, (img.Width > img.Height) ? (int?)Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Medium) : null, (img.Width > img.Height) ? null : (int?)Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Medium), false);
                imgThumb = DiffMethods.ChangeImageSize(img, Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Small), null, false);
                imgLarge = (img.Width <= Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Large) || img.Height <= Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Large)) ? img : DiffMethods.ChangeImageSize(img, (img.Width > img.Height) ? (int?)Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Large) : null, (img.Width > img.Height) ? null : (int?)Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Large), false);

                try
                {
                    imgNew.Save(Path.Combine(path, prefix), System.Drawing.Imaging.ImageFormat.Jpeg);
                    imgThumb.Save(Path.Combine(path, "thmb_" + prefix), System.Drawing.Imaging.ImageFormat.Jpeg);
                    imgLarge.Save(Path.Combine(path, "xl_" + prefix), System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch (System.Runtime.InteropServices.ExternalException ex)
                {
                    throw new System.Runtime.InteropServices.ExternalException(ex.Message);
                }
                img.Dispose();
                imgNew.Dispose();
                imgThumb.Dispose();
                if (auction_id > 0)
                    AddAuctionImage(auction_id, prefix, file.FileName);

            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //AddAuctionImage
        public long AddAuctionImage(long auction_id, string image, string uploadedfilename)
        {
            Image img = AddAuctionImg(auction_id, image, uploadedfilename);
            return (img == null) ? -1 : img.ID;
        }

        //AddAuctionImage
        public Image AddAuctionImg(long auction_id, string image, string uploadedfilename)
        {
            Image img = new Image();
            try
            {
                List<Image> images = dataContext.spAuction_GetAuctionImages(auction_id).ToList();
                img.Auction_ID = auction_id;
                img.Default = images.Count() == 0;
                img.Order = (images.Count() == 0) ? 1 : images[images.Count() - 1].Order + 1;
                img.PicturePath = image;
                img.ThumbNailPath = "thmb_" + image;
                img.LargePath = "xl_" + image;
                img.UploadedFileName = uploadedfilename;
                dataContext.Images.InsertOnSubmit(img);
                GeneralRepository.SubmitChanges(dataContext);
            }
            catch
            {
                img = null;
            }
            return img;
        }

        //RenameImageByOrder
        private bool RenameImageByOrder(Image image, long auction_id)
        {
            long tickes = DateTime.Now.Ticks;
            string path = DiffMethods.CheckOrCreateDirectory(auction_id);
            FileInfo fi1 = new FileInfo(Path.Combine(path, image.PicturePath));
            FileInfo fi2 = new FileInfo(Path.Combine(path, image.ThumbNailPath));
            FileInfo fi3 = new FileInfo(Path.Combine(path, image.LargePath));
            string fName1 = String.Format("{0}-{1}_{2}{3}", auction_id, image.Order, tickes, fi1.Extension);
            string fName2 = String.Format("thmb_{0}-{1}_{2}{3}", auction_id, image.Order, tickes, fi2.Extension);
            string fName3 = String.Format("xl_{0}-{1}_{2}{3}", auction_id, image.Order, tickes, fi3.Extension);
            image.PicturePath = fName1;
            image.ThumbNailPath = fName2;
            image.LargePath = fName3;
            try
            {
                if (fi1.Exists) fi1.CopyTo(Path.Combine(path, fName1));
                if (fi2.Exists) fi2.CopyTo(Path.Combine(path, fName2));
                if (fi3.Exists) fi3.CopyTo(Path.Combine(path, fName3));
                else image.LargePath = image.PicturePath;
            }
            catch
            {
                return false;
            }

            try
            {
                if (fi1.Exists) fi1.Delete();
                if (fi2.Exists) fi2.Delete();
                if (fi3.Exists) fi3.Delete();
            }
            catch
            {

            }
            return true;
        }

        //DeleteImage
        public JsonExecuteResult DeleteImage(long auction_id, long image_id)
        {
            try
            {
                Auction auc = GetAuction(auction_id);
                if (auc == null)
                {
                    string path = DiffMethods.GetAuctionImageDirForUser(AppHelper.CurrentUser.ID);
                    string[] files = Directory.GetFiles(path, "thmb_*.*");
                    if (files.Count() == 0) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The file doesn't exist");
                    FileInfo fi1, fi2, fi3;
                    string fName1, fName2, fName3;
                    long tickes = DateTime.Now.Ticks;
                    int index;
                    int order;
                    foreach (string f in files)
                    {
                        fi2 = new FileInfo(f);
                        fi1 = new FileInfo(Path.Combine(path, fi2.Name.Remove(0, 5)));
                        fi3 = new FileInfo(Path.Combine(path, "xl_" + fi2.Name.Remove(0, 5)));
                        if (!fi1.Exists || !fi2.Exists) continue;
                        index = fi1.Name.IndexOf("_");
                        order = Convert.ToInt32(fi1.Name.Substring(0, index));
                        if (order < image_id) continue;
                        if (order == image_id)
                        {
                            try
                            {
                                fi1.Delete();
                                fi2.Delete();
                                if (fi3.Exists) fi3.Delete();
                            }
                            catch
                            {
                            }
                            continue;
                        }
                        fName1 = String.Format("{0}_{1}{2}", (--order), tickes, fi1.Extension);
                        fName2 = String.Format("thmb_{0}_{1}{2}", order, tickes, fi2.Extension);
                        fName3 = String.Format("xl_{0}_{1}{2}", order, tickes, fi3.Extension);
                        fi1.CopyTo(Path.Combine(path, fName1));
                        fi2.CopyTo(Path.Combine(path, fName2));
                        if (fi3.Exists) fi3.CopyTo(Path.Combine(path, fName3));
                        try
                        {
                            fi1.Delete();
                            fi2.Delete();
                            if (fi3.Exists) fi3.Delete();
                        }
                        catch { }
                    }
                }
                else
                {
                    Image img = dataContext.Images.SingleOrDefault(I => I.ID == image_id);

                    string path = DiffMethods.CheckOrCreateDirectory(auction_id);

                    FileInfo fi1 = new FileInfo(Path.Combine(path, img.PicturePath));
                    FileInfo fi2 = new FileInfo(Path.Combine(path, img.ThumbNailPath));
                    FileInfo fi3 = new FileInfo(Path.Combine(path, img.LargePath));

                    try
                    {
                        if (fi1.Exists) fi1.Delete();
                        if (fi2.Exists) fi2.Delete();
                        if (fi3.Exists) fi3.Delete();
                    }
                    catch { }

                    List<Image> images = dataContext.spAuction_GetAuctionImages(auction_id).ToList();

                    if (images.Count() > 1)
                    {
                        int order = img.Order;
                        foreach (Image i in images.Where(I => I.Order > img.Order).ToList())
                        {
                            i.Order = order++;
                            RenameImageByOrder(i, auction_id);
                        }
                        img.Order = int.MaxValue;
                    }
                    if (img.Default && images.Count > 1)
                        images.OrderBy(I => I.Order).FirstOrDefault().Default = true;

                    dataContext.Images.DeleteOnSubmit(img);
                    GeneralRepository.SubmitChanges(dataContext);

                }
            }
            catch (IOException ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //MoveImage
        public JsonExecuteResult MoveImage(long auction_id, long image_id, bool isup)
        {
            try
            {
                Auction auc = GetAuction(auction_id);
                if (auc == null)
                {
                    string path = DiffMethods.GetAuctionImageDirForUser(AppHelper.CurrentUser.ID);
                    string[] files = Directory.GetFiles(path, "thmb_*.*");
                    if (files.Count() < 2)
                        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, String.Format("You can't move {0} this image.", ((isup) ? "up" : "down")));
                    FileInfo fi1, fi2, fi3, fi4, fi5, fi6;
                    string fName1, fName2, fName3;
                    long tickes = DateTime.Now.Ticks;
                    int order1, order2;
                    order1 = (int)image_id;
                    if ((order1 == 1 && isup) || (order1 == files.Count() && !isup))
                        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, String.Format("You can't move this image {0}.", ((isup) ? "up" : "down")));

                    fi2 = new FileInfo(files[order1 - 1]);
                    fi1 = new FileInfo(Path.Combine(path, fi2.Name.Remove(0, 5)));
                    fi5 = new FileInfo(Path.Combine(path, "xl_" + fi2.Name.Remove(0, 5)));
                    if (!fi1.Exists || !fi2.Exists) throw new Exception("File doesn't exist.");

                    order2 = (isup) ? order1 - 1 : order1 + 1;
                    fi4 = new FileInfo(files[order2 - 1]);
                    fi3 = new FileInfo(Path.Combine(path, fi4.Name.Remove(0, 5)));
                    fi6 = new FileInfo(Path.Combine(path, "xl_" + fi4.Name.Remove(0, 5)));
                    if (!fi3.Exists || !fi4.Exists) throw new Exception("File doesn't exist.");

                    fName1 = String.Format("{0}_{1}{2}", (isup) ? (--order1) : (++order1), tickes, fi1.Extension);
                    fName2 = String.Format("thmb_{0}_{1}{2}", order1, tickes, fi2.Extension);
                    fName3 = String.Format("xl_{0}_{1}{2}", order1, tickes, fi3.Extension);
                    fi1.CopyTo(Path.Combine(path, fName1));
                    fi2.CopyTo(Path.Combine(path, fName2));
                    if (fi5.Exists) fi5.CopyTo(Path.Combine(path, fName3));

                    try
                    {
                        fi1.Delete();
                        fi2.Delete();
                        if (fi3.Exists) fi3.Delete();
                    }
                    catch
                    {
                    }

                    fName1 = String.Format("{0}_{1}{2}", (isup) ? (++order2) : (--order2), tickes, fi1.Extension);
                    fName2 = String.Format("thmb_{0}_{1}{2}", order2, tickes, fi2.Extension);
                    fName3 = String.Format("xl_{0}_{1}{2}", order2, tickes, fi3.Extension);
                    fi3.CopyTo(Path.Combine(path, fName1));
                    fi4.CopyTo(Path.Combine(path, fName2));
                    if (fi6.Exists) fi6.CopyTo(Path.Combine(path, fName3));

                    try
                    {
                        fi3.Delete();
                        fi4.Delete();
                        if (fi6.Exists) fi3.Delete();
                    }
                    catch
                    {
                    }
                }
                else
                {
                    List<Image> images = dataContext.spAuction_GetAuctionImages(auction_id).ToList();
                    Image img = dataContext.Images.SingleOrDefault(I => I.ID == image_id);
                    if (img == null || images.Count() == 1 || (img.Order == 1 && isup) || (img.Order == images.Count() && !isup))
                        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, String.Format("You can't move {0} this image.", ((isup) ? "up" : "down")));
                    Image img2 = images.Where(I => ((I.Order <= img.Order && isup) || (I.Order >= img.Order && !isup)) && I.ID != img.ID).AsQueryable().OrderBy(((isup) ? "Order desc" : "Order asc")).FirstOrDefault();
                    int order = img.Order;
                    img.Order = img2.Order;
                    img2.Order = order;

                    if (isup && img2.Default)
                    {
                        img2.Default = false;
                        img.Default = true;
                    }
                    else if (!isup && img.Default)
                    {
                        img2.Default = true;
                        img.Default = false;
                    }

                    RenameImageByOrder(img, auction_id);
                    RenameImageByOrder(img2, auction_id);

                    GeneralRepository.SubmitChanges(dataContext);
                }
            }
            catch (IOException ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //SetImageAsDefault
        public JsonExecuteResult SetImageAsDefault(long auction_id, long image_id)
        {
            try
            {
                Auction auc = GetAuction(auction_id);
                if (auc == null)
                {
                    string path = DiffMethods.GetAuctionImageDirForUser(AppHelper.CurrentUser.ID);
                    string[] files = Directory.GetFiles(path, "thmb_*.*");
                    if (files.Count() < 2 || image_id == 1)
                        return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);

                    FileInfo fi1, fi2, fi3;
                    string fName1, fName2, fName3;
                    long tickes = DateTime.Now.Ticks;
                    int order = 1;

                    fi2 = new FileInfo(files[image_id - 1]);
                    fi1 = new FileInfo(Path.Combine(path, fi2.Name.Remove(0, 5)));
                    fi3 = new FileInfo(Path.Combine(path, "xl_" + fi2.Name.Remove(0, 5)));
                    if (!fi1.Exists || !fi2.Exists) throw new Exception("File doesn't exist.");
                    fName1 = String.Format("{0}_{1}{2}", order, tickes, fi1.Extension);
                    fName2 = String.Format("thmb_{0}_{1}{2}", order, tickes, fi2.Extension);
                    fName3 = String.Format("xl_{0}_{1}{2}", order, tickes, fi3.Extension);
                    fi1.CopyTo(Path.Combine(path, fName1));
                    fi2.CopyTo(Path.Combine(path, fName2));
                    if (fi3.Exists) fi3.CopyTo(Path.Combine(path, fName3));
                    try
                    {
                        fi1.Delete();
                        fi2.Delete();
                        if (fi3.Exists) fi3.Delete();
                    }
                    catch
                    {
                    }

                    files = files.Where(F => F.CompareTo(files[image_id - 1]) != 0).ToArray();
                    foreach (string f in files)
                    {
                        fi2 = new FileInfo(f);
                        fi1 = new FileInfo(Path.Combine(path, fi2.Name.Remove(0, 5)));
                        fi3 = new FileInfo(Path.Combine(path, "xl_" + fi2.Name.Remove(0, 5)));
                        if (!fi1.Exists || !fi2.Exists) continue;
                        fName1 = String.Format("{0}_{1}{2}", ++order, tickes, fi1.Extension);
                        fName2 = String.Format("thmb_{0}_{1}{2}", order, tickes, fi2.Extension);
                        fName3 = String.Format("xl_{0}_{1}{2}", order, tickes, fi3.Extension);
                        fi1.CopyTo(Path.Combine(path, fName1));
                        fi2.CopyTo(Path.Combine(path, fName2));
                        if (fi3.Exists) fi3.CopyTo(Path.Combine(path, fName3));

                        try
                        {
                            fi1.Delete();
                            fi2.Delete();
                            if (fi3.Exists) fi3.Delete();
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    List<Image> images = dataContext.spAuction_GetAuctionImages(auction_id).ToList();
                    Image img = dataContext.Images.SingleOrDefault(I => I.ID == image_id);
                    if (img == null || images.Count() == 1 || img.Order == 1)
                        return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
                    img.Default = true;
                    img.Order = 1;
                    RenameImageByOrder(img, auction_id);
                    int order = 1;
                    foreach (Image i in images)
                    {
                        if (i.ID == img.ID) continue;
                        i.Order = ++order;
                        i.Default = false;
                        RenameImageByOrder(i, auction_id);
                    }
                    GeneralRepository.SubmitChanges(dataContext);
                }
            }
            catch (IOException ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //ResortImages
        public JsonExecuteResult ResortImages(long auction_id)
        {
            try
            {
                Auction auc = GetAuction(auction_id);
                int order = 0;
                if (auc == null)
                {
                    string path = DiffMethods.GetAuctionImageDirForUser(AppHelper.CurrentUser.ID);
                    string[] files = Directory.GetFiles(path, "thmb_*.*");

                    FileInfo fi1, fi2, fi3;
                    long tickes = DateTime.Now.Ticks;
                    foreach (string f in files)
                    {
                        fi2 = new FileInfo(f);
                        fi1 = new FileInfo(Path.Combine(path, fi2.Name.Remove(0, 5)));
                        fi3 = new FileInfo(Path.Combine(path, "xl_" + fi2.Name.Remove(0, 5)));
                        if (!fi1.Exists || !fi2.Exists || !fi3.Exists) continue;
                        string fName1, fName2, fName3;
                        fName1 = String.Format("{0}_{1}{2}", ++order, tickes, fi1.Extension);
                        fName2 = String.Format("thmb_{0}_{1}{2}", order, tickes, fi2.Extension);
                        fName3 = String.Format("xl_{0}_{1}{2}", order, tickes, fi3.Extension);
                        fi1.CopyTo(Path.Combine(path, fName1));
                        fi2.CopyTo(Path.Combine(path, fName2));
                        fi3.CopyTo(Path.Combine(path, fName3));
                        try
                        {
                            fi1.Delete();
                            fi2.Delete();
                            if (fi3.Exists) fi3.Delete();
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    List<Image> images = GetAuctionImages(auction_id);
                    if (images.Count() == 0) return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
                    images.ForEach(i => i.Order = ++order);
                    images.First().Default = true;
                    images.Where(i => i.Order > 1).ToList().ForEach(i2 => i2.Default = false);
                    GeneralRepository.SubmitChanges(dataContext);
                }
            }
            catch (IOException ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //RenameImageByOrder
        private bool RenameThumbnail(Image image, long auction_id)
        {
            long tickes = DateTime.Now.Ticks;
            string path = DiffMethods.CheckOrCreateDirectory(auction_id);
            FileInfo fi2 = new FileInfo(Path.Combine(path, image.ThumbNailPath));
            if (!fi2.Exists) return false;
            string fName2 = String.Format("thmb_{0}-{1}_{2}{3}", auction_id, image.Order, tickes, fi2.Extension);
            fi2.CopyTo(Path.Combine(path, fName2));
            try
            {
                fi2.Delete();
            }
            catch
            {
                return false;
            }
            image.ThumbNailPath = fName2;
            return true;
        }

        //CropImage
        public JsonExecuteResult CropImage(long auction_id, string smallimage, string mediumimage, int X, int Y, int W, int H)
        {
            try
            {
                string path = DiffMethods.CheckOrCreateDirectory(auction_id, AppHelper.CurrentUser.ID);
                System.Drawing.Image img = System.Drawing.Image.FromFile(Path.Combine(path, mediumimage));
                Bitmap bmp = new Bitmap(W, H, img.PixelFormat);
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(Color.White);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.DrawImage(img, new Rectangle(0, 0, W, H), new Rectangle(X, Y, W, H), GraphicsUnit.Pixel);
                g.Dispose();
                img = (bmp.Width <= Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Small) || bmp.Height <= Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Small)) ? bmp : DiffMethods.ChangeImageSize(bmp, Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Small), Consts.GetAuctionImageSize(Consts.AuctionImagesSize.Small), false);
                img.Save(Path.Combine(path, smallimage), System.Drawing.Imaging.ImageFormat.Jpeg);
                bmp.Dispose();
                img.Dispose();

                Auction auc = GetAuction(auction_id);
                if (auc == null)
                {
                    FileInfo fi1, fi2, fi3;
                    long tickes = DateTime.Now.Ticks;
                    fi1 = new FileInfo(Path.Combine(path, smallimage));
                    fi2 = new FileInfo(Path.Combine(path, mediumimage));
                    fi3 = new FileInfo(Path.Combine(path, "xl_" + mediumimage));
                    if (fi1.Exists || fi2.Exists || fi3.Exists)
                    {
                        int order = Convert.ToInt32(fi2.Name.Substring(0, fi2.Name.IndexOf("_")));
                        string fName = String.Format("thmb_{0}_{1}{2}", order, tickes, fi1.Extension);
                        fi1.CopyTo(Path.Combine(path, fName));
                        fName = String.Format("{0}_{1}{2}", order, tickes, fi2.Extension);
                        fi2.CopyTo(Path.Combine(path, fName));
                        fName = String.Format("xl_{0}_{1}{2}", order, tickes, fi3.Extension);
                        fi3.CopyTo(Path.Combine(path, fName));
                        try
                        {
                            fi1.Delete();
                            fi3.Delete();
                            fi2.Delete();
                        }
                        catch (IOException ex)
                        {
                            //Logger.LogException("CropImage", ex);
                        }
                    }
                }
                else
                {
                    Image im = GetAuctionImages(auction_id).Where(i => i.ThumbNailPath == smallimage).FirstOrDefault();
                    if (im != null)
                    {
                        RenameThumbnail(im, auc.ID);
                        GeneralRepository.SubmitChanges(dataContext);
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //GetAuctionImgages
        public object GetAuctionImages(string sidx, string sord, int page, int rows)
        {
            string[] files = Directory.GetFiles(DiffMethods.GetUploadedImageDir(), "*.*");
            List<FileInfo> fi = files.Select(s => new FileInfo(s)).ToList();
            fi = fi.OrderBy(F => F.Name).ToList();

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = fi.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            fi = fi.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from query in fi
                    select new
                    {
                        i = query.Name,
                        cell = new string[] {
                query.Name,
                String.Format("<img src='{0}' style='max-width:64px; max-height:64px' />", DiffMethods.GetUploadedImageWeb(query.Name)),
                Path.GetFileNameWithoutExtension(query.FullName),
                Path.GetExtension(query.FullName),
                query.CreationTime.ToString()
              }
                    }).ToArray()
            };

            return jsonData;
        }

        //UploadImage
        public JsonExecuteResult UploadImage(HttpPostedFileBase file)
        {
            try
            {
                string path = DiffMethods.GetUploadedImageDir();
                System.Drawing.Image img = System.Drawing.Image.FromStream(file.InputStream);
                try
                {
                    FileInfo fi = new FileInfo(Path.Combine(path, file.FileName));
                    if (fi.Exists) fi.Delete();
                    img.Save(fi.FullName);
                }
                catch (System.Runtime.InteropServices.ExternalException ex)
                {
                    throw new System.Runtime.InteropServices.ExternalException(ex.Message);
                }
                catch (Exception ex)
                {
                    return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
                }
                finally
                {
                    img.Dispose();
                }
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //DeleteUploadedImage
        public JsonExecuteResult DeleteUploadedImages(string filenames)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                FileInfo fi;
                string[] files = filenames.Split(',');
                foreach (string f in files)
                {
                    fi = new FileInfo(Path.Combine(DiffMethods.GetUploadedImageDir(), f));
                    if (!fi.Exists)
                    {
                        sb.Append(fi.Name + ",");
                        continue;
                    }
                    try
                    {
                        fi.Delete();
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return (sb.Length > 0) ? new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "Files doesn't exist: " + sb.Remove(sb.Length - 1, 1).ToString()) : new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //EditUploadedImage
        public JsonExecuteResult EditUploadedImages(string oldfile, string newfile)
        {
            try
            {
                FileInfo fi = new FileInfo(Path.Combine(DiffMethods.GetUploadedImageDir(), oldfile));
                if (!fi.Exists) throw new Exception("The source file doesn't exist");
                FileInfo fiD = new FileInfo(Path.Combine(DiffMethods.GetUploadedImageDir(), newfile + fi.Extension));
                if (fiD.Exists) throw new Exception("The file with the same name already exists");
                fi.MoveTo(fiD.FullName);
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //AsignImages
        public JsonExecuteResult AsignImages(string[] filenames, long? event_id)
        {
            StringBuilder sbE = new StringBuilder();
            StringBuilder sbO = new StringBuilder();
            StringBuilder sbNM = new StringBuilder();
            StringBuilder sbEv = new StringBuilder();
            StringBuilder res = new StringBuilder();
            JsonExecuteResult jer;
            try
            {
                string[] filestomove = (filenames == null) ? Directory.GetFiles(DiffMethods.GetUploadedImageDir(), "*.*") : filenames;
                List<string> files = filestomove.ToList();
                if (files.Count() == 0) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The image list for asigning to lots is empty.");
                files.Sort();
                FileInfo fi;
                foreach (string file in files)
                {
                    fi = new FileInfo(Path.Combine(DiffMethods.GetUploadedImageDir(), file));
                    if (!fi.Exists)
                    {
                        sbE.AppendLine(fi.Name + ", ");
                        continue;
                    }
                    Regex regex = new Regex(@"^\d{1,14}([a-zA-Z])?$", RegexOptions.Singleline);
                    Match match = regex.Match(Path.GetFileNameWithoutExtension(fi.FullName));
                    if (!match.Success)
                    {
                        sbNM.Append(fi.Name + ", ");
                        continue;
                    }
                    regex = new Regex(@"^\d{1,14}", RegexOptions.Singleline);
                    match = regex.Match(Path.GetFileNameWithoutExtension(fi.FullName));
                    long auction_id = 0;
                    Auction auc;
                    if (!Int64.TryParse(match.Value, out auction_id) || (auc = GetAuction(auction_id)) == null)
                    {
                        sbNM.Append(fi.Name + ", ");
                        continue;
                    }
                    if (dataContext.Images.Where(I => I.Auction_ID == auc.ID && I.UploadedFileName.CompareTo(fi.Name) == 0).Count() > 0)
                    {
                        sbO.Append(fi.Name + ", ");
                        fi.Delete();
                        continue;
                    }
                    if (event_id.HasValue && auc.Event_ID != event_id.Value)
                    {
                        sbEv.Append(fi.Name + ", ");
                        continue;
                    }
                    jer = UploadImage(auc, fi.FullName);
                    if (jer.Type == JsonExecuteResultTypes.SUCCESS)
                    {
                        auc.IsPhotographed = true;
                        fi.Delete();
                    }
                    if (String.IsNullOrEmpty(jer.Message))
                        Logger.LogInfo(jer.Message);
                }
                GeneralRepository.SubmitChanges(dataContext);
                if (sbE.Length > 0)
                    res.AppendLine("<li><b>File(s) doesn't exist:</b> " + sbE.Remove(sbE.Length - 2, 2).ToString() + "</li>");
                if (sbNM.Length > 0)
                    res.AppendLine("<li><b>File(s) doesn't match:</b> " + sbNM.Remove(sbNM.Length - 2, 2).ToString() + "</li>");
                if (sbO.Length > 0)
                    res.AppendLine("<li><b>File(s) are already assigned:</b> " + sbO.Remove(sbO.Length - 2, 2).ToString() + "</li>");
                if (sbEv.Length > 0)
                    res.AppendLine("<li><b>File(s) are from another event:</b> " + sbEv.Remove(sbEv.Length - 2, 2).ToString() + "</li>");
                if (res.Length > 0) throw new Exception("This is the error/warning list:<ul>" + res.ToString() + "</ul>");
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //GetAuctionImages
        public List<Image> GetAuctionImages(long auction_id)
        {
            return dataContext.spAuction_GetAuctionImages(auction_id).ToList();
        }

        //GetImagesByAuction
        public object GetImagesByAuction(long auction_id)
        {
            List<Image> images = GetAuctionImages(auction_id);

            StringBuilder sbRow = new StringBuilder();
            foreach (Image img in images)
                sbRow.AppendFormat("<img src='{0}' style='max-width:100px; max-height:100px;vertical-align:middle' />&nbsp;", DiffMethods.GetAuctionImageWebPath(auction_id, img.ThumbNailPath));

            string[] res = new string[] { sbRow.ToString() };

            var jsonData = new
            {
                total = images.Count(),
                page = 0,
                records = images.Count(),
                rows = (
                    from s in res
                    select new
                    {
                        i = 0,
                        cell = new string[] {
                s
              }
                    }).ToArray()
            };

            images = null;

            return jsonData;
        }

        //AddWinnerToAuction
        public JsonExecuteResult AddWinnerToAuction(long auction_id, long user_id)
        {
            try
            {
                Auction auction = GetAuction(auction_id);
                if (auction.Status != (byte)Consts.AuctionStatus.Closed) throw new Exception("This lot is not closed.");
                if (auction.Event.CloseStep != 1) throw new Exception("You can't use this functionality to add the winner on this lot.");
                if (auction.Bids.Any()) throw new Exception("You can't add the winner for this lot, because this lot has already a winner");
                User user = dataContext.Users.SingleOrDefault(U => U.ID == user_id);
                if (user == null) throw new Exception("This user doesn't exists");
                if (user.UserStatus_ID != (byte)Consts.UserStatus.Active) throw new Exception("This user can't be a winner because his status is not active.");
                if (user.UserType_ID != (byte)Consts.UserTypes.Buyer && user.UserType_ID != (byte)Consts.UserTypes.SellerBuyer) throw new Exception("This user can't be a winner because his isn't a buyer.");

                Bid bid = new Bid();
                bid.Amount = bid.MaxBid = auction.Price;
                bid.Auction_ID = auction_id;
                bid.DateMade = auction.EndDate.AddMinutes(-1);
                bid.IP = Consts.UsersIPAddress;
                bid.IsProxy = false;
                bid.Quantity = auction.Quantity;
                bid.User_ID = user_id;
                dataContext.Bids.InsertOnSubmit(bid);

                BidCurrent bidcur = new BidCurrent();
                bidcur.Amount = bidcur.MaxBid = auction.Price;
                bidcur.Auction_ID = auction_id;
                bidcur.DateMade = auction.EndDate.AddMinutes(-1);
                bidcur.IP = Consts.UsersIPAddress;
                bidcur.IsProxy = false;
                bidcur.Quantity = auction.Quantity;
                bidcur.User_ID = user_id;
                bidcur.IsActive = true;
                dataContext.BidCurrents.InsertOnSubmit(bidcur);

                BidLog blog = new BidLog
                  {
                      Amount = bid.Amount,
                      Auction_ID = bid.Auction_ID,
                      DateMade = bid.DateMade,
                      IP = bid.IP,
                      IsProxy = bid.IsProxy,
                      IsProxyRaise = false,
                      MaxBid = bid.MaxBid,
                      Quantity = bid.Quantity,
                      User_ID = bid.User_ID
                  };
                dataContext.BidLogs.InsertOnSubmit(blog);

                BidLogCurrent blogcur = new BidLogCurrent
                  {
                      Amount = bid.Amount,
                      Auction_ID = bid.Auction_ID,
                      DateMade = bid.DateMade,
                      IP = bid.IP,
                      IsProxy = bid.IsProxy,
                      IsProxyRaise = false,
                      MaxBid = bid.MaxBid,
                      Quantity = bid.Quantity,
                      User_ID = bid.User_ID
                  };
                dataContext.BidLogCurrents.InsertOnSubmit(blogcur);

                BidWatchCurrent bw = new BidWatchCurrent { Auction_ID = auction_id, User_ID = user_id };
                dataContext.BidWatchCurrents.InsertOnSubmit(bw);

                EventRegistration er = dataContext.spSelect_EventRegistration(auction.Event_ID, user_id).SingleOrDefault();
                if (er == null)
                    dataContext.EventRegistrations.InsertOnSubmit(new EventRegistration { Event_ID = auction.Event_ID, User_ID = user_id });

                GeneralRepository.SubmitChanges(dataContext);

                UpdateAuctionBiddingResult(bid.Auction_ID, bid.User_ID, bid.Amount, bid.MaxBid);

                try
                {
                    System.Net.WebClient client = new System.Net.WebClient();
                    client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                    client.OpenRead(Consts.CacheClearFrontendIP + Consts.FrontEndClearARPMethod + "/" + auction_id.ToString());
                }
                catch (Exception ex)
                {
                    Logger.LogException("[" + Consts.CacheClearFrontendIP + Consts.FrontEndClearARPMethod + "/" + auction_id.ToString() + "]", ex);
                }
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //AddWinnerToAuction
        public JsonExecuteResult AddRightsToAuction(long auction_id, long user_id)
        {
            try
            {
                Auction auction = GetAuction(auction_id);
                if (auction.Status != (byte)Consts.AuctionStatus.Open) throw new Exception("This lot is not open.");
                if (auction.Event.CloseStep != 1) throw new Exception("You can't use this functionality to add the winner on this lot.");
                User user = dataContext.Users.SingleOrDefault(U => U.ID == user_id);
                if (user == null) throw new Exception("This user doesn't exists");
                if (user.UserStatus_ID != (byte)Consts.UserStatus.Active) throw new Exception("This user can't be a winner because his status is not active.");
                if (user.UserType_ID != (byte)Consts.UserTypes.Buyer && user.UserType_ID != (byte)Consts.UserTypes.SellerBuyer) throw new Exception("This user can't be a winner because his isn't a buyer.");
                AuctionBiddingRight abr = dataContext.AuctionBiddingRights.SingleOrDefault(U => U.User_ID == user_id && U.Auction_ID == auction_id);
                if (abr != null) throw new Exception("This user has already rights to participate in bidding on this lot.");

                abr = new AuctionBiddingRight { Auction_ID = auction_id, User_ID = user_id };
                dataContext.AuctionBiddingRights.InsertOnSubmit(abr);

                EventRegistration er = dataContext.EventRegistrations.SingleOrDefault(ER => ER.User_ID == user_id && ER.Event_ID == auction.Event_ID);
                if (er == null)
                    dataContext.EventRegistrations.InsertOnSubmit(new EventRegistration { Event_ID = auction.Event_ID, User_ID = user_id });

                GeneralRepository.SubmitChanges(dataContext);
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //RemoveAuctionResultsCache
        public void RemoveAuctionResultsCache(long auction_id)
        {
            CacheRepository.Remove(new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTIONDETAILRESULT", new object[] { auction_id }));
        }

        //UpdateAuctionBiddingResult
        public void UpdateAuctionBiddingResult(long auction_id, long? user_id, decimal? currentbid, decimal? maxbid)
        {
            try
            {
                AuctionResultsCurrent ar = dataContext.AuctionResultsCurrents.Where(AR => AR.Auction_ID == auction_id).FirstOrDefault();
                if (ar == null)
                {
                    ar = new AuctionResultsCurrent();
                    dataContext.AuctionResultsCurrents.InsertOnSubmit(ar);
                }
                ar.Auction_ID = auction_id;
                ar.User_ID = user_id;
                ar.CurrentBid = currentbid;
                ar.MaxBid = maxbid;
                ar.Bids = (dataContext.spBid_LogCount(auction_id).FirstOrDefault() ?? new spBid_LogCountResult()).LogCount.GetValueOrDefault(0);
                //dataContext.spUpdate_AuctionResults(ar.ID, ar.Auction_ID, ar.User_ID, ar.CurrentBid, ar.Bids, ar.MaxBid);
                dataContext.spUpdate_AuctionResultsCurrent(ar.ID, ar.Auction_ID, ar.User_ID, ar.CurrentBid, ar.Bids, ar.MaxBid);
                RemoveAuctionResultsCache(auction_id);
            }
            catch (Exception ex)
            {
                Logger.LogException(String.Format("[auction_id={0}; user={1}; cb={2}; maxbid={3}", auction_id, user_id, currentbid, maxbid), ex);
            }
        }

        //GetBidsByAuctionID
        public object GetBidsByAuctionID(string sidx, string sord, int page, int rows, long auction_id)
        {
            Event evnt = dataContext.Auctions.Where(A => A.ID == auction_id).Select(A1 => A1.Event).SingleOrDefault();
            bool iscur = evnt.IsCurrent;
            IQueryable<IBid> bids = iscur ?
              (dataContext.BidCurrents.Where(A => A.Auction_ID == auction_id).OrderByDescending(B1 => B1.Amount).ThenByDescending(B2 => B2.MaxBid).ThenBy(B3 => B3.DateMade).Cast<IBid>()) :
              (dataContext.Bids.Where(A => A.Auction_ID == auction_id).OrderByDescending(B1 => B1.Amount).ThenByDescending(B2 => B2.MaxBid).ThenBy(B3 => B3.DateMade)).Cast<IBid>();

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = bids.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            bids = bids.Skip(pageIndex * pageSize).Take(pageSize);

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from query in bids.ToList()
                    select new
                    {
                        i = query.ID,
                        cell = new string[] {
                query.ID.ToString(),
                iscur ? (query as BidCurrent).User_ID.ToString() : (query as Bid).User_ID.ToString(),
                iscur ? (query as BidCurrent).User.Login : (query as Bid).User.Login,
                query.Amount.GetPrice().ToString("0.00"),
                query.MaxBid.GetPrice().ToString("0.00"),
                query.Quantity.ToString(),
                query.DateMade.ToString(),
                query.IsProxy.ToString(),                
                query.IP
              }
                    }).ToArray()
            };

            bids = null;
            return jsonData;
        }

        //GetBidsLogByAuctionID
        public object GetBidsLogByAuctionID(string sidx, string sord, int page, int rows, long auction_id)
        {
            Event evnt = dataContext.Auctions.Where(A => A.ID == auction_id).Select(A1 => A1.Event).SingleOrDefault();
            int sec = DateTime.Now.Subtract(evnt.DateEnd).Seconds;
            bool iscur = evnt.IsCurrent;
            IQueryable<IBidLog> bids = iscur ?
              (dataContext.BidLogCurrents.Where(B => B.Auction_ID == auction_id).OrderByDescending(B1 => B1.DateMade)).Cast<IBidLog>() :
              (dataContext.BidLogs.Where(B => B.Auction_ID == auction_id).OrderByDescending(B1 => B1.DateMade)).Cast<IBidLog>();

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = bids.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            bids = bids.Skip(pageIndex * pageSize).Take(pageSize);

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from query in bids.ToList()
                    select new
                    {
                        i = query.ID,
                        cell = new string[] {
                query.ID.ToString(),
                iscur ? (query as BidLogCurrent).User_ID.ToString() : (query as BidLog).User_ID.ToString(),
                iscur ? (query as BidLogCurrent).User.Login : (query as BidLog).User.Login,
                query.Amount.GetPrice().ToString("0.00"),
                query.MaxBid.GetPrice().ToString("0.00"),
                query.Quantity.ToString(),
                query.DateMade.ToString("g"),
                query.IsAutoBid.ToString(),
                query.IsProxy.ToString(),
                query.IsProxyRaise.ToString(),
                query.IP
              }
                    }).ToArray()
            };

            bids = null;

            return jsonData;
        }

        //UpdateLotNumber
        public JsonExecuteResult UpdateLotNumber(long auction_id, short? lot)
        {
            try
            {
                Auction auction = dataContext.Auctions.Where(a => a.ID == auction_id).SingleOrDefault();
                if (auction == null) throw new Exception("The item doesn't exist");
                List<Auction> list =
                  dataContext.Auctions.Where(
                    a =>
                    a.Event_ID == auction.Event_ID && a.ID != auction_id &&
                    a.Lot.GetValueOrDefault(-1) == lot.GetValueOrDefault(0)).ToList();
                if (list.Count() > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    list.ForEach(i => sb.Append(i.ID + "|"));
                    throw new Exception("The item with the same Lot number already exists in this event: " + sb.ToString());
                }
                auction.Lot = lot;
                GeneralRepository.SubmitChanges(dataContext);
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //UpdateShipping
        public JsonExecuteResult UpdateShipping(long auction_id, decimal shipping)
        {
            try
            {
                Auction auction = dataContext.Auctions.Where(a => a.ID == auction_id).SingleOrDefault();
                if (auction == null) throw new Exception("The item doesn't exist");
                auction.Shipping = shipping;
                GeneralRepository.SubmitChanges(dataContext);
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //GetContractItems
        //public object GetContractItems(string sidx, string sord, int page, int rows, long cons_id)
        //{
        //  List<Contract> result = dataContext.spContract_List(cons_id).ToList();

        //  int pageIndex = Convert.ToInt32(page) - 1;
        //  int pageSize = rows;
        //  int totalRecords = result.Count();
        //  int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

        //  result = result.Skip(pageIndex * pageSize).Take(pageSize).ToList();

        //  var jsonData = new
        //  {
        //    total = totalPages,
        //    page = page,
        //    records = totalRecords,
        //    rows = (
        //        from query in result
        //        select new
        //        {
        //          i = query.ID,
        //          cell = new string[] {
        //            query.ID.ToString(),
        //            query.Title,
        //            query.CommissionRate.Description
        //          }
        //        }).ToArray()
        //  };
        //  return jsonData;
        //}

        ////UpdateContractItem
        //public JsonExecuteResult UpdateContractItem(int? id, string title, int commissionRate_ID, long cons_id)
        //{
        //  try
        //  {
        //    Contract contract = dataContext.Contracts.Where(C => C.ID == id.GetValueOrDefault(-1)).FirstOrDefault();
        //    if (contract==null)
        //    {
        //      contract = new Contract();
        //      dataContext.Contracts.InsertOnSubmit(contract);
        //    }
        //    contract.Consignment_ID = cons_id;
        //    contract.Title = title;
        //    contract.CommissionRate_ID = commissionRate_ID;
        //    GeneralRepository.SubmitChanges(dataContext);
        //  }
        //  catch(Exception ex)
        //  {
        //    return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
        //  }
        //  return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        //}
    }
}

