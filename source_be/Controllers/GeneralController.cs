using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Vauction.Models;
using Vauction.Models.CustomClasses;
using Vauction.Utils;
using Vauction.Utils.Autorization;
using Vauction.Utils.Lib;
using Vauction.Utils.Perfomance;

namespace Vauction.Controllers
{
  [Compress]
  public class GeneralController : BaseController
  {
    private readonly IGeneralRepository GeneralRepository;
    public GeneralController()
    {
      GeneralRepository = dataProvider.GeneralRepository;
    }

    #region get JSON
    //GetUserTypesJSON
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet, ActionOutputCacheAttribute(CachingExpirationTime.Days_01)]
    public JsonResult GetUserTypesJSON()
    {
      return JSON(GeneralRepository.GetUserTypesJSON());
    }
    //GetUserStatusesJSON
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet, ActionOutputCacheAttribute(CachingExpirationTime.Days_01)]
    public JsonResult GetUserStatusesJSON()
    {
      return JSON(GeneralRepository.GetUserStatusesJSON());
    }
    //GetCommissionRatesJSON
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet, ActionOutputCacheAttribute(CachingExpirationTime.Hours_01)]
    public JsonResult GetCommissionRatesJSON()
    {
      return JSON(GeneralRepository.GetCommissionRatesJSON());
    }
    //GetCountryStatesJSON
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet, ActionOutputCacheAttribute(CachingExpirationTime.Days_01)]
    public JsonResult GetCountryStatesJSON()
    {
      return JSON(GeneralRepository.GetCountryStatesJSON());
    }
    //GetCountriesJSON
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet, ActionOutputCacheAttribute(CachingExpirationTime.Days_01)]
    public JsonResult GetCountriesJSON()
    {
      return JSON(GeneralRepository.GetCountriesJSON());
    }
    //GetSpecialistsJSON
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet, ActionOutputCacheAttribute(CachingExpirationTime.Days_01)]
    public JsonResult GetSpecialistsJSON()
    {
      return JSON(GeneralRepository.GetSpecialistsJSON());
    }
    //GetEventsJSON
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet, ActionOutputCacheAttribute(CachingExpirationTime.Minutes_30)]
    public JsonResult GetEventsJSON()
    {
      return JSON(GeneralRepository.GetEventsJSON());
    }
    //GetEventsJSON
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet, ActionOutputCacheAttribute(CachingExpirationTime.Minutes_30)]
    public JsonResult GetEventsViewableJSON()
    {
      return JSON(GeneralRepository.GetEventsViewableJSON());
    }
    //GetPendingEventsJSON
    [VauctionAuthorize(Roles = "Root,Admin,Writer,Specialist,SpecialistViewer"), HttpGet, ActionOutputCacheAttribute(CachingExpirationTime.Minutes_30)]
    public JsonResult GetPendingEventsJSON()
    {
      return JSON(GeneralRepository.GetPendingEventsJSON());
    }
    //GetAuctionStatusesJSON
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet, ActionOutputCacheAttribute(CachingExpirationTime.Days_01)]
    public JsonResult GetAuctionStatusesJSON()
    {
      return JSON(GeneralRepository.GetAuctionStatusesJSON());
    }
    //GetMainCategoriesJSON
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet, ActionOutputCacheAttribute(CachingExpirationTime.Minutes_30)]
    public JsonResult GetMainCategoriesJSON()
    {
      return JSON(GeneralRepository.GetMainCategoriesJSON());
    }
    //GetPurchasedTypesJSON
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet, ActionOutputCacheAttribute(CachingExpirationTime.Minutes_30)]
    public JsonResult GetPurchasedTypesJSON(bool? isnull)
    {
      return JSON(GeneralRepository.GetPurchasedTypesJSON(isnull.GetValueOrDefault(false)));
    }
    //GetMainCategoriesAndForSaleJSON
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet, ActionOutputCacheAttribute(CachingExpirationTime.Minutes_30)]
    public JsonResult GetMainCategoriesAndForSaleJSON()
    {
      return JSON(GeneralRepository.GetMainCategoriesAndForSaleJSON());
    }
    //GetEventsDateTimeJSON
    [VauctionAuthorize(Roles = "Root,Admin"), HttpGet, ActionOutputCacheAttribute(CachingExpirationTime.Minutes_30)]
    public JsonResult GetEventsDateTimeJSON()
    {
      return JSON(GeneralRepository.GetEventsDateTimeJSON());
    }
    //GetEventsClosedDateTimeJSON
    [VauctionAuthorize(Roles = "Root,Admin"), HttpGet, ActionOutputCacheAttribute(CachingExpirationTime.Minutes_30)]
    public JsonResult GetEventsClosedDateTimeJSON()
    {
      return JSON(GeneralRepository.GetEventsClosedDateTimeJSON());
    }
    //GetEventsClosedDateTimeShortJSON
    [VauctionAuthorize(Roles = "Root,Admin"), HttpGet, ActionOutputCacheAttribute(CachingExpirationTime.Minutes_30)]
    public JsonResult GetEventsClosedDateTimeShortJSON()
    {
      return JSON(GeneralRepository.GetEventsClosedDateTimeShortJSON());
    }
    #endregion





    // NOT DONE

    [VauctionAuthorize(Roles = "Root,Admin")]
    public ActionResult General()
    {
      return View();
    }

    //CountryList
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult CountryList(string sidx, string sord, int page, int rows, bool _search, string Country_ID, string Title, string Code)
    {
      if (sidx == "Country_ID") sidx = "ID";
      return JSON(GeneralRepository.GetCountryList(sidx, sord, page, rows, _search, Country_ID, Title, Code));
    }
    //EditCountry
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult EditCountry(string Id, string Title, string Code, string oper)
    {
      long ID;
      bool good = false;
      if (oper == "del")
      {
        if (!Int64.TryParse(Id, out ID)) return JSON(false); ;
        good = GeneralRepository.DeleteCountry(ID);
        if (!good)
        {
          Response.StatusCode = 500;
        }
      }
      else
      {
        if (oper == "add") Id = "0";
        if (!Int64.TryParse(Id, out ID)) return JSON(false);
        good = GeneralRepository.AddEditCountry(ID, Title, Code, oper == "add");
        if (!good)
        {
          Response.StatusCode = 500;
        }
      }
      return JSON(true);
    }

    //CommissionRateList
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult CommissionRateList(string sidx, string sord, int page, int rows, bool _search, string RateID, string Description, string MaxPercent)
    {
      return JSON(GeneralRepository.GetCommissionRateList(sidx, sord, page, rows, _search, RateID, Description, MaxPercent));
    }

    //EditCommissionRateList
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult EditCommissionRateList(string Id, string Description, string MaxPercent, string oper, string longDescription)
    {
      long ID;
      decimal MaxPerc;
      bool good = false;
      if (oper == "del")
      {
        if (!Int64.TryParse(Id, out ID)) return JSON(false);
        good = GeneralRepository.DeleteCommissionRate(ID);
        if (!good)
        {
          Response.StatusCode = 500;
        }
      }
      else
      {
        if (oper == "add") Id = "0";
        if (!Int64.TryParse(Id, out ID)) return JSON(false);
        if (!Decimal.TryParse(MaxPercent, out MaxPerc)) return JSON(false);
        good = GeneralRepository.AddEditCommissionRate(ID, Description, MaxPerc, oper == "add", longDescription);
        if (!good)
        {
          Response.StatusCode = 500;
        }
      }
      return JSON(true);
    }

    //VariableList
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult VariableList(string sidx, string sord, int page, int rows, bool _search, string Varibale_ID, string Name, string Value)
    {
      if (sidx == "Varibale_ID") sidx = "ID";
      return JSON(GeneralRepository.GetVariableList(sidx, sord, page, rows, _search, Varibale_ID, Name, Value));
    }

    //EditVariableList
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult EditVariableList(string Id, string Name, string Value, string oper)
    {
      long ID;
      decimal Val;
      bool good = false;
      if (oper == "del")
      {
        if (!Int64.TryParse(Id, out ID)) return JSON(false); ;
        good = GeneralRepository.DeleteVariable(ID);
        if (!good)
        {
          Response.StatusCode = 500;
        }
      }
      else
      {
        if (oper == "add") Id = "0";
        if (!Int64.TryParse(Id, out ID)) return JSON(false);
        if (!Decimal.TryParse(Value, out Val)) return JSON(false);
        good = GeneralRepository.AddEditVariable(ID, Name, Val, oper == "add");
        if (!good)
        {
          Response.StatusCode = 500;
        }
      }
      return JSON(true);
    }

    //StateList
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult StateList(string sidx, string sord, int page, int rows, bool _search, string State_ID, string Title, string Code, string Country_ID)
    {
      if (sidx == "State_ID") sidx = "ID";
      return JSON(GeneralRepository.GetStateList(sidx, sord, page, rows, _search, State_ID, Title, Code, Country_ID));
    }

    //EditStateList
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult EditStateList(string Id, string Title, string Code, string Country_ID, string oper)
    {
      long ID;
      long country_id;
      bool good = false;
      if (oper == "del")
      {
        if (!Int64.TryParse(Id, out ID)) return JSON(false); ;
        good = GeneralRepository.DeleteState(ID);
        if (!good)
        {
          Response.StatusCode = 500;
        }
      }
      else
      {
        if (oper == "add") Id = "0";
        if (!Int64.TryParse(Id, out ID)) return JSON(false);
        if (!Int64.TryParse(Country_ID, out country_id)) return JSON(false);
        good = GeneralRepository.AddEditState(ID, Title, Code, country_id, oper == "add");
        if (!good)
        {
          Response.StatusCode = 500;
        }
      }
      return JSON(true);
    }

    //PrintLabelsForItems
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
    public ActionResult PrintLabelsForItems(string auctions, string amount, long cons_id)
    {
      string[] strA = auctions.Split(',');
      string[] strC = amount.Split(',');
      if (strA.Count() == 0 || strC.Count() == 0) return JSON("");
      Dictionary<long, byte> auctionCount = new Dictionary<long, byte>();
      for (int i = 0; i < strA.Count(); i++)
        auctionCount.Add(Convert.ToInt64(strA[i]), Convert.ToByte(strC[i]));
      return JSON(GeneralRepository.PrintLabelsForItems(auctionCount, cons_id));
    }

    //ConsignmentContractTemplate
    [VauctionAuthorize(Roles = "Root,Admin")]
    public ActionResult ConsignmentContractTemplate()
    {
      string filePath = DiffMethods.TemplateDifferentOnDisk("ConsignmentContract.txt");
      FileInfo fileInfo = new FileInfo(filePath);
      string contractTemplate = fileInfo.Exists ? System.IO.File.ReadAllText(filePath) : string.Empty;
      ViewData["ContractTemplate"] = contractTemplate;
      return View();
    }

    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult SaveConsignmentContractTemplate(string textTemplate)
    {
      try
      {
        string filePath = DiffMethods.TemplateDifferentOnDisk("ConsignmentContract.txt");
        System.IO.File.WriteAllText(filePath, textTemplate);
      }
      catch (Exception exc)
      {
        return JSON(new JsonExecuteResult(JsonExecuteResultTypes.ERROR, exc.Message));
      }
      return JSON(new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, "Data was saved successfully"));
    }

    //GetTags
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult GetTags(string sidx, string sord, int page, int rows, string title)
    {
      List<Tag> tags = GeneralRepository.GetTags().ToList();
      bool asc = sord.ToLower() == "asc";
      switch (sidx.ToLower())
      {
        case "id":
          tags = (asc ? tags.OrderBy(q => q.ID) : tags.OrderByDescending(q => q.ID)).ToList();
          break;
        default:
          tags = (asc ? tags.OrderBy(q => q.Title) : tags.OrderByDescending(q => q.Title)).ToList();
          break;
      }
      if (!string.IsNullOrWhiteSpace(title))
      {
        tags = tags.Where(t => t.Title.ToLower().Contains(title)).ToList();
      }
      int pageIndex = Convert.ToInt32(page) - 1;
      int pageSize = rows;
      int totalRecords = tags.Count();
      int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
      tags = tags.Skip(pageIndex * pageSize).Take(pageSize).ToList();

      return JSON(new
      {
        total = totalPages,
        page = page,
        records = totalRecords,
        rows = (
            from query in tags
            select new
            {
              i = query.ID,
              cell = new string[] { query.ID.ToString(), query.Title, query.IsSystem.ToString(), query.IsViewable.ToString() }
            }).ToArray()
      });
    }

    //UpdateTag
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult UpdateTag(string id, string title, string isviewable, string oper)
    {
      try
      {
        long tagID;
        bool isViewable;
        Boolean.TryParse(isviewable, out isViewable);
        if (oper == "del")
        {
          if (!Int64.TryParse(id, out tagID)) return JSON(false);
          GeneralRepository.DeleteTag(tagID);
        }
        else
        {
          if (oper == "add") id = "0";
          if (!Int64.TryParse(id, out tagID)) return JSON(false);
          GeneralRepository.UpdateTag(new Tag { ID = tagID, Title = title, IsViewable = isViewable});
        }
      }
      catch (Exception ex)
      {
        return JSON(new Object[] { false, ex.Message });
      }
      try
      {
        System.Net.WebClient client = new System.Net.WebClient();
        client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
        client.OpenRead(Consts.CacheClearFrontendIP + Consts.FrontEndClearMethod);
      }
      catch (Exception ex)
      {
        Logger.LogException("[" + Consts.CacheClearFrontendIP + Consts.FrontEndClearMethod + "]", ex);
      }
      return JSON(true);
    }
  }
}


