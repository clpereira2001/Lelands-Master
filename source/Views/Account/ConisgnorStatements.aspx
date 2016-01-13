<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<LinkParams>>" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Your Consignment Statement - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server"> 
  <% decimal version; bool isIE6 = (Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%> 
  <div id="left_column">    
    <img alt="" width="173" height="574" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_1.jpg")+"'" : String.Empty %> />
  </div>  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <div class="page_title">Your Consignment Statement</div>
    <% if (Model == null || Model.Count() == 0)
      Response.Write("You do not have any consignor statements yet.");
    else
    { %>
        Below is a list of your consingor statement(s). Auctions are listed in reverse order by begining date.<br />
        <div id="account_common" class="control" style="float: left">
          <div class="blue_box">
            <ul>
              <% foreach (LinkParams item in Model){%>
              <li>
                <%=Html.ActionLink(item.EventTitle, "ConsignmentDetail", new { controller = "Account", action = "ConsignmentDetail", id = item.ID, evnt = item.EventUrl }, new { @style = "font-size:14px" })%>
              </li>
              <% }%>
            </ul>
          </div>
        </div>
    <%} %>    
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
