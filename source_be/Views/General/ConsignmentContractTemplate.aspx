<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cntHeader" ContentPlaceHolderID="HeadContent" runat="server">
<title>Consignment Contract Template - <%=Consts.CompanyTitleName%></title>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
<h2>Consignment Contract Template
</h2>
<h3>Terms And Conditions
</h3>
<div>
<%= Html.TextArea("ContractTemplate", (string)ViewData["ContractTemplate"], new { @style = "width:700px; height:610px" })%>
</div>
<div style="font-size:10px;">
The system replaces parameter {{CommissionRate}} with the sellers commission rate<br/>
The system replaces parameter {{BuyersPremium}} with the event's buyer's fee
</div>
<div style="margin-top: 15px;">
<button id="btnSave" class="ui-button ui-state-default">Save</button>
<button id="btnCancel" class="ui-button ui-state-default" style="margin-left: 15px">Cancel</button>
</div>
</asp:Content>

<asp:Content ID="cntJS" ContentPlaceHolderID="jsContent" runat="server">
<script type="text/javascript">
  $(document).ready(function () {
    $("#btnSave").click(function () {
      $.post('SaveConsignmentContractTemplate', { textTemplate: $("#ContractTemplate").val() }, function (data) {
        if (data.Type == 0) {
          window.location.reload();
        } else {
          alert(data.Message);
        }
      }, 'json');
    });

    $("#btnCancel").click(function () {
      window.location.reload();
    });
  });
</script>
</asp:Content>

