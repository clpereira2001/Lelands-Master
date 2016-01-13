function RefundInvoice(id) {
  if (id == null) { MessageBox('Refunding invoice', 'Please select row.', '', 'info'); return; }
  ConfirmBox('Refunding invoice', 'Do you realy want to refund selected invoice?', function () {
    LoadingFormOpen();
    $.post('/Invoice/RefundInvoice', { invoice_id: id }, function (data) {
      LoadingFormClose();
      switch (data.Status) {
        case "ERROR":
          MessageBox("Refunding invoice", data.Message, '', "error");
          break;
        case "SUCCESS":
          MessageBox("Refunding invoice", "The invoice refunded successfully.", '', "info");
          break;
      }
    }, 'json');
  });
}

function RefundUserInvoice(id) {
  if (id == null) { MessageBox('Refunding invoice', 'Please select row.', '', 'info'); return; }
  ConfirmBox('Refunding invoice', 'Do you realy want to refund selected buyer invoice and all its items?', function () {
    LoadingFormOpen();
    $.post('/Invoice/RefundUserInvoice', { invoice_id: id }, function (data) {
      LoadingFormClose();
      switch (data.Status) {
        case "ERROR":
          MessageBox("Refunding invoice", data.Message, '', "error");
          break;
        case "SUCCESS":
          MessageBox("Refunding invoice", "The buyer invoice refunded successfully.", '', "info");
          break;
      }
    }, 'json');
  });
}
//------------------------------------------------------------------------------------------------------
var invoice_rowID = null;
var buyerinvoice_rowID = null;
//------------------------------------------------------------------------------------------------------
$(document).ready(function () {
  //userinvoices
  var grid = $("#i_list").jqGrid({
    url: '/Invoice/GetUserInvoicesBySearch/',
    mtype: 'GET',
    datatype: "json",
    height: 70,
    colNames: ['Invoice#', 'Event', 'Buyer', 'Net Cost', 'Shipping', 'Sales Tax', 'Late Fees', 'Total Cost', 'Amount Paid', 'Amount Due'],
    colModel: [
            { name: 'Id', index: 'Id', width: 80, hidden: false, key: true },
 		        { name: 'Event_Id', index: 'Event_Id', width: 180 },
 		        { name: 'User_ID', index: 'User_ID', width: 200 },
 		        { name: 'Amount', index: 'Amount', width: 100, align: 'right', sortable: false },
 		        { name: 'Shipping', index: 'Shipping', width: 100, align: 'right', sortable: false },
 		        { name: 'SalesTax', index: 'SalesTax', width: 100, align: 'right', sortable: false },
 		        { name: 'LateFees', index: 'LateFees', width: 100, align: 'right', sortable: false },
 		        { name: 'TCost', index: 'TCost', width: 100, align: 'right', sortable: false },
 		        { name: 'APaid', index: 'APaid', width: 100, align: 'right', sortable: false },
 		        { name: 'ADue', index: 'TDue', width: 100, align: 'right', sortable: false }
 	        ],
    loadtext: 'Loading ...',
    rowNum: 3,
    pager: '#i_pager',
    viewrecords: true,
    shrinkToFit: true,
    caption: "Invoice",
    sortname: 'Id',
    sortorder: "asc",
    postData: { _firstload: true },
    onSelectRow: function (rowID) {
      buyerinvoices_rowId = rowID;
      var ret = $("#i_list").jqGrid('getRowData', rowID);
      $("#inv_list").jqGrid('setPostData', { userinvoice_id: ret.Id });
      $("#inv_list").trigger('reloadGrid');
      $("#pay_list").jqGrid('setPostData', { userinvoice_id: ret.Id });
      $("#pay_list").trigger('reloadGrid');
      $("#buyer_list").jqGrid('setPostData', { userinvoice_id: ret.Id })
      $("#buyer_list").trigger('reloadGrid');
      $("#seller_list").jqGrid('setPostData', { userinvoice_id: ret.Id })
      $("#seller_list").trigger('reloadGrid');
    }
  });
  $("#i_list").jqGrid('navGrid', '#i_pager', { edit: false, add: false, del: false, search: false, refreshtext: "Refresh" }).navSeparatorAdd('#i_pager');
  //btnRefundInvoice
  $("#i_list").jqGrid('navButtonAdd', '#i_pager', { caption: "Refund", buttonicon: 'ui-icon-arrowrefresh-1-w', title: 'Refund selected invoice', onClickButton: function () { RefundUserInvoice(buyerinvoices_rowId); } }).navSeparatorAdd('#i_pager');
  $("#i_list").jqGrid('setPostData', { _firstload: false });

  //invoices
  $("#inv_list").jqGrid({
    url: '/Invoice/GetInvoicesByUserInvoice/',
    datatype: "json",
    mtype: 'GET',
    height: 230,
    colNames: ['Inv#', 'Lot', 'Title', 'Date', 'Net Cost', 'Shipping', 'Sales Tax', 'Late Fees', 'Total Cost', 'Is Paid', 'Is Sent'],
    colModel: [
            { name: 'Invoice_Id', index: 'Invoice_Id', width: 60, key: true, sortable: false, editable: true, editoptions: { readonly: true }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
            { name: 'Lot', index: 'Lot', width: 60, sortable: false, editable: true, editoptions: { readonly: true }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
 		        { name: 'Auction', index: 'Aucton', width: 240, sortable: false, editable: true, editoptions: { readonly: true }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
 		        { name: 'Date', index: 'Date', width: 100, sortable: false, editable: true, editoptions: { readonly: true }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
 		        { name: 'Amount', index: 'Amount', width: 100, align: 'right', sortable: false, editable: true, editrules: { edithidden: true, required: true, number: true, minValue: 0.01 }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
 		        { name: 'Shipping', index: 'Shipping', width: 100, align: 'right', sortable: false, editable: true, editrules: { edithidden: true, required: true, number: true, minValue: 0 }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
 		        { name: 'SalesTax', index: 'SalesTax', width: 100, align: 'right', sortable: false, editable: true, editrules: { edithidden: true, required: true, number: true, minValue: 0 }, formoptions: { elmprefix: "(*)", elmsuffix: ""}},
 		        { name: 'LateFees', index: 'LateFees', width: 100, align: 'right', sortable: false, editable: true, editrules: { edithidden: true, required: true, number: true, minValue: 0 }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
 		        { name: 'TCost', index: 'TCost', width: 100, align: 'right', sortable: true },
 		        { name: 'IsPaid', index: 'IsPaid', width: 80, align: 'center', sortable: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
 		        { name: 'IsSend', index: 'IsSend', width: 80, align: 'center', sortable: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "    ", elmsuffix: ""} }
 	        ],
    loadtext: 'Loading ...',
    rowNum: 10,
    rowList: [10, 20, 40, 60, 80, 100],
    pager: '#inv_pager',
    viewrecords: true,
    shrinkToFit: true,
    sortname: 'Id',
    sortorder: "asc",
    footerrow: true,
    userDataOnFooter: true,
    toolbar: [true, "bottom"],
    onSelectRow: function (id) { invoice_rowID = id; },
    loadComplete: function () {
      var udata = $("#inv_list").jqGrid('getUserData');
      if (udata.Balance != undefined)
        $("#t_inv_list").html('<span style="margin:2px 7px 2px 0px;float:right">Balance:&nbsp;&nbsp;' + udata.Balance + '</span>');
    },
    editurl: "/Invoice/EditInvoice/"
  });
  $("#inv_list").jqGrid('navGrid', '#inv_pager',
        { edit: true, edittext: "Edit", add: false, del: false, search: false, refreshtext: "Refresh" },
        { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", afterSubmit: AfterSubmitFunction, closeAfterEdit: false, bSubmit: "Save" }, {}, {}, {}, {}).navSeparatorAdd('#inv_pager');
  //btnRefundInvoice
  $("#inv_list").jqGrid('navButtonAdd', '#inv_pager', { caption: "Refund", buttonicon: 'ui-icon-arrowrefresh-1-w', title: 'Refund selected invoice', onClickButton: function () { RefundInvoice(invoice_rowID); } }).navSeparatorAdd('#inv_pager');
  
  //payments
  $("#pay_list").jqGrid({
    url: '/Invoice/GetPaymentByUserInvoice/',
    datatype: "json",
    height: 230,
    mtype: 'GET',
    colNames: ['Payment#', 'Method', 'Amount', 'Date', 'Note', 'Cleared', 'Cleared Details'],
    colModel: [
            { name: 'Payment_Id', index: 'Payment_Id', width: 80, key: true, sortable: false, editable: true, editoptions: { readonly: true }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
            { name: 'Method', index: 'Method', width: 160, sortable: false, editable: true, edittype: "select", editoptions: { value: binvoice_paymentypes }, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
 		        { name: 'Amount', index: 'Amount', width: 120, sortable: false, align: 'right', editable: true, editrules: { edithidden: true, required: true, number: true }, editoptions: { defaultValue: function () { return "0.00" } }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
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
 		        { name: 'Note', index: 'Note', width: 200, sortable: false, editable: true, edittype: "textarea", editoptions: { rows: "4", cols: "34" }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
 		        { name: 'Cleared', index: 'Cleared', width: 80, align: 'center', sortable: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
 		        { name: 'CDetails', index: 'CDetails', width: 200, sortable: false, editable: true, edittype: "textarea", editoptions: { rows: "4", cols: "34" }, formoptions: { elmprefix: "    ", elmsuffix: ""} }
 	        ],
    loadtext: 'Loading ...',
    rowNum: 10,
    rowList: [10, 20, 30],
    pager: '#pay_pager',
    viewrecords: true,
    shrinkToFit: true,
    footerrow: true,
    userDataOnFooter: true,
    editurl: "/Invoice/EditUserInvoicePayment/",
    postdata: { userinvoice_id: null }
  });
  $("#pay_list").jqGrid('navGrid', '#pay_pager',
        { edit: true, edittext: "Edit", add: true, addtext: "Add", del: false, search: false, refreshtext: "Refresh" },
        { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterEdit: true, width: 320, bSubmit: "Save", editCaption: "Edit payment", afterSubmit: AfterSubmitFunction, onclickSubmit: function (eparams) {
          var sr = $("#pay_list").jqGrid('getPostData', 'userinvoice_id');
          return { userinvoice_id: sr.userinvoice_id };
        }
        }, //edit
        {jqModal: false, modal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterAdd: true, width: 320, editData: { userinvoice_id: null }, afterSubmit: AfterSubmitFunction, bSubmit: "Save", addCaption: "Add payment",
        onclickSubmit: function (eparams) {
          var sr = $("#pay_list").jqGrid('getPostData', 'userinvoice_id');
          return { userinvoice_id: sr.userinvoice_id };
        }
      }, {}, {}, {});

  //buyer
  $("#buyer_list").jqGrid({
    url: '/User/GetInvoiceBuyer/',
    datatype: "json",
    colNames: ['User#', 'login', 'First Name', 'Last Name', 'E', 'P', 'S', 'B'],
    colModel: [
          { name: 'buyer_user_id', index: 'buyer_user_id', width: 60, key: true, sortable: false },
          { name: 'buyer_login', index: 'buyer_login', width: 60, key: true, sortable: false },
          { name: 'buyer_first_name', index: 'buyer_first_name', width: 60, sortable: false },
          { name: 'buyer_last_name', index: 'buyer_last_name', width: 60, sortable: false },
          { name: 'buyer_email', index: 'buyer_email', width: 60, sortable: false },
          { name: 'buyer_dayphone', index: 'buyer_dayphone', width: 60, sortable: false },
          { name: 'buyer_bladdress', index: 'buyer_bladdress', width: 60, sortable: false },
          { name: 'buyer_shaddress', index: 'buyer_shaddress', width: 60, sortable: false }
        ],
    loadtext: 'Loading ...',
    rowNum: 1,
    viewrecords: true,
    shrinkToFit: true,
    gridComplete: function () {
      var asd = $("#buyer_list").jqGrid('getDataIDs');
      $("#buyer_list").jqGrid('GridToForm', parseInt(asd[0]), "#buyer_form");
    }
  });

  //sellert
  var wSeller = document.body.offsetWidth - 670;
  $("#seller_list").jqGrid({
    url: '/User/GetInvoiceSellers/',
    datatype: "json",
    height: 420,
    width: wSeller,
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
    pager: '#seller_pager',
    viewrecords: true,
    shrinkToFit: false,
    caption: 'Lot owner(s)'
  });
  $("#seller_list").jqGrid('navGrid', '#seller_pager', { edit: false, add: false, del: false, search: false, refreshtext: "Refresh" });

  $("#dv_buyers_list").hide();

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
    $("#inv_list").jqGrid('setPostData', { userinvoice_id: null });
    $("#t_inv_list").css("text-align", "right").html("");
    $("#inv_list").jqGrid('clearGridData', true);
    $("#pay_list").jqGrid('setPostData', { userinvoice_id: null });
    $("#pay_list").jqGrid('clearGridData', true);
    $("#buyer_list").jqGrid('clearGridData');
    $("#buyer_list").jqGrid('setPostData', { userinvoice_id: null });
    $("#seller_list").jqGrid('clearGridData');
    $("#seller_list").jqGrid('setPostData', { userinvoice_id: null });
    $("#buyer_user_id, #buyer_login, #buyer_first_name, #buyer_last_name, #buyer_email, #buyer_dayphone, #buyer_shaddress, #buyer_bladdress").attr("value", "");
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



