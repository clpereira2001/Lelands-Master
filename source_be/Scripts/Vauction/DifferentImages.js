$(document).ready(function () {
  var items_table_w = document.body.offsetWidth - 100;
  var grid_a = $("#img_list").jqGrid({
    mtype: 'POST',
    url: '/Image/GetDifferentImages/',
    datatype: "json",
    height: '100%',
    width: items_table_w,
    colNames: ['#', 'Image', 'File Name', 'Link'],
    colModel: [
          { name: 'Image_ID', index: 'Image_ID', width: 70, key: true, search: false, hidden: true },
          { name: 'Image', index: 'Image', width: 150, search: false, sortable: false, align: 'center', editable: false },
          { name: 'FileName', index: 'FileName', width: 200, search: false, sortable: false, align: 'center', editable: false },
          { name: 'SitePath', index: 'SitePath', width: 635, search: false, sortable: false, editable: true }          
        ],
    loadtext: 'Loading ...',
    rowNum: 100,
    rowList: [10, 15, 20, 25, 30, 35, 40, 60, 80, 100],
    pager: '#img_pager',
    viewrecords: true,
    shrinkToFit: false,
    sortname: 'Image_ID',
    sortorder: 'desc',
    multiselect: true
  });
  $("#img_list").jqGrid('navGrid', '#img_pager', { edit: false, add: false, del: true, deltext: "Delete", search: false, refreshtext: "Refresh" }, {}, {}, { jqModal: false, closeOnEscape: true, closeAfterAdd: true, bSubmit: "Delete", afterSubmit: AfterSubmitFunction, url: "/Image/DeleteDifferentImage/" }, {}, {}).navSeparatorAdd('#img_pager');
  //btnAdd
  $("#img_list").jqGrid('navButtonAdd', '#img_pager', { caption: "Upload images", buttonicon: 'ui-icon-plus', title: 'Upload new images', onClickButton: function () { $("form_iupload").html('<input type="file" id="biuFile" />'); $("#form_iupload").dialog('open'); } }).navSeparatorAdd('#img_pager');  

  $("#form_iupload").dialog({
    bgiframe: true,
    autoOpen: false,
    height: 300,
    width: 420,
    modal: true,
    gridview: true,
    close: function () { $("#img_list").trigger('reloadGrid'); }
  });

  $('#biuFile').uploadify({
    'uploader': '/Content/images/uploadify.swf',
    'script': '/Image/UploadDifferentImages',
    'cancelImg': '/Content/images/cancel.png',
    'multi': 'true',
    'auto': 'true',
    'fileDesc': 'Image Files (.JPG, .GIF, .PNG)',
    'fileExt': '*.jpg;*.jpeg;*.png;*.gif',
    'method': 'POST'    
  });
});