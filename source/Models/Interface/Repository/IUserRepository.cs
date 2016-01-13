using System;
using System.Collections.Generic;

using Vauction.Utils;

namespace Vauction.Models
{
  public interface IUserRepository
  {
    User ValidateUser(string login, string password);
    User GetUserActiveAndApproved(string login);
    User GetUserActiveAndApproved(long user_id, string login);
    bool ValidatePasswordForUser(string password, long user_id);
    bool ValidateLogin(string login, long user_id);
    bool ValidateEmail(string email, long user_id);
    void AddOuterSubscription(OuterSubscription os);
    User GetUserByEmail(string email, bool iscache);
    void SubmitChanges();
    bool ActivateOuterSubscription(long outersubscription_id);
    User GetUser(long user_id, bool iscache);
    AddressCard GetAddressCard(long addresscard_id, bool iscache);
    RegisterInfo GetRegisterInfo(long user_id);
    User UpdateUser(RegisterInfo info);
    User AddUser(RegisterInfo info);
    long ConfirmUser(string login);
    bool UpdateEmailSettings(long user_id, bool IsRecievingWeeklySpecials, bool IsRecievingNewsUpdates, bool IsRecievingBidConfirmation, bool IsRecievingOutBidNotice, bool IsRecievingLotSoldNotice, bool IsRecievingLotClosedNotice);
    User SetNewUserPassword(string email);
    User GetUserByConfirmationCode(string code);
    bool UpdatePassword(long user_id, string password);
    User GenerateNewConfirmationCode(string email);
    bool UnsubscribeFromEmail(string email);
    bool UnsubscribeRegisterUser(string email);
    bool UnsubscribeRegisterUser(long user_id);
    bool UnsubscribeFromOuterSubscribtionByID(long id);
    bool SubscribeRegisterUser(User u);
    UserReference GetUserReferences(long userreference_id);
    void TryToUpdateNormalAttempts(User usr);
    void RemoveUserFromCache(long user_id, string email);
  }
}