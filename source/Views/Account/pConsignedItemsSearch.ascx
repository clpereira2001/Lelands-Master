<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AuctionFilterParams>" %>

<%@ Import Namespace="Vauction.Models.CustomModels" %>
<form method="get" action="/Account/ConsignedItems">
  <input type="hidden" name="SeachType" value="1" />
  <span style='color:Maroon; font-weight:bold'>Select the auction then hit "Go" to see your consignment(s)</span>
  <fieldset class="blue_box" style="width: 740px">
    <table class="text1" cellpadding="0" cellspacing="0" style="//padding: 0px">
      <tr>
        <td width="250">
          Select Auction
        </td>
        <td width="150">
          Select Search method
        </td>
        <td width="250">
          Enter Lot # or Keyword in Title or Description
        </td>
        <td style="//padding-left: 0px">
          &nbsp;
        </td>
      </tr>
      <tr>
        <td>
          <%= Html.DropDownList("Event_ID", (IEnumerable<SelectListItem>)ViewData["AllEvents"], new { style="width:250px" })%>
        </td>
        <td>
          <%= Html.DropDownList("Type", AuctionFilterParams.SearchMethodList, new { style = "width:150px" })%>
        </td>
        <td>
          <%= Html.TextBox("Description", Model.DataForSearchBox, new { style = "width:250px" })%>
        </td>
        <td style="//padding-left: 0px">
          <div class="custom_button" style="width: 30px;">
            <button type="submit">
              Go</button>
          </div>
        </td>
      </tr>
    </table>
  </fieldset>    
</form>