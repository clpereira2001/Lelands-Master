<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="server">
   <title>Sales Tax Report - <%=Consts.CompanyTitleName %></title>  
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
  <%
    List<SelectListItem> blankStatus = new List<SelectListItem>();
    blankStatus.Add(new SelectListItem() { Selected = true, Text = "All", Value = "-1" });
    blankStatus.Add(new SelectListItem() { Selected = false, Text = "Paid", Value = "1" });
    blankStatus.Add(new SelectListItem() { Selected = false, Text = "Unpaid", Value = "0" });
  %>
  <h2>Sales Tax Report</h2>
  <table>
  <tr>
    <td>
       <label for="ddlEvents">Event</label>
	     <%=Html.DropDownList("ddlEvents", new List<SelectListItem>(), new { @style = "width:450px" })%>
	  </td><td>   
	     <label for="ddlStatus" style="margin-left:10px">Status</label>
	     <%=Html.DropDownList("ddlStatus", blankStatus)%>
    </td><td>
       <button id="btnInitReport" >Generate</button>
    </td>
  </tr>
  </table> <br />
  <script type="text/javascript">      
    InitDropDown('/General/GetEventsClosedDateTimeJSON', '#ddlEvents', function () { $("#ddlEvents").change(); });
    $("#btnInitReport").click(function () {
      var src = "Report?report=7&event_id=" + $("#ddlEvents").val() + "&status=" + $("#ddlStatus").val();
      $("#ifrmReport").attr("src", src);
    });
    $('#btnInitReport').addClass("ui-button ui-state-default"); //ui-corner-all
    $('#btnInitReport').hover(function () { $(this).addClass("ui-state-hover"); }, function () { $(this).removeClass("ui-state-hover"); }).mousedown(function () { $(this).addClass("ui-state-active"); }).mouseup(function () { $(this).removeClass("ui-state-active"); });
  </script>
  <iframe id="ifrmReport" src="Report?report=7&event_id=-1" frameborder="0" width="1100px" height="600px" />
</asp:Content>