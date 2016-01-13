$(document).ready(function () {
  var items_table_w = document.body.offsetWidth - 100;
  var grid_a = $("#img_list").jqGrid({
    mtype: 'POST',
    url: '/Image/GetHomepageImages/',
    datatype: "json",
    height: '100%',
    width: items_table_w,
    colNames: ['#', 'Image', 'File Name', 'Order', 'Link', 'Link Title', 'Type', 'Active'],
    colModel: [
          { name: 'Image_ID', index: 'Image_ID', width: 50, key: true, search: false, sortable: false },
          { name: 'Image', index: 'Image', width: 200, search: false, sortable: false, align: 'center' },
          { name: 'FileName', index: 'FileName', width: 250, search: false, sortable: false},
          { name: 'Order', index: 'Order', width: 60, search: false, sortable: false, editable: true, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
          { name: 'Link', index: 'Link', width: 150, search: false, sortable: false, editable: true, formoptions: { elmprefix: "    ", elmsuffix: ""} },
          { name: 'LinkTitle', index: 'LinkTitle', width: 150, search: false, sortable: false, editable: true, formoptions: { elmprefix: "    ", elmsuffix: ""} },
          { name: 'ImgType', index: 'ImgType', width: 60, sortable: false, editable: true, edittype: "select", editoptions: { value: "1:Big;2:Small;3:Stripe" }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
          { name: 'IsEnabled', index: 'IsEnabled', width: 60, align: 'center', sortable: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "    ", elmsuffix: ""} }
        ],
    loadtext: 'Loading ...',
    rowNum: 10,
    rowList: [10, 15, 20, 25, 30, 35, 40, 60, 80, 100],
    pager: '#img_pager',
    viewrecords: true,
    shrinkToFit: false,
    sortname: 'IsEnabled',
    sortorder: 'desc',
    multiselect: true
  });
  $("#img_list").jqGrid('navGrid', '#img_pager', { edit: true, edittext:"Edit", add: false, del: true, deltext: "Delete", search: false, refreshtext: "Refresh" },
    { jqModal: false, closeOnEscape: true, bSubmit: "Save", bottominfo: "Fields marked with (*) are required", editCaption: "Edit", closeAfterEdit: true, afterSubmit: AfterSubmitFunction, url: "/Image/UpdateHomepageImage/" }, {}, 
    { jqModal: false, closeOnEscape: true, bSubmit: "Delete", afterSubmit: AfterSubmitFunction, url: "/Image/DeleteHomepageImage/" }, {}, {}).navSeparatorAdd('#img_pager');
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
    'script': '/Image/UploadHomepageImages',
    'cancelImg': '/Content/images/cancel.png',
    'multi': 'true',
    'auto': 'true',
    'fileDesc': 'Image Files (.JPG, .GIF, .PNG)',
    'fileExt': '*.jpg;*.jpeg;*.png;*.gif',
    'method': 'POST'    
  });
});