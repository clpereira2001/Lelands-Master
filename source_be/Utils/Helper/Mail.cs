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
using System.Collections.Generic;

namespace Vauction.Utils.Helper
{
  public class Mail
  {
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

    public static void SendWinningLetter(string FirstName, string LastName, string emailTo, long auctionID, string auctionName, string usersbid)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\winningBid.txt"));

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{auctionID}}", auctionID);      
      template.Data.Add("{{auctionName}}", auctionName);
      template.Data.Add("{{usersbid}}", usersbid);
      //template.Data.Add("{{usersmaxbid}}", usersmaxbid);
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);
      ParseCommonData(template);

      UniMail.Mailer.Enqueue(template.Render());
    }

    public static void SendEndOfAuctionLetter(string emailTo, long auctionID, string auctionName)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\endOfAuction.txt"));

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{auctionID}}", auctionID);
      template.Data.Add("{{auctionName}}", auctionName);
      ParseCommonData(template);

      UniMail.Mailer.Enqueue(template.Render());
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

    public static void SendPulledOutLetter(string FirstName, string LastName, string emailTo, long auctionID, string auctionName)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\WithdrawnItem.txt"));

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{auctionID}}", auctionID);
      template.Data.Add("{{auctionName}}", auctionName);
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);
      ParseCommonData(template);

      UniMail.Mailer.Enqueue(template.Render());
    }

    //public static void SendInvoice(string emailTo, string invoice_id)
    //{
    //  UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\Invoices.txt"));
    //  template.Encoding = System.Text.Encoding.UTF8;
    //  template.ToEmail = emailTo;      
    //  ParseCommonData(template);
    //  UniMail.Mailer.Enqueue(template.Render(HttpContext.Current.Server.MapPath(@"~\Pool\Invoices\"+invoice_id+".pdf"))); ;
    //}

    public static void Temp_SendInvoiceInformationLetter(string FirstName, string LastName, string emailTo, string invoice_id)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\PreviewAuctionInvoice.txt"));
      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);
      template.Data.Add("{{invoice_id}}", invoice_id);
      ParseCommonData(template);
      UniMail.Mailer.Enqueue(template.Render());
    }

    public static void Temp_SendInvoiceInformationLetter2(string FirstName, string LastName, string emailTo, string auctionenddate, string userinvoice_id, string salesdate, string eventurl, List<Invoice> invoices, decimal amountpaid)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\UserInvoice.txt"));
      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);
      template.Data.Add("{{auctionenddate}}", auctionenddate);
      template.Data.Add("{{UserInvoice_ID}}", userinvoice_id);
      template.Data.Add("{{SaleDate}}", salesdate);
      template.Data.Add("{{EventURL}}", eventurl);

      bool line = true;
      decimal sumcost = 0;
      decimal sumshipping = 0;
      decimal sumtax = 0;
      decimal sumfees = 0;
      decimal tmp = 0;
      bool isnotshipment = false;
      tmp = amountpaid;
      StringBuilder info = new StringBuilder();

      foreach (var item in invoices)
      {
        if (item.Shipping == 0) isnotshipment = true;
        info.AppendLine(((line)
                           ? "<tr style=\"background-color:#EFEFEF\" class=\"bordered\">"
                           : "<tr class=\"bordered\">"));
        info.AppendLine("<td style='padding-left: 5px; border:1px solid #444'>" + item.Auction.Lot.ToString() + "</td>");
        info.AppendLine("<td style='padding-left: 5px; border:1px solid #444'>" + item.Auction.Title + "</td>");
        info.AppendLine("<td style='padding-left: 5px; border:1px solid #444'>" + item.Cost.GetCurrency() + "</td>");
        info.AppendLine("</tr>");
        line = !line;
        sumcost += item.Cost;
        sumshipping += item.Shipping;
        sumtax += item.Tax;
        sumfees += item.LateFee;
      }
      info.AppendLine("<tr><td colspan=\"2\" style=\"text-align:right; font-weight:bold\">Net Cost*:&nbsp;</td><td>"+sumcost.GetCurrency()+"</td></tr>");
      info.AppendLine("<tr><td colspan=\"2\" style=\"text-align:right; font-weight:bold\">Shipping, Handiling & Insurance:&nbsp;</td><td>" + ((sumshipping == 0 || isnotshipment) ? "not calculated yet" : sumshipping.GetCurrency()) + "</td></tr>");
      info.AppendLine("<tr><td colspan=\"2\" style=\"text-align:right; font-weight:bold\">Sales Tax:&nbsp;</td><td>" + ((sumtax == 0) ? "$0.00" : sumtax.GetCurrency()) + "</td></tr>");
      info.AppendLine("<tr><td colspan=\"2\" style=\"text-align:right; font-weight:bold\">Late Fees:&nbsp;</td><td>" + ((sumfees == 0) ? "$0.00" : sumfees.GetCurrency()) + "</td></tr>");
      info.AppendLine("<tr><td colspan=\"2\" style=\"text-align:right; font-weight:bold\">Amount Paid:&nbsp;</td><td>" + ((tmp == 0) ? "$0.00" : tmp.GetCurrency()) + "</td></tr>");
      tmp = sumcost + sumfees + sumshipping + sumtax - tmp;
      info.AppendLine("<tr><td colspan=\"2\" style=\"text-align:right; font-weight:bold\">Amount Due:&nbsp;</td><td>" + ((tmp < 0 || (sumshipping == 0 || isnotshipment)) ? "not calculated yet" : tmp.GetCurrency(false)) + "</td></tr>");

      template.Data.Add("{{info}}", info.ToString());
      ParseCommonData(template);
      UniMail.Mailer.Enqueue(template.RenderHTML());
    }

    public static void Temp_SendConsignmentInformationLetter(string FirstName, string LastName, string emailTo, string consignment_id, string eventname, string auctionenddate)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\Consignments.txt"));
      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);
      template.Data.Add("{{invoice_id}}", consignment_id);
      template.Data.Add("{{eventname}}", eventname);
      template.Data.Add("{{auctionenddate}}", auctionenddate);
      ParseCommonData(template);
      UniMail.Mailer.Enqueue(template.Render());
    }

    //SendEndOfAuctionHTMLLetter
    public static void SendEndOfAuctionHTMLLetter(string email, EndOfAuction eoa)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\EndOfAuctionNoticeHTML.txt"), "mail");
      string emailTitle = (eoa.Invoices.Count() > 0 ? "Winning Auction Notification for " : "End of Auction Notice for ") + eoa.EventTitle;
      template.Subject = emailTitle;
      template.Encoding = Encoding.UTF8;
      template.ToEmail = email;

      template.Data.Add("{{email_subject}}", emailTitle);
      template.Data.Add("{{email_title}}", eoa.Invoices.Count() > 0 ? "Congratulations!" : emailTitle);
      if (eoa.Invoices.Count() > 0)
          template.Data.Add("{{email_title_2}}", eoa.UserName + ",<br />" + String.Format("Please send your payment to:<br />Lelands Collectibles, Inc.<br /> 130 Knickerbocker Avenue<br /> Suite E <br />Bohemia, NY 11716 <br /><br />International winners please contact laura@lelands.com before sending payment to receive your international shipping cost. <br /><br />You can also preview your invoice online in your account or follow <a style='font-weight:bold;color:#6C0202' href='{2}/Account/InvoiceDetailed/{0}/{1}'>this link</a>.", eoa.Invoices.First().UserInvoice_ID, eoa.Invoices.First().LinkParams.EventUrl, "{{siteUrl}}") + "<br /><br />You are the winning bidder for the following auction item(s).");
      else template.Data.Add("{{email_title_2}}", "This email is sent as a courtesy to let you know that this auction has ended.");

      StringBuilder table;

      if (eoa.Invoices.Count() > 0)
      {
        #region Invoices
        table = new StringBuilder();

        table.AppendLine("<span style='color:#490202'><strong>Winning Item Invoice</strong></span>");
        table.AppendLine("<table style='table-layout:fixed;font-size:12px;' cellpadding='0' cellspacing='0' >");
        table.AppendLine("<colgroup><col width='80px' /><col width='510px' /><col width='140px' /></colgroup>");
        table.AppendLine("<tr>");
        table.AppendLine("<td style='font-weight:bold;background-color:#6C0202;color:#FFFFFF;padding: 5px 0px 5px 10px;font-size:12px;'>Lot#</td>");
        table.AppendLine("<td style='font-weight:bold;background-color:#6C0202;color:#FFFFFF;padding: 5px 0px 5px 10px;font-size:12px;'>Title</td>");
        table.AppendLine("<td style='font-weight:bold;background-color:#6C0202;color:#FFFFFF;padding: 5px 0px 5px 10px;font-size:12px;'>Cost</td>");
        table.AppendLine("</tr>");

        foreach (InvoiceDetail invoice in eoa.Invoices)
        {
          table.AppendLine("<tr>");
          table.AppendFormat("<td style='font-size:12px;font-weight:bold;padding: 5px 0px 5px 10px;background-color:#f4f4f4;border: solid 1px #e2e2e2'>{0}</td>", invoice.LinkParams.Lot);
          table.AppendFormat("<td style='font-size:12px;font-weight:bold;padding: 5px 0px 5px 10px;background-color:#f4f4f4;border: solid 1px #e2e2e2'>{0}</td>", invoice.LinkParams.Title);
          table.AppendFormat("<td style='font-size:12px;font-weight:bold;padding: 5px 0px 5px 10px;background-color:#f4f4f4;border: solid 1px #e2e2e2'>{0}</td>", invoice.Cost.GetCurrency());
          table.AppendLine("</tr>");
        }
        table.AppendFormat("<tr><td style='font-weight:bold;color:#490202;padding: 5px 5px 5px 10px; text-align:right;font-size:12px;' colspan='2'>{0}:&nbsp;&nbsp;</td><td style='font-weight:bold;color:#222;padding: 5px 0px 5px 10px;font-size:12px;background-color:#f4f4f4;border: solid 1px #e2e2e2'>{1}</td></tr>", "Net Cost*", eoa.TotalCst.GetCurrency(false));
        table.AppendFormat("<tr><td style='font-weight:bold;color:#490202;padding: 5px 5px 5px 10px; text-align:right;font-size:12px;' colspan='2'>{0}:&nbsp;&nbsp;</td><td style='font-weight:bold;color:#222;padding: 5px 0px 5px 10px;font-size:12px;background-color:#f4f4f4;border: solid 1px #e2e2e2'>{1}</td></tr>", "Shipping, Handiling & Insurance", eoa.TotalShippingString);
        table.AppendFormat("<tr><td style='font-weight:bold;color:#490202;padding: 5px 5px 5px 10px; text-align:right;font-size:12px;' colspan='2'>{0}:&nbsp;&nbsp;</td><td style='font-weight:bold;color:#222;padding: 5px 0px 5px 10px;font-size:12px;background-color:#f4f4f4;border: solid 1px #e2e2e2'>{1}</td></tr>", "Sales Tax", eoa.TotalTax.GetCurrency(false));
        table.AppendFormat("<tr><td style='font-weight:bold;color:#490202;padding: 5px 5px 5px 10px; text-align:right;font-size:12px;' colspan='2'>{0}:&nbsp;&nbsp;</td><td style='font-weight:bold;color:#222;padding: 5px 0px 5px 10px;font-size:12px;background-color:#f4f4f4;border: solid 1px #e2e2e2'>{1}</td></tr>", "Late Fees", eoa.TotalLateFee.GetCurrency(false));
        table.AppendFormat("<tr><td style='font-weight:bold;color:#490202;padding: 5px 5px 5px 10px; text-align:right;font-size:12px;' colspan='2'>{0}:&nbsp;&nbsp;</td><td style='font-weight:bold;color:#222;padding: 5px 0px 5px 10px;font-size:12px;background-color:#f4f4f4;border: solid 1px #e2e2e2'>{1}</td></tr>", "Total Due", eoa.TotalCostString);
        table.AppendLine("</table>* Auction items include premium");
        template.Data.Add("{{invoices_info}}", table.ToString());
        #endregion
      }
      else
        template.Data.Add("{{invoices_info}}", String.Empty);


      template.Data.Add("{{separator}}", (eoa.Invoices.Count() > 0 && eoa.LoserLots.Count() > 0) ? "<br /><hr /><br />" : String.Empty);

      if (eoa.LoserLots.Count() > 0)
      {
        table = new StringBuilder();
        table.AppendLine("<p>Unfortunately, you were not a successful bidder for lot(s)</p>");
        table.AppendLine("<table style='table-layout:fixed;font-size:14px' cellpadding='0' cellspacing='0' >");
        table.AppendLine("<colgroup><col width='80px' /><col width='290px' /><col width='100px' /><col width='100px' /><col width='100px' /><col width='60px' /></colgroup>");
        table.AppendLine("<tr>");
        table.AppendLine("<td style='font-weight:bold;background-color:#6C0202;color:#FFFFFF;padding: 5px 0px 5px 10px;font-size:12px;'>Lot#</td>");
        table.AppendLine("<td style='font-weight:bold;background-color:#6C0202;color:#FFFFFF;padding: 5px 0px 5px 10px;font-size:12px;'>Title</td>");
        table.AppendLine("<td style='font-weight:bold;background-color:#6C0202;color:#FFFFFF;padding: 5px 0px 5px 10px;font-size:12px;'>Winning Bid</td>");
        table.AppendLine("<td style='font-weight:bold;background-color:#6C0202;color:#FFFFFF;padding: 5px 0px 5px 10px;font-size:12px;'>Your Bid</td>");
        table.AppendLine("<td style='font-weight:bold;background-color:#6C0202;color:#FFFFFF;padding: 5px 0px 5px 10px;font-size:12px;'>Your MaxBid</td>");
        table.AppendLine("<td style='font-weight:bold;background-color:#6C0202;color:#FFFFFF;padding: 5px 0px 5px 10px;font-size:12px;'>Bids</td>");
        table.AppendLine("</tr>");
        foreach (UserBidWatch ubw in eoa.LoserLots)
        {
          table.AppendLine("<tr>");
          table.AppendLine("<td style='font-size:12px;font-weight:bold;padding: 5px 0px 5px 10px;background-color:#f4f4f4;border: solid 1px #e2e2e2'>" + ubw.LinkParams.Lot + "</td>");
          table.AppendLine("<td style='font-size:12px;font-weight:bold;padding: 5px 0px 5px 10px;background-color:#f4f4f4;border: solid 1px #e2e2e2'>" + ubw.LinkParams.Title + "</td>");
          table.AppendLine("<td style='font-size:12px;font-weight:bold;padding: 5px 0px 5px 10px;background-color:#f4f4f4;border: solid 1px #e2e2e2'>" + ubw.CurrentBid.GetCurrency() + "</td>");
          table.AppendLine("<td style='font-size:12px;font-weight:bold;padding: 5px 0px 5px 10px;background-color:#f4f4f4;border: solid 1px #e2e2e2'>" + ubw.Amount.GetCurrency() + "</td>");
          table.AppendLine("<td style='font-size:12px;font-weight:bold;padding: 5px 0px 5px 10px;background-color:#f4f4f4;border: solid 1px #e2e2e2'>" + ubw.MaxBid.GetCurrency() + "</td>");
          table.AppendLine("<td style='font-size:12px;font-weight:bold;padding: 5px 0px 5px 10px;background-color:#f4f4f4;border: solid 1px #e2e2e2'>" + ubw.Bids + "</td>");
          table.AppendLine("</tr>");
        }
        table.AppendLine("</table><br />");

        template.Data.Add("{{loserlots}}", table.ToString());
      }
      else
        template.Data.Add("{{loserlots}}", String.Empty);

      ParseCommonData(template);
      UniMail.Mailer.Enqueue(template.RenderHTML(), "mail");
    }
  }
}