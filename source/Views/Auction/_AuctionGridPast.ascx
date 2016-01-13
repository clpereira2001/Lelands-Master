<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<IPagedList<AuctionShort>>" %>
 
<%@ Import Namespace="Vauction.Models.CustomModels" %>
<%@ Import Namespace="Vauction.Utils.Paging" %>

<% GeneralFilterParams filterParams = (GeneralFilterParams)ViewData["filterParam"];
   bool ShowPager = (ViewData["HidePager"] == null);%>
<%if (ShowPager)
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
    <span style="width: 150px; float: right; text-align: right; margin-bottom: 5px;"
      class="span-3 last viewform ie_viewform">
      <label style="font-size: 12px">
        View as:
      </label>
      <%= Html.DropDownList("ViewModeTop", BaseHelpers.GetAuctionViewModeList((Consts.AuctonViewMode)filterParams.ViewMode), new { @class = "span-3 viewselect", style= "width:60px; //width:70px;"})%>
    </span>
  </div>
</div>
<br />
<%} %>
<div id="dvContent">
  <% Html.RenderPartial(((Consts.AuctonViewMode)filterParams.ViewMode == Consts.AuctonViewMode.Table)?"~/Views/Auction/_TableViewPast.ascx":"~/Views/Auction/_GridViewPast.ascx", Model);%>
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
  <span style="width: 150px; float: right; text-align: right; margin-bottom: 5px;"
    class="span-3 last viewform ie_viewform">
    <label style="font-size: 12px">
      View as:
    </label>
    <%= Html.DropDownList("ViewModeBottom", BaseHelpers.GetAuctionViewModeList((Consts.AuctonViewMode)filterParams.ViewMode), new { @class = "span-3 viewselect", style = "width:60px;//width:70px;" })%>
  </span>
</div>
<%} %>
<script src="<%=AppHelper.CompressScript("jquery.cookie.js") %>" type="text/javascript"></script>
<script type="text/javascript">
  $(document).ready(function() {
    $("select.span-3.viewselect").change(function(e) {
      $.cookie("ViewMode", this.value, { path: "/" })
      location.reload();
    })
    $("#cbImages").click(function(e) {
      $.cookie("ImageViewMode", ($(this).attr("checked")) ? 1 : 0, { path: "/" })
      location.reload();
    })
  });
</script>

