<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Consignor Settlement Report - <%=Consts.CompanyTitleName%></title>
</asp:Content>  
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Consignor Settlement Report</h2>  
  <table>
    <tr>
      <td>
         <label for="ddlEvents">Event</label>
	       <%=Html.DropDownList("ddlEvents", new List<SelectListItem>(), new {@style="width:450px"}) %>
	    </td>
      <td>
	       <label for="ddlUsers" style="margin-left:10px">User</label>
	       <%=Html.DropDownList("ddlUsers", new List<SelectListItem>(), new {@style="width:250px"}) %>
      </td>
      <td> 
         <button id="btnInitReport" >Generate</button>
      </td>
    </tr>
  </table> <br />
  <script type="text/javascript">
    $("#ddlEvents").change(function () {
      var ev_id = $("#ddlEvents").val();
      $("#ddlUsers option").remove();
      $("#ddlUsers").append('<option value="">All</option>');
      $.post('/User/GetSellersForEvent', { event_id: ev_id }, function (data) {
        switch (data.Status) {
          case "ERROR":
            MessageBox("Get sellers", data.Message, '', "error");
            break;
          case "SUCCESS":
            $.each(data.Details.rows, function (i, item) {
              $("#ddlUsers").append('<option value="' + item.val + '">' + item.txt + '</option>');
            });
            $("#ddlUsers").change();
            break;
        }
      }, 'json');
    });
    InitDropDown('/General/GetEventsClosedDateTimeJSON', '#ddlEvents', function () { $("#ddlEvents").change(); });
    $("#btnInitReport").click(function () {
      var src = "Report?report=3&event_id=" + $("#ddlEvents").val() + "&user_id=" + $("#ddlUsers").val();
      $("#ifrmReport").attr("src", src);
    });
    $('#btnInitReport').addClass("ui-button ui-state-default"); //ui-corner-all
    $('#btnInitReport').hover(function () {$(this).addClass("ui-state-hover");},function () {$(this).removeClass("ui-state-hover");}).mousedown(function () {$(this).addClass("ui-state-active");}).mouseup(function () {$(this).removeClass("ui-state-active");});
   </script>
   <iframe id="ifrmReport" src="Report?report=3&event_id=-1" frameborder="0" width="1100px" height="600px" />
</asp:Content>