<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div id="form_event" title="Event" class="dialog_form">

  <%
    var blank = new List<SelectListItem>();
    var dt = DateTime.MinValue;
    var isSelected = true;
    while (dt.CompareTo(DateTime.MinValue.AddDays(1)) < 0)
    {
      blank.Add(new SelectListItem {Text = dt.ToString("hh:mm tt"), Value = dt.TimeOfDay.ToString(), Selected = isSelected});
      isSelected = false;
      dt = dt.AddMinutes(15);
    }
  %>
  <%= Html.Hidden("form_event_Event_ID") %>
  <%= Html.Hidden("form_event_IsCurrent") %>
  <%= Html.Hidden("form_event_type") %>
  <div id="form_event_tabs" style="margin: 0">
    <ul>
      <li><a href="#form_event_tabs-1">General</a></li>
      <li><a href="#form_event_tabs-2">Description</a></li>
      <li><a href="#form_event_tabs-3">Event categories</a></li>
      <li><a href="#form_event_tabs-4">Inactive categories</a></li>
    </ul>
    <!-- General -->
    <div id="form_event_tabs-1">
      <fieldset style="margin: -10px">
        <table id="tblSchema">
          <tr>            
            <td>
              <p>
                <label for="form_event_Title">Title:<em>*</em></label>
                <%= Html.TextBox("form_event_Title") %>
              </p>
            </td>
            <td>
              <p>
                <label for="form_event_Ordinary">Ordinary:<em>*</em></label>
                <%= Html.TextBox("form_event_Ordinary") %>              
              </p>
            </td>
          </tr>
          <tr>            
            <td>
              <p>
                <label for="form_event_StartDate_1">State Date:<em>*</em></label>
                <%= Html.Hidden("form_event_StartDate_1") %>
                <div id="form_event_StartDate" style="font-size: 60%; margin-left: 25px" />
              </p>
            </td>
            <td>
              <p>
                <label for="form_event_EndDate_1">End Date:<em>*</em></label>
                <%= Html.Hidden("form_event_EndDate_1") %>
                <div id="form_event_EndDate" style="font-size: 60%; margin-left: 25px" />
              </p>         
            </td>       
          </tr>
          <tr>            
            <td>          
              <p>
                <label for="form_event_StartDate_2">Start Time:<em>*</em></label>
                <%= Html.DropDownList("form_event_StartDate_2", blank) %>
              </p>
            </td>
            <td>
              <p>
                <label for="form_event_EndDate_2">End Time:<em>*</em></label>
                <%= Html.DropDownList("form_event_EndDate_2", blank) %>            
              </p>          
            </td>       
          </tr>
              
          <tr>        
            <td>
              <p>
                <label for="form_event_BFee">Buyer Fee:<em>*</em></label>
                <%= Html.TextBox("form_event_BFee") %>                
              </p>
            </td>
            <td>
              <p>
                <label for="form_event_CloseStep">Close Step:<em>*</em></label>
                <%= Html.TextBox("form_event_CloseStep") %>                
              </p>
            </td>       
          </tr>
          <tr>
            <td>
              <p>
                <label for="form_event_IsViewable" class="inline">Is Viewable:</label>
                <%= Html.CheckBox("form_event_IsViewable") %>                
              </p>
            </td>        
            <td>
              <p>
                <label for="form_event_IsClickable" class="inline">Is Clickable:</label>
                <%= Html.CheckBox("form_event_IsClickable") %>                
              </p>
            </td>        
          </tr> 
        </table>  
      </fieldset>    
    </div>
    <!-- Description -->
    <div id="form_event_tabs-2">
      <fieldset style="margin: -10px">
        <%= Html.TextArea("form_event_Description", "", new {@style = "height:410px;margin:-10px -5px -10px -5px;width:450px;"}) %>          
      </fieldset>
    </div>
    <!-- Event categories -->
    <div id="form_event_tabs-3">
      <fieldset style="margin: -10px">	      	        
        <div id="form_event_CategoriesMap" style="height: 440px; margin: -10px -5px -10px -5px; overflow: auto;" ></div>	       
      </fieldset>        
    </div>
    <!-- Inactive categories -->
    <div id="form_event_tabs-4">
      <fieldset style="margin: -10px">	      	        
        <table id="c_list"></table>
      </fieldset>
    </div>
  </div>
</div>