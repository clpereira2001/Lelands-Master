<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Vauction.Models.CustomModels" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
<title>Auctions participated in - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align:justify; padding:0px 10px 10px 10px;" >   <%--class="center_content"--%>
        <div class="page_title"><%= Html.ActionLink("Past Auction Bidding History", "PastAuction", "Account", new { controller = "Account", action = "PastAuction" }, new { @style = "font-size:16px" })%> > <span style="color:#000"><%=(ViewData["Event"] as IEvent).Title %></span></div>         
        <% AuctionFilterParamsEx filterParams = ViewData["filterParamEx"] as AuctionFilterParamsEx;%>
        <% Html.RenderAction("pAuctionsParticipated", "Account", new { event_id = (ViewData["Event"] as IEvent).ID, user_id=AppHelper.CurrentUser.ID, param = filterParams, page = filterParams.page, viewmode = filterParams.ViewMode, imageviewmode = filterParams.ImageViewMode });%>        
    </div>
</asp:Content>