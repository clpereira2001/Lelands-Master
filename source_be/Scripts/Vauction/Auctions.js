var auction_rowID = null;
//var inline_rowID = null; // hide
var winning_patch_number = null;
$(document).ready(function () {
  var items_table_w = document.body.offsetWidth - 100;
  var grid_a = $("#a_list").jqGrid({
    mtype: 'POST',
    url: '/Auction/GetAuctionsListForAuctionsPage/',
    datatype: "json",
    height: '100%',
    width: items_table_w,
    colNames: ['Inventory#', 'Event', 'Lot', 'Status', 'Main Category', 'Category', 'Title', 'Price', 'Reserve', 'Shipping', 'QTY', 'Tags', 'Start Date', 'End Date', 'Commission', 'Consignor', 'Page Layout', 'LOA', 'Pulled Out', 'Unsold', 'Catalog', 'Old#', 'Listed Date', 'Entered By', 'Specialist', 'High Bidder', 'Current Bid'],
    colModel: [
       { name: 'Auction_ID', index: 'Auction_ID', width: 60, key: true },
       { name: 'Event', index: 'Event', width: 140, stype: 'select', editoptions: { value: auction_table_events} },
       { name: 'Lot', index: 'Lot', width: 60, editable: false }, //, editable: true
       { name: 'Status', index: 'Status', width: 60, stype: 'select', editoptions: { value: auction_table_statuses} },
       { name: 'MainCategory', index: 'MainCategory', width: 80, stype: 'select', editoptions: { value: auction_table_maincategory} },
       { name: 'Category', index: 'Category', width: 160 },
       { name: "Title", index: "Title", width: 160},
       { hidden: true, name: 'Price', index: 'Price', width: 80, align: 'right' },
       { name: 'Reserve', index: 'Reserve', width: 80, align: 'right' },
       { name: 'Shipping', index: 'Shipping', width: 60, align: 'right', editable: false }, //, editable: true
       { name: 'Quantity', index: 'Quantity', width: 20, align: 'center', search: false },
       { name: 'Tags', index: 'Tags', width: 120, sortable: false, stype: 'select', editoptions: { value: auction_table_tags} },
       { name: 'StartDate', index: 'StartDate', width: 100, search: false },
       { name: 'EndDate', index: 'EndDate', width: 100, search: false },
       { name: 'CommRate', index: 'CommRate', width: 150, stype: 'select', editoptions: { value: auction_table_commissionrate} },
       { name: 'Seller', index: 'Seller', width: 80 },
       { name: 'PriorityDescription', index: 'PriorityDescription', width: 80, stype: 'select', editoptions: { value: auction_table_priority} },
       { name: 'LOA', index: 'LOA', width: 30, align: 'center', formatter: 'checkbox', sortable: false, search: false },
       { name: 'PulledOut', index: 'PulledOut', width: 30, align: 'center', formatter: 'checkbox', sortable: false, search: false },
       { name: 'IsUnsold', index: 'IsUnsold', width: 30, align: 'center', formatter: 'checkbox', sortable: false, search: false },
       { name: 'IsCatalog', index: 'IsCatalog', width: 50, align: 'center', formatter: 'checkbox', sortable: false, search: false },
       { name: 'OldAuction_ID', index: 'OldAuction_ID', width: 50 },
       { name: 'NotifiedOn', index: 'NotifiedOn', width: 120, search: false },
       { name: 'Enteredby', index: 'Enteredby', width: 80 },
       { name: 'Specialist', index: 'Specialist', width: 120 },
       { name: 'HighBidder', index: 'HighBidder', width: 100 },
       { name: 'CurrentBid', index: 'CurrentBid', width: 100, align: 'right' }
    ],
    loadtext: 'Loading ...',
    rowNum: 20,
    rowList: [20, 25, 30, 35, 40, 60, 80, 100, 500, 1000, 10000],
    pager: '#a_pager',
    viewrecords: true,
    shrinkToFit: false,
    sortname: 'Auction_ID',
    sortorder: 'desc',
    onSelectRow: function (id) {
      // (start)
      /*if (inline_rowID != null)
       $('#a_list').restoreRow(inline_rowID);
      inline_rowID = null;*/
      // (end)
      auction_rowID = id;
    },
    ondblClickRow: function (id) {
      OpenAuctionFormByID(id);
      // (start)
      /*if (id && id != inline_rowID) {
        $('#a_list').restoreRow(inline_rowID);
        $('#a_list').editRow(id, true, null, InlineRowEditingResult, '/Auction/UpdateLotNumber/', {}); // '/Auction/UpdateLotNumber/' '/Auction/UpdateShipping/'
       inline_rowID = id;
      }*/
      // (end)
    },
    postData: { isfirstload: true },
    subGrid: true,
    subGridUrl: '/Auction/GetImagesByAuction/',
    subGridModel: [
      {
        name: ['Images'],
        width: [1020],
        params: ['Auction_ID']
      }]
  });
  $("#a_list").jqGrid('navGrid', '#a_pager', { edit: false, add: false, del: false, search: false, refreshtext: "", afterRefresh: function () { grid_a[0].clearToolbar(); } }).navSeparatorAdd('#a_pager');
  //btnAdd
  $("#a_list").jqGrid('navButtonAdd', '#a_pager', { caption: "", buttonicon: 'ui-icon-plus', title: 'Add new lot', onClickButton: function () { InitAuctionForm(null); } });
  //btnEdit
  $("#a_list").jqGrid('navButtonAdd', '#a_pager', { caption: "", buttonicon: 'ui-icon-pencil', title: 'Edit lot', onClickButton: function () { OpenAuctionFormByID(auction_rowID); } }).navSeparatorAdd('#a_pager');
  //btnDel
  $("#a_list").jqGrid('navButtonAdd', '#a_pager', { caption: "", buttonicon: 'ui-icon-trash', title: 'Delete lot', onClickButton: function () { DeleteAuctionByID(auction_rowID); } }).navSeparatorAdd('#a_pager');
  //btnConsignor
  $("#a_list").jqGrid('navButtonAdd', '#a_pager', { caption: "Pr.contr", buttonicon: 'ui-icon-contact', title: 'Preview lot contract', onClickButton: function () { OpenConsignorFormByID(auction_rowID); } }).navSeparatorAdd('#a_pager');
  //btnConsignor
  $("#a_list").jqGrid('navButtonAdd', '#a_pager', { caption: "Pr.lot", buttonicon: 'ui-icon-image', title: 'Preview lot', onClickButton: function () { PreviewAuctionByID(auction_rowID); } }).navSeparatorAdd('#a_pager');
  //btnBid
  $("#a_list").jqGrid('navButtonAdd', '#a_pager', { caption: "Bids", buttonicon: 'ui-icon-clipboard', title: 'Bids', onClickButton: function () { AuctionBidsByID(auction_rowID); } }).navSeparatorAdd('#a_pager');
  //btnAddWinner
  $("#a_list").jqGrid('navButtonAdd', '#a_pager', { caption: "Add W.", buttonicon: 'ui-icon-star', title: 'Add winner to the closed lot without bids', onClickButton: function() { AddWinnerToAuction(auction_rowID); } }).navSeparatorAdd('#a_pager');
  //btnAddRights
  $("#a_list").jqGrid('navButtonAdd', '#a_pager', { caption: "Add R.", buttonicon: 'ui-icon-circle-plus', title: 'Add rights for user to participate in bidding on event\'s 1st close step', onClickButton: function() { AddRightsToAuction(auction_rowID); } }).navSeparatorAdd('#a_pager');

  $("#a_list").jqGrid('filterToolbar');

  $("#a_list").jqGrid('setPostData', { isfirstload: false });

  $.post('/Auction/ClearImagesForNewAuction', {}, function (data) { }, 'json');
});
//--[Functions]------------------------------------------------------------------------------------

