<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="errorTitle" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Error</title>
</asp:Content>

<asp:Content ID="errorContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Sorry, an error occurred while processing your request.
    </h2>    
    <%=ViewData["Msg"] %>
</asp:Content>
