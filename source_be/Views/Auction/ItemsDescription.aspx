<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Adding description and copy notes to the inventory - <%=Consts.CompanyTitleName %></title>
  <% Html.Script("maxlength.js"); %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Adding description and copy notes to the inventory</h2>
    <table>
      <tr>
        <td>
          <label for="event_id">Event:</label>
        </td>
        <td colspan="2">
          <%= Html.DropDownList("event_id", new List<SelectListItem>(), new { @style = "width:440px" })%>
        </td> 
      </tr>
      <tr>
        <td>
          <label for="auction_id" class="inline">Inventory:</label>
        </td>
        <td colspan="2">
          <%= Html.TextBox("auction_id", "", new { @class = "dialog_text", @readonly = "true", @res = "", @style = "width:432px" })%>
        </td>        
      </tr>
      <tr>
        <td colspan="3">
          <label for="copynotes">Copy Notes:</label><br />
          <%= Html.TextArea("copynotes", "", new { @style = "width:100%;height:100px;" })%>          
        </td>
      </tr>
      <tr>
        <td colspan="3">
          <label for="copynotes">Description: *</label><br />
          <%= Html.TextArea("description", "", new { @style = "width:100%;height:250px;" })%>
        </td>
      </tr>
      <tr>
        <td colspan="2">
          * Please try to keep within the <span id="range">0</span> range of characters
        </td>
      </tr>
      <tr>        
        <td>
          <label for="iswritten" class="inline">Written:</label><%= Html.CheckBox("iswritten")%>
        </td>
        <td style="width:280px; text-align:center">
          Character count limit: <span id="sleft" style="font-weight:bold">0</span> / <span id='stotal' style="font-weight:bold">0</span>          
        </td>
        <td style="text-align:right; vertical-align:middle">
          <button id="btnUpdate">Save</button>&nbsp;&nbsp;|&nbsp;&nbsp;<button id="btnCancel">Cancel</button>
        </td>
      </tr>      
    </table>

    <%=Html.Hidden("id") %>
    <%=Html.Hidden("priority") %>
    <%=Html.Hidden("islimitdisabled") %>

    <% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "dAuctions", Title = "Auctions list", Method = "/Auction/GetAuctionsListByEvent/", SortName = "Auction_ID", SortOrder = "desc", ColumnHeaders = "'Inv#', 'Title', 'Layout', 'Wr.', 'Priority', 'Description', 'CopyNotes', 'CheckActive','IsLimitDisabled'", Columns = "{ name: 'Auction_ID', index: 'Auction_ID', width: 50, key: true }, { name: 'Title', index: 'Title', width: 250}, { name:'PriorityDescription', index:'PriorityDescription', width:100, stype: 'select', editoptions: { value: '" + ViewData["PriorityList"] + "'} },{ name:'Step', index:'Step', width:30, align: 'center', formatter: 'checkbox', search: false }, { name: 'Priority', index: 'Priority', hidden:true, width: 1}, { name: 'Descr', index: 'Descr', hidden:true, width: 1}, { name: 'CNotes', index: 'CNotes', hidden:true, width: 1}, { name:'CheckActive', index:'CheckActive', width:1, formatter: 'checkbox', hidden:true }, { name:'IsLimitDisabled', index:'IsLimitDisabled', width:1, hidden:true }", ResultShow = "'#'+ret.Auction_ID+'. '+ret.Title", ResultID = "ret.Auction_ID", ResultElement = "auction_id", CallbackSuccess = "$('#copynotes').attr('value', ret.CNotes);$('#description').attr('value', ret.Descr);if (ret.Step=='Yes') $('#iswritten').attr('checked', true); else $('#iswritten').removeAttr('checked'); $('#id').attr('value', ret.Auction_ID); $('#stotal').html(sarr[ret.Priority]); $('#priority').attr('value', sarr[ret.Priority]);$('#description').change();if (ret.CheckActive=='Yes') $('#iswritten').removeAttr('disabled'); else $('#iswritten').attr('disabled', true); $('#islimitdisabled').attr('value',ret.IsLimitDisabled); if(ret.IsLimitDisabled=='1') {setformfieldsize($('#description'), 100000000, 'sleft'); $('#stotal').html('oo');} else setformfieldsize($('#description'), sarr[ret.Priority]+100, 'sleft'); $('#range').html(sarr[ret.Priority] + '-'+(sarr[ret.Priority]+100));", CallbackCancel = "ResetForm();" }); %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jsContent" runat="server">  
 <%--<%Html.ScriptV("ItemsDescription.js"); %>--%>
 <script src="/Scripts/Vauction/ItemsDescription.js" type="text/javascript"></script>
</asp:Content>

