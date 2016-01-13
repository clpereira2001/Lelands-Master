<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AuctionDetail>" %>

<asp:Content ID="cH" ContentPlaceHolderID="HTMLhead" runat="server">
    <title><%: String.Format("Lot {0}. {1} ~ {2} - {3}", Model.Lot, Model.Title, Model.LinkParams.EventTitle, Consts.CompanyTitleName) %></title>
    <link href="/public/css/auctiondetail.css" rel="stylesheet" />
    <script src="/public/scripts/jquery-1.8.3.min.js" type="text/javascript"> </script>
    <script src="/public/scripts/jquery.royalslider.min.js" type="text/javascript"> </script>
    <link href="/public/css/royalslider/royalslider.css" rel="stylesheet" />
    <link href="/public/css/royalslider/skins/minimal-white/rs-minimal-white.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="cM" ContentPlaceHolderID="MainContent" runat="server">

    <div class="content_new">
        <div class="topPanel">
            <div class="breadcrumb">
                <%= Html.ActionLink(Model.LinkParams.EventTitle, "Index", new {controller = "Sales", action = "Index"}, new {@style = "font-size:14px"}) %>
                &nbsp;>&nbsp;      
                <span style="font-size: 14px">
                    <%= Model.Title %>
                </span>
        
            </div>
            <div class="navigation">
                <% if (Model.PrevAuction != null)
                   { %>
                    <%= Html.ActionLink("< previous lot", "Details", new {controller = "Sales", action = "Details", id = Model.PrevAuction.ID, evnt = Model.PrevAuction.EventUrl, maincat = Model.PrevAuction.MainCategoryUrl, cat = Model.PrevAuction.CategoryUrl, lot = Model.PrevAuction.LotTitleUrl}) %>
                <% } %>
                <%= (Model.PrevAuction != null && Model.NextAuction != null) ? "&nbsp;|&nbsp" : String.Empty %>
                <% if (Model.NextAuction != null)
                   { %>
                    <%= Html.ActionLink("next lot >", "Details", new {controller = "Sales", action = "Details", id = Model.NextAuction.ID, evnt = Model.NextAuction.EventUrl, maincat = Model.NextAuction.MainCategoryUrl, cat = Model.NextAuction.CategoryUrl, lot = Model.NextAuction.LotTitleUrl}) %>
                <% } %>
            </div>
        </div>
    
        <div>
            <div class="leftcolumn">
                <% Html.RenderAction("pAuctionDetailImages", "Auction", new {auction_id = Model.LinkParams.ID}); %> 
            </div>

            <div class="rightcolumn">
                <span class="lot_title">Lot <%= Model.Lot %>: <span style="text-transform: uppercase"><%= Model.Title %></span></span>
                <div class="details">
                    <div class="leftdetails">
                        <div>
                            <%
                                if (!string.IsNullOrWhiteSpace(Model.Estimate))
                                {
                                    decimal est;
                                    decimal.TryParse(Model.Estimate, out est);
                            %>
                                <div class="span2">Estimate:</div>
                                <div class="span1"><%= est > 0 ? est.GetCurrency() : Model.Estimate %></div>
                            <%
                                }
                                else
                                { %>
                                <div class="span2">Price:</div>
                                <div class="span1"><%= Model.Price.GetCurrency() %></div>
                            <% } %>
                        </div>
                        <div class="clear"></div>
                    </div>
                </div>
                <% foreach (var collection in Model.Collections)
                   {
                %>
                    <div class="itemdescription">
                        <strong><%= collection.Title %></strong>
                    </div>
                    <div class="itemdescr">
                        <%= collection.Description %>
                    </div>
                <%
                   } %>
                <div class="itemdescription">
                    <strong>Description</strong>
                    <span class="productSocial addthis_toolbox addthis_default_style addthis_16x16_style">
                        <a class="addthis_button_email"></a>
                        <a class="addthis_button_facebook"></a>
                        <a class="addthis_button_twitter"></a>
                        <a class="addthis_button_print"></a>
                        <a class="addthis_button_pinterest_pinit"></a>
                    </span>
                </div>
                <% if (!String.IsNullOrEmpty(Model.Addendum))
                   { %>
                    <div class="pay_notice"><strong>UPDATE:</strong>&nbsp;<%= Model.Addendum %></div>
                <% } %>

                <div class="itemdescr">
                    <% Html.RenderPartial("~/Views/Auction/pAuctionDetailAdds.ascx", Model); %>
                    <%= Model.Description %>
                </div>
            </div>
        </div>
        <div class="clear">
            <center>
                <% if (Model.PrevAuction != null)
                   { %>
                    <%= Html.ActionLink("< previous lot", "Details", new {controller = "Sales", action = "Details", id = Model.PrevAuction.ID, evnt = Model.PrevAuction.EventUrl, maincat = Model.PrevAuction.MainCategoryUrl, cat = Model.PrevAuction.CategoryUrl, lot = Model.PrevAuction.LotTitleUrl}) %>
                <% } %>
                <%= (Model.PrevAuction != null && Model.NextAuction != null) ? "&nbsp;|&nbsp" : String.Empty %>
                <% if (Model.NextAuction != null)
                   { %>
                    <%= Html.ActionLink("next lot >", "Details", new {controller = "Sales", action = "Details", id = Model.NextAuction.ID, evnt = Model.NextAuction.EventUrl, maincat = Model.NextAuction.MainCategoryUrl, cat = Model.NextAuction.CategoryUrl, lot = Model.NextAuction.LotTitleUrl}) %>
                <% } %>
            </center> 
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphJS" ID="cJ" runat="server">
    <script type="text/javascript"> var addthis_config = { "data_track_addressbar": false }; </script>
    <script type="text/javascript" src="//s7.addthis.com/js/300/addthis_widget.js#pubid=ra-51a5d4c65778a843"> </script>
</asp:Content>