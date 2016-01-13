// =========================================================================================================================
// CategoriesMergingForm.ascx
// =========================================================================================================================
function InitCategoriesMergingForm() {
  $("#from_tm_list").trigger('reloadGrid');
  $("#to_tm_list").trigger('reloadGrid');
  categoriesmap_from_rowID = null;
  categoriesmap_to_rowID = null;
  $("#from_description").html("");
  $("#to_description").html("");
  $('#form_categories_megring').dialog('open');
}

function ShowDescriptionForCategoryMap(id, elem, img) {
  $(elem).hide();
  $(img).show();
  $.post('/Category/GetCategoryMapDescription', { catmap_id: id }, function(data) {
    switch (data.Status) {
      case "ERROR":
        MessageBox("Get category's description", data.Message, '', "error");
        break;
      case "SUCCESS":
        var text = "";
        if (data.Details != null) {
          text += "<ul>";
          $.each(data.Details, function(i, item) {
            text += '<li>' + item + '</li>';
          });
          text += '</ul>';
        }
        else {
          text = data.Message;
        }
        $(img).hide();
        $(elem).html(text);
        $(elem).show();
        break;
    }
  }, 'json');
}
//----------------------------------------------------------------------------------------------------------------
var categoriesmap_from_rowID = null;
var categoriesmap_to_rowID = null;

jQuery(document).ready(function() {
  $("#form_categories_megring").dialog({
    bgiframe: true,
    autoOpen: false,
    height: 620,
    width: 780,
    modal: true,
    gridview: true,
    buttons: {
      'Close': function() { $(this).dialog('close'); }
    },
    close: function() { }
  });

  $("#from_tm_list").jqGrid({
    url: '/Category/GetCategoriesMapList/',
    datatype: "json",
    height: '300',
    width: '350',
    datatype: 'json',
    mtype: 'POST',
    colNames: ['CM#', 'MainCat#', 'MainCat', 'Category#', 'Category', 'Active', 'Default'],
    colModel: [
          { name: 'CatMap_ID', index: 'CatMap_ID', width: 70, key: true, editable: true, formoptions: { elmprefix: "    ", elmsuffix: "" }, editoptions: { readonly: true} },
          { name: 'MainCategory_ID', index: 'MainCategory_ID', width: 60, editable: true, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" }, edittype: 'custom', editoptions: { custom_element: my_input, custom_value: my_value} },
          { name: 'MainCat', index: 'MainCat', width: 200, editable: false },
          { name: 'Category_ID', index: 'Category_ID', width: 60, editable: true, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" }, edittype: 'custom', editoptions: { custom_element: my_input2, custom_value: my_value} },
          { name: 'Cat', index: 'Cat', width: 200, editable: false }, 		     
 		      { name: 'IsActive', index: 'IsActive', width: 80, align: 'center', search: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "    ", elmsuffix: ""} }, 		     
 		      { name: 'IsDefault', index: 'IsDefault', width: 80, align: 'center', search: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
 	      ],
    loadtext: 'Loading ...',
    rowNum: 100000,
    viewrecords: true,
    shrinkToFit: true,
    sortname: 'MainCategory_ID',
    sortorder: 'asc',
    onSelectRow: function(id) { categoriesmap_from_rowID = id; ShowDescriptionForCategoryMap(id, "#from_description", "#imgLoadFrom"); }
  });

  $("#to_tm_list").jqGrid({
    url: '/Category/GetCategoriesMapList/',
    datatype: "json",
    height: '300',
    width: '350',
    datatype: 'json',
    mtype: 'POST',
    colNames: ['CM#', 'MainCat#', 'MainCat', 'Category#', 'Category', 'Active', 'Default'],
    colModel: [
          { name: 'CatMap_ID', index: 'CatMap_ID', width: 70, key: true, editable: true, formoptions: { elmprefix: "    ", elmsuffix: "" }, editoptions: { readonly: true} },
          { name: 'MainCategory_ID', index: 'MainCategory_ID', width: 60, editable: true, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" }, edittype: 'custom', editoptions: { custom_element: my_input, custom_value: my_value} },
          { name: 'MainCat', index: 'MainCat', width: 200, editable: false },
          { name: 'Category_ID', index: 'Category_ID', width: 60, editable: true, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" }, edittype: 'custom', editoptions: { custom_element: my_input2, custom_value: my_value} },
          { name: 'Cat', index: 'Cat', width: 200, editable: false },
 		      { name: 'IsActive', index: 'IsActive', width: 80, align: 'center', search: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
 		      { name: 'IsDefault', index: 'IsDefault', width: 80, align: 'center', search: false, formatter: 'checkbox', editable: true, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "    ", elmsuffix: ""} },
 	      ],
    loadtext: 'Loading ...',
    rowNum: 100000,
    viewrecords: true,
    shrinkToFit: true,
    sortname: 'MainCategory_ID',
    sortorder: 'asc',
    onSelectRow: function(id) { categoriesmap_to_rowID = id; ShowDescriptionForCategoryMap(id, "#to_description", "#imgLoadTo"); }
  });

  $("#btnMove").click(function() {
    if (categoriesmap_from_rowID == null) { MessageBox('Merging category', 'Please select the source category.', '', 'info'); return; }
    if (categoriesmap_to_rowID == null) { MessageBox('Merging category', 'Please select the destination category.', '', 'info'); return; }
    $("#btnMove").hide();
    $("#imgMove").show();
    $.post('/Category/MergingCategoriesMap', { cm_from: categoriesmap_from_rowID, cm_to: categoriesmap_to_rowID }, function(data) {
      switch (data.Status) {
        case "ERROR":
          MessageBox('Add category', data.Message, '', 'error');
          break;
        case "SUCCESS":
          ShowDescriptionForCategoryMap(categoriesmap_from_rowID, "#from_description", "#imgLoadFrom");
          ShowDescriptionForCategoryMap(categoriesmap_to_rowID, "#to_description", "#imgLoadTo");
          break;
      }
      $("#imgMove").hide();
      $("#btnMove").show();
    }, 'json');
  });
});