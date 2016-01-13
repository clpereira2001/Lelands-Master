<%@ Page Language="C#"  MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>Web Rules / Terms - <%=ConfigurationManager.AppSettings["CompanyName"] %></title>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align:justify; padding:0px 20px 20px 20px; font-size:14px;">
	<div class="page_title">Web Rules / Terms</div>
    <% Event evnt = ViewData["CurrentEvent"] as Event; %>
    <h4>
        <u><%:String.Format("{0} {1}",evnt.Title, (evnt.Title.IndexOf("uction")>0)?String.Empty:"Auction") %> Rules</u>
    </h4>
        You may register for this auction, up to <%:evnt.DateEnd.AddDays(-2).ToLongDateString() %> 8:00PM EST, by filling out <%= Html.ActionLink("our online Registration form", "Register", "Account", new { controller = "Account", action = "Register" }, new { @style = "font-size:14px; font-weight:bold" })%> or by calling 631-244-0077 during our business hours.
    <br /><br />
    <h4>
       <u>VERY IMPORTANT!!!</u>
    </h4>
    This is a catalogue auction. <br>
Bidding can take place on the internet, phone or by fax. Please call to confirm all faxed bids.<br> 
Invoices will be sent by e-mail and regular mail.<br>
        The contact information for this auction is:  <br>
        Lelands.com, 130 Knickerbocker Ave - Suite E, Bohemia, NY 11716<br />
        ~ phone: 631-244-0077 or 631-244-3604 ~ fax: 631-750-5216 ~  <br />

          email: <a href="mailto:info@lelands.com" style="font-weight:bold; font-size:14px;">info@lelands.com</a><br />
          <br />
          All payments will be sent to this   location and all items will   be shipped from this location . All items that have been sold and unpaid after 21 days from the   selling date are subject to being resold without notification.<br>
          Certain works of art in this catalog are being sold subject to a contract of consignment.

    <br /><br />
    <h4>
        <u>THERE IS A 10 MINUTE RULE</u>
    </h4>
        This is a catalogue auction. The final day of bidding is <%:evnt.DateEnd.ToLongDateString() %>. The auction will close to initial bidders at 9pm EST on the closing day. This means that if you have not bid on a lot before 9pm you may not bid on it after 9pm. Therefore, if you are the only bidder on a lot at 9pm you will be instantly declared the winner. Lelands reserves the right to extend this rule for any reason. For all other lots, the auction will continue until the bidding stops on all lots for 10 minutes. Our auctions generally go on well into the night. If you do not want to stay up late, we have the absentee bidding option for your convenience.       
     <br /><br />
    <h4>
        <u>BIDDING INCREMENTS</u>
    </h4>
    Items will be sold to the highest bidder. Bids can be no lower than the &quot reserve &quot or minimum price listed. The current high bid must be raised in increments of at least 10% for items currently at or above $250 or $25 for items currently below $250. Absentee bids are placed in 21% increments. This eliminates the possibility of a bidder occupying 2 consecutive bids. To place a straight bid over the current bid, uncheck  &quot Proxy Bid &quot , whereby the bidding increments go in 10% raises. Lelands reserves the right to accept or reject any or all bids, and/or pull items from the auction. Placing a bid constitutes acceptance of the terms and conditions of sale as outlined here. Lelands' record of final sale shall be conclusive. *An asterisk indicates that the lot has a confidential reserve. The confidential reserve is not higher than the low end of the estimate. To place a straight bid over the current bid, uncheck "Use Proxy Bidding", whereby the bidding increments go in 10% raises.
     <br /><br />
    <h4>
         <u>PLACING ABSENTEE BIDS</u>
    </h4>
        IIf you wish, you may place an Absentee Bid to avoid the need to continually bid. Simply place your maximum bid in the space provided, and Lelands' system will automatically bid for you. Absentee Bids will be executed competitively only as they are bid against by other bidders. For example, assume you have the current high bid at $10,000 and you have placed an absentee (ceiling) bid of $15,000. If another bid comes in ($11,000), your absentee bid will automatically kick in and raise the high bid to $12,100. This will continue until your absentee bid has been outbid. If there are no other bids, you will win the lot for $12,100, plus buyer’s premium. Absentee bids are placed in increments of 21% to avoid the possibility of one bidder occupying two consecutive bids. A 19.5% buyer's premium will be added to all winning bids. All absentee bids will remain confidential.
     <br /><br />
    <h4>
        <u>WINNING BIDS</u>
    </h4>
    A 19.5% buyer’s premium will be added to each piece on your invoice. You are also responsible for all shipping and handling costs, which includes shipping, packing, labor, materials &amp insurance. Our shipping rates are industry competitive. All items will be shipped UPS Ground unless other prior arrangements are made. There is no charge for pickups. Lelands is not responsible for customs, duties, brokerage and/or similar charges on items shipped internationally. New York residents and anyone picking up in New York without a valid New York State resale certificate must pay 8.625% sales tax. This includes buyers with multiple residences. ALL SALES ARE FINAL. NO RETURNS. Note: For items that cannot be shipped by UPS (i.e. too large) clients are responsible to make their own arrangements to have their pieces shipped. We will supply you with the names of at least two shippers that we have had good past dealings with. Lelands takes no responsibility for the use of these services. All sales are final. There will be no returns for any reason. 
     <br /><br />
    <h4>
        <u>PREVIEWS</u>
    </h4>
    Previews are by appointment at our Bohemia, NY location.. Preview appointments are available once you receive an auction catalogue or the auction goes live. However, we will be more than happy to supply you with phone descriptions, additional images and will also send photocopies and faxes upon request (as time permits). Call 631-244-0077
     <br /><br />
    <h4>
        <u>FINAL BIDS AND SHIPPING</u>
    </h4>
    For items that cannot be shipped by UPS (i.e. too large) clients are responsible to make their own arrangements to have their pieces shipped from Lelands' offices. We will supply you with the names of at least two shippers that we have had good past dealings with. Lelands takes no responsibility for the use of these services. There will be no returns unless the piece(s) are not as described.
     <br /><br />
      <i>NOTE</i>: Items offered in our sales are being sold without frames, please consider the frames a bonus when buying a lot. We will not be responsible for any damage to the frame at all. We appreciate your participating in our sales but we auction collectibles and paintings not the frames they are in.
      <br /><br />
    <h4>
        <u>PAYMENT</u>
    </h4>
    Payment is due within 14 days after notification. Failure to pay invoices promptly can result in suspension of your auction privileges. Personal checks are accepted but without prior credit terms, merchandise will be held until your check clears. This takes ten business days. We accept personal checks, money orders, cash and wire transfers. 
     <br /><br />
    <h4>
        <u>NON-PAYMENT BY BUYER</u>
    </h4>
        If payment in full is not received within 30 days after the date of the invoice, Lelands reserves the right, without further notice to the buyer, to (a) charge to the buyer’s credit card any balance remaining on the buyer’s invoice; and/or (b) resell any or all of the items won by buyer. In addition, a service charge of 1.5% per month will be applied to any outstanding balance after 30 days. Buyer agrees to pay all of Lelands’ costs, including attorneys’ fees, incurred in attempting to collect any sums due Lelands from buyer. 
       <br /><br />
    <h4>
        <u>AUTHENTICITY</u>
    </h4>
        Lelands stands by the authenticity of everything it sells for a period of three years from the date of the auction. It is up to the client to verify authenticity within that period of time. However, Lelands and its agents will be the final determinant of the authenticity of each and every piece it sells. We are not bound by the opinion of grading services, outside authenticators, or so-called experts. Letters of authenticity are only available for those pieces where &quot LOA &quot is listed in the catalogue copy. Otherwise, your invoice and that alone will serve as your letter of authenticity. 
     <br /><br />
    <h4>
        <u>RIGHTS AND OBLIGATIONS</u>
    </h4>
        The respective rights and obligations of the parties with respect to the Rules and Conditions of Sale and the conduct of the auction shall be governed and interpreted by the laws of New York State. By bidding at an auction, whether by internet, telephone, present or by agent, the buyer shall be deemed to have consented to the exlusive juristiction of the courts of New York and the Federal courts sitting in such sate. 
     <br /><br />
    <h4>
        <u>ACCEPTANCE OF CONDITIONS</u>
    </h4>
    Placement of a bid in this auction constitutes acceptance of these terms and conditions.
	</div>
</asp:Content>