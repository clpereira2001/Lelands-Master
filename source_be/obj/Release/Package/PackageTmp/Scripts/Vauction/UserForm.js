function InitUserForm(data, success_func, loading_func) {
  if (loading_func != null) loading_func();
  if (data == -1 || data == null) {    
    $("#form_user_User_ID").attr("value", '0');
    $("#form_user_UserType_ID").val(4);
    $("#form_user_UserStatus_ID").val(1);
    $("#form_user_CommissionRate_ID").val(0);
    $("#form_user_b_Country").val(1);
    $("#form_user_b_State").val(0);
    $("#form_user_s_Country").val(1);
    $("#form_user_s_State").val(0);
    $("#form_user input:text").attr("value", "");    
    $("#form_user_tabs-1 input[type=check]").removeAttr("checked");    
    $("#form_user_s_BillingLikeShipping").attr("checked", true);    
    $("#form_user_FailedAttempts").attr("value", '0');
    $("#form_user_tabs-4 input[type=text]").attr("disabled", true);
    $("#form_user_tabs-4 select").attr("disabled", true);
    $("#form_user_CommissionRate_ID").attr("disabled", true);
    $("#form_user_Password").removeAttr("disabled");
    $("#form_user_IsConfirmed").attr("checked", true);
    $("#form_user_s_Notes").attr("value", "");
    $("#form_user_Catalog").attr("checked", false);
    $("#form_user_PostCards").attr("checked", false);
    $("#signatureTab").parent().hide();
  } else {
    $("#form_user_User_ID").attr("value", data.user_id);//general 
    $("#form_user_Login").attr("value", data.login);
    $("#form_user_Password").attr("value", data.pw);
    $("#form_user_UserType_ID").val(data.usertype);
    $("#form_user_UserStatus_ID").val(data.status);
    $("#form_user_CommissionRate_ID").val(data.commrate);
    $("#form_user_Email").attr("value", data.email);
    $("#form_user_DateIN").attr("value", data.datein);
    $("#form_user_IsConfirmed").attr("checked", data.isconfirmed);
    $("#form_user_IsModifyed").attr("checked", data.ismodifyed);
    $("#form_user_DayPhone").attr("value", data.dph);
    $("#form_user_EveningPhone").attr("value", data.eph);
    $("#form_user_MobilePhone").attr("value", data.mph);
    $("#form_user_Fax").attr("value", data.fax);
    $("#form_user_Tax").attr("value", data.tax);
    $("#form_user_IP").attr("value", data.ip);    
    $("#form_user_LastAttempt").attr("value", data.lastattempt);
    $("#form_user_FailedAttempts").attr("value", data.failedattempts);
    $("#form_user_RecieveWeeklySpecials").attr("checked", data.rws);
    $("#form_user_RecieveNewsUpdates").attr("checked", data.rnu);
    $("#form_user_IsRecievingBidConfirmation").attr("checked", data.rbc);
    $("#form_user_IsRecievingOutBidNotice").attr("checked", data.robn);
    $("#form_user_IsRecievingLotSoldNotice").attr("checked", data.rlsn);
    $("#form_user_IsRecievingLotClosedNotice").attr("checked", data.rlcn);
    $("#form_user_Catalog").attr("checked", data.iscatalog);
    $("#form_user_PostCards").attr("checked", data.ispostcards);
    $("#form_user_AH1").attr("value", data.ah1);
    $("#form_user_PN1").attr("value", data.pn1);
    $("#form_user_LBD1").attr("value", data.lbd1);
    $("#form_user_AH2").attr("value", data.ah2);
    $("#form_user_PN2").attr("value", data.pn2);
    $("#form_user_LBD2").attr("value", data.lbd2);
    $("#form_user_EBID").attr("value", data.ebid);
    $("#form_user_Feedback").attr("value", data.evf);
    $("#form_user_b_FirstName").attr("value", data.b_first);//billing
    $("#form_user_b_MiddleName").attr("value", data.b_middle);
    $("#form_user_b_LastName").attr("value", data.b_last);    
    $("#form_user_b_Company").attr("value", data.b_comp);
    $("#form_user_b_Address1").attr("value", data.b_addr1);
    $("#form_user_b_Address2").attr("value", data.b_addr2);
    $("#form_user_b_City").attr("value", data.b_city);
    $("#form_user_b_State").val(data.b_state);
    $("#form_user_b_Zip").attr("value", data.b_zip);
    $("#form_user_b_Country").val(data.b_country);
    $("#form_user_b_InternationalState").attr("value", data.b_istate);    
    $("#form_user_s_BillingLikeShipping").attr("checked", data.bls);//shipping
    SetShippingInfo(data.bls);
    if (!data.bls) {
      $("#form_user_s_FirstName").attr("value", data.s_first);
      $("#form_user_s_MiddleName").attr("value", data.s_middle);
      $("#form_user_s_LastName").attr("value", data.s_last);      
      $("#form_user_s_Company").attr("value", data.s_comp);
      $("#form_user_s_Address1").attr("value", data.s_addr1);
      $("#form_user_s_Address2").attr("value", data.s_addr2);
      $("#form_user_s_City").attr("value", data.s_city);
      $("#form_user_s_State").val(data.s_state);
      $("#form_user_s_Zip").attr("value", data.s_zip);
      $("#form_user_s_Country").val(data.s_country);
      $("#form_user_s_InternationalState").attr("value", data.s_istate);      
    }
    $("#form_user_CommissionRate_ID").attr("disabled", !data.isnotseller);   
    $("#form_user_Password").attr("disabled", data.ispwdisapled);
    $("#form_user_tabs-4 input:text").attr("disabled", data.bls);
    $("#form_user_tabs-4 select").attr("disabled", data.bls);
    $("#form_user_s_Notes").attr("value", data.notes);
    $("#signatureImg").attr('src', data.signatureUrl);
    if (data.showSignature) {
      $("#signatureTab").parent().show();
      } else {
      $("#signatureTab").parent().hide();
    }
  }
  $("#btnCancelSignature").click();
  f_success_user = success_func;  
  $('#form_user').dialog('open');
}

