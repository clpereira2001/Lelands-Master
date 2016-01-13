<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%--<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
  Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>--%>
<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Consignors Statement - <%=Consts.CompanyTitleName%></title>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <h2>
    Consignors Statement Report</h2>
  <%
    List<SelectListItem> blank = new List<SelectListItem>();
    List<SelectListItem> blankStatus = new List<SelectListItem>
                                         {
                                           new SelectListItem{Selected = true, Text = "All", Value = "-1"},
                                           new SelectListItem{Selected = false, Text = "Paid", Value = "1"},
                                           new SelectListItem{Selected = false, Text = "Pending", Value = "0"},
                                           new SelectListItem{Selected = false, Text = "Amount Due", Value = "2"}
                                         };
%>
  <table>
    <tr>
      <td>
        <label for="ddlEvents">Event</label>
        <%=Html.DropDownList("ddlEvents", blank, new {@style="width:420px"}) %>
      </td><td>
        <label for="ddlUsers" style="margin-left: 10px">User</label>
        <%=Html.DropDownList("ddlUsers", blank, new {@style="width:230px"}) %>
      </td><td>
        <label for="ddlStatus" style="margin-left: 10px">Status</label>
        <%=Html.DropDownList("ddlStatus", blankStatus)%>
      </td><td>
        <label for="ddlDate" style="margin-left: 10px">Date</label>
        <%=Html.TextBox("ddlDate", DateTime.Now.ToString("d"), new { @style = "width:100px" })%>
      </td><td>
        <button id="btnInitReport" >Generate</button>
      </td>
    </tr>
  </table>
  <br />
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
      var src = "Report?report=0&event_id=" + $("#ddlEvents").val() + "&user_id=" + $("#ddlUsers").val() + "&status=" + $("#ddlStatus").val() + "&date=" + $("#ddlDate").attr("value");
      $("#ifrmReport").attr("src", src);      
    });
    $('#btnInitReport').addClass("ui-button ui-state-default"); //ui-corner-all
    $('#btnInitReport').hover(function () { $(this).addClass("ui-state-hover"); }, function () { $(this).removeClass("ui-state-hover"); }).mousedown(function () { $(this).addClass("ui-state-active"); }).mouseup(function () { $(this).removeClass("ui-state-active"); });
  </script>

  <iframe id="ifrmReport" src="Report?report=0&event_id=-1" frameborder="0" width="1100px" height="600px" />
</asp:Content>