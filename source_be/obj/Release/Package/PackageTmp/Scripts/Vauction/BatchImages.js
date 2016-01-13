$(document).ready(function() {
  var items_table_w = document.body.offsetWidth - 100;
  $("#img_list").jqGrid({
    mtype: 'POST',
    url: '/Image/GetAuctionImages/',
    datatype: "json",
    height: '100%',
    width: items_table_w,
    colNames: ['#', 'Image', 'File Name', 'File Extension', 'Upload Date'],
    colModel: [
          { name: 'Image_ID', index: 'Image_ID', width: 70, key: true, search: false, hidden: true },
          { name: 'Image', index: 'Image', width: 150, search: false, sortable: false, align: 'center', editable: false },
          { name: 'Title', index: 'Title', width: 435, search: false, sortable: false, editable: true },
          { name: 'Extension', index: 'Extension', width: 80, search: false, sortable: false, editable: false, align: 'center' },
          { name: 'Date', index: 'Date', width: 150, search: false, sortable: false, align: 'center', editable: false }
        ],
    loadtext: 'Loading ...',
    rowNum: 10,
    rowList: [10, 15, 20, 25, 30, 35, 40, 60, 80, 100],
    pager: '#img_pager',
    viewrecords: true,
    shrinkToFit: false,
    sortname: 'Image_ID',
    sortorder: 'desc',
    multiselect: true
  });
  $("#img_list").jqGrid('navGrid', '#img_pager', { edit: true, edittext: "Edit", add: false, del: true, deltext: "Delete", search: false, refreshtext: "Refresh" },
    { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterEdit: true, bSubmit: "Save", editCaption: "Edit file name", afterSubmit: AfterSubmitFunction, url: "/Image/EditImage/" }, //edit
    {}, //add
    {jqModal: false, closeOnEscape: true, closeAfterAdd: true, bSubmit: "Delete", afterSubmit: AfterSubmitFunction, url: "/Image/DeleteImage/" }, //delete
    {}, {}).navSeparatorAdd('#img_pager');
  //btnAdd
  $("#img_list").jqGrid('navButtonAdd', '#img_pager', { caption: "Upload images", buttonicon: 'ui-icon-plus', title: 'Upload new images', onClickButton: function() { $("form_iupload").html('<input type="file" id="biuFile" />'); $("#form_iupload").dialog('open'); } }).navSeparatorAdd('#img_pager');
  //btnAsign
  $("#img_list").jqGrid('navButtonAdd', '#img_pager', { caption: "Asign images", buttonicon: 'ui-icon-shuffle', title: 'Asign images to lots', onClickButton: function() { AsignImages(); } }).navSeparatorAdd('#img_pager');

  $("#form_iupload").dialog({
    bgiframe: true,
    autoOpen: false,
    height: 300,
    width: 420,
    modal: true,
    gridview: true,
    close: function() { $("#img_list").trigger('reloadGrid'); }
  });

  $('#biuFile').uploadify({
    'uploader': '/Content/images/uploadify.swf',
    'script': '/Image/UploadPicture',
    'cancelImg': '/Content/images/cancel.png',
    'multi': 'true',
    'auto': 'true',
    'fileDesc': 'Image Files (.JPG, .GIF, .PNG)',
    'fileExt': '*.jpg;*.jpeg;*.png;*.gif',
    'method': 'POST'
    //    ,onError: function(a, b, c, d) {
    //      if (d.status == 404)
    //        alert("Could not find upload script. Use a path relative to: ");
    //      else if (d.type === "HTTP")
    //        alert("error " + d.type + ": " + d.status);
    //      else if (d.type === "File Size")
    //        alert(c.name + " " + d.type + " Limit: " + Math.round(d.sizeLimit / 1024) + "KB");
    //      else
    //        alert("error " + d.type + ": " + d.text);
    //    }
  });
});
//--[Functions]------------------------------------------------------------------------------------
function AsignImages(id) {
  var ids = $("#img_list").jqGrid('getGridParam', 'selarrrow');
  InitEventFormForAsignImage(ids);  
}