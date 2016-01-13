<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Vauction.Utils.Html" %>
<%@ Import Namespace="Vauction.Utils.Perfomance" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Email settings < <%=Consts.CompanyTitleName %></title>
  <% Html.Script("hashtable.js"); %>
  <% Html.Script("validation.js"); %>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
<% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
  <div id="left_column">
    <img alt="" width="173" height="461" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_2.jpg")+"'" : String.Empty %> />
  </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">    
    <div class="page_title">Email Settings</div>
    <% using (Html.BeginForm()){ %>
    <%=Html.AntiForgeryToken(Consts.AntiForgeryToken) %>
    <table style="table-layout: fixed;">
      <colgroup>
        <col width="50px" />
        <col width="50px" />
      </colgroup>
      <tr>
        <td>
          I want to receive Auction Announcements:
        </td>
        <td>
          <%= Html.CheckBox("IsRecievingNewsUpdates", Convert.ToBoolean(ViewData["IsRecievingNewsUpdates"]))%>
        </td>
      </tr>
      <tr>
        <td>
          I want to receive Weekly Specials:
        </td>
        <td>
          <%= Html.CheckBox("IsRecievingWeeklySpecials", Convert.ToBoolean(ViewData["IsRecievingWeeklySpecials"]))%>
        </td>
      </tr>
      <tr>
        <td>
          I want to receive Bid Confirmations:
        </td>
        <td>
          <%= Html.CheckBox("IsRecievingBidConfirmation", Convert.ToBoolean(ViewData["IsRecievingBidConfirmation"]))%>
        </td>
      </tr>
      <tr>
        <td>
          I want to receive OutBid notices:
        </td>
        <td>
          <%= Html.CheckBox("IsRecievingOutBidNotice", Convert.ToBoolean(ViewData["IsRecievingOutBidNotice"]))%>
        </td>
      </tr>
      <tr>
        <td>
          I want to receive Lot Sold notices:
        </td>
        <td>
          <%= Html.CheckBox("IsRecievingLotSoldNotice", Convert.ToBoolean(ViewData["IsRecievingLotSoldNotice"]))%>
        </td>
      </tr>
      <tr>
        <td>
          I want to receive Lot Closed notices:
        </td>
        <td>
          <%= Html.CheckBox("IsRecievingLotClosedNotice", Convert.ToBoolean(ViewData["IsRecievingLotClosedNotice"]))%>
        </td>
      </tr>
    </table>    
    <% if (ViewData["DataSaved"] != null && Convert.ToBoolean(ViewData["DataSaved"])) { %>
          <strong>The settings were saved successfully.</strong><br /><br />
    <%} %>
          <%= Html.SubmitWithClientValidation("Update")%>
    <% } %>
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