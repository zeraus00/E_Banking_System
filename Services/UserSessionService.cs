using Data.Constants;
using Data.Models.Authentication;
using Data.Models.Finance;
using Data.Models.User;
using Exceptions;
using System.Security.Claims;
using ViewModels;

namespace Services
{
    /// <summary>
    /// Class for handling user session.
    /// Starting and ending user sessions.
    /// Storing and retrieving session-specific information.
    /// </summary>
    public class UserSessionService
    {
        private readonly ClaimsHelperService _claimsHelper;
        private readonly SessionStorageService _sessionStorage;
        private readonly UserDataService _dataService;

        public UserSessionService(ClaimsHelperService claimsHelper, SessionStorageService sessionStorage, UserDataService dataService)
        {
            _claimsHelper = claimsHelper;
            _sessionStorage = sessionStorage;
            _dataService = dataService;
        }

        /// <summary>
        /// Receives a ClaimsPrincipal parameter and converts it into a UserSession model to then be stored in the session storage.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal object.</param>
        /// <returns></returns>
        /// <exception cref="UserNotFoundException">Thrown if no UserAuth or UserInfo is found.</exception>
        public async Task StartUserSession(ClaimsPrincipal principal)
        {
            try
            {
                int userAuthId = Convert.ToInt32(_claimsHelper.GetClaimValue(principal, CustomClaimTypes.USERAUTH_ID));
                int userInfoId = Convert.ToInt32(_claimsHelper.GetClaimValue(principal, CustomClaimTypes.USERINFO_ID));

                //  Throws a UserNotFoundException if UserAuth is not found.
                UserAuth userAuth = await _dataService.TryGetUserAuthAsync(userAuthId, includeAccounts: true);

                //  Throws a UserNotFoundException if UserInfo is not found.
                UserInfo userInfo = await _dataService.TryGetUserInfoAsync(userInfoId, includeUserName: true);

                /*  GET USER SESSION FIELDS  */

                //  Get full name.
                string fullName = await _dataService.GetUserFullName(userInfo) ?? "NAME_NOT_FOUND.";

                //  Get account list.
                List<Account> accountList = userAuth.Accounts.ToList();

                //  When starting a session, use the first account in the list as the current account.
                Account firstAccount;
                int firstAccountId = 0;
                string firstAccountNumber = string.Empty;
                string firstAccountName = "ACCOUNT_NAME_NOT_FOUND";
                List<int> accountIdList = new();

                if (accountList.Any())
                {
                    firstAccount = accountList[0];
                    firstAccountId = firstAccount.AccountId;
                    firstAccountNumber = firstAccount.AccountNumber;
                    firstAccountName = firstAccount.AccountName;
                    foreach (var account in accountList)
                    {
                        accountIdList.Add(account.AccountId);
                    }
                }

                /*  ASSIGN USER SESSION FIELDS  */
                UserSession userSession = new UserSession
                {
                    CurrentUserName = fullName,
                    CurrentUserEmail = userAuth.Email,
                    CurrentUserContact = userInfo.ContactNumber,
                    ActiveAccountId = firstAccountId,
                    ActiveAccountNumber = firstAccountNumber,
                    ActiveAccountName = firstAccountName,
                    UserAccountIdList = accountIdList
                };

                /*  CREATE A SESSION    */
                await _sessionStorage.StoreSessionAsync(SessionSchemes.USER_SESSION, userSession);
            } catch (UserNotFoundException)
            {
                //  Handle Exceptions
                throw;
            }
        }

        /// <summary>
        /// Get current user session.
        /// </summary>
        /// <returns>The UserSession object containing the session details.</returns>
        /// <exception cref="SessionNotFoundException">Thrown if no session is found.</exception>
        public async Task<UserSession> GetUserSession()
        {
            try
            {
                return (await _sessionStorage.FetchSessionAsync<UserSession>(SessionSchemes.USER_SESSION));
            }
            catch (SessionNotFoundException)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates the session details with the current active account selected by the user.
        /// Fetches the session details from the session storage, updates it, then stores it back.s
        /// </summary>
        /// <param name="accountId">The account id of the new selected active account.</param>
        /// <returns></returns>
        /// <exception cref="SessionNotFoundException">Thrown if no session is found.</exception>
        /// <exception cref="AccountNotFoundException">Thrown if no account is found.</exception>
        public async Task UpdateCurrentAccountInSession(int accountId)
        {
            try
            {
                //  Retrieve user session details from session storage.
                //  Throws SessionNotFoundException if sesion is not found.
                UserSession userSession = await _sessionStorage.FetchSessionAsync<UserSession>(SessionSchemes.USER_SESSION);

                //  Retrieve the account number of the new selected account.
                //  Throws AccountNotFoundException if account is not found.
                Account newActiveAccount = await _dataService.GetAccountAsync(accountId);

                userSession.ActiveAccountId = newActiveAccount.AccountId;
                userSession.ActiveAccountNumber = newActiveAccount.AccountNumber;
                userSession.ActiveAccountName = newActiveAccount.AccountName;

                /*  STORE THE SESSION BACK TO SESSION STORAGE   */
                await _sessionStorage.StoreSessionAsync(SessionSchemes.USER_SESSION, userSession);
            } 
            catch (SessionNotFoundException)
            {
                throw;
            }
            catch (AccountNotFoundException)
            {
                throw;
            }

        }

        /// <summary>
        /// End user session.
        /// </summary>
        /// <returns></returns>
        public async Task EndSession()
        {
            await _sessionStorage.DeleteSessionAsync(SessionSchemes.USER_SESSION);
        }
    }
}