function OpenAuctionFormByID(id) {
  if (id == null) { MessageBox('Edit auction', 'Please select the row', '', 'info'); return; }
  $.post('/Auction/GetAuctionJSON', { auction_id: id }, function(data) { LoadingFormOpen(); InitAuctionForm(data); LoadingFormClose(); }, 'json');
}

function OpenConsignorFormByID(id) {
  if (id == null) { MessageBox('Consignor statement details', 'Please select row.', '', 'info'); return; }
  LoadingFormOpen();
  $.post('/Auction/GetConsignorStatementByAuctionJSON', { auction_id: id }, function(data) { InitConsignorForm(data); LoadingFormClose(); }, 'json');
}

function PreviewAuctionByID(id) {
  if (id == null) { MessageBox('Preview auction', 'Please select row.', '', 'info'); return; }
  window.showModalDialog(sitepath + '/Auction/AuctionPreview/'+id, "Auction preview", "border=thin; center=1;dialogTop=1; dialogLeft=1; dialogWidth=940px; dialogHeight=700px");
}

function DeleteAuctionByID(id) {
  if (id == null) { MessageBox('Deleting the auction', 'Please select row.', '', 'info'); return; }
  ConfirmBox('Deleting the auction', 'Do you really want to delete selected auction?', function() {
    LoadingFormOpen();
    $.post('/Auction/DeleteAuction', { auction_id: id }, function(data) {
      LoadingFormClose();
      switch (data.Status) {
        case "ERROR":
          MessageBox("Deleting auction", data.Message, '', "error");
          break;
        case "SUCCESS":
          $("#a_list").jqGrid('delRowData', id);
          MessageBox("Deleting auction", "The auction was deleted successfully.", '', "info");
          break;
      }
    }, 'json');
  });
}

