<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="c2" ContentPlaceHolderID="HeadContent" runat="server">
<%--  <%= Html.CompressCss(Url) %>
  <%= Html.CompressJs(Url) %>
  <% Html.Clear(); %>
  <link href="<%=this.ResolveUrl("~/Scripts/jsTree/themes/checkbox/style.css") %>" rel="stylesheet" type="text/css" />
  <script src="<%=this.ResolveUrl("~/Scripts/jsTree/jquery.tree.js") %>" type="text/javascript"></script>
  <script src="<%=this.ResolveUrl("~/Scripts/jsTree/plugins/jquery.tree.checkbox.js") %>" type="text/javascript"></script>--%>
  <%--<script src="/Scripts/Cookies.js" type="text/javascript"></script>  --%>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <%--<div id="form_event_CategoriesMap" style="margin:-10px -5px -10px -5px; overflow:auto; height:440px" ></div>
    <%=ViewData["InvoicesCount"] %>--%>
    <%--<div id="dvTemp"></div>--%>

    <input type="button" value="Send" id="semail"/>

</asp:Content>

<asp:Content ID="as" ContentPlaceHolderID="jsContent" runat="server">
<script>
//  $(document).ready(function () {
//    $("#semail").click(function() {
//      LoadingFormOpen();
//      $.post('/Event/SendEndOfAuctionLetters', { event_id: 915 }, function(data) {
//        LoadingFormClose();
//        switch (data.Status) {
//        case "ERROR":
//          MessageBox("Sending letters", data.Message, '', "error");
//          break;
//        case "SUCCESS":
//          MessageBox("Close current event", data.Message, '', "info");
//          break;
//        }
//      }, 'json');
//    });
//  });
</script>

 <%-- <script type="text/javascript">
    $(document).ready(function () {
      $("#form_event_CategoriesMap").tree({
        data: { 
          type: "json",
          async: true,
          opts: {
            method: "POST",
            url: "/Category/GetCategoriesMapTreeJson"
          }
        },
        ui: {
          theme_name: "checkbox"
        },
        plugins: {
          checkbox: { three_state: true }
        },
        callback: {
          onselect: function (node, tree_obj) {//example: $(node).attr("id");        
            alert(0);
          }
        }
      });
    });
  </script>
--%>
  <%--<script type="text/javascript" language="javascript">
    function DisplayVisits() {
      var numVisits = GetCookie("numVisits");
      numVisits = (numVisits) ? parseInt(numVisits) + 1  : numVisits = 1;
      $("#dvTemp").html(numVisits == 1 ? "This is your first visit." : "You have visited this page " + numVisits + " times.");
      var today = new Date();
      today.setTime(today.getTime() + 365 /*days*/ * 24 /*hours*/ * 60 /*minutes*/ * 60 /*seconds*/ * 1000 /*milliseconds*/);
      SetCookie("numVisits", numVisits, today);
    }
    $(document).ready(function () {
      DisplayVisits();
    });
  </script>--%>
</asp:Content>