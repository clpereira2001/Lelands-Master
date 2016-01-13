using System.Configuration;
using System.Web.Mvc;
using System.Web.UI;
using Vauction.Configuration;
using Vauction.Models;

namespace Vauction.Controllers
{
  [ValidateInput(false), OutputCache(Location = OutputCacheLocation.None, NoStore = true, Duration = 0)]
  public class BaseController : Controller
  {
    public IVauctionDataProvider dataProvider;

    public IVauctionConfiguration Config { get; protected set; }
    //public static IUserRepository UserRepository { get; protected set; }
    //protected IEventRepository EventRepository { get; set; }
    //protected IAuctionRepository AuctionRepository { get; set; }
    //protected IBidRepository BidRepository { get; set; }
    //protected IInvoiceRepository InvoiceRepository { get; set; }
    //protected IGeneralRepository GeneralRepository { get; set; }
    //protected ICategoryRepository CategoryRepository { get; set; }
    //protected IReportRepository ReportRepository { get; set; }
    
    public BaseController()
    {
      Config = (IVauctionConfiguration)ConfigurationManager.GetSection("Vauction");

      dataProvider = Config.DataProvider.GetInstance();
      //UserRepository = dataProvider.UserRepository;
      //EventRepository = dataProvider.EventRepository;
      //AuctionRepository = dataProvider.AuctionRepository;
      //BidRepository = dataProvider.BidRepository;
      //InvoiceRepository = dataProvider.InvoiceRepository;
      //GeneralRepository = dataProvider.GeneralRepository;
      //CategoryRepository = dataProvider.CategoryRepository;
      //ReportRepository = dataProvider.ReportRepository;
    }

    public JsonResult JSON(object obj)
    {
      return Json(obj, JsonRequestBehavior.AllowGet);
    }
  }
}