function InitConsignorForm(data) {
  if (data == null) {
    $("#form_consignor_Cons_ID").attr('value', '0');
    $("#form_consignor_Seller").attr('title', '').attr('value', '').attr('res', '');
    defaultCommission = 0;
    $("#form_consignor_CommissionRate").val(defaultCommission);
    $("#form_consignor_ConsDate").attr("value", DateToString(new Date()));
    InitDropDown('/General/GetPendingEventsJSON', '#form_consignor_Event');
  } else {
    $("#form_consignor_Cons_ID").attr('value', data.id);
    $("#form_consignor_Seller").attr('value', data.u_t).attr('title', data.u_t).attr('res', data.u);
    $("#form_consignor_CommissionRate").val(data.c);
    $("#form_consignor_Specialist").val(data.s);
    $("#form_consignor_Contract_StatusID").html(data.ccst);
    $("#form_consignor_Contract_Text").val(data.cct);
    $("#contractFileLink").attr("href", "/Auction/GetConsignmentContract?consignmentID=" + data.id);
    if (data.ccs == 0) {
      $("#contractFileLink, #conscontract, #btnRegenerateContract").hide();
      $("#contractText, #btnResetContractText, #btnSaveContractText").show();
    } else {
      $("#conscontract").attr('data', '/Auction/ShowConsignmentContract?consignmentID=' + data.id);
      $("#contractFileLink, #btnResetContractText, #conscontract, #btnRegenerateContract").show();
      $("#contractText, #btnSaveContractText").hide();
    }
    if (data.ccs == 2) {
      $("#btnSaveContractText, #btnResetContractText, #btnRegenerateContract").hide();
    }
    defaultCommission = data.c;
    selectedevent = data.e;
    InitDropDown('/General/GetEventsJSON', '#form_consignor_Event', function () { $("#form_consignor_Event").val(data.e); });
    $("#form_consignor_ConsDate").attr("value", data.cd);
  }
  InitConsignorStatmentTabs(data != null);
  SetDefaultNewItemForm();
  SetDefaultNewItemForm2();
  $('#form_consignor').dialog('open');
}
function InitConsignorStatmentTabs(visible) {
  if (visible) {
    $("#dAuctions_list").jqGrid('setPostData', { owner_id: $("#form_consignor_Seller").attr('res') });
    $("form_consignor_tabs-2, #form_consignor_tabs-3, #form_consignor_tabs-4").show(); //#form_consignor_tabs-5,
    $("#btnUpdateConsignmentStatement").text("Update consignor statement");
    $("#form_consignor_Event, #form_consignor_Seller, #btnAddNewUser").attr("disabled", "true");
    var i = $("#form_consignor_Cons_ID").attr('value');
    //$("#con_list").jqGrid('setPostData', { cons_id: i });
    $("#pay_list").jqGrid('setPostData', { cons_id: i });
    $("#item_list").jqGrid('setPostData', { cons_id: i });
    $("#form_consignor_MainCategory").val(1);
    SetCategoryByMainCategory();
    $("*", "#form_consignor").removeClass("input-validation-error");
  } else {
    $("#dAuctions_list").jqGrid('setPostData', { owner_id: null });
    $("#form_consignor_tabs-2, #form_consignor_tabs-3, #form_consignor_tabs-4").hide(); //#form_consignor_tabs-5,
    $("#btnUpdateConsignmentStatement").text("Add new consignor statement");
    $("#form_consignor_Event, #form_consignor_Seller,#btnAddNewUser").removeAttr("disabled");
    //$("#con_list").jqGrid('setPostData', { cons_id: null });
    $("#pay_list").jqGrid('setPostData', { cons_id: null });
    $("#item_list").jqGrid('setPostData', { cons_id: null });
  }
  $('#dAuctionsA_list,#pay_list,#item_list, #dSellers_list').jqGrid('clearGridData');
}
function SetDefaultNewItemForm() {
  $("#form_consignor_OldInventory").attr('title', '').attr('value', '').attr('res', '');
  $("#form_consignor_Price,#form_consignor_Title,#form_consignor_CopyNotes,#form_consignor_PhotoNotes,#form_consignor_Description").attr("value", "");
  $("#form_consignor_Description").change();
  $("#form_consignor_LOA").removeAttr("checked");
  $("#form_consignor_Auction_ID").attr("value", "-1");
  $("#form_consignor_Step").val(0);
  form_consignor_SetDescritptionMaxLenght($("#form_consignor_Priority").val());
  $('#form_consignor_sleft').html('0').css('color', '#222');
  $("#form_consignor_DisableLimit").attr("checked", "checked").change();
}
function SetDefaultNewItemForm2() {
  $("#form_consignor_Quantity").attr("value", "1");
  $("#form_consignor_MainCategory").val(1);
  SetCategoryByMainCategory();
  $("#form_consignor_Category").attr('title', '').attr('value', '').attr('res', '');
  $("#form_consignor_CommissionRate").val(defaultCommission);
  $("#form_consignor_Priority").val(4);
  $("#form_consignor_Priority").change();
  $("#form_consignor_PurchasedPrice,#form_consignor_SoldPrice").attr("value", "");
  $("#form_consignor_PurchasedWay,#form_consignor_SoldWay").val(-1);
}
function TransferModelToConsignmentStatementForm(data) {
  var msg = data.Message;
  var det = '';
  if (data.Details != null) {
    det += '<ul>';
    var exists = false;
    $.each(data.Details, function (i, item) {
      exists = true;
      switch (item.field) {
        case 'User_ID': $("#form_consignor_Seller").addClass("input-validation-error"); break;
        case 'Event_ID': $("#form_consignor_Event").addClass("input-validation-error"); break;
        case 'CommissionRate_ID': $("#form_consignor_CommissionRate").addClass("input-validation-error"); break;
        case 'Quantity': $("#form_consignor_Quantity").addClass("input-validation-error"); break;
        case 'MainCategory_ID': $("#form_consignor_MainCategory").addClass("input-validation-error"); break;
        case 'Category_ID': $("#form_consignor_Category").addClass("input-validation-error"); break;
        case 'Priority': $("#form_consignor_Priority").addClass("input-validation-error"); break;
        case 'OldInventory': $("#form_consignor_OldInventory").addClass("input-validation-error"); break;
        case 'Price': $("#form_consignor_Price").addClass("input-validation-error"); break;
        case 'LOA': $("#form_consignor_LOA").addClass("input-validation-error"); break;
        case 'Title': $("#form_consignor_Title").addClass("input-validation-error"); break;
        case 'CopyNotes': $("#form_consignor_CopyNotes").addClass("input-validation-error"); break;
        case 'PhotoNotes': $("#form_consignor_PhotoNotes").addClass("input-validation-error"); break;
        case 'Description': $("#form_consignor_Description").addClass("input-validation-error"); break;
        default: exists = false; break;
      }
      det += (exists) ? ('<li>' + item.message + '</li>') : '';
    });
    det += '</ul>';
  }
  MessageBox("Adding new item to consignor statement", msg, det, "error");
}
function UpdateConsignmentStatementForm() {
  return {
    ID: $("#form_consignor_Cons_ID").attr('value'),
    User_ID: $("#form_consignor_Seller").attr('res'),
    Event_ID: $("#form_consignor_Event").val(),
    ConsDate: $("#form_consignor_ConsDate").attr("value"),
    Specialist_ID: $("#form_consignor_Specialist").val()
  }
}
function AddNewItemForConsignorStatementForm() {
  return {
    User_ID: $("#form_consignor_Seller").attr('res'),
    Event_ID: $("#form_consignor_Event").val(),
    Auction_ID: $("#form_consignor_Auction_ID").attr("value"),
    CommissionRate_ID: $("#form_consignor_CommissionRate").val(),
    Quantity: $("#form_consignor_Quantity").attr("value"),
    MainCategory_ID: $("#form_consignor_MainCategory").val(),
    Category_ID: $("#form_consignor_Category").attr('res'),
    Priority: $("#form_consignor_Priority").val(),
    OldInventory: $("#form_consignor_OldInventory").attr('res'),
    Price: $("#form_consignor_Price").attr("value"),
    ListingStep: $("#form_consignor_Step").val(),
    LOA: $("#form_consignor_LOA").attr("checked"),
    Title: $("#form_consignor_Title").attr("value"),
    CopyNotes: $("#form_consignor_CopyNotes").attr("value"),
    PhotoNotes: $("#form_consignor_PhotoNotes").attr("value"),
    Description: $("#form_consignor_Description").attr("value"),
    IsLimitDisabled: $("#form_consignor_DisableLimit").attr("checked"),
    PurchasedPrice: $("#form_consignor_PurchasedPrice").attr("value"),
    SoldPrice: $("#form_consignor_SoldPrice").attr("value"),
    PurchasedWay: $("#form_consignor_PurchasedWay").val(),
    SoldWay: $("#form_consignor_SoldWay").val()
  };
}
function AddNewItemForConsignor() {
  var desc = $('#form_consignor_Description').val().length;
  var lim = sarr[$("#form_consignor_Priority").val()] + 100;
  if (desc > lim && !$("#form_consignor_DisableLimit").attr("checked")) {
    MessageBox("Adding/Updating item", "Warning! You are over your character limit for this item. Please revise your copy", '', "error");
    $("#form_auction_Description").addClass("input-validation-error");
    return;
  }
  $("*", "#form_consignor").removeClass("input-validation-error");
  ConfirmBox('Saving item', 'Do you really want to save this item into the \'' + $("#form_consignor_MainCategory option:selected").text() + ' > ' + $("#form_consignor_Category").attr('value') + '\' category?', function () {
    LoadingFormOpen();
    $.post('/Auction/AddNewItemForConsignorStatement', { item: Sys.Serialization.JavaScriptSerializer.serialize(AddNewItemForConsignorStatementForm()) }, function (data) {
      LoadingFormClose();
      switch (data.Status) {
        case "ERROR":
          TransferModelToConsignmentStatementForm(data);
          break;
        case "SUCCESS":
          //$("#item_list").trigger('reloadGrid');
          SetDefaultNewItemForm();
          if (!addMore) SetDefaultNewItemForm2();
          MessageBox("Adding/Updating item", "Item was saved successfully.", '', "info");
          break;
      }
    }, 'json');
  });
}
function UpdateConsignmentStatement() {
  $("*", "#form_consignor").removeClass("input-validation-error");
  var id = $("#form_consignor_Seller").attr("res");
  if (id == '') {
    MessageBox("Update consignor statement", "Please select the consignor.", '', "error");
    $("#form_consignor_Seller").addClass("input-validation-error");
    return;
  }
  LoadingFormOpen();
  $.post('/Auction/UpdateConsignorStatement', { cons: Sys.Serialization.JavaScriptSerializer.serialize(UpdateConsignmentStatementForm()) }, function (data) {
    LoadingFormClose();
    switch (data.Status) {
      case "ERROR":
        MessageBox("Updating consignor statement", data.Message, '', "error");
        break;
      case "SUCCESS":
        selectedevent = $("#form_consignor_Event").val();
        $("#form_consignor_Cons_ID").attr('value', data.Details);
        InitConsignorStatmentTabs(true);
        MessageBox("Update consignor statement", "Consignor statement information saved successfully.", '', "info");
        break;
    }
  }, 'json');
}
function InitNewSeller() {
  var t = $("#form_user_Login").attr('value') + ' (' + $("#form_user_b_FirstName").attr('value') + ' ' + $("#form_user_b_LastName").attr('value') + ')';
  $("#form_consignor_Seller").attr('title', t).attr('value', t);
  t = $("#form_user_User_ID").attr('value');
  $("#form_consignor_Seller").attr('res', t);
  t = $("#form_user_CommissionRate_ID").val();
  $("#form_consignor_CommissionRate").val(t);
  $("#dSellers_list").trigger('reloadGrid');
}
function InitNewSellerLoading() {
  $("#form_user_UserType_ID option").attr("disabled", "true");
  $("#form_user_UserType_ID option[value=5],#form_user_UserType_ID option[value=6]").removeAttr("disabled");
  $("#form_user_UserType_ID").val(6);
  $("#form_user_UserType_ID").change();
  $("#form_user_UserStatus_ID").val(2);
}
function EditItemByID(id) {
  var id = $("#item_list").jqGrid('getGridParam', 'selrow');
  if (id == null) { MessageBox('Edit item', 'Please select item.', '', 'info'); return; }
  var ret = $("#item_list").jqGrid('getRowData', id);
  $("#form_consignor_Auction_ID").attr("value", ret.Auction_ID);
  $("#form_consignor_CommissionRate").val(ret.CommissionRate_ID);
  $("#form_consignor_Quantity").attr("value", ret.Quantity);
  $("#form_consignor_MainCategory").val(ret.MainCategory_ID);
  SetCategoryByMainCategory();
  $("#form_consignor_Priority").val(ret.Priority);
  $("#form_consignor_Step").val(ret.ListingStep);
  form_consignor_SetDescritptionMaxLenght(ret.Priority);
  $("#form_consignor_Price").attr("value", ret.Price);
  if (ret.LOA == 'Yes') $("#form_consignor_LOA").attr("checked", "true"); else $("#form_consignor_LOA").removeAttr("checked");
  $("#form_consignor_Title").attr("value", ret.Title);
  $("#form_consignor_CopyNotes").attr("value", ret.CopyNotes);
  $("#form_consignor_PhotoNotes").attr("value", ret.PhotoNotes);
  $("#form_consignor_Description").attr("value", ret.Description);
  $("#form_consignor_Description").change();
  $("#form_consignor_tabs").tabs('select', 2);
  $("#form_consignor_OldInventory").attr('res', ret.OldAuction_ID).attr('value', ret.OldAuction_Title).attr('title', ret.OldAuction_Title);
  $("#form_consignor_Category").attr('res', ret.Category_ID).attr('value', ret.CategoryTitle).attr('title', ret.CategoryTitle);
  if (parseInt(ret.IsLimitDisabled) == 1) $("#form_consignor_DisableLimit").attr("checked", "true"); else $("#form_consignor_DisableLimit").removeAttr("checked");
  $("#form_consignor_DisableLimit").change();
  $("#form_consignor_PurchasedPrice").attr("value", ret.PurchasedPrice);
  $("#form_consignor_SoldPrice").attr("value", ret.SoldPrice);
  $("#form_consignor_PurchasedWay").val(ret.PurchasedWay);
  $("#form_consignor_SoldWay").val(ret.SoldWay);
}
function form_consignor_SetDescritptionMaxLenght(priority) {
  $('#form_consignor_stotal').html(sarr[priority]);
  $('#form_consignor_range').html(sarr[priority] + '-' + (sarr[priority] + 100));
  setformfieldsize($('#form_consignor_Description'), sarr[priority] + 100, 'form_consignor_sleft');
}
function form_consignor_DisableDescriptionLenght() {
  $('#form_consignor_stotal').html("oo");
  setformfieldsize($('#form_consignor_Description'), 8000, 'form_consignor_sleft');
}
function SetCategoryByMainCategory() {
  $('#form_consignor_Category').attr('title', '').attr('value', '').attr('res', '');
  $("#dCategory_list").jqGrid('setPostData', { maincat_id: $("#form_consignor_MainCategory").val(), event_id: selectedevent });
  $("#dCategory_list").trigger('reloadGrid');
}
function SetDefaultCommissionAndClearSeller() {
  $('#form_consignor_CommissionRate').val(0);
  $('#form_consignor_Seller').attr('title', '').attr('value', '').attr('res', '');
}
function ActivateSeller(id, status, commrate_id) {
  if (status != 2) {
    ConfirmBox('Selecting seller', 'The user status is not Active. Do you want to activate the user account?', function () {
      LoadingFormOpen();
      $.post('/User/ActivateUserWithoutEmail', { user_id: id }, function (data) {
        LoadingFormClose();
        switch (data.Status) {
          case "ERROR":
            SetDefaultCommissionAndClearSeller();
            MessageBox("Activating user account", data.Message, '', "error");
            break;
          case "SUCCESS":
            $('#form_consignor_CommissionRate').val(commrate_id);
            break;
        }
      }, 'json');
      return;
    }, function () {
      SetDefaultCommissionAndClearSeller();
    });
  } else
    $('#form_consignor_CommissionRate').val(commrate_id);
}
function ValidSeller(id, type, status, commrate_id, strtype) {
  if (parseInt(type) != 5 && parseInt(type) != 6) {
    ConfirmBox('Selecting seller', 'The user type is ' + strtype + '. Do you want to change the user type and set it as SellerBuyer?', function () {
      LoadingFormOpen();
      $.post('/User/ChangeUserTypeToSellerBuyer', { user_id: id }, function (data) {
        LoadingFormClose();
        switch (data.Status) {
          case "ERROR":
            SetDefaultCommissionAndClearSeller();
            MessageBox("Changing the user type", data.Message, '', "error");
            break;
          case "SUCCESS":
            ActivateSeller(id, status, commrate_id);
            break;
        }
      }, 'json');
      return;
    }, function () { SetDefaultCommissionAndClearSeller(); });
  } else
    ActivateSeller(id, status, commrate_id);
}
//----------------------------------------------------------------------------------------------------------------
var addMore = false;
var defaultCommission = 0;
var sarr = [0, 7900, 750, 650, 650];
var selectedevent = null;
$(document).ready(function () {
  $("#form_consignor").dialog({
    bgiframe: true,
    autoOpen: false,
    height: 700,
    width: 790,
    modal: true,
    gridview: true,
    position: ['center', 'top']
  });
  $("#form_consignor_tabs").tabs();

  $("#form_consignor_accordion").accordion({
    autoHeight: false,
    navigation: true
  });

  /*
  $("#con_list").jqGrid({
  mtype: 'GET',
  url: '/Auction/GetContractItems/',
  datatype: "json",
  height: 520,
  width: 725,
  colNames: ['ID', 'Item', 'Commission Rate'],
  colModel: [
  { name: 'Contract_ID', index: 'Contract_ID', width: 1, key: true, sortable: false, hidden: true },
  { name: 'Title', index: 'Title', width: 500, sortable: false, editable: true, editrules: { edithidden: true, required: true }, editoptions: { defaultValue: function () { return "" } }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
  { name: 'CommissionRate_ID', index: 'CommissionRate_ID', width: 190, sortable: false, editable: true, edittype: "select", editoptions: { value: consignment_table_committionrates }, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: ""} }
  ],
  loadtext: 'Loading ...',
  rowNum: 25,
  rowList: [25, 30, 40, 50, 60, 70, 80, 90, 100, 200, 300, 400, 500],
  pager: '#con_pager',
  shrinkToFit: false,
  sortname: 'Contract_ID',
  sortorder: 'asc',
  postData: { cons_id: null },
  editurl: "/Auction/UpdateContractItem/"
  });
  $("#con_list").jqGrid('navGrid', '#con_pager', { edit: true, add: true, del: true, search: false, refreshtext: "" },
  { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterEdit: true, width: 350, bSubmit: "Save", editCaption: "Edit item", afterSubmit: AfterSubmitFunction, onclickSubmit: function (eparams) {
  var sr = $("#con_list").jqGrid('getPostData', 'cons_id');
  return { cons_id: sr.cons_id };
  }
  }, //edit
  {jqModal: false, modal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterAdd: true, width: 350, editData: { cons_id: null }, bSubmit: "Save", afterSubmit: AfterSubmitFunction, addCaption: "Add item",
  onclickSubmit: function (eparams) {
  var sr = $("#con_list").jqGrid('getPostData', 'cons_id');
  return { cons_id: sr.cons_id };
  }
  }, {}, {}, {});
  */

  $("#pay_list").jqGrid({
    url: '/Invoice/GetPaymentByStatement/',
    datatype: "json",
    method: 'GET',
    height: 500,
    width: 725,
    colNames: ['Payment#', 'Method', 'Amount', 'Date', 'Note', 'Cleared', 'Cleared Details'],
    colModel: [
             { name: 'Payment_Id', index: 'Payment_Id', width: 70, key: true, sortable: false, editable: true, editoptions: { readonly: true }, formoptions: { elmprefix: "    ", elmsuffix: "" } },
             { name: 'Method', index: 'Method', width: 120, sortable: false, editable: true, edittype: "select", editoptions: { value: consignment_table_paymenttypes }, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" } },
  		        { name: 'Amount', index: 'Amount', width: 70, sortable: false, editable: true, editrules: { edithidden: true, required: true, number: true, minValue: 0.01 }, editoptions: { defaultValue: function () { return "0.00" } }, formoptions: { elmprefix: "(*)", elmsuffix: "" }, align: 'right' },
  		        {
  		          name: 'Date', index: 'Date', width: 80, sortable: false, editable: true,
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
  		        { name: 'Note', index: 'Note', width: 100, sortable: false, editable: true, edittype: "textarea", editoptions: { rows: "4", cols: "34" }, formoptions: { elmprefix: "    ", elmsuffix: "" } },
  		        { name: 'Cleared', index: 'Cleared', width: 50, align: 'center', sortable: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "    ", elmsuffix: "" } },
  		        { name: 'CDetails', index: 'CDetails', width: 250, sortable: false, editable: true, edittype: "textarea", editoptions: { rows: "4", cols: "34" }, formoptions: { elmprefix: "    ", elmsuffix: "" } }
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
    postData: { cons_id: null }
  });
  $("#pay_list").jqGrid('navGrid', '#pay_pager',
         { edit: true, edittext: "Edit", add: true, addtext: "Add", del: false, search: false, refreshtext: "Refresh" },
         {
           jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterEdit: true, width: 320, bSubmit: "Save", editCaption: "Edit payment", afterSubmit: AfterSubmitFunction, onclickSubmit: function (eparams) {
             var sr = $("#pay_list").jqGrid('getPostData', 'cons_id');
             return { cons_id: sr.cons_id };
           }
         }, //edit
         {
           jqModal: false, modal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterAdd: true, width: 320, editData: { cons_id: null }, bSubmit: "Save", afterSubmit: AfterSubmitFunction, addCaption: "Add payment",
           onclickSubmit: function (eparams) {
             var sr = $("#pay_list").jqGrid('getPostData', 'cons_id');
             return { cons_id: sr.cons_id };
           }
         }, {}, {}, {});

  var auction_item_rowID = null;
  $("#item_list").jqGrid({
    mtype: 'GET',
    url: '/Auction/GetAuctionsListByConsignor/',
    datatype: "json",
    height: 520,
    width: 725,
    colNames: ['Auction#', 'Lot', 'Title', 'Description', 'Category', 'Reserve', 'Quantity', 'Commission Rate', 'LOA', 'Copy Notes', 'Photo Notes', 'Layout', 'Old#', 'Entered By', 'Writing Step', 'CommissionRate', 'MainCategory_ID', 'Category_ID', 'Priority', 'CategoryTitle', 'Price', 'IsPrinted', 'Old_Title', 'StepList_ID', 'IsLimitDisabled', 'PurchasedWay', 'PurchasedPrice', 'SoldWay', 'SoldPrice'],
    colModel: [
       { name: 'Auction_ID', index: 'Auction_ID', width: 60, key: true, sortable: false },
       { name: 'Lot', index: 'Lot', width: 30, sortable: false, hidden: true },
       { name: 'Title', index: 'Title', width: 340, sortable: false },
       { hidden: true, name: 'Description', index: 'Description', width: 140, sortable: false },
       { name: 'Category', index: 'Category', width: 160, sortable: false },
       { name: 'Reserve', index: 'Reserve', width: 80, sortable: false },
       { name: 'Quantity', index: 'Quantity', width: 20, sortable: false, align: 'center', hidden: true },
       { name: 'Commission', index: 'Commission', width: 120, sortable: false },
       { name: 'LOA', index: 'LOA', width: 30, align: 'center', formatter: 'checkbox', sortable: false },
       { name: 'CopyNotes', index: 'CopyNotes', width: 120, sortable: false },
       { name: 'PhotoNotes', index: 'PhotoNotes', width: 120, sortable: false },
       { name: 'Layout', index: 'Layout', width: 80, sortable: false },
       { name: 'OldAuction_ID', index: 'OldAuction_ID', width: 50, sortable: false },
       { name: 'Specialist', index: 'Specialist', width: 60, sortable: false },
       { name: 'ListingStepDescription', index: 'ListingStepDescription', width: 100, sortable: false },
       { name: 'CommissionRate_ID', index: 'CommissionRate_ID', width: 1, hidden: true },
       { name: 'MainCategory_ID', index: 'MainCategory_ID', width: 1, hidden: true },
       { name: 'Category_ID', index: 'Category_ID', width: 1, hidden: true },
       { name: 'Priority', index: 'Priority', width: 1, hidden: true },
       { name: 'CategoryTitle', index: 'CategoryTitle', width: 1, hidden: true },
       { name: 'Price', index: 'Price', width: 1, hidden: true },
       { name: 'IsPrinted', index: 'IsPrinted', width: 1, hidden: true },
       { name: 'OldAuction_Title', index: 'OldAuction_Title', width: 1, hidden: true },
       { name: 'ListingStep', index: 'ListingStep', width: 1, hidden: true },
       { name: 'IsLimitDisabled', index: 'IsLimitDisabled', width: 1, hidden: true },
       { name: 'PurchasedWay', index: 'PurchasedWay', width: 1, hidden: true },
       { name: 'PurchasedPrice', index: 'PurchasedPrice', width: 1, hidden: true },
       { name: 'SoldWay', index: 'SoldWay', width: 1, hidden: true },
       { name: 'SoldPrice', index: 'SoldPrice', width: 1, hidden: true }
    ],
    loadtext: 'Loading ...',
    rowNum: 500,
    rowList: [25, 30, 40, 50, 60, 70, 80, 90, 100, 200, 300, 400, 500],
    pager: '#item_pager',
    shrinkToFit: false,
    sortname: 'Auction_ID',
    sortorder: 'asc',
    postData: { cons_id: null },
    onSelectRow: function (id) { auction_item_rowID = id; }
  });
  $("#item_list").jqGrid('navGrid', '#item_pager', { edit: false, add: false, del: false, search: false, refreshtext: "", afterRefresh: function () { } });
  $("#item_list").jqGrid('navButtonAdd', '#item_pager', { caption: "", buttonicon: 'ui-icon-pencil', title: 'Edit item', onClickButton: function () { EditItemByID(); } }).navSeparatorAdd('#item_pager');
  $("#item_list").jqGrid('navButtonAdd', '#item_pager', { caption: "", buttonicon: 'ui-icon-trash', title: 'Delete item', onClickButton: function () { DeleteItemFromForm(auction_item_rowID); } });
  $("#item_list").jqGrid('navButtonAdd', '#item_pager', { caption: "", buttonicon: 'ui-icon-print', title: 'Print labels for items', onClickButton: function () { OpenPrintingLabelsForm(); } }).navSeparatorAdd('#item_pager');

  InitDropDown('/General/GetCommissionRatesJSON', '#form_consignor_CommissionRate');
  InitDropDown('/General/GetSpecialistsJSON', '#form_consignor_Specialist');
  InitDropDown('/Category/GetMainCategoryJSON', '#form_consignor_MainCategory', function () { $("#form_consignor_MainCategory").change(); });
  InitDropDown('/General/GetPurchasedTypesJSON?isnull=true', '#form_consignor_PurchasedWay');
  InitDropDown('/General/GetPurchasedTypesJSON?isnull=true', '#form_consignor_SoldWay');

  $("#form_consignor_MainCategory").change(function () {
    SetCategoryByMainCategory();
  });

  $("#form_consignor_Seller").click(function () { $('#dSellers').dialog('open'); });
  $("#form_consignor_Category").click(function () { $('#dCategory').dialog('open'); });
  $("#form_consignor_OldInventory").click(function () { $('#dAuctions').dialog('open'); });

  $("#btnAddNewUser").click(function () {
    InitUserForm(-1, InitNewSeller, InitNewSellerLoading);
  });
  $("#btnPreviewUser").click(function () {
    var id = $("#form_consignor_Seller").attr("res");
    if (id == '') { MessageBox('Preview consignor info', 'Please select the consignor.', '', 'info'); return; }
    $.post('/User/GetUserDataJSON', { user_id: id }, function (data) { InitUserForm(data, InitNewSeller, InitNewSellerLoading); }, 'json');
  });

  $("#btnUpdateConsignmentStatement").click(function () {
    UpdateConsignmentStatement();
  });
  $("#btnSave").click(function () {
    addMore = false;
    AddNewItemForConsignor();
  });
  $("#btnSaveAdd").click(function () {
    addMore = true;
    AddNewItemForConsignor();
  });
  $("#btnReset").click(function () {
    SetDefaultNewItemForm();
    SetDefaultNewItemForm2();
  });

  $("#btnResetOldInventory").click(function () {
    $("#form_consignor_OldInventory").attr('value', '').attr('title', '').attr('res', '');
  });

  $("#form_consignor_Priority").change(function () {
    var check = $("#form_consignor_DisableLimit").attr("checked");
    if (check) return;
    var priority = $("#form_consignor_Priority").val();
    if (!check && ($("#form_consignor_Description").attr("value").length > (sarr[priority] + 100))) {
      MessageBox("Changing the page layout", "The number of characters for this type of your copy will be limited to " + (sarr[priority] + 100) + ". You are over your character limit for this item. Please revise your copy.", "", "info");
      $('#form_consignor_stotal').html(sarr[priority]);
      $('#form_consignor_range').html(sarr[priority] + '-' + (sarr[priority] + 100));
    }
    else
      form_consignor_SetDescritptionMaxLenght(priority);
  });

  $("#form_consignor_DisableLimit").change(function () {
    if ($("#form_consignor_DisableLimit").attr("checked"))
      form_consignor_DisableDescriptionLenght();
    else {
      var priority = $("#form_consignor_Priority").val();
      if (($("#form_consignor_Description").attr("value").length > (sarr[priority] + 100))) {
        MessageBox("Changing the page layout", "The number of characters for this type of your copy will be limited to " + (sarr[priority] + 100) + ". You are over your character limit for this item. Please revise your copy.", "", "info");
        $('#form_consignor_stotal').html(sarr[priority]);
        $('#form_consignor_range').html(sarr[priority] + '-' + (sarr[priority] + 100));
      }
      else
        form_consignor_SetDescritptionMaxLenght(priority);
    }
  });

  $('#form_consignor_Description').change(function () {
    if ($("#form_consignor_DisableLimit").attr("checked")) {
      $('#form_consignor_sleft').css('color', 'green');
    } else {
      $('#form_consignor_sleft').html($('#form_consignor_Description').val().length);
      $('#form_consignor_sleft').css('color', parseInt($('#form_consignor_sleft').html()) <= parseInt(sarr[$("#form_consignor_Priority").val()]) ? 'green' : 'red');
    }
  });
  $("#form_consignor_Description").keydown(function (event) {
    $("#form_consignor_Description").change();
  });
  $("#form_consignor_Description").bind('paste', function (e) {
    var el = $(this);
    setTimeout(function () {
      if ($("#form_consignor_DisableLimit").attr("checked")) return;
      $("#form_consignor_Description").change();
      var text = $(el).val();
      var lim = sarr[$("#form_consignor_Priority").val()] + 100;
      if ($("#form_consignor_Step").val() < 2 && (text.length >= lim - 5 && text.length <= lim)) {
        MessageBox("Adding/Updating item", "Your description for this item is close to Maximum Characters. Please make sure copy is complete.", '', "info");
      } else if (text.length > lim) {
        MessageBox("Adding/Updating item", "Warning! You are over your character limit for this item. Please revise your copy", '', "error");
      }
    }, 100);
  });
  $("#form_consignor_print").dialog({
    bgiframe: true,
    autoOpen: false,
    height: 400,
    width: 480,
    modal: true,
    gridview: true
  });

  $("#btnSetDefault").click(function () {
    $("#printingLabels input:text").each(function (i, item) {
      $(item).attr("value", $(item).attr("default"));
    });
  });
  $("#btnClear").click(function () {
    $("#printingLabels input:text").attr("value", "0");
  });
  $("#btnPrint").click(function () {
    var auctions_id = [];
    var count = [];
    $("#printingLabels input:text").each(function (i, item) {
      var val = $(item).attr("value");
      if (parseInt(val) > 0) {
        auctions_id.push($(item).attr("cons_id"));
        count.push(val);
      }
    });
    if (auctions_id.length == 0) return;
    var c_id = $("#form_consignor_Cons_ID").val();
    $.post('/General/PrintLabelsForItems/', { auctions: auctions_id.join(","), amount: count.join(","), cons_id: c_id }, function (data) {
      if (data == '') return;
      window.open("", "").document.write(data);
      $('#form_consignor_print').dialog('close');
    }, 'json');
  });

  $("#form_consignor_Quantity").attr("disabled", "true");
});

/*----------------------------------------------------------------*/
function OpenPrintingLabelsForm() {
  var rows = $("#item_list").getDataIDs();
  if (rows == null) { MessageBox('Print labels', 'There are no auctions for this consignor statement', '', 'info'); return; }
  var c_id = $("#form_consignor_Cons_ID").attr('value');
  LoadingFormOpen();
  $.post('/Auction/GetAuctionsListByConsignorForPrint/', { sidx: "ID", sord: "asc", page: 1, rows: 10000000, cons_id: c_id }, function (data) {
    LoadingFormClose();
    if (data.total = 0) return;
    $("#printingLabels tbody").empty();
    $.each(data.rows, function (i, item) {
      var d = (item.cell[3] == '0') ? '1' : '0';
      $("#printingLabels > tbody:last").append("<tr><td>[" + item.cell[0] + "] " + item.cell[2] + "</td><td><input type='text' cons_id='" + item.i + "' default='" + d + "' id='print_edit_" + item.i + "' value='" + d + "' style='text-align:center' /></td></tr>");
    });
    $('#form_consignor_print').dialog('open');
  }, 'json');
}

function DeleteItemFromForm(id) {
  if (id == null) { MessageBox('Delete item', 'Please select the item.', '', 'info'); return; }
  ConfirmBox('Delete item', 'Do you really want to delete this item?', function () {
    LoadingFormOpen();
    $.post('/Auction/DeleteAuction', { auction_id: id }, function (data) {
      LoadingFormClose();
      switch (data.Status) {
        case "ERROR":
          MessageBox("Delete item", data.Message, '', "error");
          break;
        case "SUCCESS":
          $("#item_list").jqGrid('delRowData', data.Details);
          MessageBox("Delete item", "The item was deleted successfuly.", '', "info");
          break;
      }
    }, 'json');
  });
}

$("#btnSaveContractText").click(function () {
  $.post('/Auction/UpdateConsignmentContractText', { consignmentID: $("#form_consignor_Cons_ID").val(), contractText: $("#form_consignor_Contract_Text").val() }, function (data) {
    switch (data.Status) {
      case "SUCCESS":
        $("#form_consignor_Contract_StatusID").html(data.Details.StatusText);
        $("#contractFileLink").attr("href", data.Details.DownloadLink);
        $("#contractFileLink, #btnResetContractText, #conscontract").show();
        $("#conscontract").attr('data', data.Details.ShowLink);
        $("#contractText, #btnSaveContractText").hide();
        break;
    }
    alert(data.Message);
  }, 'json');
});

$("#btnRegenerateContract").click(function () {
  $.post('/Auction/RegenerateConsignmentContract', { consignmentID: $("#form_consignor_Cons_ID").val() }, function (data) {
    switch (data.Status) {
      case "SUCCESS":
        var lnk = $("#conscontract").attr('data');
        $("#conscontract").attr('data', lnk);
        break;
    }
    alert(data.Message);
  }, 'json');
});

$("#btnResetContractText").click(function () {
  $.post('/Auction/ResetConsignmentContract', { consignmentID: $("#form_consignor_Cons_ID").val() }, function (data) {
    switch (data.Status) {
      case "SUCCESS":
        $("#form_consignor_Contract_StatusID").html(data.Details.StatusText);
        $("#contractFileLink, #conscontract").hide();
        $("#form_consignor_Contract_Text").val(data.Details.ContractText);
        $("#contractText, #btnSaveContractText").show();
        break;
    }
    alert(data.Message);
  }, 'json');
});

