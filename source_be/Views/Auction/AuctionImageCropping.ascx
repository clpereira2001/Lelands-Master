<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div id="form_cropping" title="Create a thumbnail" class="dialog_form">
  <%=Html.Hidden("form_cropping_auction_id", 0)%>
  <%=Html.Hidden("form_cropping_smallimage", "")%>
  <%=Html.Hidden("form_cropping_mediumimage", "")%>
  <%=Html.Hidden("form_cropping_X", 0)%>
  <%=Html.Hidden("form_cropping_Y", 0)%>
  <%=Html.Hidden("form_cropping_W", 96)%>
  <%=Html.Hidden("form_cropping_H", 96)%>
  <table>
    <tr>
      <td>
        <div id="form_cropping_Source"  style="width:500px;height:500px;padding:0;margin:5px">
          <img width="500px" height="500px" id="form_cropping_imgSource" />
        </div>
      </td>
      <td style="vertical-align:top;font-weight:bold;text-align:center">
        Preview<br />
        <div style="width:96px;height:96px;overflow:hidden;padding:0;margin:5px;border:1px solid #888">
          <img id="form_cropping_imgPreview" />
        </div>
      </td>
    </tr>
  </table>
</div>