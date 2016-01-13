<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div id="categories_tree" >
  <span> Loading...<br /><img src="<%=AppHelper.CompressImage("ajax-loader.gif") %>" alt="" /> </span>  
</div>
<%
  string panel = "#dv" + (ViewData["MainCategory"] != null ? ((ViewData["MainCategory"] is MainCategory) ? (ViewData["MainCategory"] as MainCategory).ID : Convert.ToInt32(ViewData["MainCategory"])) : -1);
 %>
<script type="text/javascript">
  $(document).ready(function () {
    $.get('/Auction/GetCategoriesTree', function (data) {      
      $("#categories_tree").html(data);
      $(".category_list_head").click(function () {
        $(this).next(".category_list_body").slideToggle(500);
        $(this).children("#imgCollapse").toggle();
        $(this).children("#imgExpand").toggle();
      });      
      $('<%=panel %>').next(".category_list_body").css("display", "block");
      $('<%=panel %>').children("#imgCollapse,#imgExpand").toggle();
    });
  });
</script>