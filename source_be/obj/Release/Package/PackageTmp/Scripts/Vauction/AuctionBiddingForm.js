function InitAuctionBidsForm(data) {
  $("#form_auction_bidding_bids_list").jqGrid('setPostData', { auction_id: data, isfirstload: false });
  $("#form_auction_bidding_bids_list").trigger('reloadGrid');
  $("#form_auction_bidding_bidslog_list").jqGrid('setPostData', { auction_id: data, isfirstload: false });
  $("#form_auction_bidding_bidslog_list").trigger('reloadGrid');
  $('#form_auction_bidding').dialog('open');
}
//----------------------------------------------------------------------------------------------------------------
$(document).ready(function () {
  $("#form_auction_bidding").dialog({
    bgiframe: true,
    autoOpen: false,
    height: 560,
    width: 805,
    modal: true,
    gridview: true
  });
  $("#form_auction_bidding_tabs").tabs();

  $("#form_auction_bidding_bids_list").jqGrid({
    url: '/Auction/GetBidsByAuctionID/',
    datatype: "json",
    height: 400,
    mtype: 'POST',
    width: 740,
    colNames: ['Bid#', 'User#', 'User', 'Bid', 'Max. Bid', 'QTY', 'Placed Date', 'Proxy', 'IP'],
    colModel: [
      { hidden: true, name: 'BidID', index: 'BidID', width: 60, align: 'left', sortable: false, key: true },
      { name: 'UserID', index: 'UserID', width: 80, editable: true, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" }, edittype: 'custom', editoptions: { custom_element: my_input, custom_value: my_value }
      },
      { name: 'User', index: 'User', width: 120, sortable: false, editable: false },
      { name: 'Amount', index: 'Amount', width: 90, align: 'right', sortable: false, editable: true, edittype: "select", editrules: { edithidden: true, required: true, number: true, minValue: 1 }, editoptions: { value: '1:1' }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
      { name: 'MaxBid', index: 'MaxBid', width: 90, align: 'right', sortable: false, editable: true, edittype: "select", editrules: { edithidden: true, required: true, number: true, minValue: 1 }, editoptions: { value: '1:1' }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
      { hidden: true, name: 'Quantity', index: 'Quantity', width: 40, align: 'center', sortable: false, editable: false, editrules: { edithidden: false, required: true, number: true, minValue: 1 }, editoptions: { defaultValue: function () { return "1" } }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
      { name: 'DateMade', index: 'DateMade', width: 150, sortable: false, editable: true,
        editoptions: {
          defaultValue: function () {
            var currentTime = new Date();
            var month = parseInt(currentTime.getMonth() + 1);
            month = month <= 9 ? "0" + month : month;
            var day = currentTime.getDate();
            day = day <= 9 ? "0" + day : day;
            var year = currentTime.getFullYear();
            return month + "/" + day + "/" + year + " " + parseInt(currentTime.getHours()) + ":" + parseInt(currentTime.getMinutes()) + ":" + parseInt(currentTime.getSeconds());
          }
        }, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" }
      },
      { name: 'IsProxy', index: 'IsProxy', width: 40, align: 'center', sortable: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "(*)", elmsuffix: "" }, editrules: { required: true} },
      { name: 'IP', index: 'IP', width: 105, sortable: false, editable: true, editrules: { edithidden: true, required: false }, editoptions: { defaultValue: function () { return "" } }, formoptions: { elmprefix: "    ", elmsuffix: ""} }
    ],
    rowNum: 60,
    rowList: [20, 40, 60, 80],
    pager: '#form_auction_bidding_bids_pager',
    viewrecords: true,
    shrinkToFit: false,
    postData: { auction_id: null, isfirstload: true },
    editurl: "/Auction/PlaceBid/"
  });
  $("#form_auction_bidding_bids_list").jqGrid('navGrid', '#form_auction_bidding_bids_pager',
    { edit: true, edittext: "Edit", add: true, addtext: "Add", del: true, deltext: 'Delete', search: false, refreshtext: "Refresh" },
    { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterEdit: true, width: 320, bSubmit: "Save", editCaption: "Edit bid", afterSubmit: AfterSubmitFunction, recreateForm: true,
      onclickSubmit: function (eparams) {
        var sr = $("#form_auction_bidding_bids_list").jqGrid('getPostData', 'auction_id');
        return { auction_id: sr.auction_id };
      }, beforeInitData: function (formid) {
        var sr = $("#form_auction_bidding_bids_list").jqGrid('getPostData', 'auction_id');
        $("#form_auction_bidding_bids_list").setColProp('Amount', { editoptions: { dataUrl: '/Auction/GetBidsAmountForAuction?auction_id=' + sr.auction_id} });
        $("#form_auction_bidding_bids_list").setColProp('MaxBid', { editoptions: { dataUrl: '/Auction/GetBidsAmountForAuction?auction_id=' + sr.auction_id} });
      }
    }, //edit
    {jqModal: false, modal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterAdd: true, width: 320, bSubmit: "Save", addCaption: "Place Bid", afterSubmit: AfterSubmitFunction, recreateForm: true,
    onclickSubmit: function (eparams) {
      var sr = $("#form_auction_bidding_bids_list").jqGrid('getPostData', 'auction_id');
      return { auction_id: sr.auction_id };
    }, beforeInitData: function (formid) {
      var sr = $("#form_auction_bidding_bids_list").jqGrid('getPostData', 'auction_id');
      $("#form_auction_bidding_bids_list").setColProp('Amount', { editoptions: { dataUrl: '/Auction/GetBidsAmountForAuction?auction_id=' + sr.auction_id} });
      $("#form_auction_bidding_bids_list").setColProp('MaxBid', { editoptions: { dataUrl: '/Auction/GetBidsAmountForAuction?auction_id=' + sr.auction_id} });
    }
  }, //add
    {jqModal: false, closeOnEscape: true, closeAfterAdd: true, bSubmit: "Delete", afterSubmit: AfterSubmitFunction, url: "/Auction/DeleteBid/", onclickSubmit: function (eparams) {
      var sr = $("#form_auction_bidding_bids_list").jqGrid('getPostData', 'auction_id');
      return { auction_id: sr.auction_id };
    }
  }, //delete
    {}, {}).navSeparatorAdd('#form_auction_bidding_bids_pager');

  // BID LOG TAB
  $("#form_auction_bidding_bidslog_list").jqGrid({
    url: '/Auction/GetBidsLogByAuctionID/',
    datatype: "json",
    height: 400,
    mtype: 'POST',
    width: 740,
    colNames: ['Bid#', 'User#', 'User', 'Bid', 'Max. Bid', 'QTY', 'Placed Date', 'Auto', 'Proxy', 'Pr.Raise', 'IP'],
    colModel: [
      { hidden: true, name: 'BidID', index: 'BidID', width: 60, align: 'left', sortable: false, key: true },
      { name: 'UserID', index: 'UserID', width: 60, editable: true, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" }, edittype: 'custom', editoptions: { custom_element: my_input, custom_value: my_value }
      },
      { name: 'User', index: 'User', width: 120, sortable: false, editable: false },
      { name: 'Amount', index: 'Amount', width: 80, align: 'right', sortable: false, editable: true,edittype: "select", editrules: { edithidden: true, required: true, number: true }, editoptions: {  value:'1:1' }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
      { name: 'MaxBid', index: 'MaxBid', width: 80, align: 'right', sortable: false, editable: true, edittype: "select", editrules: { edithidden: true, required: true, number: true }, editoptions: { value:'1:1' }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
      { hidden: true, name: 'Quantity', index: 'Quantity', width: 30, align: 'center', sortable: false, editable: false, editrules: { edithidden: false, required: true, number: true, minValue: 1 }, editoptions: { defaultValue: function () { return "1" } }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
      { name: 'DateMade', index: 'DateMade', width: 150, sortable: false, editable: true,
        editoptions: {
          defaultValue: function () {
            var currentTime = new Date();
            var month = parseInt(currentTime.getMonth() + 1);
            month = month <= 9 ? "0" + month : month;
            var day = currentTime.getDate();
            day = day <= 9 ? "0" + day : day;
            var year = currentTime.getFullYear();
            return month + "/" + day + "/" + year + " " + parseInt(currentTime.getHours()) + ":" + parseInt(currentTime.getMinutes()) + ":" + parseInt(currentTime.getSeconds());
          }
        }, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" }
      },
      { name: 'IsAutoBid', index: 'IsAutoBid', width: 30, align: 'center', sortable: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "(*)", elmsuffix: "" }, editrules: { required: true} },
      { name: 'IsProxy', index: 'IsProxy', width: 40, align: 'center', sortable: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "(*)", elmsuffix: "" }, editrules: { required: true} },
      { name: 'IsProxyRaise', index: 'IsProxyRaise', width: 40, align: 'center', sortable: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "(*)", elmsuffix: "" }, editrules: { required: true} },
      { name: 'IP', index: 'IP', width: 90, sortable: false, editable: true, editrules: { edithidden: true, required: false }, editoptions: { defaultValue: function () { return "" } }, formoptions: { elmprefix: "    ", elmsuffix: ""} }
    ],
    rowNum: 60,
    rowList: [20, 40, 60, 80],
    pager: '#form_auction_bidding_bidslog_pager',
    viewrecords: true,
    shrinkToFit: false,
    postData: { auction_id: null, isfirstload: true },
    editurl: "/Auction/UpdateBidLog/"
  });
  $("#form_auction_bidding_bidslog_list").jqGrid('navGrid', '#form_auction_bidding_bidslog_pager',
    { edit: true, edittext: "Edit", add: true, addtext: "Add", del: true, deltext: 'Delete', search: false, refreshtext: "Refresh" },
    { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterEdit: true, width: 320, bSubmit: "Save", editCaption: "Edit bid", afterSubmit: AfterSubmitFunction, recreateForm: true,
      onclickSubmit: function (eparams) {
        var sr = $("#form_auction_bidding_bidslog_list").jqGrid('getPostData', 'auction_id');
        return { auction_id: sr.auction_id };
      }, beforeInitData: function (formid) {
        var sr = $("#form_auction_bidding_bids_list").jqGrid('getPostData', 'auction_id');
        $("#form_auction_bidding_bidslog_list").setColProp('Amount', { editoptions: { dataUrl: '/Auction/GetBidsAmountForAuction?auction_id=' + sr.auction_id} });
        $("#form_auction_bidding_bidslog_list").setColProp('MaxBid', { editoptions: { dataUrl: '/Auction/GetBidsAmountForAuction?auction_id=' + sr.auction_id} });
      }
    }, //edit
    {jqModal: false, modal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterAdd: true, width: 320, bSubmit: "Add", addCaption: "Add Bid", afterSubmit: AfterSubmitFunction, recreateForm: true,
    onclickSubmit: function (eparams) {
      var sr = $("#form_auction_bidding_bidslog_list").jqGrid('getPostData', 'auction_id');
      return { auction_id: sr.auction_id };
    }, beforeInitData: function (formid) {
      var sr = $("#form_auction_bidding_bids_list").jqGrid('getPostData', 'auction_id');
      $("#form_auction_bidding_bidslog_list").setColProp('Amount', { editoptions: { dataUrl: '/Auction/GetBidsAmountForAuction?auction_id=' + sr.auction_id} });
      $("#form_auction_bidding_bidslog_list").setColProp('MaxBid', { editoptions: { dataUrl: '/Auction/GetBidsAmountForAuction?auction_id=' + sr.auction_id} });
    }
  }, //add
    {jqModal: false, closeOnEscape: true, closeAfterAdd: true, bSubmit: "Delete", afterSubmit: AfterSubmitFunction, url: "/Auction/DeleteBidLog/",
    onclickSubmit: function (eparams) {
      var sr = $("#form_auction_bidding_bidslog_list").jqGrid('getPostData', 'auction_id');
      return { auction_id: sr.auction_id };
    }
  }, //delete
    {}, {}).navSeparatorAdd('#form_auction_bidding_bidslog_pager');

});
/*----------------------------------------------------------------*/

function my_input(value, options) {
  var usr = '';
  var id = $("#form_auction_bidding_bids_list").jqGrid('getGridParam', 'selrow');
  if (id) {
    var ret = $("#form_auction_bidding_bids_list").jqGrid('getRowData', id);
    usr = ret.User;
  }
  return $('<input type="text" class = "dialog_text" readonly = "true" res = "' + value + '" style="width:182px" onclick="open_dialog_user(this)" value="' + usr + '" />');
}

function my_value(value) {
  return value.attr("res");
}

function open_dialog_user(control) {
  OpenChooseDialog_form_auction_bidding_dBuyer(control);
}


//OpenningAuctionForm
//function OpenAuctionFormByConsignmentID(id) {
//  if (id == null) { MessageBox('Add auction', 'Please select the consignor statement', '', 'info'); return; }
//  LoadingFormOpen();
//  $.post('/Auction/GetAuctionByConsignmentJSON', { consingment_id: id }, function(data) {
//    if (data.Status != null && data.Status == "ERROR") {
//      TransferModelToAuctionForm(data);
//    } else
//    InitAuctionFormByConsignment(data);
//    LoadingFormClose();
//  }, 'json');
//}