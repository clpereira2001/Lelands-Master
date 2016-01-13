<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AuctionDetail>" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>
<%
  AuctionUserInfo aui = (ViewData["AuctionUserInfo"] as AuctionUserInfo) ?? new AuctionUserInfo();
  BiddingResult bidresult = ViewData["BiddingResult"] as BiddingResult;  
  List<SelectListItem> BidAmountItems = new List<SelectListItem>();
  List<SelectListItem> BidAmountItemsProxy = new List<SelectListItem>();
  List<SelectListItem> temp = new List<SelectListItem>();
  decimal MinBid = bidresult.MinBid + (bidresult.WinningBid != null ? bidresult.Increment : 0);
  decimal StartBid = MinBid;
 
  for (int i = 0; i < Consts.DropDownSize; i++)
  {
    SelectListItem sli = new SelectListItem {Text = StartBid.GetCurrency(), Value = StartBid.GetPrice().ToString()};
    BidAmountItems.Add(sli);
    temp.Add(sli);
    StartBid += Consts.GetIncrement(StartBid, false);
  }

  SessionUser cuser = AppHelper.CurrentUser;
  if (cuser!=null &&  bidresult.WinningBid != null && bidresult.WinningBid.User_ID == cuser.ID && Math.Abs(bidresult.WinningBid.Amount-bidresult.UsersTopBid.Amount)<1) 
  {
    for (int i = 0; i < temp.Count;i++)
    {
      decimal value = Convert.ToDecimal(temp[i].Value);
      if (value>= bidresult.UsersTopBid.MaxBid + Consts.GetIncrement(bidresult.UsersTopBid.MaxBid, true)) break;
      temp.RemoveAt(i);
      i--;
    }
    //temp.RemoveRange(0, 1);
  }
  for (int i = 0; i < temp.Count; i+=2) BidAmountItemsProxy.Add(temp[i]);
%>

<div style="font-size:12px">
  <table cellspacing="0" cellpadding="0" border="0" id="bid" style="margin-bottom: 0px !important;table-layout:fixed;">
    <colgroup><col width="450px" /><col width="450px" /></colgroup>
    <% if (cuser != null)
       {%>
        <% using (Html.BeginForm())
           { %>
        <%=Html.AntiForgeryToken(Consts.AntiForgeryToken)%>
      <tr>         
        <td class="bordered" style="border-top-width:0px; border-left-width:0px; border-bottom-width:0px; border-right: 1px solid #ccc">
        <%= Html.Hidden("id", Model.LinkParams.ID)%>
        <%= Html.Hidden("BidAmount", MinBid.GetPrice())%>
        <span style="vertical-align:bottom">
             <% Response.Write(!aui.IsInWatchList ? Html.ActionLink("Add this item to your Watch List", "AddItemToWatchList", "Account", new { controller = "Account", action = "AddItemToWatchList", id = Model.LinkParams.ID }, new { @style = "font-size:14px" }).ToHtmlString() : ("You are currenlty watching this lot in your " + Html.ActionLink("WATCH LIST", "WatchBid", "Account", new { controller = "Account", action = "WatchBid" }, new { @style = "font-size:14px;text-decoration:underline" }).ToHtmlString()));%>
        </span><br /><br />
        <span class="descr_bidding_param">Minimum Bid:</span><span id="tbMinBid"><%=MinBid.GetCurrency()%></span><br />
        <span class="descr_bidding_param">Use Proxy Bidding:</span>
          <%= Html.CheckBox("ProxyBidding", true)%>
        <br />
        <span class="descr_bidding_param">Bid Amount:</span>          
          <%= Html.DropDownList("ba1", BidAmountItemsProxy, new { @style = "width:100px" })%>
          <%= Html.DropDownList("ba2", BidAmountItems, new { @style = "width:100px" })%>
          <script type="text/javascript">$("#ba2").hide(); $("#ProxyBidding").attr("checked", "true");</script>
          <br />
      </td>    
      <td style="vertical-align:top;padding:10px 20px" rowspan="2">
          <b>How to bid:</b><br />
          1. Select your maximum bid amount<br />
          2. Preview your bid<br />
          3. Submit your final bid
      </td>
    </tr>
    <tr>
      <td class="bordered" style="border-top-width:0px; border-left-width:0px; border-bottom-width:0px;border-right: 1px solid #ccc">
      <%if (cuser.IsBidder && cuser.ID != Model.Owner_ID)
        {%>
            <button type="submit" style="border: 0 none; width: 115px !important; display: block; margin-left:110px;" class="cssbutton small white">
              <span>Preview Bid</span>
            </button>
          <%}
        else
        {%><center>
        <%if (cuser.IsAdmin)
          {%>        
          <span style="padding:5px;color:#008000;border:dotted 1px #008000; margin:5px">Administrator can't place bid on the items.</span><br /><br />
        <%}
          else if (cuser.IsSeller)
          {%>        
          <span style="padding:5px;color:#008000;border:dotted 1px #008000; margin:5px">Seller can't place bid on the items.</span><br /><br />
        <%}
          else if (cuser.IsSellerBuyer && cuser.ID == Model.Owner_ID)
          {%>        
          <span style="padding:5px;color:#008000;border:dotted 1px #008000; margin:5px">You can't place bid on your own items.</span><br /><br />
        <%} %> 
        </center>
        <%}%>          
      </td>
    </tr>
    <%}
       } %>
  </table>
</div>

<script type="text/javascript">
  function InitBiddingDropDowns(IsProxy) {
    if (!IsProxy) {
      $("#ba1").hide(); $("#ba2").show(); $("#ba2").change();
    } else {
      $("#ba2").hide(); $("#ba1").show(); $("#ba1").change();
    }
  }
  $("#ProxyBidding").click(function () {
    InitBiddingDropDowns($("#ProxyBidding").attr("checked"));
  });
  InitBiddingDropDowns(true);
  $("#ba1").change(function () {
    $("#BidAmount").attr("value", $("#ba1").val());
  });
  $("#ba2").change(function () {
    $("#BidAmount").attr("value", $("#ba2").val());
  });
  $("#ba1").change();  
 </script>