//SetShippingInfo
function SetShippingInfo(BillingLikeShipping) {
  if (BillingLikeShipping) {    
    $("#form_user_s_FirstName").attr("value", $("#form_user_b_FirstName").attr("value"));
    $("#form_user_s_MiddleName").attr("value", $("#form_user_b_MiddleName").attr("value"));
    $("#form_user_s_LastName").attr("value", $("#form_user_b_LastName").attr("value"));    
    $("#form_user_s_Company").attr("value", $("#form_user_b_Company").attr("value"));
    $("#form_user_s_Address1").attr("value", $("#form_user_b_Address1").attr("value"));
    $("#form_user_s_Address2").attr("value", $("#form_user_b_Address2").attr("value"));
    $("#form_user_s_City").attr("value", $("#form_user_b_City").attr("value"));    
    $("#form_user_s_State").val($("#form_user_b_State").val());
    $("#form_user_s_Zip").attr("value", $("#form_user_b_Zip").attr("value"));
    $("#form_user_s_Country").val($("#form_user_b_Country").val());
    $("#form_user_s_InternationalState").attr("value", $("#form_user_b_InternationalState").attr("value"));    
  }
}

//SubmitUserForm
function SubmitUserForm() {
  SetShippingInfo($("#form_user_s_BillingLikeShipping").attr("checked"));
  return {
    ID: $("#form_user_User_ID").attr("value"),
    Login: $("#form_user_Login").attr("value"),
    Password: $("#form_user_Password").attr("value"),
    UserType: $("#form_user_UserType_ID").val(),
    UserStatus: $("#form_user_UserStatus_ID").val(),
    CommissionRate: $("#form_user_CommissionRate_ID").val(),
    Email: $("#form_user_Email").attr("value"),
    IsConfirmed: $("#form_user_IsConfirmed").attr("checked"),
    IsModifyed: $("#form_user_IsModifyed").attr("checked"),
    DayPhone: $("#form_user_DayPhone").attr("value"),
    EveningPhone: $("#form_user_EveningPhone").attr("value"),
    MobilePhone: $("#form_user_MobilePhone").attr("value"),
    Fax: $("#form_user_Fax").attr("value"),
    Tax: $("#form_user_Tax").attr("value"),
    FaildAttempts: $("#form_user_FailedAttempts").attr("value"),
    RecieveWeeklySpecials: $("#form_user_RecieveWeeklySpecials").attr("checked"),
    RecieveNewsUpdates: $("#form_user_RecieveNewsUpdates").attr("checked"),
    RecieveBidConfirmation: $("#form_user_IsRecievingBidConfirmation").attr("checked"),
    RecieveOutBidNotice: $("#form_user_IsRecievingOutBidNotice").attr("checked"),
    RecieveLotSoldNotice: $("#form_user_IsRecievingLotSoldNotice").attr("checked"),
    RecieveLotClosedNotice: $("#form_user_IsRecievingLotClosedNotice").attr("checked"),
    AuctionHouse1:$("#form_user_AH1").attr("value"),
    PhoneNumber1:$("#form_user_PN1").attr("value"),
    LastBidDate1:$("#form_user_LBD1").attr("value"),
    AuctionHouse2:$("#form_user_AH2").attr("value"),
    PhoneNumber2:$("#form_user_PN2").attr("value"),
    LastBidDate2:$("#form_user_LBD2").attr("value"),
    eBayBidderID: $("#form_user_EBID").attr("value"),
    eBayFeedback: $("#form_user_Feedback").attr("value"),
    BillingFirstName: $("#form_user_b_FirstName").attr("value"),
    BillingMiddleName: $("#form_user_b_MiddleName").attr("value"),
    BillingLastName: $("#form_user_b_LastName").attr("value"),    
    BillingCompany: $("#form_user_b_Company").attr("value"),
    BillingAddress1: $("#form_user_b_Address1").attr("value"),
    BillingAddress2: $("#form_user_b_Address2").attr("value"),
    BillingCity: $("#form_user_b_City").attr("value"),
    BillingState: $("#form_user_b_State option:selected").text(),
    BillingState_ID: $("#form_user_b_State").val(),
    BillingZip: $("#form_user_b_Zip").attr("value"),
    BillingCountry: $("#form_user_b_Country").val(),
    BillingInternationalState: $("#form_user_b_InternationalState").attr("value"),    
    BillingLikeShipping: $("#form_user_s_BillingLikeShipping").attr("checked"),
    ShippingFirstName: $("#form_user_s_FirstName").attr("value"),
    ShippingMiddleName: $("#form_user_s_MiddleName").attr("value"),
    ShippingLastName: $("#form_user_s_LastName").attr("value"),    
    ShippingCompany: $("#form_user_s_Company").attr("value"),
    ShippingAddress1: $("#form_user_s_Address1").attr("value"),
    ShippingAddress2: $("#form_user_s_Address2").attr("value"),
    ShippingCity: $("#form_user_s_City").attr("value"),
    ShippingState: $("#form_user_s_State option:selected").text(),
    ShippingState_ID: $("#form_user_s_State").val(),
    ShippingZip: $("#form_user_s_Zip").attr("value"),
    ShippingCountry: $("#form_user_s_Country").val(),
    ShippingInternationalState: $("#form_user_s_InternationalState").attr("value"),
    Notes: $("#form_user_s_Notes").attr("value"),
    IsCatalog : $("#form_user_Catalog").attr("checked"),
    IsPostCards : $("#form_user_PostCards").attr("checked")
  };
}

