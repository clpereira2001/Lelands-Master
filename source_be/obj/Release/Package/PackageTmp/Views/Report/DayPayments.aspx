<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Daily payments - <%=ConfigurationManager.AppSettings["CompanyName"]%></title>  
</asp:Content>
 
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Daily Payments</h2>
  <table>
  <tr>
    <td>
       <label for="ddlDateFrom" style="margin-left:10px">From</label>
	     <%=Html.TextBox("ddlDateFrom", DateTime.Now.ToString("d"))%>
    </td><td>
	     <label for="ddlDateTo" style="margin-left:10px">To</label>
	     <%=Html.TextBox("ddlDateTo", DateTime.Now.ToString("d"))%>
    </td>
    <td><button id="btnInitReport" >Generate</button></td>
  </tr>
  </table> <br />
  <script type="text/javascript">    
    $("#btnInitReport").click(function () {
      var src = "Report?report=5&date=" + $("#ddlDateFrom").attr("value") + "&dateTo=" + $("#ddlDateTo").attr("value");
      $("#ifrmReport").attr("src", src);
    });
    $('#btnInitReport').addClass("ui-button ui-state-default"); //ui-corner-all
    $('#btnInitReport').hover(function () { $(this).addClass("ui-state-hover"); }, function () { $(this).removeClass("ui-state-hover"); }).mousedown(function () { $(this).addClass("ui-state-active"); }).mouseup(function () { $(this).removeClass("ui-state-active"); });
  </script>
  <iframe id="ifrmReport" src="Report?report=5" frameborder="0" width="1100px" height="600px" />
</asp:Content>