<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div id="form_biupload" title="Select the event" class="dialog_form">
  <fieldset style="margin:5px;">
    <table id="tblSchema">
      <tr>            
        <td>
          <p>
            <label for="form_biupload_event">Event:<em>*</em></label>
            <%=Html.DropDownList("form_biupload_event", new List<SelectListItem>(), new { @style = "width:300px" })%>
          </p>
        </td>
      </tr>
    </table>
  </fieldset>
</div>