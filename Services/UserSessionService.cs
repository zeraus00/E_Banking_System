using Data.Constants;
using Data.Enums;
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
        /// Starts a session based on the user's role.
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">
        /// Thrown if required claims (UserAuthId or UserInfoId) are not found in the provided <paramref name="principal"/>.
        /// </exception>
        /// <exception cref="UserNotFoundException">
        /// Thrown if no corresponding <see cref="UserAuth"/> or <see cref="UserInfo"/> is found in the data source.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the user's role id is invalid.
        /// </exception>
        public async Task StartSessionBasedOnRole(ClaimsPrincipal principal)
        {
            try
            {
                int roleId = _claimsHelper.GetRoleId(principal);
                switch (roleId)
                {
                    case (int)RoleTypes.Administrator:
                        await StartAdminSession(principal);
                        break;
                    case (int)RoleTypes.User:
                        await StartUserSession(principal);
                        break;
                    case (int)RoleTypes.Employee:
                        //  start employee session
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("RoleId");
                }
            }
            catch (NullReferenceException)
            {
                //  Handle exception
                throw;
            }
            catch (UserNotFoundException)
            {
                //  Handle exception
                throw;
            }
            catch (ArgumentOutOfRangeException)
            {
                //  Handle exception
                throw;
            }
        }
        /// <summary>
        /// Starts a user session by extracting user authentication and profile information from the provided <see cref="ClaimsPrincipal"/>.
        /// Converts the retrieved data into a <see cref="UserSession"/> model and stores it in session storage.
        /// </summary>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> containing the user's authentication and profile claims.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="NullReferenceException">
        /// Thrown if required claims (UserAuthId or UserInfoId) are not found in the provided <paramref name="principal"/>.
        /// </exception>
        /// <exception cref="UserNotFoundException">
        /// Thrown if no corresponding <see cref="UserAuth"/> or <see cref="UserInfo"/> is found in the data source.
        /// </exception>
        public async Task StartUserSession(ClaimsPrincipal principal)
        {
            try
            {
                //  Get UserAuthId and UserInfoId from claims.
                //  Throws NullReferenceException if claim is not found.
                int userAuthId = _claimsHelper.GetUserAuthId(principal);
                int userInfoId = _claimsHelper.GetUserInfoId(principal);

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
            } 
            catch (NullReferenceException)
            {
                //  Handle Exceptions
                throw;
            }
            catch (UserNotFoundException)
            {
                //  Handle Exceptions
                throw;
            }
        }
        /// <summary>
        /// Starts an admin session by extracting user information from the provided <see cref="ClaimsPrincipal"/>.
        /// </summary>
        /// <param name="principal">The claims principal containing the user's authentication and information claims.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="NullReferenceException">
        /// Thrown if required claims (UserAuthId or UserInfoId) are not found in the provided <paramref name="principal"/>.
        /// </exception>
        /// <exception cref="UserNotFoundException">
        /// Thrown if the user corresponding to the UserAuthId or UserInfoId does not exist.
        /// </exception>
        public async Task StartAdminSession(ClaimsPrincipal principal)
        {
            try
            {
                //  Get UserAuthId and UserInfoId from claims.
                //  Throws NullReferenceException if claims are not found.
                int userAuthId = _claimsHelper.GetUserAuthId(principal);
                int userInfoId = _claimsHelper.GetUserInfoId(principal);

                //  Throws UserNotFoundException if UserAuth is not found.
                UserAuth userAuth = await _dataService.TryGetUserAuthAsync(userAuthId);

                //  Throws UserNotFoundException if UserAuth is not found.
                UserInfo userInfo = await _dataService.TryGetUserInfoAsync(userInfoId, includeUserName: true);

                /*  GET ADMIN SESSION FIELDS */
                string email = userAuth.Email;
                string fullname = await _dataService.GetUserFullName(userInfo) ?? "NAME_NOT_FOUND";

                /*  ASSIGN ADMIN SESSION FIELDS */
                AdminSession adminSession = new AdminSession()
                {
                    FullName = fullname,
                    Email = email
                };

                //  Create a session.
                await _sessionStorage.StoreSessionAsync(SessionSchemes.ADMIN_SESSION, adminSession);
            }
            catch (NullReferenceException)
            {
                //  Handle exceptions
                throw;
            }
            catch (UserNotFoundException)
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
        /// Get current admin session.
        /// </summary>
        /// <returns>The AdminSession object containing the session details.</returns>
        /// <exception cref="SessionNotFoundException">Thrown if no session is found.</exception>
        public async Task<AdminSession> GetAdminSession()
        {
            try
            {
                return (await _sessionStorage.FetchSessionAsync<AdminSession>(SessionSchemes.ADMIN_SESSION));
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
        public async Task<object> GetAdminControlledSession(string sessionScheme, AdminSession? adminSession = null) 
        {
            try
            {
                if (adminSession is null)
                    adminSession = await GetAdminSession();

                var sessions = adminSession.Sessions;
                return sessions.GetValueOrDefault(sessionScheme) ?? throw new AdminControlledSessionNotFound(sessionScheme);
            }
            catch (SessionNotFoundException)
            {
                throw;
            }
            catch (AdminControlledSessionNotFound)
            {
                throw;
            }
        }
        public async Task EndAdminControlledSession(string sessionScheme) => await UpdateAdminControlledSession(sessionScheme, null);
        public async Task UpdateAdminControlledSession(string sessionScheme, object? value)
        {
            try
            {
                AdminSession adminSession = await GetAdminSession();

                adminSession.Sessions[sessionScheme] = value;

                await _sessionStorage.StoreSessionAsync(SessionSchemes.ADMIN_SESSION, adminSession);
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
            await _sessionStorage.DeleteSessionAsync(SessionSchemes.ADMIN_SESSION);
        }
    }
}
