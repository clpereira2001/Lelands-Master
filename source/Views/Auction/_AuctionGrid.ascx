﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<IPagedList<AuctionShort>>" %>

<%@ Import Namespace="Vauction.Models.CustomModels" %>
<%@ Import Namespace="Vauction.Utils.Paging" %>

<%GeneralFilterParams filterParams = (GeneralFilterParams)ViewData["filterParam"];  
  bool IsReshreshLinks = ViewData["PageParams"] != null;
  bool ShowPager = (ViewData["HidePager"] == null);
  if (ShowPager)
  {%>
<div style="vertical-align: middle; font-size: 12px; float: right">
  Show image(s):
  <% if (Convert.ToBoolean(filterParams.ImageViewMode))
     { %>
  <input type="checkbox" id="cbImages" checked />
  <%}
     else
     { %>
  <input type="checkbox" id="cbImages" />
  <%} %>
</div>
<br class="clear" />
<div>
  <div class="pager_pos" style="vertical-align: middle">
    <%if (Model.TotalItemCount > Model.PageSize)
      { %>
    <%= Html.Pager(Model.PageSize, Model.PageNumber, Model.TotalItemCount, CollectParameters.CollectAll())%>
    <%} %>

    <span style="width: 130px; float: right; text-align: right; margin-bottom: 5px;"
      class="span-3 last viewform ie_viewform">
      <label style="font-size: 12px">
        View as:
      </label>
      <%= Html.DropDownList("ViewModeTop", BaseHelpers.GetAuctionViewModeList((Consts.AuctonViewMode)filterParams.ViewMode), new { @class = "span-3 viewselect", style= "width:60px; //width:70px;"})%>
    </span>
    <% if (IsReshreshLinks) { %>
    <span style="width: 160px; float: right; text-align: right; margin-bottom: 5px;margin-right: 5px;">
      <span style="color:#800;cursor:pointer;font-size:12px;vertical-align:middle" id="lnkRBR"><img src="<%=AppHelper.CompressImage("refresh.gif") %>" /> <u>refresh current bids</u></span>
      <span id="lnkRBR_loading" style="color:#800;font-size:12px"><img src="<%=AppHelper.CompressImage("ajax-loader_small.gif") %>" alt="" />&nbsp;refreshing current bids</span>
      <script type="text/javascript">$("#lnkRBR_loading").hide();</script>
    </span>
    <%} %>
  </div>
</div>
<br />
<%} %>
<div id="dvContent">
  <% Html.RenderPartial(((Consts.AuctonViewMode)filterParams.ViewMode == Consts.AuctonViewMode.Table)?"~/Views/Auction/_TableView.ascx":"~/Views/Auction/_GridView.ascx", Model); %>
</div>
<% 
  if (ShowPager)
  {
%><br />
<div>
  <%if (Model.TotalItemCount > Model.PageSize)
    { %>
  <div class="pager_pos">
    <%= Html.Pager(Model.PageSize, Model.PageNumber, Model.TotalItemCount, CollectParameters.CollectAll())%>
  </div>
  <%} %>
  <span style="width: 130px; float: right; text-align: right; margin-bottom: 5px;"
    class="span-3 last viewform ie_viewform">
    <label style="font-size: 12px">
      View as:
    </label>
    <%= Html.DropDownList("ViewModeBottom", BaseHelpers.GetAuctionViewModeList((Consts.AuctonViewMode)filterParams.ViewMode), new { @class = "span-3 viewselect", style = "width:60px;//width:70px;" })%>
  </span>
  <% if (IsReshreshLinks) { %>
  <span style="width: 160px; float: right; text-align: right; margin-bottom: 5px;margin-right: 5px;">
      <span style="color:#800;cursor:pointer;font-size:12px;vertical-align:middle" id="lnkRBR_b"><img src="<%=AppHelper.CompressImage("refresh.gif") %>" /> <u>refresh current bids</u></span>
      <span id="lnkRBR_loading_b" style="color:#800;font-size:12px"><img src="<%=AppHelper.CompressImage("ajax-loader_small.gif") %>" alt="" />&nbsp;refreshing current bids</span>      
    </span>
    <%} %>
</div>
<%} %>
<%=Html.Hidden("hd_pgpms", ViewData["PageParams"] as string) %>
<script src="<%=AppHelper.CompressScript("jquery.cookie.js") %>" type="text/javascript"></script>
<% if (IsReshreshLinks) { %><script src="<%=AppHelper.CompressScript("table.js") %>" type="text/javascript"></script><%} %>
<script type="text/javascript">
  $(document).ready(function () {
    $("#lnkRBR_loading_b").hide();

    $("select.span-3.viewselect").change(function (e) {
      $.cookie("ViewMode", this.value, { path: "/" })
      location.reload();
    })
    $("#cbImages").click(function (e) {
      $.cookie("ImageViewMode", ($(this).attr("checked")) ? 1 : 0, { path: "/" })
      location.reload();
    })    
    <% if (IsReshreshLinks) { %>
    $("#lnkRBR, #lnkRBR_b").click(function () {
      $("#lnkRBR,#lnkRBR_b").hide();
      $("#lnkRBR_loading,#lnkRBR_loading_b").show();      
      <% if ((Consts.AuctonViewMode)((GeneralFilterParams)ViewData["filterParam"]).ViewMode == Consts.AuctonViewMode.Table) { %>
        UpdateTableView();
      <%} else { %>
        UpdateGridView();
      <%} %>
    });
    <%} %>
  });
</script>