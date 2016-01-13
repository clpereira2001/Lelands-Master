<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" MasterPageFile="~/Views/Shared/Site.Master" %>
<asp:Content ID="head" ContentPlaceHolderID="HTMLhead" runat="server">  
  <title><%:String.Format("Categories < {0} < {1}", (ViewData["CurrentEvent"] as Event).Title, ConfigurationManager.AppSettings["CompanyName"]) %></title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="leftImg" runat="server"> 
 <% decimal version; bool isIE6 = (Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%> 
  <div id="left_column">    
    <img alt="" width="173" height="574" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_1.jpg")+"'" : String.Empty %> />
  </div>  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <% Event evnt = ViewData["CurrentEvent"] as Event; %>  
  <div id="categories" class="center_content">  
    <div class="page_title"> <%:String.Format("{0} > Categories", evnt.Title)%> </div>
    <% Html.RenderAction("pCategory", "Auction", new { event_id = evnt.ID });%>
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