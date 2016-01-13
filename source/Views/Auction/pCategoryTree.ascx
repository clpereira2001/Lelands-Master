<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<int>" %>
<div id="categories_tree" >
  <%=ViewData["CatTree"] %>
</div>
<script type="text/javascript">
  $(document).ready(function () {
    $(".category_list_head").click(function () {
      $(this).next(".category_list_body").slideToggle(500);
      $(this).children("#imgCollapse").toggle();
      $(this).children("#imgExpand").toggle();
    });
    $('#dv<%=Model %>').next(".category_list_body").css("display", "block");
    $('#dv<%=Model %>').children("#imgCollapse,#imgExpand").toggle();    
  });
</script>