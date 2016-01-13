<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<CategoryMenuItem>>" %>
<%
  var event_title = ViewData["EventTitleUrl"].ToString();
  var event_id = Convert.ToInt64(ViewData["EventID"]);
  int A, B, count = 0, listcount = 0;
  CategoryMenuItem[] cmi;
  var rootCategories = Model != null ? Model.GroupBy(M => M.MainCategory_ID) : null;
  var tags = ViewData["Tags"] as List<IdTitleCount> ?? new List<IdTitleCount>();
%>

<ul class="past_categories_menu">
  <li>
    <a href="#" style="color: #000; font-size: 14px; font-weight: normal; position: relative; text-decoration: underline;">Categories</a>
    <ul>
      <% if (rootCategories != null)
         { %>
        <%
          foreach (var category in rootCategories)
          {
            if (category.Count() == 0) continue;
            listcount = category.Count();
            cmi = category.ToArray();
            A = (int) Math.Ceiling(listcount*0.5);
            B = category.Count() - A;
        %>
          <li>
            <a href="#" title=""><%= category.FirstOrDefault().LinkParams.MainCategoryTitle %> <em>&nbsp;</em></a>
            <table cellpadding="0" cellspacing="0">
              <colgroup>
                <col width="300px" />
                <%= (listcount > 2) ? "<col width=\"300px\" />" : "<col width=\"1px\" />" %></colgroup>
              <%
                for (var i = 0; i < A; i++)
                {
                  Response.Write("<tr>");
                  Response.Write("<td>");
                  Response.Write(Html.ActionLink(String.Format("{0} [{1}]", cmi[i].LinkParams.CategoryTitle, cmi[i].AuctionCount), "PastCategoriesView", new {controller = "Auction", action = "PastCategoriesView", id = cmi[i].LinkParams.ID, evnt = cmi[i].LinkParams.EventUrl, maincat = cmi[i].LinkParams.MainCategoryUrl, cat = cmi[i].LinkParams.CategoryUrl}));
                  Response.Write("</td>");

                  Response.Write("<td>");
                  if (i + A < listcount)
                  {
                    Response.Write(Html.ActionLink(String.Format("{0} [{1}]", cmi[i + A].LinkParams.CategoryTitle, cmi[i + A].AuctionCount), "PastCategoriesView", new {controller = "Auction", action = "PastCategoriesView", id = cmi[i + A].LinkParams.ID, evnt = cmi[i + A].LinkParams.EventUrl, maincat = cmi[i + A].LinkParams.MainCategoryUrl, cat = cmi[i + A].LinkParams.CategoryUrl}));
                  }
                  else Response.Write("&nbsp;");

                  Response.Write("</td>");
                  Response.Write("</tr>");
                }
              %>
            </table>
          </li>

          <% count++;
             if (count < rootCategories.Count())
             { %>
            <li>
              <hr />
            </li>
          <% } %>
        <% }
          foreach (var tag in tags)
          {
        %>
          <li>
            <hr />
          </li>
          <li>
            <%= Html.ActionLink(tag.Title, "Tcategory", new {controller = "Auction", action = "Tcategory", id = tag.ID, tag = UrlParser.TitleToUrl(tag.Title), EventID = event_id}) %>
          </li>
      <%
          }
         } %>
    </ul>
  </li>
  <li>
    <%= Html.ActionLink("Price Realized", "PriceRealized", new {controller = "Auction", action = "PriceRealized", id = event_id, evnt = event_title, maincat = UrlParser.TitleToUrl("Price Realized")}, new {@style = "color:#000;font-size:14px;font-weight:normal;text-decoration:underline;"}) %>
  </li>
  <li>
    <%= Html.ActionLink("My Bids", "MyBids", new {controller = "Auction", action = "MyBids", id = event_id, evnt = event_title, maincat = UrlParser.TitleToUrl("My Bids")}, new {@style = "color:#000;font-size:14px;font-weight:normal;text-decoration:underline;"}) %>
  </li>
  <li>
    <%= Html.ActionLink("Updates", "PastAuctionUpdates", new {controller = "Auction", action = "PastAuctionUpdates", id = event_id, evnt = event_title, maincat = "Updates"}, new {@style = "color:#000;font-size:14px;font-weight:normal;text-decoration:underline;"}) %>
  </li>
</ul>

<script type="text/javascript">
  $(document).ready(function() {
    $(".past_categories_menu li").hover(function() {
      $(this).children("ul").animate({ opacity: "show", top: "20" }, "fast");
    }, function() {
      $(this).children("ul").animate({ opacity: "hide", top: "30" }, "fast");
    });

    $(".past_categories_menu li ul li").hover(function() {
      $(this).children("a:first").css("background", 'url(\"<%= AppHelper.ImageMenuUrl("bg_sumbenu_selected.png") %>\") repeat-x left').css("color", "#FFF");
      $(this).children("table").show();
    }, function() {
      $(this).children("a:first").css("background", "#e0e0e0").css("color", "#222");
      $(this).children("table").hide();
    });

    $("#aCurrentAuction").next("ul").children("li").hover(function() {
      $(".past_categories_menu").hide();
    }, function() {
      $(".past_categories_menu").show();
    });
    $("#aCurrentAuction").next("ul").children("a").hover(function() {
      $(".past_categories_menu").hide();
    }, function() {
      $(".past_categories_menu").show();
    });
  });
</script>