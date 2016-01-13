using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Vauction.Models;
using Vauction.Utils.Autorization;
using Vauction.Utils.Perfomance;

namespace Vauction.Controllers
{
  [HandleError, Compress]
  public class ImageController : BaseController
  {
    private IAuctionRepository AuctionRepository;
    private IGeneralRepository GeneralRepository;
    public ImageController()
    {
      AuctionRepository = dataProvider.AuctionRepository;
      GeneralRepository = dataProvider.GeneralRepository;
    }

    //BatchImagesUpload
    [VauctionAuthorize(Roles = "Root,Admin")]
    public ActionResult BatchImagesUpload()
    {
      return View();
    }

    //GetUploadedImagesList    
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult GetAuctionImages(string sidx, string sord, int page, int rows)
    {
      return JSON(AuctionRepository.GetAuctionImages(sidx, sord, page, rows));
    }

    //DeleteImage
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult DeleteImage(string id, string oper)
    {
      return (String.IsNullOrEmpty(id))?JSON(false):JSON(AuctionRepository.DeleteUploadedImages(id));
    }

    //EditImage
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult EditImage(string id, string oper, string Title)
    {
      return (String.IsNullOrEmpty(id)) ? JSON(false) : JSON(AuctionRepository.EditUploadedImages(id, Title));
    }

    //UploadPicture
    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult UploadPicture(HttpPostedFileBase fileData)
    {
      if (fileData == null) return Content("error");
      AuctionRepository.UploadImage(fileData);
      return Content("ok");
    }

    //AsignImages
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult AsignImages(string[] images, long? event_id)
    {
      return JSON(AuctionRepository.AsignImages(images, event_id));
    }

    //BatchImagesUpload
    [VauctionAuthorize(Roles = "Root,Admin")]
    public ActionResult EmailImages()
    {
      return View();
    }

    //GetEmailImages
    [VauctionAuthorize(Roles = "Root,Admin"), HttpPost]
    public JsonResult GetEmailImages(string sidx, string sord, int page, int rows)
    {
      return JSON(GeneralRepository.GetEmailImages(sidx, sord, page, rows));
    }

    //UploadPicture
    [HttpPost]
    public ActionResult UploadEmailImages(HttpPostedFileBase fileData)
    {
      if (fileData == null) return Content("error");
      GeneralRepository.UploadEmailImage(fileData);
      return Content("ok");
    }

    //DeleteEmailImage    
    [VauctionAuthorize(Roles = "Root,Admin"), HttpPost]
    public JsonResult DeleteEmailImage(string id, string oper)
    {
      return (String.IsNullOrEmpty(id)) ? JSON(false) : JSON(GeneralRepository.DeleteEmailImages(id));
    }


    //DifferentImages
    [VauctionAuthorize(Roles = "Root,Admin")]
    public ActionResult DifferentImages()
    {
      return View();
    }

    //GetEmailImages
    [VauctionAuthorize(Roles = "Root,Admin"), HttpPost]
    public JsonResult GetDifferentImages(string sidx, string sord, int page, int rows)
    {
      return JSON(GeneralRepository.GetDifferentImages(sidx, sord, page, rows));
    }

    //UploadDifferentImages
    [HttpPost]
    public ActionResult UploadDifferentImages(HttpPostedFileBase fileData)
    {
      if (fileData == null) return Content("error");
      GeneralRepository.UploadDifferentImage(fileData);
      return Content("ok");
    }

    //DeleteDifferentImage    
    [VauctionAuthorize(Roles = "Root,Admin"), HttpPost]
    public JsonResult DeleteDifferentImage(string id, string oper)
    {
      return (String.IsNullOrEmpty(id)) ? JSON(false) : JSON(GeneralRepository.DeleteDifferentImages(id));
    }
    
    //HomepageImages
    [VauctionAuthorize(Roles = "Root,Admin")]
    public ActionResult HomepageImages()
    {
      return View();
    }

    //GetHomepageImages
    [VauctionAuthorize(Roles = "Root,Admin"), HttpPost]
    public JsonResult GetHomepageImages(string sidx, string sord, int page, int rows)
    {
      return JSON(GeneralRepository.GetHomepageImages(sidx, sord, page, rows));
    }

    //UploadHomepageImages
    [HttpPost]
    public ActionResult UploadHomepageImages(HttpPostedFileBase fileData)
    {
      if (fileData == null) return Content("error");
      GeneralRepository.UploadHomepageImage(fileData);
      return Content("ok");
    }

    //DeleteHomepageImage    
    [VauctionAuthorize(Roles = "Root,Admin"), HttpPost]
    public JsonResult DeleteHomepageImage(string id, string oper)
    {
      if (string.IsNullOrEmpty(id)) return JSON(false);
      List<long> ids = new List<long>();
      foreach (string s in id.Split(','))
        ids.Add(Convert.ToInt64(s));
      return JSON(GeneralRepository.DeleteHomepageImages(ids.ToArray()));
    }

    //UpdateHomepageImage    
    [VauctionAuthorize(Roles = "Root,Admin"), HttpPost]
    public JsonResult UpdateHomepageImage(string id, int Order, string Link, string LinkTitle, int ImgType, bool IsEnabled)
    {
      return String.IsNullOrEmpty(id)?JSON(false):JSON(GeneralRepository.UpdateHomepageImage(Convert.ToInt64(id), Order, Link, LinkTitle, ImgType, IsEnabled));
    }
  }
}
