<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<HotNew>>" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Related Links - <%=ConfigurationManager.AppSettings["CompanyName"] %></title>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
  <div style="text-align:justify; padding:0px 20px 20px 20px;font-size:12px">
    <div class="page_title">Lelands Latest News</div>

    <% if (Model.Count == 0)
         Response.Write("Sorry, there are no news for now."); %>
    <table cellpadding="0" cellspacing="0">
      <% string address;
         int count = 0;
        foreach (HotNew hn in Model) { %>
        <tr>          
          <td style='vertical-align:top;width:190px'>
            <% address = (String.IsNullOrEmpty(hn.ImagePath) || !System.IO.File.Exists(Server.MapPath("/public/images/hotnews/" + hn.ImagePath))) ? "/public/images/hotnews/buzz_blank.jpg" : "/public/images/hotnews/" + hn.ImagePath;%>
               <img src="<%=AppHelper.CompressImagePath(address) %>" />
          </td>
          <td style='text-align:justify;vertical-align:top'>
            <span style='font-weight:bold;color:#222'><%=hn.Title %></span><br />
            <span style='color:#666;font-style:italic'><%=hn.NewsDate.ToLongDateString() %></span><br /><br />
            <span style='color:#555'><%=hn.Description %></span>
          </td>
        </tr>
        <% if (++count != Model.Count){ %>
        <tr><td colspan="2"><hr /></td></tr>
        <%} %>
      <%} %>
    </table>
    <div class="back_link"><%=Html.ActionLink("Return to the Home page", "Index", new { controller = "Home", action = "Index" })%></div>
	</div>
</asp:Content>