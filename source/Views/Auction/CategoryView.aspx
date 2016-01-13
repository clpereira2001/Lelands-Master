<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="Vauction.Utils.Paging" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title><%=((String.IsNullOrEmpty(ViewData["Title"] as string)) ? Consts.CompanyTitleName : ViewData["Title"] as string)%></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
 <div id="left_column">
  <% if (Convert.ToBoolean(ViewData["IsMainCategory"]))
     { %>
        <% decimal version; bool isIE6 = (Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%> 
        <img alt="" width="173" height="574" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_1.jpg")+"'" : String.Empty %> />
        <% if (!isIE6)
           { %>
          <script type="text/javascript">
            $(document).ready(function() {
              $("#left_column img").attr("src", '<%=AppHelper.CompressImage("left_side_banner_1.jpg") %>');
            });
        </script>
        <%} %>
  <%} else { %>
    <%--<%Html.RenderPartial("~/Views/Auction/_CategoryList.ascx"); %>  --%>
    <% EventCategoryDetail ecd = ViewData["ECDetail"] as EventCategoryDetail; %>
    <% Html.RenderAction("pCategoryTree", "Auction", new { event_id=ecd.LinkParams.Event_ID, maincategory_id = (ViewData["MainCategory"] != null ? ((ViewData["MainCategory"] is MainCategory) ? (ViewData["MainCategory"] as MainCategory).ID : Convert.ToInt32(ViewData["MainCategory"])) : -1) });%>  
  <%} %>
  </div>
</asp:Content>
<asp:Content ID="main" ContentPlaceHolderID="MainContent" runat="server"> 
  <% bool IsMainCategory = Convert.ToBoolean(ViewData["IsMainCategory"]); %> 
  <div class="center_content">
    <% EventCategoryDetail ecd = ViewData["ECDetail"] as EventCategoryDetail;
       if (ecd.IsCurrent)
       { %>
    <div class="pay_notice" style="margin:10px 0px 10px -10px;width:760px;-margin-left:-0px">
      Do not bid unless you intend to pay. You are entering into a binding contract.
      All items must be paid in full within 14 days.</div>
      <%}
       else Response.Write("<br/>"); %>
    <%  if (!IsMainCategory)
        {%>
        <div class="span-16">
         <%= Html.ActionLink(ecd.LinkParams.EventTitle, "Category", new { controller = "Auction", action = "Category", id = ecd.LinkParams.Event_ID, evnt = ecd.LinkParams.EventUrl, maincat = "All-Categories" }, new { @style = "font-size:16px; font-weight:bold;" })%>
          >
          <%= Html.ActionLink(ecd.LinkParams.MainCategoryTitle, "CategoryView", new { controller = "Auction", action = "CategoryView", id = ecd.LinkParams.EventCategory_ID, evnt =ecd.LinkParams.EventUrl , maincat = ecd.LinkParams.MainCategoryUrl }, new { @style = "font-size:16px; font-weight:bold;" })%>
          >
         <%=ecd.LinkParams.CategoryTitle %>
        </div>
        <br class="clear" />
        
        <span style="font-size:12px">All auctions end <b><%=ecd.DateEnd %> EST</b></span>
        <br /><br />    
        <% CategoryFilterParams filterParams = ViewData["filterParam"] as CategoryFilterParams;%>
        <% Html.RenderAction(ecd.IsClickable && ecd.Step < 2 ? "pCategoryViewCurrent" : "pCategoryView", "Auction", new { event_id = ecd.LinkParams.Event_ID, iscurrentevent = ecd.IsCurrent, param = filterParams, page = filterParams.page, viewmode = filterParams.ViewMode, imageviewmode = filterParams.ImageViewMode, eventstep=ecd.Step });%>
    <%}
       else
       { 
    %>
    <div class="span-16" id="auction_title" style="font-size:16px">
      <%= Html.ActionLink(ecd.LinkParams.EventTitle, "Category", new { controller = "Auction", action = "Category", id = ecd.LinkParams.Event_ID, evnt=ecd.LinkParams.EventUrl, maincat = "All-Categories" }, new { @style = "font-size:16px; font-weight:bold;" })%>
      >
      <%=ecd.LinkParams.MainCategoryTitle%>
    </div>
    <br /><br />
    <% Html.RenderAction("pCategoryViewMainCategory", "Auction", new { event_id = ecd.LinkParams.Event_ID, maincategory_id=ecd.LinkParams.MainCategory_ID });%>
   <% }%>
  </div>  
</asp:Content>