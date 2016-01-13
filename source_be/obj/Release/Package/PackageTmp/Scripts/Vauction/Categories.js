// =========================================================================================================================
// Categories.aspx
// =========================================================================================================================
$(document).ready(function() {
  var categories_w = (document.body.offsetWidth - 140) * 0.5;
  var categories_rowID = null;
  var categoriesmap_rowID = null;
  var main_categories_rowID = null;

  $("#mc_list").jqGrid({
    url: '/Category/GetMainCategoriesList/',
    datatype: "json",
    height: '100%',
    width: categories_w,
    colNames: ['MC#', 'Title', 'Description', 'Active', 'Priority', 'Date Entered', 'Entered By', 'Last Updated'],
    colModel: [
          { name: 'MainCat_ID', index: 'MainCat_ID', width: 40, key: true, editable: false, formoptions: { elmprefix: "    ", elmsuffix: "" }, editoptions: { readonly: true} },
 		      { name: 'Title', index: 'Title', width: 160, editable: true, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" }, editrules: { edithidden: true} },
 		      { hidden:true, name: 'Descr', index: 'Descr', editable: true, width: 60, formoptions: { elmprefix: "    ", elmsuffix: "" }, editrules: { edithidden: true} },
 		      { name: 'IsActive', index: 'IsActive', width: 40, align: 'center', search: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
 		      { name: 'Priority', index: 'Priority', width: 50, align: 'center', editable: true, editrules: { required: true, number: true, minValue: 1 }, formoptions: { elmprefix: "(*)", elmsuffix: "" }, editoptions: { defaultValue: function() { return "1" } } },
 		      { name: 'DateEntered', index: 'DateEntered', width: 140, editable: false, searchoptions: { dataInit: function(el) { $(el).datepicker({ dateFormat: 'mm/dd/yy' }); } } },
 		      { name: 'Owner_ID', index: 'Owner_ID', width: 50, editable: false, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
 		      { name: 'LastUpdate', index: 'LastUpdate', width: 160, search: false }
 	      ],
    loadtext: 'Loading ...',
    rowNum: 20,
    rowList: [20, 25, 30, 35, 40, 60, 80, 100],
    pager: 'mc_pager',
    viewrecords: true,    
    shrinkToFit: false,
    sortname: 'Priority',
    sortorder: 'asc',
    caption: 'Main Categories List',
    onSelectRow: function(id) { main_categories_rowID = id; },
    editurl: "/Category/UpdateMainCategory/",
    loadComplete: function() {
      $("#mt_list").jqGrid('setGridHeight', parseInt($("#mc_list").css("height")) + parseInt($("#mc_pager").css("height")) - parseInt($("#mt_pager").css("height")));
    }
  });
  $("#mc_list").jqGrid('navGrid', '#mc_pager',
  { edit: true, edittext: "Edit", add: true, addtext: "Add", del: true, deltext: "Delete", search: false, refreshtext: "Refresh", afterRefresh: function() { $("#mc_list")[0].clearToolbar(); } },
  { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterEdit: true, bSubmit: "Save", editCaption: "Edit MainCategory", afterSubmit: AfterSubmitFunction },
  { jqModal: false, modal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterAdd: true, bSubmit: "Save", addCaption: "Add MainCategory", afterSubmit: AfterSubmitFunction },
  { afterSubmit: AfterSubmitFunction },
  {}, {})
  $("#mc_list").jqGrid('filterToolbar');

  $("#c_list").jqGrid({
    url: '/Category/GetCategoriesList/',
    datatype: "json",
    height: '100%',
    width: categories_w,
    colNames: ['Cat#', 'Title', 'Description', 'Active', 'Priority', 'Date Entered', 'Entered By', 'Last Updated'],
    colModel: [
          { name: 'Cat_ID', index: 'Cat_ID', width: 40, key: true, editable: false, formoptions: { elmprefix: "    ", elmsuffix: "" }, editoptions: { readonly: true} },
 		      { name: 'Title', index: 'Title', width: 160, editable: true, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
 		      { hidden: true, name: 'Description', index: 'Description', editable: true, width: 60, formoptions: { elmprefix: "    ", elmsuffix: "" }, editrules: { edithidden: true} },
 		      { name: 'IsActive', index: 'IsActive', width: 40, align: 'center', search: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
 		      { name: 'Priority', index: 'Priority', align: 'center', width: 50, editable: true, editrules: { required: true, number: true, minValue: 1 }, formoptions: { elmprefix: "(*)", elmsuffix: ""}, editoptions: { defaultValue: function() { return "1" } } },
 		      { name: 'DateEntered', index: 'DateEntered', width: 140, editable: false, searchoptions: { dataInit: function(el) { $(el).datepicker({ dateFormat: 'mm/dd/yy' }); } } },
 		      { name: 'Owner_ID', index: 'Owner_ID', width: 50, editable: false, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
 		      { name: 'LastUpdate', index: 'LastUpdate', width: 160, search:false }
 	      ],
    loadtext: 'Loading ...',
    rowNum: 15,
    rowList: [15, 20, 25, 30, 35, 40, 60, 80, 100],
    pager: 'c_pager',
    viewrecords: true,
    shrinkToFit: false,
    sortname: 'Title',
    caption: 'Categories List',
    sortorder: 'asc',
    onSelectRow: function(id) { categories_rowID = id; },
    editurl: "/Category/UpdateCategory/"
  });
  $("#c_list").jqGrid('navGrid', '#c_pager',
  { edit: true, edittext: "Edit", add: true, addtext: "Add", del: true, deltext: "Delete", search: false, refreshtext: "Refresh", afterRefresh: function() { $("#c_list")[0].clearToolbar(); } },
  { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterEdit: true, bSubmit: "Save", editCaption: "Edit category", afterSubmit: AfterSubmitFunction },
  { jqModal: false, modal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterAdd: true, bSubmit: "Save", addCaption: "Add category", afterSubmit: AfterSubmitFunction },
  { afterSubmit: AfterSubmitFunction },
  {}, {})
  $("#c_list").jqGrid('filterToolbar');

  $("#cm_list").jqGrid({
    url: '/Category/GetCategoriesMapList/',
    datatype: "json",
    height: '100%',
    width: categories_w,
    colNames: ['CM#', 'MC#', 'Main Category', 'Cat#', 'Category', 'Description', 'Default', 'Intro', 'Active', 'Priority', 'Entered By', 'Last Updated'],
    colModel: [
          { name: 'CatMap_ID', index: 'CatMap_ID', width: 50, key: true, editable: false, formoptions: { elmprefix: "    ", elmsuffix: "" }, editoptions: { readonly: true} },
          { name: 'MainCategory_ID', index: 'MainCategory_ID', width: 50, editable: true, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" }, edittype: 'custom', editoptions: { custom_element: my_input, custom_value: my_value} },
          { name: 'MainCat', index: 'MainCat', width: 100, editable: false },
          { name: 'Category_ID', index: 'Category_ID', width: 40, editable: true, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" }, edittype: 'custom', editoptions: { custom_element: my_input2, custom_value: my_value} },
          { name: 'Cat', index: 'Cat', width: 200, editable: false }, 		      
 		      { hidden: true, name: 'Descr', index: 'Descr', editable: false, width: 60, formoptions: { elmprefix: "    ", elmsuffix: "" }, editrules: { edithidden: true} },
 		      { name: 'IsDefault', index: 'IsDefault', width: 40, align: 'center', search: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
 		      { hidden: true, name: 'Intro', index: 'Intro', editable: false, width: 60, formoptions: { elmprefix: "    ", elmsuffix: "" }, editrules: { edithidden: true} },
 		      { name: 'IsActive', index: 'IsActive', width: 40, align: 'center', search: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
 		     { name: 'Priority', index: 'Priority', width: 50, align: 'center', editable: true, editrules: { required: true, number: true, minValue: 1 }, formoptions: { elmprefix: "(*)", elmsuffix: "" }, editoptions: { defaultValue: function() { return "1" } } },
 		      { name: 'Owner_ID', index: 'Owner_ID', width: 60},
 		      { name: 'LastUpdate', index: 'LastUpdate', width: 160, search:false } 		      
 	      ],
    loadtext: 'Loading ...',
    rowNum: 25,
    rowList: [20, 25, 30, 35, 40, 60, 80, 100],
    pager: 'cm_pager',
    viewrecords: true,
    shrinkToFit: false,
    sortname: 'Descr',
    sortorder: 'asc',
    caption : 'Categories Tree List',
    onSelectRow: function(id) { categoriesmap_rowID = id; },
    editurl: "/Category/UpdateCategoiesMap/",
    toolbar: [true, "top"]
  });
  $("#cm_list").jqGrid('navGrid', '#mc_pager',
  { edit: true, edittext: "Edit", add: true, addtext: "Add", del: true, deltext: "Delete", search: false, refreshtext: "Refresh", afterRefresh: function() { $("#cm_list")[0].clearToolbar(); } },
  { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterEdit: true, bSubmit: "Save", editCaption: "Edit MainCategory", afterSubmit: AfterSubmitFunction },
  { jqModal: false, modal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterAdd: true, bSubmit: "Save", addCaption: "Add MainCategory", afterSubmit: AfterSubmitFunction },
  { afterSubmit: AfterSubmitFunction },
  {}, {})
  $("#cm_list").jqGrid('filterToolbar');

  $("#t_cm_list").append("<button style='height:20px;font-size:-3; margin-left:10px' id='btnMegre'>Megring categories</button>");

  $("#btnMegre").addClass("ui-button ui-state-default");
  $("#btnMegre").hover(
		  function() {
		    $(this).addClass("ui-state-hover");
		  },
		  function() {
		    $(this).removeClass("ui-state-hover");
		  }
	  ).mousedown(function() {
	    $(this).addClass("ui-state-active");
	  })
	  .mouseup(function() {
	    $(this).removeClass("ui-state-active");
	  });
  $("#btnMegre").click(function() { InitCategoriesMergingForm() });
});

//--[Functions]------------------------------------------------------------------------------------
function my_input(value, options) {
  var cat = '';
  var id = $("#cm_list").jqGrid('getGridParam', 'selrow');
  if (id) {
    var ret = $("#cm_list").jqGrid('getRowData', id);
    cat = ret.MainCat;
  }
  return $('<input type="text" class = "dialog_text" readonly = "true" res = "' + value + '" style="width:182px" onclick="open_dialog_maincategory(this)" value="' + cat + '" />');
}

function my_value(value) {
  return value.attr("res");
}

function open_dialog_maincategory(control) {
  $("#dMainCategory_list").jqGrid('setPostData', { IsActive: true });
  $("#dMainCategory_list").trigger('reloadGrid');
  OpenChooseDialog_dMainCategory(control);
}

function my_input2(value, options) {
  var cat = '';
  var id = $("#cm_list").jqGrid('getGridParam', 'selrow');
  if (id) {
    var ret = $("#cm_list").jqGrid('getRowData', id);
    cat = ret.Cat;
  }
  return $('<input type="text" class = "dialog_text" readonly = "true" res = "' + value + '" style="width:182px" onclick="open_dialog_category(this)" value="' + cat + '" />');
}

function open_dialog_category(control) {
  $("#dCategory_list").jqGrid('setPostData', { IsActive: true });
  $("#dCategory_list").trigger('reloadGrid');
  OpenChooseDialog_dCategory(control);
}