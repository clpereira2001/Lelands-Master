<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="server">
   <title>Auction Paid Report - <%=Consts.CompanyTitleName %></title>  
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Auction Paid Report</h2>
  <table>
  <tr>
    <td>
       <label for="ddlEvents">Event</label>
	     <%=Html.DropDownList("ddlEvents", new List<SelectListItem>(), new { @style = "width:550px" })%>       
    </td>
    <td>
        <button id="btnInitReport" >Generate</button>
    </td>
  </tr>
  </table>
  <br />
  <script type="text/javascript">
    InitDropDown('/General/GetEventsClosedDateTimeJSON', '#ddlEvents');    
    $("#btnInitReport").click(function () {
      var src = "Report?report=1&event_id=" + $("#ddlEvents").val();
      $("#ifrmReport").attr("src", src);
    });
    $('#btnInitReport').addClass("ui-button ui-state-default"); //ui-corner-all
    $('#btnInitReport').hover(function () { $(this).addClass("ui-state-hover"); }, function () { $(this).removeClass("ui-state-hover"); }).mousedown(function () { $(this).addClass("ui-state-active"); }).mouseup(function () { $(this).removeClass("ui-state-active"); });
  </script>
  <iframe id="ifrmReport" src="Report?report=1&event_id=-1" frameborder="0" width="1100px" height="600px" />
</asp:Content>