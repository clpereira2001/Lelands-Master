<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <title>My Account - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
  <% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
  <div id="left_column">
    <img alt="" width="173" height="461" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_2.jpg")+"'" : String.Empty %> />
  </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <% SessionUser cuser = AppHelper.CurrentUser; %>
  <div id="account_common" class="control" style="float: left">
    <div class="page_title"> Hello, <strong><%=cuser.FirstName %></strong>!</div>
    <div class="blue_box">
      <ul>
        <li>
          <%= Html.ActionLink("Update / Change Your information", "Profile", "Account") %>
        </li>
        <li>
          <%= Html.ActionLink("Past Auction Bidding History", "PastAuction", "Account")%>
        </li>
        <li>
          <%= Html.ActionLink("Your Auction Invoices", "AuctionInvoices", "Account")%>
        </li>
        <li>
          <%= Html.ActionLink("Edit Your Mail Settings", "EditMailSettings", "Account")%>
        </li>
        <% if (cuser.IsSellerType)
           {%>
        <li>
          <%= Html.ActionLink("Your Consignment Statement", "ConisgnorStatements", "Account")%>
        </li>
        <li>
          <div class="attention_text">
            <%= Html.ActionLink("Consigned Items", "ConsignedItems", "Account")%>
          </div>
        </li>
        <%} %>
        <li>
          <%= Html.ActionLink("Logout", "LogOff", "Account")%>
        </li>
      </ul>
    </div>
  </div>
</asp:Content>
<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
 <%decimal version; if (!(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE"))
  { %>
  <script type="text/javascript">
    $(document).ready(function() {
      $("#left_column img").attr("src", '<%=AppHelper.CompressImage("left_side_banner_2.jpg") %>');
    });
  </script>
  <%} %>
</asp:Content>