function AddWinnerToAuction(id) {
  if (id == null) { MessageBox('Add winner', 'Please select row.', '', 'info'); return; }
  ConfirmBox('Add winner', 'Do you really want to add a winner to this auction?', function() {
    winning_patch_number = 1;
    $('#auction_dBuyer').dialog('open');
  });
}

function AddWinnerToAuctionAfterSelect(id, userid) {
  if (userid == null) { MessageBox('Add winner', 'Please select the user.', '', 'info'); return; }
  LoadingFormOpen();
  $.post('/Auction/AddWinnerToAuction', { auction_id: id, user_id:userid }, function(data) {
    LoadingFormClose();
    switch (data.Status) {
      case "ERROR":
        MessageBox("Add winner", data.Message, '', "error");
        break;
      case "SUCCESS":
        MessageBox("Add winner", "The winner was added successfully.", '', "info");
        break;
    }
  }, 'json');
}

function AddRightsToAuction(id) {
  if (id == null) { MessageBox('Add rights', 'Please select row.', '', 'info'); return; }
  ConfirmBox('Add rights', 'Do you really want to add rights for the user to this auction?', function() {
    winning_patch_number = 2;
    $('#auction_dBuyer').dialog('open');
  });
}

function AddRightsToAuctionAfterSelect(id, userid) {
  if (userid == null) { MessageBox('Add rights', 'Please select the user.', '', 'info'); return; }
  LoadingFormOpen();
  $.post('/Auction/AddRightsToAuction', { auction_id: id, user_id: userid }, function(data) {
    LoadingFormClose();
    switch (data.Status) {
      case "ERROR":
        MessageBox("Add rights", data.Message, '', "error");
        break;
      case "SUCCESS":
        MessageBox("Add rights", "The rights for selected user were added successfully.", '', "info");
        break;
    }
  }, 'json');
}

function AuctionBidsByID(id) {
  if (id == null) { MessageBox('Auction bids', 'Please select row.', '', 'info'); return; }
  InitAuctionBidsForm(id);
}