<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="chtHead" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Auctions - <%=Consts.CompanyTitleName%></title>
  <% Html.Script("maxlength.js"); %>
  <% Html.Script("swfobject.js"); %>
  <% Html.Script("jquery.uploadify.v2.1.4.min.js"); %>
  <% Html.Script("jquery.Jcrop.min.js"); %>
  <% Html.Style("uploadify.css"); %>
  <% Html.Style("jquery.Jcrop.css"); %>
  <link href="../../Content/plugins/jquery.multiSelect.css" rel="stylesheet" />
  <style type="text/css">
    .multiSelect { width: 415px!important;}
  </style>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Auctions</h2>
  <table id="a_list"></table>
  <div id="a_pager"></div>
  <% Html.RenderPartial("~/Views/Auction/AuctionForm.ascx"); %>
  <% Html.RenderPartial("~/Views/Auction/ConsignorForm.ascx"); %>
  <% Html.RenderPartial("~/Views/Auction/AuctionBiddingForm.ascx"); %>
  <% Html.RenderPartial("~/Views/Auction/AuctionImageCropping.ascx"); %>

  <%= Html.TextBox("form_auction_Winner", "", new { @class = "dialog_text", @readonly = "true", @res = "", @style="display:none" })%>
  <% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "auction_dBuyer", Title = "Buyers list", Method = "/User/GetBuyersList/", SortName = "Login", SortOrder = "asc", ColumnHeaders = "'User#', 'Login', 'Name', 'Email', 'Phone', 'CommissionRate', 'LocationForAuctions'", Columns = "{ name: 'User_ID', index: 'User_ID', width: 50, key: true }, { name: 'Login', index: 'Login', width: 80 }, { name: 'FN', index: 'FN', width: 120}, { name: 'Email', index: 'Email', width: 100}, { name: 'Phone', index: 'Phone', width: 80 }, { name: 'CommRate_ID', index: 'CommRate_ID', width: 1, hidden:true }, {name:'LocationForAuctions',index:'LocationForAuctions',width:1,hidden:true}", ResultElement = "form_auction_Winner", ResultShow = "ret.Login+' ('+ret.FN+')'", ResultID = "ret.User_ID", CallbackSuccess = "if (winning_patch_number==1) AddWinnerToAuctionAfterSelect(auction_rowID, ret.User_ID); else AddRightsToAuctionAfterSelect(auction_rowID, ret.User_ID);" }); %>
  <% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "form_auction_bidding_dBuyer", Title = "Buyers list", Method = "/User/GetBuyersAndHBList/", SortName = "Login", SortOrder = "asc", ColumnHeaders = "'User#', 'Login', 'Name', 'Email', 'Phone', 'CommissionRate', 'LocationForAuctions'", Columns = "{ name: 'User_ID', index: 'User_ID', width: 50, key: true }, { name: 'Login', index: 'Login', width: 80 }, { name: 'FN', index: 'FN', width: 120}, { name: 'Email', index: 'Email', width: 100}, { name: 'Phone', index: 'Phone', width: 80 }, { name: 'CommRate_ID', index: 'CommRate_ID', width: 1, hidden:true }, {name:'LocationForAuctions',index:'LocationForAuctions',width:1,hidden:true}", ResultElement = "form_auction_bidding_User", ResultShow = "ret.Login+' ('+ret.FN+')'", ResultID = "ret.User_ID" }); %>
</asp:Content>

<asp:Content ID="cntJS" ContentPlaceHolderID="jsContent" runat="server">
  <script type="text/javascript">
    var auction_table_events = "<%=ViewData["EventsList"] %>";
    var auction_table_statuses = "<%=ViewData["AuctionStatusesList"] %>";
    var auction_table_tags = "<%=ViewData["AuctionTagsList"] %>";
    var auction_table_commissionrate = "<%=ViewData["CommissionRateList"] %>";
    var auction_table_maincategory = "<%=ViewData["MainCategoryList"] %>";
    var auction_table_priority = "<%=ViewData["PriorityList"] %>";
    var consignment_table_paymenttypes = "<%=ViewData["PaymentTypes"] %>";
    var sitepath = "<%=ConfigurationManager.AppSettings["backendURL"] %>";
  </script>  
  <script src="<%=ResolveUrl("~/Scripts/Vauction/Auctions.js") %>" type="text/javascript"></script>
  <script src="<%=ResolveUrl("~/Scripts/Vauction/Images.js") %>" type="text/javascript"></script>
  <script src="<%=ResolveUrl("~/Scripts/Vauction/AuctionForm.js") %>" type="text/javascript"></script>
  <script src="<%=ResolveUrl("~/Scripts/Vauction/ConsignorForm.js") %>" type="text/javascript"></script>
  <script src="<%=ResolveUrl("~/Scripts/Vauction/UserForm.js") %>" type="text/javascript"></script>
  <script src="<%=ResolveUrl("~/Scripts/Vauction/AuctionBiddingForm.js") %>" type="text/javascript"></script>
  <script src="<%=ResolveUrl("~/Scripts/Vauction/AuctionImageCropping.js") %>" type="text/javascript"></script>  
  <script type="text/javascript" src="../../Scripts/plugins/jquery.multiSelect.js"></script>
<script type="text/javascript">
  $(document).ready(function () {
    $("#form_auction_tags").multiSelect({ oneOrMoreSelected: '*' });
  })
</script>
</asp:Content>
