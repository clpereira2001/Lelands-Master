<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cntHeader" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Bid Log Preview - <%=Consts.CompanyTitleName%></title>  
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Bid Log Preview</h2>
  <table id="list"></table>
  <div id="pager"></div>
  
</asp:Content>

<asp:Content ID="cntJS" ContentPlaceHolderID="jsContent" runat="server">
  <%
    StringBuilder sbEvents = new StringBuilder();
    sbEvents.AppendFormat("<option value='{0}'>{1}</option>", -1, "All events");

    long currentEventID = -1;    
    foreach (Vauction.Models.IEvent evnt in (ViewData["Events"] as List<Vauction.Models.IEvent>))
    {
      sbEvents.AppendFormat("<option value='{0}' {2}>{1}</option>", evnt.ID, evnt.Title, (evnt.IsCurrent)?"selected":String.Empty);
      if (evnt.IsCurrent)currentEventID = evnt.ID;
    }
  %>
  <script type="text/javascript">
    var w = document.body.offsetWidth - 100;
    jQuery(document).ready(function() {
      jQuery("#list").jqGrid({
        url: '/Auction/BidLogList/',
        datatype: "json",
        height: "100%",
        width: w,
        colNames: ['Bid#', 'Event', 'Lot#', 'User', 'Bid', 'Max. Bid', 'Quantity', 'Placed Date', 'IP address', 'Proxy', 'Proxy raise', 'Retracted', 'Retr. Date', 'Retr. IP addr.'],
        colModel: [
   		    { name: 'Id', index: 'Id', width: 60, align: 'left' },
   		    { name: 'EventTitle', index: 'EventTitle', width: 140, sortable: false },
   		    { name: 'AuctionLot', index: 'AuctionLot', width: 60, sortable: false },
   		    { name: 'UserLogin', index: 'UserLogin', width: 120, sortable: false },
   		    { name: 'Amount', index: 'Amount', width: 100, align: 'right' }, //, formatter: 'currency', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, prefix: "$ "}
   		    {name: 'MaxBid', index: 'MaxBid', width: 100, align: 'right' }, //, formatter: 'currency', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, prefix: "$ "}
   		    { name: 'Quantity', index: 'Quantity', width: 60, align: 'right' },
   		    { name: 'DateMade', index: 'DateMade', width: 160 },
   		    { name: 'IP', index: 'IP', width: 100 },
   		    { name: 'IsProxy', index: 'IsProxy', width: 70, align: 'center' },
   		    { name: 'IsProxyRaise', index: 'IsProxyRaise', width: 70, align: 'center' },
   		    { name: 'IsRetracted', index: 'IsRetracted', width: 70, align: 'center' },
   		    { name: 'RetDate', index: 'RetDate', width: 160 },
   		    { name: 'RetractIP', index: 'RetractIP', width: 100 }
   	    ],
        loadtext: 'Loading Bid Log ...',
        rowNum: 20,
        rowList: [20, 40, 60],
        pager: '#pager',
        sortname: 'DateMade',
        viewrecords: true,
        sortorder: "desc",
        shrinkToFit: false,
        toolbar: [true, "top"],
        postData: { event_id: parseInt("<%=currentEventID %>") }
      });
      jQuery("#list").jqGrid('navGrid', '#pager', { edit: false, add: false, del: false, search: false, refreshtext: "Refresh" });
      
      $("#t_list").append("<span style='height:20px;font-size:-3; margin-left:10px'>Event: </span>");
      $("#t_list").append("<select id='t_list_event' style='height:20px;font-size:-3; margin-left:10px'><%=sbEvents.ToString() %></select>");
      $("#t_list").children("#t_list_event").change(function() {
        $("#list").jqGrid('setPostData', { event_id: parseInt($(this).val()) });
        $("#list").trigger("reloadGrid");
      });
    })
  </script>
</asp:Content>
