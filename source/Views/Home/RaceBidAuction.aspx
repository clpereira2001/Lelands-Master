<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
 <% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
<div id="left_column">    
    <img alt="" width="173" height="574" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_1.jpg")+"'" : String.Empty %> />
  </div>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div class="center_content" >
  <div class="page_title">#1 Official Race Bid Auction</div>
  <table>
  <tr>
    <td><img alt="#1 Official Race Bid Auction" src="<%=AppHelper.CompressImage("first_race_bid_auction.png") %>" /></td>
    <td style="vertical-align:top;text-align:justify">Iditarod, "The Last Great Race on Earth". It's a 1,049 mile sled dog race across the great state of Alaska. The 2011 Iditarod leaves Anchorage on Saturday, March 5th with the winner expected to reach the finish line in Nome approximately 10 days later. Here's your chance to own a very special Iditarod collector's item, a #1 Official Race Bib, signed by the winner of the 2011 Iditarod.<br /><%=Html.ActionLink("CLICK HERE", "Register", new { controller = "Account", action = "Register" }, new { @style = "float-size:14px;font-weight:bold" })%> to register as an official bidder for this highly coveted item of sports memorabilia.</td>    
   </tr>
  </table>  
</div>
</asp:Content>

<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
<%decimal version; if (!(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE"))
    { %>
  <script type="text/javascript">
    $(document).ready(function () {
      $("#left_column img").attr("src", '<%=AppHelper.CompressImage("left_side_banner_1.jpg") %>');
    });
  </script>
  <%} %>
</asp:Content>
