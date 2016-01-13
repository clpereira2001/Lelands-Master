function InitAuctionForm(data) {
    $(".checked > input").removeAttr("checked");
    $(".checked").removeClass("checked");
    if (data == null) {
        $(".multiSelect").val("Select options");
        $("#form_auction_Auction_ID").attr('value', '0');
        $('#dAuctionsA_list').jqGrid('setPostData', { owner_id: -1 });
        $('#dAuctionsA_list').jqGrid('clearGridData');
        $("#form_auction_Seller").attr('title', '');
        $("#form_auction_Seller").attr('value', '');
        $("#form_auction_Seller").attr('res', '');
        $("#form_auction_CommissionRate").val(0);
        $("#form_auction_Title").attr('value', '');
        $("#form_auction_Quantity").attr('value', '1');
        $("#form_auction_Price").attr('value', '1');
        $("#form_auction_Reserve").attr('value', '1');
        $("#form_auction_Shipping").attr('value', '1');
        $("#form_auction_Lot").attr("value", '');
        $("#form_auction_Priority").val(4);
        form_auction_SetDescritptionMaxLenght(4);
        $("#form_auction_Description").attr("value", '');
        $('#form_auction_Description').change();
        $("#form_auction_DisableLimit").attr("checked", "checked");
        $("#form_auction_DisableLimit").change();
        $("#form_auction_CopyNotes").attr("value", '');
        $("#form_auction_PhotoNotes").attr("value", '');
        $("#form_auction_Estimate").attr("value", '');
        $("#form_auction_Addendum").attr("value", '');
        $("#form_auction_Step").val(0);
        $("#form_auction_LOA, #form_auction_IsPulledOut, #form_auction_IsUnsold,#form_auction_Catlog,#form_auction_Catlog, #form_auction_IsPrinted, #form_auction_Photographed, #form_auction_InLayout").removeAttr("checked");
         
        $("#form_auction_OldInventory").attr('title', '').attr('value', '').attr('res', '');
        $("#form_auction_collection").attr('title', '').attr('value', '').attr('res', '');
        $("#form_auction_Status").val(0);
        isneedtochangecategory = true;
        $("#form_auction_MainCategory").val(1);
        InitDropDown('/General/GetPendingEventsJSON', '#form_auction_Event', function () {
            $("#form_auction_Event").val(0);
            $("#form_auction_Event").change();
            $("#form_auction_MainCategory").change();
        });
        $.post('/Auction/ClearImagesForNewAuction', {}, function (data) { }, 'json');
        $("#form_auction_Event").removeAttr("disabled");
        $("#form_auction_Seller").removeAttr("disabled");
        $('#img_list').jqGrid('setPostData', { auction_id: 0 });
        $('#img_list').jqGrid('clearGridData');
        InitUploader(0, current_user_id);
        $("#form_auction_PurchasedPrice,#form_auction_SoldPrice").attr("value", "");
        $("#form_auction_PurchasedWay,#form_auction_SoldWay").val(-1);
    } else {
        $("#form_auction_Auction_ID").attr("value", data.id);
        $("#form_auction_Seller").attr('title', data.o_n);
        $("#form_auction_Seller").attr('value', data.o_n);
        $("#form_auction_Seller").attr('res', data.o);
        $("#form_auction_CommissionRate").val(data.cr);
        $("#form_auction_Status").attr('value', data.s);
        $("#form_auction_Title").attr('value', data.t);
        $("#form_auction_Quantity").attr('value', data.q);
        $("#form_auction_StartDate").attr('value', data.sd);
        $("#form_auction_EndDate").attr('value', data.ed);
        $("#form_auction_Lot").attr('value', data.lot);
        $("#form_auction_Price").attr('value', data.p);
        $("#form_auction_Reserve").attr('value', data.r);
        $("#form_auction_Estimate").attr('value', data.est);
        $("#form_auction_Shipping").attr('value', data.sh);
        $("#form_auction_Step").val(data.ls);
        $("#form_auction_Priority").val(data.pr);
        form_auction_SetDescritptionMaxLenght(data.pr);
        $("#form_auction_LOA").attr("checked", data.loa);
        $("#form_auction_Addendum").attr('value', data.add);
        $("#form_auction_OldInventory").attr('title', data.old_n);
        $("#form_auction_OldInventory").attr('value', data.old_n);
        $("#form_auction_OldInventory").attr('res', data.old);
        $("#form_auction_collection").attr('title', data.collection);
        $("#form_auction_collection").attr('value', data.collection);
        $("#form_auction_collection").attr('res', data.collectionID);
        $("#form_auction_IsPulledOut").attr("checked", data.pul);
        $("#form_auction_IsUnsold").attr("checked", data.uns);
        $("#form_auction_Catlog").attr("checked", data.catl);
        $("#form_auction_IsPrinted").attr("checked", data.pri);
        $("#form_auction_Photographed").attr("checked", data.isphot);
        $("#form_auction_InLayout").attr("checked", data.inlay);
        $("#form_auction_Description").attr("value", data.d);
        $('#form_auction_Description').change();
        $("#form_auction_CopyNotes").attr("value", data.crn);
        $("#form_auction_PhotoNotes").attr("value", data.phn);
        isneedtochangecategory = false;
        $("#form_auction_MainCategory").val(data.mc);
        $("#form_auction_Category").attr('title', data.cat_n).attr('value', data.cat_n).attr('res', data.cat);
        InitDropDown('/General/GetEventsJSON', '#form_auction_Event', function () {
            $("#form_auction_Event").val(data.ev);
            $("#form_auction_MainCategory").change();
            isneedtochangecategory = true;
        });
        $('#dAuctionsA_list').jqGrid('setPostData', { owner_id: data.o });
        $('#dAuctionsA_list').trigger('reloadGrid');
        $("#form_auction_Event").attr("disabled", "true");
        $("#form_auction_Seller").attr("disabled", "true");
        $("#form_auction_DisableLimit").attr("checked", data.dislim);
        $("#form_auction_DisableLimit").change();
        $('#img_list').jqGrid('setPostData', { auction_id: data.id });
        $('#img_list').trigger('reloadGrid');
        InitUploader(data.id, current_user_id);

        $("#form_auction_PurchasedPrice").attr("value", data.purchp);
        $("#form_auction_SoldPrice").attr("value", data.soldp);
        $("#form_auction_PurchasedWay").val(data.purchw);
        $("#form_auction_SoldWay").val(data.soldw);
        var tagText = "Select options--";
        if (data.tags.length > 0) {
            tagText = "";
            $.each(data.tags, function (index, value) {
                $("[name='form_auction_tags'][value='" + value.ID + "']").attr("checked", "checked").parent().addClass("checked");
                tagText += (value.Title + ", ");
            });
        }
        $(".multiSelect").val(tagText.substring(0, tagText.length - 2));
    }
    $('#dSellersA_list').jqGrid('clearGridData');
    $("#form_auction").dialog('open');
}

