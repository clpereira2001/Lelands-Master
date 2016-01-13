function InitDOWForm(data) {
  if (data == null) {
    $("#form_dow_Auction_ID").attr('value', '0');    
    $("#form_dow_Title").attr('value', '');    
    $("#form_dow_Price").attr('value', '');
    $("#form_dow_Shipping").attr('value', '');    
    $("#form_dow_Description").attr("value", '');
    $("#form_dow_OldInventory").attr('title', '').attr('value', '').attr('res', '');
    $("#form_dow_MainCategory").val(0);    
    $("#form_dow_Category").attr('title', '').attr('value', '').attr('res', '');
    $("#form_dow_Status").val(0);
    $("#form_dow_Addendum").attr("value", '');
    $("#form_dow_CommissionRate").val(0);
    $("#form_dow_Seller").attr('title', '').attr('value', '').attr('res', '');
    $("#form_dow_OldInventory").attr('title', '').attr('value', '').attr('res', '');
    $('#dAuctions_list').jqGrid('setPostData', { owner_id: -1 });
    $.post('/Auction/ClearImagesForNewAuction', {}, function (data) { }, 'json');
    $("#form_dow_iframe").attr("src", auction_auctionimageform_url + "?auction_id=0");
  } else {
    $("#form_dow_Auction_ID").attr("value", data.id);
    $("#form_dow_Status").attr('value', data.s);
    $("#form_dow_Title").attr('value', data.t);
    $("#form_dow_Price").attr('value', data.p);    
    $("#form_dow_Shipping").attr('value', data.sh);
    $("#form_dow_OldInventory").attr('title', data.old_n).attr('value', data.old_n).attr('res', data.old);
    $("#form_dow_Description").attr("value", data.d);    
    $("#form_dow_iframe").attr("src", auction_auctionimageform_url + "?auction_id=" + data.id);
    $("#form_dow_Seller").attr('title', data.o_n).attr('value', data.o_n).attr('res', data.o);
    $("#form_dow_CommissionRate").val(data.cr);
    $("#form_dow_Addendum").attr("value", data.add);
    $("#form_dow_Category").attr('title', data.cat_n).attr('value', data.cat_n).attr('res', data.cat);
    $("#form_dow_MainCategory").val(data.mcat);
  }
  $("#form_dow").dialog('open');  
}

//SubmitDOWForm
function SubmitDOWForm() {
  return {
    ID: $("#form_dow_Auction_ID").attr("value"),
    Status_ID: $("#form_dow_Status").attr("value"),
    Title: $("#form_dow_Title").attr("value"),
    Description: $('#form_dow_Description').attr("value"),
    Addendum: $('#form_dow_Addendum').attr("value"),
    MainCategory_ID: $('#form_dow_MainCategory').val(),
    Category_ID: $('#form_dow_Category').attr("res"),
    Price: $("#form_dow_Price").attr("value"),    
    Shipping: $("#form_dow_Shipping").attr("value"),
    OldAuction_ID: $("#form_dow_OldInventory").attr("res"),    
    Owner_ID: $("#form_dow_Seller").attr("res"),
    CommissionRate_ID: $("#form_dow_CommissionRate").val()
  };
}

//TransferModelToAuctionForm
function TransferModelToDOWForm(data) {
  var msg = data.Message;
  var det = '';
  if (data.Details != null) {
    det += '<ul>';
    var exists = false;
    $.each(data.Details, function (i, item) {
      exists = true;
      switch (item.field) {
        case "Status_ID": $("#form_dow_Status").addClass("input-validation-error"); break;
        case "Title": $("#form_dow_Title").addClass("input-validation-error"); break;
        case "Price": $("#form_dow_Price").addClass("input-validation-error"); break;        
        case "Shipping": $("#form_dow_Shipping").addClass("input-validation-error"); break;
        case "OldAuction_ID": $("#form_dow_OldInventory").addClass("input-validation-error"); break;
        case "Owner_ID": $("#form_auction_Seller").addClass("input-validation-error"); break;
        case "CommissionRate_ID": $("#form_dow_CommissionRate").addClass("input-validation-error"); break;
        case "Category_ID": $("#form_dow_Category").addClass("input-validation-error"); break;
        case "MainCategory_ID": $("#form_dow_MainCategory").addClass("input-validation-error"); break;
        default: exists = false; break;
      }
      det += (exists) ? ('<li>' + item.message + '</li>') : '';
    });
    det += '</ul>';
  }
  MessageBox("Saving For Sale item", msg, det, "error");
}

