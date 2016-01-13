using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Vauction.Utils;
using Vauction.Utils.Graphics;
using Vauction.Utils.Lib;
using Vauction.Utils.Helper;
using Vauction.Models.CustomClasses;

namespace Vauction.Models
{
  public class UserRepository : IUserRepository
  {
    private VauctionDataContext dataContext;

    public UserRepository(VauctionDataContext dataContext)
    {
      this.dataContext = dataContext;
    }

    #region get user / administrator, update login info
    //GetUserAdministrator (login, password)
    public User GetUserAdministrator(string login, string password)
    {
      return dataContext.Users.Where(p => p.Login.ToLower() == login.ToLower() && p.Password.ToLower() == password.ToLower() && p.IsConfirmed
              && p.UserStatus_ID == (byte)Consts.UserStatus.Active && (p.UserType_ID == (byte)Consts.UserTypes.Admin || p.UserType_ID == (byte)Consts.UserTypes.Root || p.UserType_ID == (byte)Consts.UserTypes.Specialist || p.UserType_ID == (byte)Consts.UserTypes.Writer || p.UserType_ID == (byte)Consts.UserTypes.SpecialistViewer)).SingleOrDefault();
    }

    //GetUserAdministrator (user_id, login)
    public User GetUserAdministrator(long user_id, string login)
    {
      return dataContext.Users.Where(p => p.ID == user_id && p.Login.ToLower() == login.ToLower() && p.IsConfirmed
              && p.UserStatus_ID == (byte)Consts.UserStatus.Active && (p.UserType_ID == (byte)Consts.UserTypes.Admin || p.UserType_ID == (byte)Consts.UserTypes.Root || p.UserType_ID == (byte)Consts.UserTypes.Specialist || p.UserType_ID == (byte)Consts.UserTypes.Writer || p.UserType_ID == (byte)Consts.UserTypes.SpecialistViewer)).SingleOrDefault();
    }

    //GetUserAdministrator (login)
    public User GetUserAdministrator(string login)
    {
      return dataContext.Users.Where(p => p.Login.ToLower() == login.ToLower() && p.IsConfirmed
              && p.UserStatus_ID == (byte)Consts.UserStatus.Active && (p.UserType_ID == (byte)Consts.UserTypes.Admin || p.UserType_ID == (byte)Consts.UserTypes.Root || p.UserType_ID == (byte)Consts.UserTypes.Specialist || p.UserType_ID == (byte)Consts.UserTypes.Writer || p.UserType_ID == (byte)Consts.UserTypes.SpecialistViewer)).SingleOrDefault();
    }

    //UserLogInSuccessfuly
    public void UserLogInSuccessfuly(IUser usr)
    {
      try
      {
        usr.LastAttempt = DateTime.Now;
        usr.FailedAttempts = 0;
        usr.IP = Consts.UsersIPAddress;
        GeneralRepository.SubmitChanges(dataContext);
      }
      catch (Exception ex)
      {
        Logger.LogException("[UserLogInSuccessfuly(id=" + usr.ID + "]\n", ex);
      }
    }
    #endregion

    #region get users list
    // GetUsersListSearchBox
    public object GetUsersListSearchBox(string sidx, string sord, int page, int rows, byte usersearchtype, long? user_id, string login, string fn, string email, string phone)
    {
      string flname = String.IsNullOrEmpty(fn) ? String.Empty : fn.Replace(" ", "%");
      int? totalrecords = 0;
      int pageindex = (page > 0) ? page - 1 : 0;
      dataContext.CommandTimeout = 600000;
      var res = dataContext.spUser_View_BuyerSellers(usersearchtype, user_id, login, flname, email, phone, sidx.ToLower(), sord == "desc", pageindex, rows, ref totalrecords);
      if (totalrecords.GetValueOrDefault(0) == 0) return new { total = 0, page = page, records = 0 };
      return new
      {
        total = (int)Math.Ceiling((float)totalrecords.GetValueOrDefault(0) / (float)rows),
        page = page,
        records = totalrecords.GetValueOrDefault(0),
        rows = (
            from query in res
            select new
            {
              i = query.User_ID,
              cell = new string[] {
                query.User_ID.ToString(),
                query.Login,              
                query.FLName,
                query.Email,
                query.HomePhone,
                query.CommissionRate_ID.ToString(),
                ((Consts.UserTypes)query.UserType.GetValueOrDefault((byte)Consts.UserTypes.Buyer)).ToString(),
                query.UserType.GetValueOrDefault((byte)Consts.UserTypes.Buyer).ToString(),
                query.UserStatus.GetValueOrDefault(1).ToString()
              }
            }).ToArray()
      };
    }

