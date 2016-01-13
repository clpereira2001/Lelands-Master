using System;
using System.Data;
using System.Web.Mvc;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Vauction.Models;
using System.Text;

namespace Vauction.Utils.Helpers
{
  public class Mail
  {
    public static void SendRegisterConfirmation(string emailTo, string loginName, string confirmationUrl, string firstName, string lastName)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\registerConfirm.txt"));

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{loginName}}", loginName);
      template.Data.Add("{{firstName}}", firstName);
      template.Data.Add("{{lastName}}", lastName);
      template.Data.Add("{{confirmUrl}}", confirmationUrl);
      ParseCommonData(template);
      UniMail.Mailer.Send(template.Render());
    }

    public static void SendApprovalConfirmation(string emailTo, string loginName, string firstName, string lastName)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\AccountApproved.txt"));

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{loginName}}", loginName);
      template.Data.Add("{{firstName}}", firstName);
      template.Data.Add("{{lastName}}", lastName);
      ParseCommonData(template);
      UniMail.Mailer.Send(template.Render());
    }

    public static void SendOutBidLetter(string FirstName, string LastName, string emailTo, string lot, string auctionName, string currentSuccessfulBid, string auctionEndDate, string newBidUrl)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\outBid.txt"));
      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;      
      template.Data.Add("{{lot}}", lot);
      template.Data.Add("{{auctionName}}", auctionName);
      template.Data.Add("{{currentSuccessfulBid}}", currentSuccessfulBid);
      template.Data.Add("{{auctionEndDate}}", auctionEndDate);
      template.Data.Add("{{newBidUrl}}", newBidUrl);
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);
      ParseCommonData(template);
      UniMail.Mailer.Enqueue(template.Render());
    }

    public static void SendOutBidMultipleLetter(string emailTo, long auctionID, string loginName, string auctionName, string currentSuccessfulBid, string auctionEndDate, string newBidUrl)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\outBidMultiple.txt"));

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{auctionID}}", auctionID.ToString());
      template.Data.Add("{{loginName}}", loginName);
      template.Data.Add("{{auctionName}}", auctionName);
      template.Data.Add("{{currentSuccessfulBid}}", currentSuccessfulBid);
      template.Data.Add("{{auctionEndDate}}", auctionEndDate);
      template.Data.Add("{{newBidUrl}}", ConfigurationManager.AppSettings["siteUrl"] + newBidUrl);


      ParseCommonData(template);

      UniMail.Mailer.Send(template.Render());
    }

    private static void ParseCommonData(UniMail.Template template)
    {
      template.Data.Add("{{copyrightDate}}", ConfigurationManager.AppSettings["copyrightDate"]);
      template.Data.Add("{{CompanyName}}", ConfigurationManager.AppSettings["CompanyName"]);
      template.Data.Add("{{CompanyAddress}}", ConfigurationManager.AppSettings["CompanyAddress"]);
      template.Data.Add("{{companyCity}}", ConfigurationManager.AppSettings["companyCity"]);

      template.Data.Add("{{companyState}}", ConfigurationManager.AppSettings["companyState"]);
      template.Data.Add("{{companyZip}}", ConfigurationManager.AppSettings["companyZip"]);
      template.Data.Add("{{siteName}}", ConfigurationManager.AppSettings["siteName"]);
      template.Data.Remove("{{siteUrl}}");
      template.Data.Add("{{siteUrl}}", ConfigurationManager.AppSettings["siteUrl"]);
      template.Data.Add("{{siteEmail}}", ConfigurationManager.AppSettings["siteEmail"]);
    }

    public static void ResendConfirmationCode(string emailTo, string loginName, string confirmationUrl)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\resendConfirmationCode.txt"));

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{loginName}}", loginName);
      template.Data.Add("{{confirmation}}", confirmationUrl);
      ParseCommonData(template);
      UniMail.Mailer.Send(template.Render());
    }

    public static void ForgotPassword(string emailTo, string loginName, string password, string url)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\passwordReset.txt"));
      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{loginName}}", loginName);
      template.Data.Add("{{password}}", password);
      template.Data.Add("{{confirmurl}}", url);
      ParseCommonData(template);      
      UniMail.Mailer.Send(template.Render());
    }

    internal static void SendMailTheLot(string emailTo, string loginName, string ownerEmail, string messageText, long auctionID, string auctionTitle, string auctionDescription, string auctionUrl)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\MailTheLot.txt"));

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{loginName}}", loginName);
      template.Data.Add("{{ownerEmail}}", ownerEmail);
      template.Data.Add("{{messageText}}", messageText);

      template.Data.Add("{{auctionID}}", auctionID);
      template.Data.Add("{{auctionTitle}}", auctionTitle);
      template.Data.Add("{{auctionDescription}}", auctionDescription);
      template.Data.Add("{{auctionUrl}}", auctionUrl);

      ParseCommonData(template);

      UniMail.Mailer.Send(template.Render()); ;
    }

    public static void SendDOWFailedLetter(string emailTo, long auctionID, string loginName)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\dowFailed.txt"));

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{auctionID}}", auctionID);
      template.Data.Add("{{loginName}}", loginName);
      ParseCommonData(template);
      UniMail.Mailer.Send(template.Render()); ;
    }

    public static void SendFreeEmailRegisterConfirmation(string emailTo, string firstName, string lastName, string url)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\registerFreeEmailConfirm.txt"));

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{FirstName}}", firstName);
      template.Data.Add("{{LastName}}", lastName);
      template.Data.Add("{{Url}}", url);
      ParseCommonData(template);
      UniMail.Mailer.Send(template.Render()); ;
    }

    public static void SendSuccessfulBidLetter(string FirstName, string LastName, string emailTo, string lot, string auctionName, string usersbid, string usersmaxbid, string auctionEndDate, string newBidUrl)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\successfulBid.txt"));

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;      
      template.Data.Add("{{lot}}", lot);
      template.Data.Add("{{auctionName}}", auctionName);
      template.Data.Add("{{usersbid}}", usersbid);
      template.Data.Add("{{usersmaxbid}}", usersmaxbid);
      template.Data.Add("{{auctionEndDate}}", auctionEndDate);
      template.Data.Add("{{newBidUrl}}", newBidUrl);
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);
      ParseCommonData(template);

      UniMail.Mailer.Enqueue(template.Render()); ;
    }

    public static void SendSuccessfulBidUpdateLetter(string FirstName, string LastName, string emailTo, string lot, string auctionName, string usersbid, string usersmaxbid, string auctionEndDate, string newBidUrl, bool IsMaxBidOrAmount)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\bidUpdate.txt"));
      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;      
      template.Data.Add("{{lot}}", lot);
      template.Data.Add("{{auctionName}}", auctionName);
      template.Data.Add("{{usersbid}}", usersbid);
      template.Data.Add("{{usersmaxbid}}", usersmaxbid);
      template.Data.Add("{{auctionEndDate}}", auctionEndDate);
      template.Data.Add("{{newBidUrl}}", newBidUrl);
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);
      if (IsMaxBidOrAmount)
      {
        template.Data.Add("{{change_maxbid}}", "maximum bid was raised to");
        template.Data.Add("{{was_not_change_maxbid}}", "");
        template.Data.Add("{{change_bid}}", "current bid in the amount of");
        template.Data.Add("{{was_not_change_bid}}", "was not change");
      }
      else
      {
        template.Data.Add("{{change_maxbid}}", "current maximum bid in the amount of");
        template.Data.Add("{{was_not_change_maxbid}}", "was not change");
        template.Data.Add("{{change_bid}}", "bid was raised to");
        template.Data.Add("{{was_not_change_bid}}", "");
      }
      ParseCommonData(template);

      UniMail.Mailer.Enqueue(template.Render()); ;
    }
    
    public static void SendEndOfAuctionLetter(string emailTo, long auctionID, string loginName, string auctionName, string auctionEndDate, string newBidUrl)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\endOfAuction.txt"));

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{auctionID}}", auctionID);
      template.Data.Add("{{loginName}}", loginName);

      template.Data.Add("{{auctionName}}", auctionName);
      template.Data.Add("{{auctionEndDate}}", auctionEndDate);
      template.Data.Add("{{newBidUrl}}", newBidUrl);

      ParseCommonData(template);

      UniMail.Mailer.Send(template.Render()); ;
    }

    public static void SendMessageFromConsignor(string FirstName, string LastName, string Email, string Phone, string BestTime, string Description)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\MessageFromConsignor.txt"));
      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = ConfigurationManager.AppSettings["ConsignorMessagesEmail"];
      template.Data.Add("{{firstname}}", FirstName);
      template.Data.Add("{{lastname}}", LastName);
      template.Data.Add("{{email}}", Email);
      template.Data.Add("{{phone}}", Phone);
      template.Data.Add("{{best}}", BestTime);
      template.Data.Add("{{description}}", Description);
      ParseCommonData(template);

      UniMail.Mailer.Send(template.Render());
    }
  }
}