function TransferModelToUserForm(data) {
  var msg = data.Message;
  var det = '';  
  if (data.Details != null) {
    det += '<ul>';
    var exists = false;
    $.each(data.Details, function(i, item) {
      exists = true;
      switch (item.field) {
        case "Login": $("#form_user_Login").addClass("input-validation-error"); break;
        case "Password": $("#form_user_Password").addClass("input-validation-error"); break;
        case "UserType": $("#form_user_UserType_ID").addClass("input-validation-error"); break;
        case "UserStatus": $("#form_user_UserStatus_ID").addClass("input-validation-error"); break;
        case "CommissionRate": $("#form_user_CommissionRate_ID").addClass("input-validation-error"); break;
        case "Email": $("#form_user_Email").addClass("input-validation-error"); break;
        case "Fax": $("#form_user_Fax").addClass("input-validation-error"); break;
        case "IsConfirmed": $("#form_user_IsConfirmed").addClass("input-validation-error"); break;
        case "IsModifyed": $("#form_user_IsModifyed").addClass("input-validation-error"); break;
        case "DayPhone": $("#form_user_DayPhone").addClass("input-validation-error"); break;
        case "EveningPhone": $("#form_user_EveningPhone").addClass("input-validation-error"); break;
        case "MobilePhone": $("#form_user_MobilePhone").addClass("input-validation-error"); break;        
        case "FaildAttempts": $("#form_user_FailedAttempts").addClass("input-validation-error"); break;
        case "RecieveWeeklySpecials": $("#form_user_RecieveWeeklySpecials").addClass("input-validation-error"); break;
        case "RecieveNewsUpdates": $("#form_user_RecieveNewsUpdates").addClass("input-validation-error"); break;
        case "RecieveBidConfirmation": $("#form_user_IsRecievingBidConfirmation").addClass("input-validation-error"); break;
        case "RecieveOutBidNotice": $("#form_user_IsRecievingOutBidNotice").addClass("input-validation-error"); break;
        case "RecieveLotSoldNotice": $("#form_user_IsRecievingLotSoldNotice").addClass("input-validation-error"); break;
        case "RecieveLotClosedNotice": $("#form_user_IsRecievingLotClosedNotice").addClass("input-validation-error"); break;        
        case "BillingFirstName": $("#form_user_b_FirstName").addClass("input-validation-error"); break;
        case "BillingMiddleName": $("#form_user_b_MiddleName").addClass("input-validation-error"); break;
        case "BillingLastName": $("#form_user_b_LastName").addClass("input-validation-error"); break;        
        case "BillingCompany": $("#form_user_b_Company").addClass("input-validation-error"); break;
        case "BillingAddress1": $("#form_user_b_Address1").addClass("input-validation-error"); break;
        case "BillingAddress2": $("#form_user_b_Address2").addClass("input-validation-error"); break;
        case "BillingCity": $("#form_user_b_City").addClass("input-validation-error"); break;
        case "BillingState": $("#form_user_b_State").addClass("input-validation-error"); break;
        case "BillingState_ID": $("#form_user_b_State").addClass("input-validation-error"); break;
        case "BillingZip": $("#form_user_b_Zip").addClass("input-validation-error"); break;
        case "BillingCountry": $("#form_user_b_Country").addClass("input-validation-error"); break;
        case "BillingInternationalState": $("#form_user_b_InternationalState").addClass("input-validation-error"); break;
        case "BillingLikeShipping": $("#form_user_s_BillingLikeShipping").addClass("input-validation-error"); break;
        case "ShippingFirstName": $("#form_user_s_FirstName").addClass("input-validation-error"); break;
        case "ShippingMiddleName": $("#form_user_s_MiddleName").addClass("input-validation-error"); break;
        case "ShippingLastName": $("#form_user_s_LastName").addClass("input-validation-error"); break;        
        case "ShippingCompany": $("#form_user_s_Company").addClass("input-validation-error"); break;
        case "ShippingAddress1": $("#form_user_s_Address1").addClass("input-validation-error"); break;
        case "ShippingAddress2": $("#form_user_s_Address2").addClass("input-validation-error"); break;
        case "ShippingCity": $("#form_user_s_City").addClass("input-validation-error"); break;
        case "ShippingState": $("#form_user_s_State").addClass("input-validation-error"); break;
        case "ShippingState_ID": $("#form_user_s_State").addClass("input-validation-error"); break;
        case "ShippingZip": $("#form_user_s_Zip").addClass("input-validation-error"); break;
        case "ShippingCountry": $("#form_user_s_Country").addClass("input-validation-error"); break;
        case "ShippingInternationalState": $("#form_user_s_InternationalState").addClass("input-validation-error"); break;        
        default: exists = false; break;
      }
      det += (exists) ? ('<li>' + item.message + '</li>') : '';
    });
    det += '</ul>';
  }
  MessageBox("Saving user information", msg, det, "error");
}