//GetStartEndDate
function GetStartEndDate(evnt_id) {
    $.post('/Event/GetEventDataJSON', { event_ID: evnt_id }, function (data) {
        if (data == null) return;
        $("#form_auction_StartDate").attr("value", data.s_1 + ' ' + data.s_2);
        $("#form_auction_EndDate").attr("value", data.e_1 + ' ' + data.e_2);
        $("#dCategoryA_list").jqGrid('setPostData', { event_id: data.id });
        $("#dCategoryA_list").trigger('reloadGrid');
    }, 'json');
}

//SubmitAuctionForm
function SubmitAuctionForm() {
    var tags = [];
    $.each($(".checked > input"), function (i, item) {
        var tagID = $(item).val();
        if (tagID > 0) {
            tags.push(tagID);
        }
    });
    return {
        ID: $("#form_auction_Auction_ID").attr("value"),
        Event_ID: $("#form_auction_Event").val(),
        Owner_ID: $("#form_auction_Seller").attr("res"),
        CommissionRate_ID: $("#form_auction_CommissionRate").val(),
        Status_ID: $("#form_auction_Status").attr("value"),
        Title: $("#form_auction_Title").attr("value"),
        Lot: $("#form_auction_Lot").attr("value"),
        MainCategory_ID: $("#form_auction_MainCategory").val(),
        Category_ID: $("#form_auction_Category").attr("res"),
        Quantity: $("#form_auction_Quantity").attr("value"),
        Price: $("#form_auction_Price").attr("value"),
        Reserve: $("#form_auction_Reserve").attr("value"),
        Estimate: $("#form_auction_Estimate").attr("value"),
        Shipping: $("#form_auction_Shipping").attr("value"),
        ListingStep: $("#form_auction_Step").val(),
        Priority: $("#form_auction_Priority").val(),
        LOA: $("#form_auction_LOA").attr("checked"),
        IsPhotographed: $("#form_auction_Photographed").attr("checked"),
        IsInLayout: $("#form_auction_InLayout").attr("checked"),
        Addendum: $("#form_auction_Addendum").attr("value"),
        OldAuction_ID: $("#form_auction_OldInventory").attr("res"),
        CollectionID: $("#form_auction_collection").attr("res"),
        PulledOut: $("#form_auction_IsPulledOut").attr("checked"),
        IsUnsold: $("#form_auction_IsUnsold").attr("checked"),
        IsCatalog: $("#form_auction_Catlog").attr("checked"),
        IsPrinted: $("#form_auction_IsPrinted").attr("checked"),
        Description: $("#form_auction_Description").attr("value"), //tinyMCE.get('form_auction_Description').getContent(),
        IsLimitDisabled: $("#form_auction_DisableLimit").attr("checked"),
        CopyNotes: $("#form_auction_CopyNotes").attr("value"),
        PhotoNotes: $("#form_auction_PhotoNotes").attr("value"),
        StartDate: $("#form_auction_StartDate").attr("value"),
        EndDate: $("#form_auction_EndDate").attr("value"),
        PurchasedPrice: $("#form_auction_PurchasedPrice").attr("value"),
        SoldPrice: $("#form_auction_SoldPrice").attr("value"),
        PurchasedWay: $("#form_auction_PurchasedWay").val(),
        SoldWay: $("#form_auction_SoldWay").val(),
        Tags: tags
    };
}