//UpdateDOW
function UpdateDOW() {
  $("*", "#form_dow").removeClass("input-validation-error");
  LoadingFormOpen();
  $.post('/Auction/UpdateDOW', { auction: Sys.Serialization.JavaScriptSerializer.serialize(SubmitDOWForm()) }, function (data) {
    LoadingFormClose();
    switch (data.Status) {
      case "ERROR":
        TransferModelToDOWForm(data);
        break;
      case "SUCCESS":
        $("#form_dow_Auction_ID").attr("value", data.Details);
        $("#form_dow").dialog('close');
        break;
    }
  }, 'json');
}

//InitNewSeller
function InitNewSeller() {
  var t = $("#form_user_Login").attr('value') + ' (' + $("#form_user_b_FirstName").attr('value') + ' ' + $("#form_user_b_LastName").attr('value') + ')';
  $("#form_dow_Seller").attr('title', t);
  $("#form_dow_Seller").attr('value', t);
  t = $("#form_user_User_ID").attr('value');
  $("#form_dow_Seller").attr('res', t);
  t = $("#form_user_CommissionRate_ID").val();
  $("#form_dow_CommissionRate").val(t);
  $("#dSellers_list").trigger('reloadGrid');
}

//InitNewSellerLoading
function InitNewSellerLoading() {
  $("#form_user_UserType_ID option").attr("disabled", "true");
  $("#form_user_UserType_ID option[value=5]").removeAttr("disabled");
  $("#form_user_UserType_ID option[value=6]").removeAttr("disabled");
  $("#form_user_UserType_ID").val(6);
  $("#form_user_UserType_ID").change();
  $("#form_user_UserStatus_ID").val(2);
}

//----------------------------------------------------------------------------------------------------------------
$(document).ready(function () {
  $("#form_dow").dialog({
    bgiframe: true,
    autoOpen: false,
    height: 690,
    width: 800,
    modal: true,
    gridview: true,
    position: ['center', 'top'],    
    buttons: {
      'Cancel': function () { $(this).dialog('close'); },
      'Save': function () { UpdateDOW(); }
    },
    close: function () { $("*", "#form_dow").removeClass("input-validation-error"); }
  });
  $("#form_dow_tabs").tabs();

  InitDropDown('/General/GetAuctionStatusesJSON', '#form_dow_Status');
  InitDropDown('/General/GetCommissionRatesJSON', '#form_dow_CommissionRate');
  InitDropDown('/General/GetMainCategoriesAndForSaleJSON', '#form_dow_MainCategory');

  $("#form_dow_Seller").click(function () { $('#dSellers').dialog('open'); });
  $("#form_dow_Category").click(function () { $('#dCategory').dialog('open'); });
  $("#form_dow_OldInventory").click(function () { $('#dAuctions').dialog('open'); });

  $("#form_dow_btnResetOldInventory").click(function () {
    $("#form_dow_OldInventory").attr('value', '').attr('title', '').attr('res', '');
  });

  //$("#img_list").jqGrid('setGridHeight', '450px');

  $("#form_dow_btnPreviewUser").click(function () {
    var id = $("#form_dow_Seller").attr("res");
    if (id == '') { MessageBox('Preview consignor info', 'Please select the consignor.', '', 'info'); return; }
    LoadingFormOpen();
    $.post('/User/GetUserDataJSON', { user_id: id }, function (data) { InitUserForm(data, InitNewSeller, InitNewSellerLoading); LoadingFormClose(); }, 'json');
  });
});
/*----------------------------------------------------------------*/

//OpenningDOWForm
function OpenDOWFormByID(id) {
  if (id == null) { MessageBox('Edit DOW', 'Please select the row', '', 'info'); return; }
  $.post('/Auction/GetDOWJSON', { auction_id: id }, function (data) { LoadingFormOpen(); InitDOWForm(data); LoadingFormClose(); }, 'json');
}