function UpdateUser() {
  $("*", "#form_user").removeClass("input-validation-error");
  LoadingFormOpen();
  $.post('/User/UpdateUser', { user: Sys.Serialization.JavaScriptSerializer.serialize(SubmitUserForm()), newSignature: $("#newSignature").val() }, function (data) {
    LoadingFormClose();
    switch (data.Status) {
      case "ERROR":
        TransferModelToUserForm(data);
        break;
      case "SUCCESS":
        $("#form_user_User_ID").attr("value", data.Details);
        $("#form_user").dialog('close');        
        if (f_success_user != null) f_success_user();
        break;
    }
  }, 'json');
}

//InitUserFormDropDowns
function InitUserFormDropDowns() {
  InitDropDown('/General/GetUserTypesJSON', '#form_user_UserType_ID');
  InitDropDown('/General/GetUserStatusesJSON', '#form_user_UserStatus_ID');
  InitDropDown('/General/GetCommissionRatesJSON', '#form_user_CommissionRate_ID');
  InitDropDownWithGroup('/General/GetCountryStatesJSON', '#form_user_b_State');
  InitDropDown('/General/GetCountriesJSON', '#form_user_b_Country');
  InitDropDownWithGroup('/General/GetCountryStatesJSON', '#form_user_s_State');
  InitDropDown('/General/GetCountriesJSON', '#form_user_s_Country');  
}

var f_success_user = function() { };
//----------------------------------------------------------------------------------------------------------------
$(document).ready(function () {  
  $("#form_user").dialog({
    bgiframe: true,
    autoOpen: false,
    height: 630,
    width: 790,
    modal: true,
    gridview: true,
    buttons: {
      'Cancel': function () { $(this).dialog('close'); },
      'Save': function () { UpdateUser(); }
    },
    close: function () { $("*", "#form_user").removeClass("input-validation-error"); }
  });
  $("#form_user_tabs").tabs();

  $("#form_user_IP").attr("disabled", "true");
  $("#form_user_DateIN").attr("disabled", "true");
  $("#form_user_LastAttempt").attr("disabled", "true");

  InitUserFormDropDowns();

  $("#form_user_UserType_ID").change(function () {
    var value = $("#form_user_UserType_ID").val();
    $("#form_user_CommissionRate_ID").attr("disabled", (value != 5 && value != 6));
  });

  $("#form_user_s_BillingLikeShipping").change(function () {
    var ischecked = $("#form_user_s_BillingLikeShipping").attr("checked");
    $("#form_user_tabs-4 input:text").attr("disabled", ischecked);
    $("#form_user_tabs-4 select").attr("disabled", ischecked);
    SetShippingInfo(ischecked);
  });
});