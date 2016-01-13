<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title><%= ((String.IsNullOrEmpty(ViewData["Title"] as string)) ? Consts.CompanyTitleName : ViewData["Title"] as string) %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
  <div id="left_column">
    <img alt="" width="173" height="574" src='<% = AppHelper.CompressImage("left_side_banner_1.jpg") %>' />
  </div>
</asp:Content>
<asp:Content ID="main" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <% var evnt = ViewData["Event"] as Event ?? new Event(); %>
    <% var tag = ViewData["Tag"] as Tag ?? new Tag(); %>
    <div class="page_title" style="margin-bottom: 0;"><%= evnt.Title %> &gt;  <%= tag.Title %> </div>
    <span style="font-size: 12px">All auctions end <b><%= evnt.DateEnd.ToString("G") %> EST</b></span>
    <br />
    <br />
    <% var filterParams = ViewData["filterParam"] as TagFilterParams ?? new TagFilterParams(); %>
    <% Html.RenderAction("TagProducts", "Auction", new {eventID = evnt.ID, param = filterParams}); %>
  </div>
</asp:Content>