    // GetBuyersAndHBList
    public object GetBuyersAndHBList(string sidx, string sord, int page, int rows, bool _search, long? User_ID, string Login, string FN, string Email, string Phone)
    {
      byte orderby = 0;
      switch (sidx)
      {
        case "User_ID":
          orderby = (byte)((sord == "desc") ? 0 : 1);
          break;
        case "Login":
          orderby = (byte)((sord == "desc") ? 2 : 3);
          break;
        case "FN":
          orderby = (byte)((sord == "desc") ? 4 : 5);
          break;
        case "Email":
          orderby = (byte)((sord == "desc") ? 6 : 7);
          break;
        case "Phone":
          orderby = (byte)((sord == "desc") ? 8 : 9);
          break;
      }
      dataContext.CommandTimeout = 10 * 60 * 1000;
      List<sp_GetUsersSearchListResult> users = dataContext.sp_GetUsersSearchList(User_ID, Login, FN, Email, Phone, AppHelper.CurrentUser.IsRoot, orderby, 3).ToList();

      int pageIndex = Convert.ToInt32(page) - 1;
      int pageSize = rows;
      int totalRecords = users.Count();
      int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

      users = new List<sp_GetUsersSearchListResult>(users.Skip(pageIndex * pageSize).Take(pageSize));

      var jsonData = new
      {
        total = totalPages,
        page = page,
        records = totalRecords,
        rows = (
            from query in users
            select new
            {
              i = query.User_Id,
              cell = new string[] {
                query.User_Id.ToString(),
                query.Login,              
                query.FN,
                query.Email,
                query.Phone
              }
            }).ToArray()
      };

      users = null;

      return jsonData;
    }
    #endregion

