using System.Data.Linq;
using System.Linq;
using System.Collections.Generic;
using Vauction.Utils.Lib;
using Vauction.Utils.Perfomance;

namespace Vauction.Models
{
  public class InvoiceRepository : IInvoiceRepository
  {
    #region init
    private VauctionDataContext dataContext;
    private ICacheDataProvider CacheRepository;

    public InvoiceRepository(VauctionDataContext dataContext, ICacheDataProvider CacheRepository)
    {
      this.dataContext = dataContext;
      this.CacheRepository = CacheRepository;
    }

    public void SubmitChanges()
    {
      try
      {
        dataContext.SubmitChanges();
      }
      catch (ChangeConflictException e)
      {
        Logger.LogWarning(e.Message);
        foreach (ObjectChangeConflict occ in dataContext.ChangeConflicts)
        {
          occ.Resolve(RefreshMode.KeepCurrentValues);
        }
      }
    }
    #endregion

    //GetUserInvoice
    public UserInvoice GetUserInvoice(long userinvoice_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.INVOICES, "GETUSERINVOICE",
                                                new object[] { userinvoice_id }, CachingExpirationTime.Hours_01);
      UserInvoice ui = CacheRepository.Get(dco) as UserInvoice;
      if (ui != null) return ui;
      ui = dataContext.spSelect_UserInvoices(userinvoice_id).FirstOrDefault();
      if (ui != null)
      {
        dco.Data = ui;
        CacheRepository.Add(dco);
      }
      return ui;
    }

    //GetConsignment
    public Consignment GetConsignment(long consignment_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.INVOICES, "GETCONSIGNMENT",
                                                new object[] { consignment_id }, CachingExpirationTime.Hours_01);
      Consignment result = CacheRepository.Get(dco) as Consignment;
      if (result != null) return result;
      result = dataContext.spSelect_Consignment(consignment_id).FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
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
      SubmitChanges();
      return consignmentContract;

    }

    //GetSpecialist
    public Specialist GetSpecialist(long specialistID)
    {
      return dataContext.Specialists.FirstOrDefault(t => t.ID == specialistID);
    }

    //GetPriceRealizedForEvent
    public List<PriceRealized> GetPriceRealizedForEvent(long event_id)
    {
      return (from p in dataContext.spInvoice_View_PriceRealizedForEvent(event_id)
              select new PriceRealized
              {
                ID = p.Auction_ID,
                Lot = p.Lot.GetValueOrDefault(0),
                Title = p.Title,
                Price = p.Cost,
                LinkParams = new LinkParams { EventTitle = p.EventTitle, MainCategoryTitle = p.MainCategoryTitle, CategoryTitle = p.CategoryTitle }
              }).ToList();
    }

    //GetUserInvoicesForPage
    public List<LinkParams> GetUserInvoicesForPage(long user_id)
    {
      return (from p in dataContext.spInvoice_View_UserInvoices(user_id)
              select new LinkParams
              {
                ID = p.UserInvoice_ID,
                EventTitle = p.EventTitle
              }).ToList();
    }

    //GetUserInvoicePaidAmount
    public decimal GetUserInvoicePaidAmount(long userinvoice_id)
    {
      return dataContext.spInvoice_PaymentSum(userinvoice_id).FirstOrDefault().Amount;
    }

    //GetInvoicesByUserInvoice
    public List<InvoiceDetail> GetInvoiceDetailsByUserInvoice(long userinvoice_id)
    {
      return (from p in dataContext.spInvoice_View_GetInvoicesByUserInvoices(userinvoice_id)
              select new InvoiceDetail
              {
                UserInvoice_ID = p.UserInvoices_ID,
                Invoice_ID = p.Invoice_ID,
                LateFee = p.LateFee,
                Auction_ID = p.Auction_ID,
                Cost = p.Cost,
                Lot = p.Lot.HasValue ? p.Lot.Value : (short)0,
                Title = p.Title,
                LinkParams = new LinkParams { EventTitle = p.EventTitle, MainCategoryTitle = p.MainCategoryTitle, CategoryTitle = p.CategoryTitle },
                SaleDate = p.SaleDate,
                Shipping = p.Shipping,
                Tax = p.Tax
              }).ToList();
    }

    //GetConsignorStatementsForPage
    public List<LinkParams> GetConsignorStatementsForPage(long user_id)
    {
      return (from p in dataContext.spInvoice_ConsignorStatements(user_id)
              select new LinkParams
              {
                ID = p.Consignment_ID,
                EventTitle = p.EventTitle
              }).ToList();
    }

    //GetConsignmentDetailsByConsignmentID
    public List<ConsignmentDetail> GetConsignmentDetailsByConsignmentID(long consignmentID)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.INVOICES, "GETCONSIGNMENTDETAILSBYCONSIGNMENTID",
                                                new object[] { consignmentID }, CachingExpirationTime.Hours_01);
      List<ConsignmentDetail> result = CacheRepository.Get(dco) as List<ConsignmentDetail>;
      if (result != null && result.Any()) return result;
      result = (from p in dataContext.spInvoice_View_GetConsignmentByConsignments(consignmentID)
                select new ConsignmentDetail
                {
                  Consignment_ID = p.Consignment_ID,
                  Invoice_ID = p.Invoice_ID.GetValueOrDefault(0),
                  Reserve = p.Reserve.GetValueOrDefault(0),
                  Cost = p.Cost,
                  Amount = p.Amount,
                  LinkParams = new LinkParams { EventTitle = p.EventTitle, MainCategoryTitle = p.MainCategoryTitle, CategoryTitle = p.CategoryTitle, Lot = p.Lot.HasValue ? p.Lot.Value : (short)0, Title = p.Title, ID = p.Auction_ID },
                  CommissionRate = p.CommRate
                }).ToList();
      if (result.Any())
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //GetConsignmentTotals
    public UICInvoice GetConsignmentTotals(long consignment_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.INVOICES, "GETCONSIGNMENTTOTALS",
                                                new object[] { consignment_id }, CachingExpirationTime.Hours_01);
      UICInvoice result = CacheRepository.Get(dco) as UICInvoice;
      if (result != null) return result;
      int? totalamount = 0;
      result =
        (from p in
           dataContext.spInvoice_View_ConsignorStatements(consignment_id, null, null, null, 0, 1, ref totalamount)
         select new UICInvoice
                  {
                    AmountDue = p.ADue.GetValueOrDefault(0),
                    AmountPaid = p.APaid.GetValueOrDefault(0),
                    TotalCost = p.TotalCost.GetValueOrDefault(0)
                  }).FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }
  }
}
