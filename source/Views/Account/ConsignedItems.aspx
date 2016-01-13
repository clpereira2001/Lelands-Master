<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IPagedList<Auction>>" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>
<%@ Import Namespace="Vauction.Utils.Paging" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Consigned Items - <%=Consts.CompanyTitleName %>
  </title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
 <% decimal version; bool isIE6 = (Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%> 
  <div id="left_column">    
    <img alt="" width="173" height="574" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_1.jpg")+"'" : String.Empty %> />
  </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <div class="page_title">Consigned Items</div>

    <%SessionUser cuser = AppHelper.CurrentUser; 
      Vauction.Models.CustomModels.AuctionFilterParams filterParams = ViewData["filterParam"] as Vauction.Models.CustomModels.AuctionFilterParams;
       Html.RenderAction("pConsignedItemsSearch", "Account", new { user_id = cuser.ID, param = filterParams });%>
    <br />
    <% Event evnt = ViewData["CurrentEvent"] as Event; %>
    <% Html.RenderAction((!evnt.IsCurrent || (filterParams.Event_ID.HasValue && (filterParams.Event_ID.Value != evnt.ID || (filterParams.Event_ID.Value == evnt.ID && !evnt.IsCurrent)))) ? "pConsignedItemsPast" : "pConsignedItemsCurrent", "Account", new { user_id = cuser.ID, param = filterParams, page = filterParams.page, viewmode = filterParams.ViewMode, imageviewmode = filterParams.ImageViewMode});%>
  </div>
</asp:Content>
<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
<%decimal version; if (!(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE"))
    { %>
  <script type="text/javascript">
    $(document).ready(function() {
      $("#left_column img").attr("src", '<%=AppHelper.CompressImage("left_side_banner_1.jpg") %>');
    });
  </script>
  <%} %>
</asp:Content>