using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace Vauction.Utils.Helpers
{
    public class DownloadResult : ActionResult
    {
        public DownloadResult()
        {
        }
        public DownloadResult(string downloadContent, string fileName, string contentType)
        {
            DownloadContent = downloadContent;
            DownloadFileName = fileName;
            ContentType = contentType;
        }
        public string DownloadContent
        {
            get;
            set;
        }
        public string DownloadFileName
        {
            get;
            set;
        }
        public string ContentType
        {
            get;
            set;
        }
        public override void ExecuteResult(ControllerContext context) 
        {
            if (!String.IsNullOrEmpty(DownloadContent))
            {
                context.HttpContext.Response.AddHeader("content-disposition",
                    "attachment; filename=" + DownloadFileName);
                context.HttpContext.Response.ContentType = ContentType;                                
                context.HttpContext.Response.Write(DownloadContent);
            }
    }


    }
}