//TransferModelToAuctionForm
function TransferModelToAuctionForm(data) {
    var msg = data.Message;
    var det = '';
    if (data.Details != null) {
        det += '<ul>';
        var exists = false;
        $.each(data.Details, function (i, item) {
            exists = true;
            switch (item.field) {
                case "Category_ID": $("#form_auction_Category").addClass("input-validation-error"); break;
                case "Owner_ID": $("#form_auction_Seller").addClass("input-validation-error"); break;
                case "Title": $("#form_auction_Title").addClass("input-validation-error"); break;
                case "Price": $("#form_auction_Price").addClass("input-validation-error"); break;
                case "Quantity": $("#form_auction_Quantity").addClass("input-validation-error"); break;
                case "Reserve": $("#form_auction_Reserve").addClass("input-validation-error"); break;
                case "StartDate": $("#form_auction_StartDate").addClass("input-validation-error"); break;
                case "EndDate": $("#form_auction_EndDate").addClass("input-validation-error"); break;
                case "Shipping": $("#form_auction_Shipping").addClass("input-validation-error"); break;
                case "OldAuction_ID": $("#form_auction_OldInventory").addClass("input-validation-error"); break;
                case "Lot": $("#form_auction_Lot").addClass("input-validation-error"); break;
                case "ListingStep": $("#form_auction_Step").addClass("input-validation-error"); break;
                default: exists = false; break;
            }
            det += (exists) ? ('<li>' + item.message + '</li>') : '';
        });
        det += '</ul>';
    }
    MessageBox("Auction saving", msg, det, "error");
}

