using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Vauction.Utils.Autorization;
using Vauction.Utils.Lib;
using Vauction.Utils.Perfomance;
using Vauction.Models;

//using sharp = iTextSharp.text;
//using iTextSharp.text.pdf;
//using iTextSharp.text.xml;
using System.Data;

namespace Vauction.Controllers
{
  [VauctionAuthorize(Roles = "Root")]
  public class TempController : BaseController
  {
    public ActionResult Index()
    {
      return View();
    }

    [NonAction]
    public ActionResult SendInvoices()
    {
      //GeneralRepository.Temp_SendUserInvoices();
      return View("Index");
    }

    public ActionResult Test(long id)
    {
      List<string> list = dataProvider.GeneralRepository.Temp_CreateSpreadsheets(id);
      if (list.Count() == 0) return View("Index");
      string folder = String.Format("{0:yyyyMMdd}", DateTime.Now);
      System.IO.FileInfo fi;
      Response.BufferOutput = true;
      Response.Clear();
      Response.ContentType = "application/zip";
      Response.AddHeader("content-disposition", "attachment; filename=" + folder + ".zip");

      using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
      {
        foreach (string path in list)
        {
          fi = new System.IO.FileInfo(path);
          if (!fi.Exists) continue;
          zip.AddFile(path, folder);
        }
        zip.Save(Response.OutputStream);
      }
      System.Threading.Thread.Sleep(10000);
      Response.End();
      foreach (string path in list)
      {
        fi = new System.IO.FileInfo(path);
        if (!fi.Exists) continue;
        fi.Delete();
      }
      return RedirectToRoute("Index");
    }

    public ActionResult Test2(long id)
    {
      System.IO.FileInfo fi = new System.IO.FileInfo(dataProvider.GeneralRepository.Temp_CreateSpreadsheetW(id));
      string folder = System.IO.Path.GetFileNameWithoutExtension(fi.FullName);
      if (!fi.Exists) return RedirectToRoute("Index");
      Response.Clear();
      Response.ContentType = "application/zip";
      Response.AddHeader("content-disposition", "attachment; filename=" + folder + ".zip");
      using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
      {
        zip.AddFile(fi.FullName, folder);
        zip.Save(Response.OutputStream);
      }
      System.Threading.Thread.Sleep(30000);
      Response.End();
      fi.Delete();
      return View("Index");
    }

    public ActionResult Test3(long id)
    {
      List<string> list = dataProvider.GeneralRepository.Temp_CreateSpreadsheetsID(id);
      if (list.Count() == 0) return View("Index");
      string folder = String.Format("{0:yyyyMMdd}", DateTime.Now);
      System.IO.FileInfo fi;
      Response.Clear();
      Response.ContentType = "application/zip";
      Response.AddHeader("content-disposition", "attachment; filename=" + folder + ".zip");

      using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
      {
        foreach (string path in list)
        {
          fi = new System.IO.FileInfo(path);
          if (!fi.Exists) continue;
          zip.AddFile(path, folder);
        }
        zip.Save(Response.OutputStream);
      }
      System.Threading.Thread.Sleep(10000);
      Response.End();
      foreach (string path in list)
      {
        fi = new System.IO.FileInfo(path);
        if (!fi.Exists) continue;
        fi.Delete();
      }
      return View("Index");
    }

    [NonAction]
    public ActionResult SendInvoices(long id)
    {
      ViewData["InvoicesCount"] = dataProvider.GeneralRepository.SendInvoices(id);
      return View("Index");
    }

    [NonAction]
    public ActionResult SendConsignments(long id)
    {
      ViewData["InvoicesCount"] = dataProvider.GeneralRepository.SendConsignments(id);
      return View("Index");
    }

    public ActionResult SendAfterClosingEmails(int id)
    {
      dataProvider.EventRepository.SendAfterClosingEmails(id);
      return View("Index");
    }
  }
}
