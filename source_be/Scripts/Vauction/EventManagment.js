function SetEventAsCurrent() {
  ConfirmBox('Set event as current', 'Do you really want to set this event as current and start bidding?', function() {
    $('#ddlEvents').attr('disabled', 'true');
    $("#imgLoadFirst").show();
    $("#spFirst").html('Set event as current (post the event)');
    LoadingFormOpen();
    $.post('/Event/SetEventAsCurrent', { event_id: $("#ddlEvents").val() }, function(data) {
      LoadingFormClose();
      switch (data.Status) {
        case "ERROR":
          MessageBox("Setting event as current", data.Message, '', "error");
          break;
        case "SUCCESS":
          MessageBox("Setting event as current", "The event is set as current and the bidding for this event is started.", '', "info");
          $("#eventDescription").html("This event is current.");
          $("#spFirst").html('Close current event (Step 1)');
          $("#spSecond").html('Close current event (Step 2)');
          $("#imgLoadFirst").hide();
          break;
      }
      $('#ddlEvents').removeAttr('disabled');
      $("#imgLoadFirst").hide();
      $("#ddlEvents").change();
    }, 'json');
  });
}

function CloseCurrentEvent(st) {
  ConfirmBox('Close current event', 'Do you really want to close current event (step ' + (st == 1 ? '1' : '2') + ')?', function() {
    $('#ddlEvents').attr('disabled', 'true');
    if (st == 1) {
      $("#imgLoadFirst").show();
      $("#spFirst").html('Close current event (Step 1)');
    } else {
      $("#imgLoadSecond").show();
      $("#spSecond").html('Close current event (Step 2)');
    }
    LoadingFormOpen();
    $.post('/Event/StopCurrentEvent', { event_id: $("#ddlEvents").val(), step: st }, function(data) {
      LoadingFormClose();
      switch (data.Status) {
        case "ERROR":
          MessageBox("Close current event", data.Message, '', "error");
          break;
        case "SUCCESS":
          if (parseInt(data.Details) == 1) {
            MessageBox("Close current event", data.Message, '', "info");
            $("#spFirst").html('Close current event (Step 1)');
            $("#spSecond").html('<a href="#" id="aCloseCurrentEventStep2" name="aCloseCurrentEventStep2" onclick="CloseCurrentEvent(2)">Close current event (Step 2)</a>');
            $("#imgLoadFirst").hide();
          } else {
            $("#imgLoadSecond").hide();
            MessageBox("Close current event", data.Message, '', "info");
            SendAfterAuctionEmails();
          }
          break;
      }
      $('#ddlEvents').removeAttr('disabled');
      $("#imgLoadFirst").hide();
      $("#ddlEvents").change();
    }, 'json');
  });
}

function SendAfterAuctionEmails() {
  ConfirmBox('Sending end of auction emails', 'Do you want to send end of auction emails?', function () {
    $("#imgLoadSecond").show();
    LoadingFormOpen();
    $.post('/Event/SendEndOfAuctionLetters', { event_id: $("#ddlEvents").val() }, function (data2) {
      $("#imgLoadSecond").hide();
      LoadingFormClose();
      switch (data2.Status) {
        case "ERROR":
          MessageBox("Sending letters", data2.Message, '', "error");
          break;
        case "SUCCESS":
          MessageBox("Close current event", data2.Message, '', "info");
          break;
      }
    }, 'json');
  });
}

