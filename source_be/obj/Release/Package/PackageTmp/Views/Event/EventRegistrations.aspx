<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Events Registrations - <%=Consts.CompanyTitleName%></title> 
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Events Registrations</h2>
  <table id="e_list"></table>
  <div id="e_pager"></div> 
  
    <div id="form_eventreg" title="Event Registration" class="dialog_form">
    <fieldset style="margin:-10px;">
      <table>
      <tr>
      <td>
          <p>
            <label for="form_eventreg_Event">Event:<em>*</em></label>            
            <%= Html.DropDownList("form_consignor_Event", new List<SelectListItem>())%>
          </p>
        </td>
      </tr>
      <tr>
      <td id="tdUser">
        <p>
           <label for="form_eventreg_User">User:</label>            
          <%= Html.TextBox("form_eventreg_User", "", new { @class = "dialog_text", @readonly = "true", @res = "" })%>
        </p>
      </td>
      <td id="tdHB">
        <p>
           <label for="form_eventreg_HBNumber">The number of house bidders:</label>            
          <%= Html.TextBox("form_eventreg_HBNumber", ViewData["HouseBiddersAmountForEventRegstration"])%>
        </p>
      </td>
    </tr>
    </table>
  </fieldset>
  </div>
  
  <% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "dBuyer", Title = "Buyers list", Method = "/User/GetBuyersList/", SortName = "Login", SortOrder = "asc", ColumnHeaders = "'User#', 'Login', 'Name', 'Email', 'Phone', 'CommissionRate', 'LocationForAuctions'", Columns = "{ name: 'User_ID', index: 'User_ID', width: 50, key: true }, { name: 'Login', index: 'Login', width: 80 }, { name: 'FN', index: 'FN', width: 120}, { name: 'Email', index: 'Email', width: 100}, { name: 'Phone', index: 'Phone', width: 80 }, { name: 'CommRate_ID', index: 'CommRate_ID', width: 1, hidden:true }, {name:'LocationForAuctions',index:'LocationForAuctions',width:1,hidden:true}", ResultElement = "form_eventreg_User", ResultShow = "ret.Login+' ('+ret.FN+')'", ResultID = "ret.User_ID" }); %> 
    
</asp:Content>

<asp:Content ID="cntJS" ContentPlaceHolderID="jsContent" runat="server">
  <script type="text/javascript">
    var evreg_table_events = "<%=ViewData["EventsList"] %>";
    var evreg_table_usertype = "<%=ViewData["UserTypes"] %>";
  </script>  
  <%Html.ScriptV("EventRegistrations.js"); %>
</asp:Content>