    #region changing user data
    //ChangeUserTypeToSellerBuyer
    public JsonExecuteResult ChangeUserTypeToSellerBuyer(long user_id)
    {
      User user = GetUser(user_id);
      if (user == null) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The user doesn't exist. Operation failed");
      if ((!user.IsBuyer && !user.IsSeller) || user.IsSellerBuyer)
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't change the user type to SellerBuyer.");
      try
      {
        user.UserType_ID = (byte)Consts.UserTypes.SellerBuyer;
        GeneralRepository.SubmitChanges(dataContext);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //ActivatingUserWithoutEmail
    public JsonExecuteResult ActivatingUserWithoutEmail(long user_id)
    {
      User user = GetUser(user_id);
      if (user == null) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The user doesn't exist. Operation failed");
      if (user.UserStatus_ID == (byte)Consts.UserStatus.Active) return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
      try
      {
        user.UserStatus_ID = (byte)Consts.UserStatus.Active;
        user.IsConfirmed = true;
        GeneralRepository.SubmitChanges(dataContext);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }
    #endregion





    //NOT DONE





    //GetUserType
    public byte GetUserTypeID(string type)
    {
      UserType ut = dataContext.UserTypes.Where(U => U.Title.CompareTo(type) == 0).SingleOrDefault();
      return (ut != null) ? ut.ID : (byte)0;
    }

    // GetUser
    public User GetUser(long User_ID)
    {
      return dataContext.Users.SingleOrDefault(U => U.ID == User_ID);
    }

    // GetUser
    public User GetUser(string login)
    {
      return dataContext.Users.SingleOrDefault(u => u.Login.CompareTo(login) == 0);
    }

    //GetUserDataJSON
    public object GetUserDataJSON(long user_id)
    {
      User user = dataContext.Users.SingleOrDefault(U => U.ID == user_id);
      if (user == null) return null;
      string filePath = DiffMethods.SignatureImagesOnDisk(string.Format("userSignature{0}.png", user.ID));
      FileInfo fileInfo = new FileInfo(filePath);
      var jsonData = new
      {
        user_id = user.ID,
        login = user.Login,
        pw = user.PasswordEncrypted,
        usertype = user.UserType_ID,
        status = user.UserStatus_ID,
        commrate = user.CommissionRate_ID,
        email = user.Email,
        datein = user.DateRegistered.ToString(),
        isconfirmed = user.IsConfirmed,
        ismodifyed = user.IsModifyed,
        dph = user.DayPhone,
        eph = user.EveningPhone,
        mph = user.MobilePhone,
        fax = user.Fax,
        tax = user.TaxpayerID,
        ip = user.IP,
        lastattempt = (user.LastAttempt.HasValue) ? user.LastAttempt.Value.ToString() : "",
        failedattempts = (user.FailedAttempts.HasValue) ? user.FailedAttempts.Value : 0,
        rws = user.RecieveWeeklySpecials,
        rnu = user.RecieveNewsUpdates,
        rbc = user.IsRecievingBidConfirmation,
        robn = user.IsRecievingOutBidNotice,
        rlsn = user.IsRecievingLotSoldNotice,
        rlcn = user.IsRecievingLotClosedNotice,
        notes = user.Notes ?? String.Empty,
        isnotseller = user.IsSellerType,
        ispwdisapled = AppHelper.CurrentUser.IsAdmin && (user.IsAdmin || user.IsRoot) && AppHelper.CurrentUser.ID != user.ID,
        ah1 = (user.UserReference_1 != null) ? user.UserReference_1.AuctionHouse : String.Empty,
        pn1 = (user.UserReference_1 != null) ? user.UserReference_1.PhoneNumber : String.Empty,
        lbd1 = (user.UserReference_1 != null) ? user.UserReference_1.LastBidPlaced : String.Empty,
        ah2 = (user.UserReference_2 != null) ? user.UserReference_2.AuctionHouse : String.Empty,
        pn2 = (user.UserReference_2 != null) ? user.UserReference_2.PhoneNumber : String.Empty,
        lbd2 = (user.UserReference_2 != null) ? user.UserReference_2.LastBidPlaced : String.Empty,
        ebid = user.EbayID,
        evf = user.EbayFeedback,
        iscatalog = user.IsCatalog,
        ispostcards = user.IsPostCards,
        b_first = (user.Billing_AddressCard_ID.HasValue) ? user.AddressCard_Billing.FirstName : String.Empty,
        b_middle = (user.Billing_AddressCard_ID.HasValue) ? user.AddressCard_Billing.MiddleName : String.Empty,
        b_last = (user.Billing_AddressCard_ID.HasValue) ? user.AddressCard_Billing.LastName : String.Empty,
        b_comp = (user.Billing_AddressCard_ID.HasValue) ? user.AddressCard_Billing.Company : String.Empty,
        b_addr1 = (user.Billing_AddressCard_ID.HasValue) ? user.AddressCard_Billing.Address1 : String.Empty,
        b_addr2 = (user.Billing_AddressCard_ID.HasValue) ? user.AddressCard_Billing.Address2 : String.Empty,
        b_city = (user.Billing_AddressCard_ID.HasValue) ? user.AddressCard_Billing.City : String.Empty,
        b_state = (user.Billing_AddressCard_ID.HasValue) ? user.AddressCard_Billing.State_ID : 0,
        b_zip = (user.Billing_AddressCard_ID.HasValue) ? user.AddressCard_Billing.Zip : String.Empty,
        b_country = (user.Billing_AddressCard_ID.HasValue) ? user.AddressCard_Billing.Country_ID : 1,
        b_istate = (user.Billing_AddressCard_ID.HasValue) ? user.AddressCard_Billing.InternationalState : String.Empty,
        bls = user.BillingLikeShipping,
        s_first = (user.AddressBillingLikeShipping != null) ? user.AddressBillingLikeShipping.FirstName : String.Empty,
        s_middle = (user.AddressBillingLikeShipping != null) ? user.AddressBillingLikeShipping.MiddleName : String.Empty,
        s_last = (user.AddressBillingLikeShipping != null) ? user.AddressBillingLikeShipping.LastName : String.Empty,
        s_comp = (user.AddressBillingLikeShipping != null) ? user.AddressBillingLikeShipping.Company : String.Empty,
        s_addr1 = (user.AddressBillingLikeShipping != null) ? user.AddressBillingLikeShipping.Address1 : String.Empty,
        s_addr2 = (user.AddressBillingLikeShipping != null) ? user.AddressBillingLikeShipping.Address2 : String.Empty,
        s_city = (user.AddressBillingLikeShipping != null) ? user.AddressBillingLikeShipping.City : String.Empty,
        s_state = (user.AddressBillingLikeShipping != null) ? user.AddressBillingLikeShipping.State_ID : 0,
        s_zip = (user.AddressBillingLikeShipping != null) ? user.AddressBillingLikeShipping.Zip : String.Empty,
        s_country = (user.AddressBillingLikeShipping != null) ? user.AddressBillingLikeShipping.Country_ID : 1,
        s_istate = (user.AddressBillingLikeShipping != null) ? user.AddressBillingLikeShipping.InternationalState : String.Empty,
        showSignature = user.UserType_ID == (byte)Consts.UserTypes.Specialist || user.UserType_ID == (byte)Consts.UserTypes.SpecialistViewer,
        signatureUrl = fileInfo.Exists ? DiffMethods.SignatureImagesWeb(fileInfo.Name) : DiffMethods.PublicImages("blank_image.jpg")
      };
      return jsonData;
    }

    // UsersList
    public IQueryable<User> UsersList()
    {
      return (from A in dataContext.Users
              select A);
    }

    //GetUsersList
    public object GetUsersList(string sidx, string sord, int page, int rows, bool _search, long? user_id, string login, byte? status, byte? usertype, int? commrate_id, string name, string email, DateTime? datein, string dayphone, string eveningphone)
    {
      byte orderby = 0;
      switch (sidx)
      {
        case "Login":
          orderby = (byte)((sord == "desc") ? 2 : 3);
          break;
        case "Name":
          orderby = (byte)((sord == "desc") ? 4 : 5);
          break;
        case "IsConfirmed":
          orderby = (byte)((sord == "desc") ? 6 : 7);
          break;
        case "Status":
          orderby = (byte)((sord == "desc") ? 8 : 9);
          break;
        case "UserType":
          orderby = (byte)((sord == "desc") ? 10 : 11);
          break;
        case "Email":
          orderby = (byte)((sord == "desc") ? 12 : 13);
          break;
        case "DateIN":
          orderby = (byte)((sord == "desc") ? 14 : 15);
          break;
        default:
          orderby = (byte)((sord == "desc") ? 0 : 1);
          break;
      }
      dataContext.CommandTimeout = 10 * 60 * 1000;
      List<sp_GetUsersListResult> users = dataContext.sp_GetUsersList(user_id, login, name, status, usertype, email, datein, commrate_id, dayphone, eveningphone, AppHelper.CurrentUser.IsRoot, orderby).ToList();

      int pageIndex = Convert.ToInt32(page) - 1;
      int pageSize = rows;
      int totalRecords = users.Count();
      int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

      users = new List<sp_GetUsersListResult>(users.Skip(pageIndex * pageSize).Take(pageSize));

      var jsonData = new
      {
        total = totalPages,
        page = page,
        records = totalRecords,
        rows = (
            from query in users
            select new
            {
              i = query.User_ID,
              cell = new string[] {
                query.User_ID.ToString(),
                query.BidderID,
                query.Passw,
                query.UserNameFull,
                query.IsConfirmed.ToString(),
                query.UStatus,
                query.UType,
                query.Email,
                query.DateRegistered.ToString(),
                query.CommRate,                
                query.DayPhone,
                query.EveningPhone,
                query.BillingAddress,
                query.ShippingAddress
              }
            }).ToArray()
      };

      users = null;

      return jsonData;
    }

    // GetBuyers
    public IEnumerable<User> GetBuyers(bool only_active)
    {
      return from U in dataContext.Users
             where (U.UserStatus_ID == (byte)Consts.UserStatus.Active || !only_active)
               && (U.UserType_ID == (byte)Consts.UserTypes.Buyer || U.UserType_ID == (byte)Consts.UserTypes.SellerBuyer)
             orderby U.Login
             select U;
    }





    // GetInvoiceUser
    public object GetInvoiceUser(long id, bool IsUserInvoice)
    {
      List<User> user = (IsUserInvoice) ? (from UI in dataContext.UserInvoices
                                           where UI.ID == id
                                           select UI.User).ToList() : (from UI in dataContext.Consignments
                                                                       where UI.ID == id
                                                                       select UI.User).ToList();

      if (user.Count == 0) return null;

      var jsonData = new
      {
        total = 1,
        page = 1,
        records = 1,
        rows = (from query in user
                select new
                {
                  i = query.ID,
                  cell = new string[] {
                query.ID.ToString(),
                query.Login,
                query.AddressCard_Billing.FirstName,
                query.AddressCard_Billing.LastName,
                query.Email,
                query.AddressCard_Billing.HomePhone,
                query.BillingAddressFull_1,
                query.ShippingAddressFull_1
              }
                }).ToArray()
      };

      return jsonData;
    }

    // GetInvoiceOwners
    public object GetInvoiceOwners(string sidx, string sord, int page, int rows, long userinvoice_id)
    {
      List<Invoice> invoices = ((from UI in dataContext.UserInvoices
                                 join I in dataContext.Invoices on UI.ID equals I.UserInvoices_ID
                                 where UI.ID == userinvoice_id
                                 orderby I.Auction.Priority ascending, I.Auction.ID ascending
                                 select I)).ToList();

      int pageIndex = Convert.ToInt32(page) - 1;
      int pageSize = rows;
      int totalRecords = invoices.Count();
      int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

      invoices = invoices.Skip(pageIndex * pageSize).Take(pageSize).ToList();

      var jsonData = new
      {
        total = totalPages,
        page = page,
        records = totalRecords,
        rows = (
            from query in invoices
            select new
            {
              i = query.Auction.Owner_ID,
              cell = new string[] {
                (query.Auction.Lot.HasValue)?query.Auction.Lot.ToString():String.Empty,
                query.Auction.Title,
                query.Auction.Owner_ID.ToString(),
                query.Auction.User.Login,
                query.Auction.User.UserNameFull,
                query.Auction.User.Email,
                query.Auction.User.AddressCard_Billing.HomePhone,
                query.Auction.User.BillingAddressFull_2,
                query.Auction.User.ShippingAddressFull_2
              }
            }).ToArray()
      };

      invoices = null;

      return jsonData;
    }

    // GetInvoiceBuyers
    public object GetInvoiceBuyers(string sidx, string sord, int page, int rows, long cons_id)
    {
      List<Invoice> invoices = ((from C in dataContext.Consignments
                                 join I in dataContext.Invoices on C.ID equals I.Consignments_ID
                                 join IB in dataContext.Invoices on I.Auction_ID equals IB.Auction_ID
                                 where C.ID == cons_id && IB.UserInvoices_ID.HasValue
                                 orderby I.Auction.ID
                                 select IB)).ToList();

      int pageIndex = Convert.ToInt32(page) - 1;
      int pageSize = rows;
      int totalRecords = invoices.Count();
      int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

      invoices = new List<Invoice>(invoices.Skip(pageIndex * pageSize).Take(pageSize));

      var jsonData = new
      {
        total = totalPages,
        page = page,
        records = totalRecords,
        rows = (
            from query in invoices
            select new
            {
              i = query.User_ID,
              cell = new string[] {
                (query.Auction.Lot.HasValue)?query.Auction.Lot.ToString():String.Empty,
                query.Auction.Title,
                query.User_ID.ToString(),
                query.User.Login,
                query.User.UserNameFull,
                query.User.Email,
                query.User.AddressCard_Billing.HomePhone,
                query.User.BillingAddressFull_2,
                query.User.ShippingAddressFull_2
              }
            }).ToArray()
      };

      invoices = null;

      return jsonData;
    }

    //ValidateLogin
    public bool ValidateLogin(string login, long ID)
    {
      return (from p in dataContext.Users
              where p.Login.ToLower().CompareTo(login.Trim().ToLower()) == 0 && p.ID != ID
              select p).Count() == 0;
    }

    //ValidateEmail
    public bool ValidateEmail(string email, long ID)
    {
      return (from p in dataContext.Users
              where p.Email.ToLower().CompareTo(email.Trim().ToLower()) == 0 && p.ID != ID
              select p).Count() == 0;
    }

    //UpdateUserForm
    public object UpdateUserForm(string user, ModelStateDictionary ModelState, string newSignature)
    {
      long user_id = 0;
      try
      {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        UserRegistration usr = serializer.Deserialize<UserRegistration>(user);

        if (!AppHelper.CurrentUser.IsRoot && AppHelper.CurrentUser.ID != usr.ID && (usr.UserType == (byte)Consts.UserTypes.Admin || usr.UserType == (byte)Consts.UserTypes.Root))
          return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't create/update this user.");

        usr.ConfirmPassword = usr.Password;
        usr.ConfirmEmail = usr.Email;
        if (usr.UserType == (byte)Consts.UserTypes.Buyer || usr.UserType == (byte)Consts.UserTypes.HouseBidder)
        {
          usr.CommissionRate = CommissionRate.DefaultCommission;
        }

        if (usr.UserType == (byte)Consts.UserTypes.Seller || usr.UserType == (byte)Consts.UserTypes.SellerBuyer)
          usr.IsPostCards = true;

        usr.ValidateWithoutConfim(ModelState);

        if (ModelState.IsValid)
        {
          if (!UpdateUser(usr))
            return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The user's information wasn't saved.");
          user_id = usr.ID;
          if (!string.IsNullOrWhiteSpace(newSignature))
          {
            SignatureToImage signature = new SignatureToImage();
            Bitmap signatureImage = signature.SigJsonToImage(newSignature);
            string filePath = DiffMethods.SignatureImagesOnDisk(string.Format("userSignature{0}.png", usr.ID));
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists) fileInfo.Delete();
            signatureImage.Save(filePath);
          }
        }
        else
        {
          ModelState.Remove("user");
          if (usr.BillingLikeShipping)
          {
            KeyValuePair<string, ModelState>[] res = (from M in ModelState where M.Key.Contains("Shipping") select M).ToArray();
            if (res.Count() > 0)
            {
              foreach (KeyValuePair<string, ModelState> r in res)
                ModelState.Remove(r);
            }
          }
          var errors = (from M in ModelState select new { field = M.Key, message = M.Value.Errors.FirstOrDefault().ErrorMessage }).ToArray();
          return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "Please correct the errors and try again.", errors);
        }
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, "", user_id);
    }

    //UpdateUser
    public bool UpdateUser(UserRegistration info)
    {
      bool newUser = info.ID < 1;
      User user = (!newUser) ? GetUser(info.ID) : new User();
      try
      {
        using (TransactionScope ts = new TransactionScope())
        {
          if (newUser) dataContext.Users.InsertOnSubmit(user);
          user.Login = info.Login;
          user.Password = info.Password;
          user.UserType_ID = info.UserType;
          user.UserStatus_ID = info.UserStatus;
          user.CommissionRate_ID = info.CommissionRate;
          user.Email = info.Email;
          user.DateRegistered = (newUser) ? DateTime.Now : user.DateRegistered;
          user.IsConfirmed = info.IsConfirmed;
          user.IsModifyed = info.IsModifyed;
          user.DayPhone = info.DayPhone;
          user.EveningPhone = info.EveningPhone;
          user.MobilePhone = info.MobilePhone;
          user.Fax = info.Fax;
          user.TaxpayerID = info.Tax;
          user.FailedAttempts = info.FaildAttempts;
          user.RecieveWeeklySpecials = info.RecieveWeeklySpecials;
          user.RecieveNewsUpdates = info.RecieveNewsUpdates;
          user.IsRecievingBidConfirmation = info.RecieveBidConfirmation;
          user.IsRecievingOutBidNotice = info.RecieveOutBidNotice;
          user.IsRecievingLotClosedNotice = info.RecieveLotClosedNotice;
          user.IsRecievingLotSoldNotice = info.RecieveLotSoldNotice;
          user.EbayID = info.eBayBidderID;
          user.EbayFeedback = info.eBayFeedback;
          user.Notes = info.Notes;
          user.IsCatalog = info.IsCatalog;
          user.IsPostCards = info.IsPostCards;

          AddressCard ac = (newUser || user.AddressCard_Billing == null) ? new AddressCard() : user.AddressCard_Billing;
          ac.FirstName = info.BillingFirstName;
          ac.MiddleName = info.BillingMiddleName;
          ac.LastName = info.BillingLastName;
          ac.Company = info.BillingCompany;
          ac.Address1 = info.BillingAddress1;
          ac.Address2 = info.BillingAddress2;
          ac.City = info.BillingCity;
          ac.State = info.BillingState;
          ac.State_ID = info.BillingState_ID;
          ac.Zip = info.BillingZip;
          ac.Country_ID = info.BillingCountry;
          ac.InternationalState = info.BillingInternationalState;
          if (newUser || user.AddressCard_Billing == null)
          {
            dataContext.AddressCards.InsertOnSubmit(ac);
            user.AddressCard_Billing = ac;
          }

          user.BillingLikeShipping = info.BillingLikeShipping;

          ac = (newUser || user.AddressCard_Shipping == null) ? new AddressCard() : user.AddressCard_Shipping;
          ac.FirstName = (info.BillingLikeShipping) ? info.BillingFirstName : info.ShippingFirstName;
          ac.MiddleName = (info.BillingLikeShipping) ? info.BillingMiddleName : info.ShippingMiddleName;
          ac.LastName = (info.BillingLikeShipping) ? info.BillingLastName : info.ShippingLastName;
          ac.Company = (info.BillingLikeShipping) ? info.BillingCompany : info.ShippingCompany;
          ac.Address1 = (info.BillingLikeShipping) ? info.BillingAddress1 : info.ShippingAddress1;
          ac.Address2 = (info.BillingLikeShipping) ? info.BillingAddress2 : info.ShippingAddress2;
          ac.City = (info.BillingLikeShipping) ? info.BillingCity : info.ShippingCity;
          ac.State = (info.BillingLikeShipping) ? info.BillingState : info.ShippingState;
          ac.State_ID = (info.BillingLikeShipping) ? info.BillingState_ID : info.ShippingState_ID;
          ac.Zip = (info.BillingLikeShipping) ? info.BillingZip : info.ShippingZip;
          ac.Country_ID = (info.BillingLikeShipping) ? info.BillingCountry : info.ShippingCountry;
          ac.InternationalState = (info.BillingLikeShipping) ? info.BillingInternationalState : info.ShippingInternationalState;
          if (newUser || user.AddressCard_Shipping == null)
          {
            dataContext.AddressCards.InsertOnSubmit(ac);
            user.AddressCard_Shipping = ac;
          }

          UserReference ur = (newUser || user.UserReference_1 == null) ? new UserReference() : user.UserReference_1;
          ur.AuctionHouse = info.AuctionHouse1;
          ur.PhoneNumber = info.PhoneNumber1;
          ur.LastBidPlaced = info.LastBidDate1;
          if (newUser || user.UserReference_1 == null)
          {
            dataContext.UserReferences.InsertOnSubmit(ur);
            user.UserReference_1 = ur;
          }

          ur = (newUser || user.UserReference_2 == null) ? new UserReference() : user.UserReference_2;
          ur.AuctionHouse = info.AuctionHouse2;
          ur.PhoneNumber = info.PhoneNumber2;
          ur.LastBidPlaced = info.LastBidDate2;
          if (newUser || user.UserReference_2 == null)
          {
            dataContext.UserReferences.InsertOnSubmit(ur);
            user.UserReference_2 = ur;
          }

          GeneralRepository.SubmitChanges(dataContext);
          ts.Complete();
          info.ID = user.ID;
        }
      }
      catch (ChangeConflictException cce)
      {
        Logger.LogException(cce);
        throw cce;
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
        throw ex;
      }
      return true;
    }

    //DeleteUser
    public object DeleteUser(long user_id)
    {
      User user = GetUser(user_id);
      if (user == null) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The user doesn't exist. Operation failed");
      if ((!AppHelper.CurrentUser.IsRoot && (user.IsAdmin || user.IsRoot)) || (AppHelper.CurrentUser.ID == user.ID))
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't delete this user.");
      try
      {
        long count = dataContext.Auctions.Where(A => A.Owner_ID == user_id).Count();
        if (count > 0)
          return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't delete this user, because he is an owner of existing auctions in the system. You can set the status for this user as Inactive or delete all his auctions");
        count = dataContext.Consignments.Where(U => U.User_ID == user_id).Count();
        if (count > 0)
          return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't delete this user, because there is the consignor statement(s) for this user in the system. You can set the status for this user as Inactive or delete all the consignor statements.");
        dataContext.Users.DeleteOnSubmit(user);
        GeneralRepository.SubmitChanges(dataContext);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //ActivatingUser
    public JsonExecuteResult ActivatingUser(long user_id)
    {
      User user = GetUser(user_id);
      if (user == null) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The user doesn't exist. Operation failed");
      if (user.UserStatus_ID == (byte)Consts.UserStatus.Active) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The user status is Active already. Operation failed.");
      try
      {
        user.UserStatus_ID = (byte)Consts.UserStatus.Active;
        user.IsConfirmed = true;
        GeneralRepository.SubmitChanges(dataContext);
        Mail.SendApprovalConfirmation(user.Email, user.Login, user.AddressCard_Billing.FirstName, user.AddressCard_Billing.LastName);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }



    //GetSellersForEvent
    public JsonExecuteResult GetSellersForEvent(long event_id)
    {
      List<User> users = dataContext.Auctions.Where(A1 => A1.Event_ID == event_id).Select(A2 => A2.User).Distinct().ToList();
      var jsonData = new
      {
        rows = (
            from U in users
            orderby U.Login ascending
            select new
            {
              val = U.ID,
              txt = String.Format("{0} ({1})", U.Login, U.UserNameFull)
            }).ToArray()
      };
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, "", jsonData);
    }

    //GetBuyersForEvent
    public JsonExecuteResult GetBuyersForEvent(long event_id)
    {
      List<User> users = dataContext.UserInvoices.Where(U => U.Event_ID == event_id).Select(U1 => U1.User).Distinct().ToList();
      var jsonData = new
      {
        rows = (
            from U in users
            orderby U.Login ascending
            select new
            {
              val = U.ID,
              txt = String.Format("{0} ({1})", U.Login, U.UserNameFull)
            }).ToArray()
      };
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, "", jsonData);
    }
  }
}