//----------------------------------------------------------------------------------------------------------------
$(document).ready(function () {
  
  InitDropDown('/General/GetEventsViewableJSON?tmp=' + Math.round(Math.random() * 10), '#ddlEvents', function () {
    $("#ddlEvents").change();
  });
  $("#ddlEvents").change(function() {
    $.post('/Event/GetEventDataJSON', { event_ID: $("#ddlEvents").val() }, function(data) {
      if (data == null) return;
      $("#spFirst").html('&nbsp;');
      $("#spSecond").html('&nbsp;');
      $("#imgLoadFirst").hide();
      $("#imgLoadSecond").hide();
      curDate = new Date();
      var queryDate = data.s_1;
      dateParts = queryDate.match(/(\d+)/g);
      startDate = new Date(dateParts[0], dateParts[1] - 1, dateParts[2])
      queryDate = data.e_1;
      dateParts = queryDate.match(/(\d+)/g);
      endDate = new Date(dateParts[0], dateParts[1] - 1, dateParts[2])

      if (data.clst == 2) {
        $("#eventDescription").html("This event is closed.");
      } else if ((curDate - startDate) / 86400000 >= 1 && !data.ic) {
        $("#eventDescription").html("This event is pending (old).");
      } else if (!data.ic) {
        $("#eventDescription").html("This event is pending.");
        $("#spFirst").html('<a href="#" id="aSetEventAsCurrent" name="aSetEventAsCurrent" onclick="SetEventAsCurrent()">Set event as current (open the event)</a>');
        $("#spSecond").html('&nbsp;');
      } else if (data.ic) {
        if (data.clst == 0) {
          $("#eventDescription").html("This event is current.");
          $("#spFirst").html('<a href="#" id="aCloseCurrentEventStep1" name="aCloseCurrentEventStep1" onclick="CloseCurrentEvent(1)">Close current event (Step 1)</a>');
          $("#spSecond").html('Close current event (Step 2)');          
        } else {
          $("#eventDescription").html("This event is current.");
          $("#spFirst").html('Close current event (Step 1)');          
          $("#spSecond").html('<a href="#" id="aCloseCurrentEventStep2" name="aCloseCurrentEventStep2" onclick="CloseCurrentEvent(2)">Close current event (Step 2)</a>');
        }
      }

      $("#stat_list").jqGrid('setPostData', { event_id: data.id });
      $("#stat_list").trigger("reloadGrid");
      $("#a_list").jqGrid('setPostData', { event_id: data.id, isfirstload: false });
      $("#a_list").clearGridData(false);
      $("#bl_list").jqGrid('setPostData', { event_id: data.id, isfirstload: false });
      $("#bl_list").clearGridData(false);
      $("#t_bl_list, #tb_bl_list").html("&nbsp;");
    }, 'json');
  });

  $("#stat_list").jqGrid({
    url: '/Auction/BiddingStatistic/',
    datatype: "json",
    height: "100%",
    colNames: ['Without bids', 'One bid', 'More than one bid'],
    colModel: [
        { name: 'Zero', index: 'Zero', width: 185, align: 'center', sortable: false },
        { name: 'One', index: 'One', width: 185, align: 'center', sortable: false },
        { name: 'More', index: 'More', width: 190, align: 'center', sortable: false }
      ],
    loadtext: 'Loading ...',
    rowNum: 1,
    viewrecords: false,
    shrinkToFit: true,
    pager: '#stat_pager',
    caption: "Bidding statistic",
    postData: { event_id: null }
  });
  $("#stat_list").jqGrid('navGrid', '#stat_pager', { edit: false, add: false, del: false, search: false, refreshtext: "Refresh" });

  $("#a_list").jqGrid({
    url: '/Auction/ShortDetailedListEventManagment/',
    datatype: "json",
    height: 400,
    colNames: ['Auction#', 'Lot#', 'Title', 'Current Bid', 'Max Bid', 'Bids'],
    colModel: [
      { name: 'auction_id', index: 'auction_id', width: 1, hidden: true },
      { name: 'Lot', index: 'Lot', width: 50 },
      { name: 'Title', index: 'Title', width: 200 },
      { name: 'CurrentBid', index: 'CurrentBid', width: 100, align: 'right' },
      { name: 'MaxBid', index: 'MaxBid', width: 100, align: 'right' },
      { name: 'BidNumber', index: 'BidNumber', width: 80, align: 'center' }
    ],
    rowNum: 18,
    rowList: [18, 36, 54],
    pager: '#a_pager',
    viewrecords: false,
    shrinkToFit: true,
    sortname: 'BidNumber',
    sortorder: "desc",
    caption: "Current event Lots",
    postData: { event_id: null, isfirstload: true },
    subGrid: true,
    subGridUrl: '/Auction/AllBiddersForAuction/',
    subGridModel: [
      { name: ['User', 'User Type', 'Bid', 'Max. Bid', 'Placed Date', 'Q.'],
        width: [40, 100, 90, 90, 220, 10],
        params: ['auction_id']
}]
  });
  $("#a_list").jqGrid('navGrid', '#a_pager', { edit: false, add: false, del: false, search: false, refreshtext: "Refresh", afterRefresh: function() { $("#a_list")[0].clearToolbar(); } });
  $("#a_list").jqGrid('filterToolbar');
  $("#a_list").jqGrid('setPostData', { event_id: null, isfirstload: false });

  var w = document.body.offsetWidth - 700;
  $("#bl_list").jqGrid({
    url: '/Auction/BidLogListEventManagment/',
    datatype: "json",
    height: "100%",
    width: w,
    colNames: ['Bid#', 'Lot#', 'Title', 'User', 'Bid', 'Max. Bid', 'Placed Date', 'Proxy', 'Pr.Raise'],
    colModel: [
 		    { hidden: true, name: 'Id', index: 'Id', width: '60px', align: 'left', sortable: false },
 		    { name: 'AuctionLot', index: 'AuctionLot', width: '40px', sortable: false },
 		    { name: 'AuctionTitle', index: 'AuctionTitle', width: '100px', sortable: false },
 		    { name: 'UserLogin', index: 'UserLogin', width: '100px', sortable: false },
 		    { name: 'Amount', index: 'Amount', width: '80px', align: 'right', sortable: false },
 		    { name: 'MaxBid', index: 'MaxBid', width: '80px', align: 'right', sortable: false },
 		    { name: 'DateMade', index: 'DateMade', width: '120px', sortable: false },
 		    { name: 'IsProxy', index: 'IsProxy', width: '40px', align: 'center', sortable: false },
 		    { name: 'IsProxyRaise', index: 'IsProxyRaise', width: '40px', align: 'center', sortable: false }
 	    ],
    rowNum: 20,
    rowList: [20, 40, 60],
    pager: '#bl_pager',
    sortname: 'DateMade',
    viewrecords: true,
    sortorder: "desc",
    shrinkToFit: true,
    toolbar: [true, "both"],
    caption: "Bid Log Preview",
    postData: { event_id: null, isfirstload: true },
    loadComplete: function() {
      var udata = $("#bl_list").jqGrid('getUserData');
      $("#t_bl_list").css("text-align", "left").html("<span style='margin:2px 0px 2px 7px'>The number of new bids for last 10 minutes:&nbsp;&nbsp;" + udata.totalbid + "</span>");
      $("#tb_bl_list").css("text-align", "right").html("<span style='margin:2px 7px 2px 0px'>Bids total:&nbsp;&nbsp;" + udata.totalbidamount + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Buyer's premium:&nbsp;&nbsp;" + udata.buyers + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Total amount:&nbsp;&nbsp;" + udata.total + "</span>");
    }
  });
  $("#bl_list").jqGrid('navGrid', '#bl_pager', { edit: false, add: false, del: false, search: false, refreshtext: "Refresh" });
  $("#bl_list").jqGrid('setPostData', { event_id: null, isfirstload: false });
});