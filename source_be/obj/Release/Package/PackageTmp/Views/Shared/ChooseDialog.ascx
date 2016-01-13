<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ChooseDialogParam>" %>

<div id="<%=Model.Name %>" title="<%=Model.Title %>">
  <table id="<%=Model.Name %>_list"></table>
  <div id="<%=Model.Name %>_pager"></div>
  <%--<div id="<%=Model.Name %>_filter" style="margin-left:30%;display:none">Search</div>  --%>
</div>

<script type="text/javascript">
  var rowID_<%=Model.Name %> = -1;
  var result_control_<%=Model.Name %> = null;

  function OpenChooseDialog_<%=Model.Name %>(control){
    result_control_<%=Model.Name %> = control;
    $("#<%=Model.Name %>").dialog('open');    
  }

  $(document).ready(function() {
    $("#<%=Model.Name %>").dialog({
      bgiframe: true,
      autoOpen: false,
      height: 420,
      width: 490,
      modal: true,
      gridview: true,
      buttons: {
        Cancel: function() {
          $("#<%=Model.ResultElement %>").attr("res", '').attr("value", '').attr("title", '');
          if (result_control_<%=Model.Name %>!=null) {              
              $(result_control_<%=Model.Name %>).attr("res", '').attr("value", '').attr("title", '');
            }
          <%=Model.CallbackCancel %>
          $(this).dialog('close');
        },
       'Select': function() {
         if (rowID_<%=Model.Name %>!=-1){
            var ret = $("#<%=Model.Name %>_list").jqGrid('getRowData', rowID_<%=Model.Name %>);
            $("#<%=Model.ResultElement %>").attr("res", <%=Model.ResultID %>).attr("value", <%=Model.ResultShow %>).attr("title", <%=Model.ResultShow %>);
            if (result_control_<%=Model.Name %>!=null) {              
              $(result_control_<%=Model.Name %>).attr("res", <%=Model.ResultID %>).attr("value", <%=Model.ResultShow %>).attr("title", <%=Model.ResultShow %>);
            }
            <%=Model.CallbackSuccess %>
          } else {
            $("#<%=Model.ResultElement %>").attr("res", '').attr("value", '').attr("title", '');
            if (result_control_<%=Model.Name %>!=null) {
              $(result_control_<%=Model.Name %>).attr("res", '').attr("value", '').attr("title", '');
            }         
          }
          $(this).dialog('close');                   
        }        
      },
      close: function() {
        <%=Model.CallbackClose %>
      }
    });

    $("#<%=Model.Name %>_list").jqGrid({
     url: "<%=Model.Method %>",
     datatype: "json",
     mtype: 'GET',
     height: 240,
     width: 460,
     colNames: [<%=Model.ColumnHeaders %>],
     colModel: [
        <%=Model.Columns %>
      ],
     loadtext: 'Loading ...',
     rowNum: 10,
     rowList: [10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200],
     pager: '#<%=Model.Name %>_pager',
     viewrecords: true,
     shrinkToFit: false,
     sortname: "<%=Model.SortName %>",
     sortorder: "<%=Model.SortOrder %>",
     editurl: "<%=Model.EditURL %>",
     postData:{_firstload : 'true'},     
     onSelectRow: function(id) { rowID_<%=Model.Name %> = id; },     
     ondblClickRow: function(id){ var ret = $("#<%=Model.Name %>_list").jqGrid('getRowData', id);
          $("#<%=Model.ResultElement %>").attr("res", <%=Model.ResultID %>).attr("value", <%=Model.ResultShow %>).attr("title", <%=Model.ResultShow %>);
            if (result_control_<%=Model.Name %>!=null) {              
              $(result_control_<%=Model.Name %>).attr("res", <%=Model.ResultID %>).attr("value", <%=Model.ResultShow %>).attr("title", <%=Model.ResultShow %>);
            }
          <%=Model.CallbackSuccess %>
          $("#<%=Model.Name %>").dialog('close');}     
    });
    $("#<%=Model.Name %>_list").jqGrid('navGrid', '#<%=Model.Name %>_pager', { edit: <%=(String.IsNullOrEmpty(Model.Edit))?"false":"true" %>, edittext: "Edit", add: <%=(String.IsNullOrEmpty(Model.Add))?"false":"true" %>, addtext: "Add", del: <%=(String.IsNullOrEmpty(Model.Delete))?"false":"true" %>, deltext: "Delete", search: false, refreshtext: "Refresh", afterRefresh: function () { $("#<%=Model.Name %>_list")[0].clearToolbar(); } },
    {<%=Model.Edit %>},
    {<%=Model.Add %>},
    {<%=Model.Delete %>},
    {},
    {});
    
    $("#<%=Model.Name %>_list").jqGrid('filterToolbar');
    $("#<%=Model.Name %>_list").jqGrid('setPostData', {_firstload:'false'} );
  });
</script>