//UpdateAuction
function UpdateAuction() {
    var desc = $('#form_auction_Description').val().length;
    var lim = sarr[$("#form_auction_Priority").val()] + 100;
    if (desc > lim && !$("#form_auction_DisableLimit").attr("checked")) {
        MessageBox("Auction saving", "Warning! You are over your character limit for this item. Please revise your copy", '', "error");
        $("#form_auction_Description").addClass("input-validation-error");
        return;
    }
    $("*", "#form_auction").removeClass("input-validation-error");
    LoadingFormOpen();
    $.post('/Auction/UpdateAuction', { auction: Sys.Serialization.JavaScriptSerializer.serialize(SubmitAuctionForm()) }, function (data) {
        LoadingFormClose();
        switch (data.Status) {
            case "ERROR":
                TransferModelToAuctionForm(data);
                break;
            case "SUCCESS":
                $("#form_auction_Auction_ID").attr("value", data.Details);
                $("#form_auction").dialog('close');
                break;
        }
    }, 'json');
}

//InitNewSeller
function InitNewSeller() {
    var t = $("#form_user_Login").attr('value') + ' (' + $("#form_user_b_FirstName").attr('value') + ' ' + $("#form_user_b_LastName").attr('value') + ')';
    $("#form_auction_Seller").attr('title', t);
    $("#form_auction_Seller").attr('value', t);
    t = $("#form_user_User_ID").attr('value');
    $("#form_auction_Seller").attr('res', t);
    t = $("#form_user_CommissionRate_ID").val();
    $("#form_auction_CommissionRate").val(t);
    $("#dSellersA_list").trigger('reloadGrid');
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

//form_auction_SetDescritptionMaxLenght
function form_auction_SetDescritptionMaxLenght(priority) {
    $('#form_auction_stotal').html(sarr[priority]);
    $('#form_auction_description_range').html(sarr[priority] + '-' + (sarr[priority] + 100));
    setformfieldsize($('#form_auction_Description'), sarr[priority] + 100, 'form_auction_sleft');
}
function form_auction_DisableDescriptionLenght() {
    $('#form_auction_stotal').html("oo");
    $('#form_auction_description_range').html('7900-8000');
    setformfieldsize($('#form_auction_Description'), 8000, 'form_auction_sleft');
}

//----------------------------------------------------------------------------------------------------------------
var sarr = [0, 7900, 750, 650, 650];
var isneedtochangecategory = true;
$(document).ready(function () {
    $("#form_auction").dialog({
        bgiframe: true,
        autoOpen: false,
        height: 780,
        width: 800,
        modal: true,
        gridview: true,
        beforeclose: function (event, ui) {
            $("#img_list").jqGrid('clearGridData');

        },
        buttons: {
            'Cancel': function () { $(this).dialog('close'); },
            'Save': function () { UpdateAuction(); }
        },
        close: function () { $("*", "#form_auction").removeClass("input-validation-error"); }
    });
    $("#form_auction_tabs").tabs();

    InitDropDown('/General/GetCommissionRatesJSON', '#form_auction_CommissionRate');
    InitDropDown('/General/GetAuctionStatusesJSON', '#form_auction_Status');
    InitDropDown('/General/GetMainCategoriesJSON', '#form_auction_MainCategory');
    InitDropDown('/General/GetPurchasedTypesJSON?isnull=true', '#form_auction_PurchasedWay');
    InitDropDown('/General/GetPurchasedTypesJSON?isnull=true', '#form_auction_SoldWay');

    $("#form_auction_Seller").click(function () { $('#dSellersA').dialog('open'); });
    $("#form_auction_Category").click(function () { $('#dCategoryA').dialog('open'); });
    $("#form_auction_OldInventory").click(function () { $('#dAuctionsA').dialog('open'); });
    $("#form_auction_collection").click(function () { $('#dAuctionsCollection').dialog('open'); });

    $("#form_auction_MainCategory").change(function () {
        if (isneedtochangecategory) $('#form_auction_Category').attr('title', '').attr('value', '').attr('res', '');
        $("#dCategoryA_list").jqGrid('setPostData', { maincat_id: $("#form_auction_MainCategory").val(), event_id: $("#form_auction_Event").val() });
        $("#dCategoryA_list").trigger('reloadGrid');
    });

    $("#form_auction_btnPreviewUser").click(function () {
        var id = $("#form_auction_Seller").attr("res");
        if (id == '') { MessageBox('Preview consignor info', 'Please select the consignor.', '', 'info'); return; }
        LoadingFormOpen();
        $.post('/User/GetUserDataJSON', { user_id: id }, function (data) { InitUserForm(data, InitNewSeller, InitNewSellerLoading); LoadingFormClose(); }, 'json');
    });

    $("#form_auction_Event").change(function () {
        var e_id = $("#form_auction_Event").val();
        GetStartEndDate(e_id);
    });

    $("#form_auction_btnResetOldInventory").click(function () {
        $("#form_auction_OldInventory").attr('value', '').attr('title', '').attr('res', '');
    });

    $("#form_auction_btnResetCollection").click(function () {
        $("#form_auction_collection").attr('value', '').attr('title', '').attr('res', '');
    });

    $("#form_auction_StartDate").attr("disabled", "true");
    $("#form_auction_EndDate").attr("disabled", "true");
    $("#form_auction_Quantity").attr('disabled', 'true');

    $("#form_auction_Priority").change(function () {
        var check = $("#form_auction_DisableLimit").attr("checked");
        if (check) return;
        var priority = $("#form_auction_Priority").val();
        if ($("#form_auction_Description").attr("value").length > (sarr[priority] + 100)) {
            MessageBox("Changing the page layout", "The number of characters for this type of your copy will be limited to " + (sarr[priority] + 100) + ". You are over your character limit for this item. Please revise your copy.", "", "info");
            $('#form_auction_stotal').html(sarr[priority]);
            $('#form_auction_description_range').html(sarr[priority] + '-' + (sarr[priority] + 100));
        }
        else
            form_auction_SetDescritptionMaxLenght(priority);
    });

    $("#form_auction_DisableLimit").change(function () {
        if ($("#form_auction_DisableLimit").attr("checked"))
            form_auction_DisableDescriptionLenght();
        else {
            var priority = $("#form_auction_Priority").val();
            if (($("#form_auction_Description").attr("value").length > (sarr[priority] + 100))) {
                MessageBox("Changing the page layout", "The number of characters for this type of your copy will be limited to " + (sarr[priority] + 100) + ". You are over your character limit for this item. Please revise your copy.", "", "info");
                $('#form_auction_stotal').html(sarr[priority]);
                $('#form_auction_description_range').html(sarr[priority] + '-' + (sarr[priority] + 100));
            }
            else
                form_auction_SetDescritptionMaxLenght(priority);
        }
    });

    $('#form_auction_Description').change(function () {
        if ($("#form_auction_DisableLimit").attr("checked")) {
            $('#form_auction_sleft').css('color', 'green');
        } else {
            $('#form_auction_sleft').html($('#form_auction_Description').val().length);
            $('#form_auction_sleft').css('color', parseInt($('#form_auction_sleft').html()) <= parseInt(sarr[$("#form_auction_Priority").val()]) ? 'green' : 'red');
        }
    });
    $("#form_auction_Description").keydown(function (event) {
        $("#form_auction_Description").change();
    });
    $("#form_auction_Description").bind('paste', function (e) {
        var el = $(this);
        setTimeout(function () {
            if ($("#form_auction_DisableLimit").attr("checked")) return;
            $("#form_auction_Description").change();
            var text = $(el).val();
            var lim = sarr[$("#form_auction_Priority").val()] + 100;
            if ($("#form_auction_Step").val() < 2 && (text.length >= lim - 5 && text.length <= lim)) {
                MessageBox("Auction saving", "Your description for this item is close to Maximum Characters. Please make sure copy is complete.", '', "info");
            } else if (text.length > lim) {
                MessageBox("Auction saving", "Warning! You are over your character limit for this item. Please revise your copy", '', "error");
            }
        }, 100);
    });

    InitImageGrid("#img_list", "#img_pager", 470);
});
/*----------------------------------------------------------------*/
//OpenAuctionFormByID
function OpenAuctionFormByID(id) {
    if (id == null) { MessageBox('Edit auction', 'Please select the row', '', 'info'); return; }
    $.post('/Auction/GetAuctionJSON', { auction_id: id }, function (data) { LoadingFormOpen(); InitAuctionForm(data); LoadingFormClose(); }, 'json');
}