<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Events Managment - <%=Consts.CompanyTitleName%></title>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Events Managment</h2>
  <table id="tblSchema">
    <tr>
      <td>
        Event: <%=Html.DropDownList("ddlEvents", new List<SelectListItem>())%>
        <span id="eventDescription" style="margin-left:10px">&nbsp;</span>        
      </td>      
      <% if (AppHelper.CurrentUser.IsRoot)
         { %>
      <td>
        <span id="spFirst">&nbsp;</span>
        <img src="<%=this.ResolveUrl("~/Content/images/blue-loading.gif") %>" id="imgLoadFirst" style="display:none; width:14px; height:14px;vertical-align:baseline" />
       </td>
       <td>
       <span id="spSecond">&nbsp;</span>
        <img src="<%=this.ResolveUrl("~/Content/images/blue-loading.gif") %>" id="imgLoadSecond" style="display:none; width:14px; height:14px;vertical-align:baseline" />
      </td>
     <%}else{ %>
      <td>&nbsp;</td>
     <%} %>
    </tr>
    <tr>
      <td>
        <table id="stat_list"></table>
        <div id="stat_pager"></div>
      </td>
      <td rowspan="2" colspan="2">
        <table id="bl_list"></table>
        <div id="bl_pager"></div>
      </td>
    </tr>
    <tr>
      <td>
        <table id="a_list"></table>
        <div id="a_pager"></div>
        <div id="a_filter" style="margin-left:30%;display:none">Search</div>
      </td>
    </tr>
  </table>  
</asp:Content>

<asp:Content ID="cntJS" ContentPlaceHolderID="jsContent" runat="server">  
  <%--<%Html.ScriptV("EventManagment.js"); %>--%>
  <script src="/Scripts/Vauction/EventManagment.js"></script>
</asp:Content>

