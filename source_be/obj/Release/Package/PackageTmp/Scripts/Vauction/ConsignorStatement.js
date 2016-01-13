function SellToBuyer(id, uid) {  
  if (id == null) { MessageBox('Sell to buyer', 'Please select row.', '', 'info'); return; }
  ConfirmBox('Sell to buyer', 'Do you realy want to sell this item to buyer?', function () {
    LoadingFormOpen();
    $.post('/Invoice/SellLotToBuyer', { auction_id: id, user_id:uid }, function (data) {
      LoadingFormClose();
      switch (data.Status) {
        case "ERROR":
          MessageBox("Sell to buyer", data.Message, '', "error");
          break;
        case "SUCCESS":
          MessageBox("Sell to buyere", "The item sold to buyer successfully.", '', "info");
          break;
      }
    }, 'json');
  });
}
var unsolditem_id = null;
$(document).ready(function () {
  //consignments
  var grid = $("#i_list").jqGrid({
    url: '/Invoice/GetStatementsBySearch/',
    datatype: "json",
    height: 70,
    mtype: 'GET',
    colNames: ['Statement#', 'Event', 'Seller', 'Pending', 'Total Cost', 'Amount Paid', 'Amount Due', 'User_id'],
    colModel: [
            { name: 'Id', index: 'Id', width: 80, key: true },
 		        { name: 'Event_Id', index: 'Event_Id', width: 160 },
 		        { name: 'User', index: 'User', width: 260 },
 		        { name: 'Pending', index: 'Pending', width: 140, align: 'right', sortable: false },
 		        { name: 'TCost', index: 'TCost', width: 140, align: 'right', sortable: false },
 		        { name: 'APaid', index: 'APaid', width: 140, align: 'right', sortable: false },
 		        { name: 'ADue', index: 'TDue', width: 140, align: 'right', sortable: false },
 		        { name: 'Owner_ID', index: 'Owner_ID', width: 10, hidden: true }
 	        ],
    loadtext: 'Loading ...',
    rowNum: 3,
    pager: '#i_pager',
    viewrecords: true,
    shrinkToFit: true,
    caption: "Consignor Statements",
    sortname: 'Id',
    sortorder: "asc",
    postData: { _firstload: true },
    onSelectRow: function (rowID) {
      var ret = $("#i_list").jqGrid('getRowData', rowID);
      $("#inv_list").jqGrid('setPostData', { cons_id: ret.Id });
      $("#inv_list").trigger('reloadGrid');
      $("#uns_list").jqGrid('setPostData', { cons_id: ret.Id });
      $("#uns_list").trigger('reloadGrid');
      $("#pay_list").jqGrid('setPostData', { cons_id: ret.Id });
      $("#pay_list").trigger('reloadGrid');
      $("#seller_list").jqGrid('setPostData', { cons_id: ret.Id })
      $("#seller_list").trigger('reloadGrid');
      $("#buyer_list").jqGrid('setPostData', { cons_id: ret.Id })
      $("#buyer_list").trigger('reloadGrid');
    }
  });
  $("#i_list").jqGrid('navGrid', '#i_pager', { edit: false, add: false, del: false, search: false, refreshtext: "Refresh" });
  $("#i_list").jqGrid('setPostData', { _firstload: false });

  //invoices
  $("#inv_list").jqGrid({
    mtype: 'POST',
    url: '/Invoice/GetInvoicesByConsignorStatement/',
    datatype: "json",
    height: 230,
    colNames: ['Stat#', 'Lot', 'Title', 'Date', 'Status', 'Winning Bid', 'Commission', 'Fee', 'Cons. Total', 'Paid To Seller'],
    colModel: [
            { name: 'Consugnment_Id', index: 'Consugnment_Id', width: 60, key: true, sortable: false, editable: true, editoptions: { readonly: true }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
            { name: 'Lot', index: 'Lot', width: 60, sortable: false, editable: true, editoptions: { readonly: true }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
 		        { name: 'Auction', index: 'Aucton', width: 240, sortable: false, editable: true, editoptions: { readonly: true }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
 		        { name: 'Date', index: 'Date', width: 100, sortable: false },
 		        { name: 'Status', index: 'Status', width: 100, sortable: false },
 		        { name: 'Amount', index: 'Amount', width: 100, align: 'right', sortable: false, editable: true, editoptions: { readonly: true }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
 		        { name: 'CommRate', index: 'CommRate', width: 100, align: 'left', sortable: false, editable: true, edittype: "select", editoptions: { value: cstatement_commrates }, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
 		        { name: 'Fee', index: 'Fee', width: 100, align: 'right', sortable: false, editable: false },
 		        { name: 'TCost', index: 'TCost', width: 100, align: 'right', sortable: false },
 		        { name: 'IsPaid', index: 'IsPaid', width: 100, align: 'center', sortable: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "    ", elmsuffix: ""} }
 	        ],
    loadtext: 'Loading ...',
    rowNum: 10,
    rowList: [10, 20, 40, 60, 80, 100],
    pager: '#inv_pager',
    viewrecords: true,
    shrinkToFit: true,
    sortname: 'Lot',
    sortorder: "asc",
    footerrow: true,
    userDataOnFooter: true,
    toolbar: [true, "bottom"],
    editurl: "/Invoice/EditStatementInvoice/",
    loadComplete: function () {
      var udata = $("#inv_list").jqGrid('getUserData');
      if (udata.Balance != undefined)
        $("#t_inv_list").html('<span style="margin:2px 7px 2px 0px;float:right">Balance:&nbsp;&nbsp;' + udata.Balance + '</span>');
    }
  });
  $("#inv_list").jqGrid('navGrid', '#inv_pager',
        { edit: true, edittext: "Edit", add: false, del: false, search: false, refreshtext: "Refresh" },
        { jqModal: false, closeOnEscape: true, afterSubmit: AfterSubmitFunction, bottominfo: "Fields marked with (*) are required", closeAfterEdit: false, bSubmit: "Save" }, {}, {}, {}, {});

  //unsold items
  $("#uns_list").jqGrid({
    url: '/Auction/GetConsignorStatementUnsoldItems/',
    datatype: "json",
    height: 230,
    colNames: ['Auction#', 'Lot', 'Title', 'Status', 'Price', 'Reserve', 'Estimate', 'Current Bid', 'Bids'],
    colModel: [
            { name: 'Auction_Id', index: 'Auction_Id', width: 60, sortable: false, key:true },
 		        { name: 'Lot', index: 'Lot', width: 80, sortable: false },
 		        { name: 'Title', index: 'Title', width: 310, sortable: false },
 		        { name: 'Status', index: 'Status', width: 100, sortable: false },
 		        { name: 'Price', index: 'Price', width: 100, align: 'right', sortable: false },
 		        { name: 'Reserve', index: 'Reserve', width: 100, align: 'right', sortable: false },
 		        { name: 'Estimate', index: 'Estimate', width: 100, sortable: false },
 		        { name: 'CurrentBid', index: 'CurrentBid', width: 100, align: 'right', sortable: false },
 		        { name: 'Bids', index: 'Bids', width: 80, align: 'right', sortable: false }
 	        ],
    loadtext: 'Loading ...',
    rowNum: 10,
    rowList: [10, 20, 40, 60, 80, 100],
    pager: '#uns_pager',
    viewrecords: true,
    shrinkToFit: true,
    sortname: 'Lot',
    onSelectRow: function (id) { unsolditem_id = id; },
    sortorder: "asc"
  });
  $("#uns_list").jqGrid('navGrid', '#uns_pager', { edit: false, add: false, del: false, search: false, refreshtext: "Refresh" }).navSeparatorAdd('#uns_pager');
  $("#uns_list").jqGrid('navButtonAdd', '#uns_pager', { caption: "Sell to buyer", buttonicon: 'ui-icon-suitcase', title: 'Sell this lot to buyer', onClickButton: function () { $('#dBuyers').dialog('open'); } }).navSeparatorAdd('#uns_pager');

  //payments
  $("#pay_list").jqGrid({
    url: '/Invoice/GetPaymentByStatement/',
    datatype: "json",
    height: 230,
    colNames: ['Payment#', 'Method', 'Amount', 'Date', 'Note', 'Cleared', 'Cleared Details'],
    colModel: [
             { name: 'Payment_Id', index: 'Payment_Id', width: 80, key: true, sortable: false, editable: true, editoptions: { readonly: true }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
             { name: 'Method', index: 'Method', width: 160, sortable: false, editable: true, edittype: "select", editoptions: { value: cstatement_paymentypes }, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
  		        { name: 'Amount', index: 'Amount', width: 120, sortable: false, editable: true, editrules: { edithidden: true, required: true, number: true }, editoptions: { defaultValue: function () { return "0.00" } }, formoptions: { elmprefix: "(*)", elmsuffix: "" }, align: 'right' },
  		        { name: 'Date', index: 'Date', width: 100, sortable: false, editable: true,
  		          editoptions: {
  		            defaultValue: function () {
  		              var currentTime = new Date();
  		              var month = parseInt(currentTime.getMonth() + 1);
  		              month = month <= 9 ? "0" + month : month;
  		              var day = currentTime.getDate();
  		              day = day <= 9 ? "0" + day : day;
  		              var year = currentTime.getFullYear();
  		              return month + "/" + day + "/" + year;
  		            }
  		          }, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" }
  		        },
  		        { name: 'Note', index: 'Note', width: 250, sortable: false, editable: true, edittype: "textarea", editoptions: { rows: "4", cols: "34" }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
  		        { name: 'Cleared', index: 'Cleared', width: 80, align: 'center', sortable: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
  		        { name: 'CDetails', index: 'CDetails', width: 250, sortable: false, editable: true, edittype: "textarea", editoptions: { rows: "4", cols: "34" }, formoptions: { elmprefix: "    ", elmsuffix: ""} }
  	        ],
    loadtext: 'Loading ...',
    rowNum: 10,
    rowList: [10, 20, 30],
    pager: '#pay_pager',
    viewrecords: true,
    shrinkToFit: true,
    footerrow: true,
    userDataOnFooter: true,
    editurl: "/Invoice/EditStatementPayment/",
    postdata: { cons_id: null }
  });
  $("#pay_list").jqGrid('navGrid', '#pay_pager',
         { edit: true, edittext: "Edit", add: true, addtext: "Add", del: false, search: false, refreshtext: "Refresh" },
         { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterEdit: true, width: 320, bSubmit: "Save", editCaption: "Edit payment", afterSubmit: AfterSubmitFunction, onclickSubmit: function (eparams) {
           var sr = $("#pay_list").jqGrid('getPostData', 'cons_id');
           return { cons_id: sr.cons_id };
         }
         }, //edit
         {jqModal: false, modal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterAdd: true, width: 320, editData: { cons_id: null }, bSubmit: "Save", addCaption: "Add payment", afterSubmit: AfterSubmitFunction,
         onclickSubmit: function (eparams) {
           var sr = $("#pay_list").jqGrid('getPostData', 'cons_id');
           return { cons_id: sr.cons_id };
         }
       }, {}, {}, {});

  //seller
  $("#seller_list").jqGrid({
    url: '/User/GetConsignSeller/',
    datatype: "json",
    colNames: ['User#', 'login', 'First Name', 'Last Name', 'E', 'P', 'S', 'B'],
    colModel: [
         { name: 'seller_user_id', index: 'seller_user_id', width: 60, key: true, sortable: false },
         { name: 'seller_login', index: 'seller_login', width: 60, key: true, sortable: false },
         { name: 'seller_first_name', index: 'seller_first_name', width: 60, sortable: false },
         { name: 'seller_last_name', index: 'seller_last_name', width: 60, sortable: false },
         { name: 'seller_email', index: 'seller_email', width: 60, sortable: false },
         { name: 'seller_dayphone', index: 'seller_dayphone', width: 60, sortable: false },
         { name: 'seller_bladdress', index: 'seller_bladdress', width: 60, sortable: false },
         { name: 'seller_shaddress', index: 'seller_shaddress', width: 60, sortable: false }
       ],
    loadtext: 'Loading ...',
    rowNum: 1,
    viewrecords: true,
    shrinkToFit: true,
    gridComplete: function () {
      var asd = $("#seller_list").jqGrid('getDataIDs');
      $("#seller_list").jqGrid('GridToForm', parseInt(asd[0]), "#seller_form");
    }
  });

  //buyer
  var wBuyer = document.body.offsetWidth - 670;
  $("#buyer_list").jqGrid({
    url: '/User/GetConsignBuyers/',
    datatype: "json",
    height: 420,
    width: wBuyer,
    colNames: ['Lot', 'Title', 'User#', 'Login', 'First & Last Name', 'Email', 'Day Phone', 'Billing address', 'Shipping address'],
    colModel: [
           { name: 'Lot', index: 'Lot', width: 40, sortable: false },
           { name: 'Title', index: 'Title', width: 60, sortable: false },
           { name: 'User_ID', index: 'User_ID', width: 60, sortable: false },
           { name: 'Login', index: 'Login', width: 80, sortable: false },
           { name: 'FL', index: 'FL', width: 140, sortable: false },
           { name: 'Email', index: 'Email', width: 100, sortable: false },
           { name: 'Phone', index: 'Phone', width: 80, sortable: false },
           { name: 'Billing', index: 'Billing', width: 200, sortable: false },
           { name: 'Shipping', index: 'Shipping', width: 200, sortable: false }
          ],
    loadtext: 'Loading ...',
    rowNum: 18,
    rowList: [18, 36, 54, 100],
    pager: '#buyer_pager',
    viewrecords: true,
    shrinkToFit: false,
    caption: 'Lot buyer(s)'
  });
  $("#buyer_list").jqGrid('navGrid', '#buyer_pager', { edit: false, add: false, del: false, search: false, refreshtext: "Refresh" });

  $("#dv_sellers_list").hide();

  $(function () {
    $("#tabs").tabs();
  });

  InitDropDown('/General/GetEventsClosedDateTimeShortJSON', '#event_id', function () {
    var last_id = $("#event_id option:first").val();
    var html = $("#event_id").html();
    $("#event_id option").remove();
    $("#event_id").append('<option value="-1"></option>' + html);
    $("#event_id").val(last_id);
  });

  $("#btnSearch").click(function () {
    if ($("#event_id").val() == -1 && $("#invoice_id").attr("value") == '' && $("#auction_id").attr("res") == '' && $("#user_id").attr("res") == '') {
    } else {
      $("#i_list").jqGrid('setPostData', {
        invoice_id: $("#invoice_id").attr("value"),
        event_id: $("#event_id").val(),
        auction_id: $("#auction_id").attr("res"),
        user_id: $("#user_id").attr("res"),
        _firstload: false
      });
    }
    $("#i_list").trigger("reloadGrid");
    $("#t_inv_list").css("text-align", "right").html("");
    $("#inv_list").jqGrid('setPostData', { cons_id: null });
    $("#inv_list").jqGrid('clearGridData', true);
    $("#uns_list").jqGrid('setPostData', { cons_id: null });
    $("#uns_list").jqGrid('clearGridData', true);
    $("#pay_list").jqGrid('setPostData', { cons_id: null });
    $("#pay_list").jqGrid('clearGridData', true);
    $("#seller_list").jqGrid('clearGridData');
    $("#seller_list").jqGrid('setPostData', { cons_id: null });
    $("#buyer_list").jqGrid('clearGridData');
    $("#buyer_list").jqGrid('setPostData', { cons_id: null });
    $("#seller_user_id, #seller_login, #seller_first_name, #seller_last_name, #seller_email, #seller_dayphone, #seller_shaddress, #seller_bladdress").attr("value", "");
  });

  $("#btnReset").click(function () {
    $("#invoice_id").attr("value", "");
    $("#event_id").val(-1);
    $("#auction_id").attr("res", "").attr("value", "");
    $("#user_id").attr("res", "").attr("value", "");
  });

  $("#invoice_id").keydown(function (event) {
    if (event.keyCode == 13) {
      $("#btnSearch").click();
    }
  });

  $("#auction_id").click(function () { $('#dAuctions').dialog('open'); })
  $("#user_id").click(function () { $('#dUsers').dialog('open'); })

  $(".reset_text").click(function () {
    $("#" + $(this).attr("elem")).attr("res", "").attr("value", "");
  });
});