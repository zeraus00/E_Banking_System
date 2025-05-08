using Data.Constants;
using Data.Enums;
using Data.Models.Authentication;
using Data.Models.Finance;
using Data.Models.User;
using Exceptions;
using Services.AuthenticationManagement;
using Services.DataManagement;
using System.Security.Claims;
using ViewModels.RoleControlledSessions;
using ViewModels.Sessions;

namespace Services.SessionsManagement
{
    /// <summary>
    /// Class for handling user session.
    /// Starting and ending user sessions.
    /// Storing and retrieving session-specific information.
    /// </summary>
    public class UserSessionService
    {
        private readonly ClaimsHelperService _claimsHelper;
        private readonly DataMaskingService _dataMaskingService;
        private readonly SessionStorageService _sessionStorage;
        private readonly UserDataService _dataService;

        public UserSessionService(
            ClaimsHelperService claimsHelper,
            DataMaskingService dataMaskingService,
            SessionStorageService sessionStorage,
            UserDataService dataService)
        {
            _dataMaskingService = dataMaskingService;
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
                    case (int)RoleTypeIDs.Administrator:
                        await StartAdminSession(principal);
                        break;
                    case (int)RoleTypeIDs.User:
                        await StartUserSession(principal);
                        break;
                    case (int)RoleTypeIDs.Employee:
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
                //UserAuth userAuth = await _dataService.TryGetUserAuthAsync(userAuthId);
                string userEmail = await _dataService.TryGetUserAuthEmail(userAuthId);
                string maskedEmail = _dataMaskingService.MaskEmail(userEmail);

                //  Throws a UserNotFoundException if UserInfo is not found.
                UserInfo userInfo = await _dataService.TryGetUserInfoAsync(userInfoId, includeUserName: true);

                //  Get Accounts linked to the online account of the user.
                List<UserInfoAccount> userAccountLinks = await _dataService
                    .GetUserAccountLinks(userInfoId, includeAccount: true, isLinkedToOnlineAccount: true);
                List<LinkedAccount> userAccountList = new();
                //  Get LinkedAccount list.
                if (userAccountLinks.Any())
                {
                    foreach (var userAccountLink in userAccountLinks)
                    {
                        LinkedAccount accountSession = new()
                        {
                            UserAccessRoleId = userAccountLink.AccessRoleId,
                            AccountId = userAccountLink.AccountId,
                            AccountName = userAccountLink.Account.AccountName,
                            AccountNumber = new DataMaskingService().MaskAccountNumber(userAccountLink.Account.AccountNumber),
                            AccountContactNo = userAccountLink.Account.AccountContactNo,
                            AccountStatusId = userAccountLink.Account.AccountStatusTypeId
                        };
                        userAccountList.Add(accountSession);
                    }
                }

                //  Create UserSession.
                UserSession userSession = new()
                {
                    UserAuthId = userAuthId,
                    UserInfoId = userInfoId,
                    CurrentUserName = await _dataService.GetUserFullName(userInfo) ?? "NAME_NOT_FOUND.",
                    CurrentUserEmail = maskedEmail,
                    CurrentUserContact = userInfo.ContactNumber,
                    LinkedAccountList = userAccountList
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
                UserInfo userInfo = await _dataService.TryGetUserInfoAsync(userInfoId, includeUserName: true);

                /*  GET ADMIN SESSION FIELDS */
                string email = await _dataService.TryGetUserAuthEmail(userAuthId);
                string maskedEmail = _dataMaskingService.MaskEmail(email);
                string fullname = await _dataService.GetUserFullName(userInfo) ?? "NAME_NOT_FOUND";

                /*  ASSIGN ADMIN SESSION FIELDS */
                AdminSession adminSession = new AdminSession()
                {
                    FullName = fullname,
                    Email = maskedEmail
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
            => await _sessionStorage.FetchSessionAsync<UserSession>(SessionSchemes.USER_SESSION);
        /// <summary>
        /// Get current admin session.
        /// </summary>
        /// <returns>The AdminSession object containing the session details.</returns>
        /// <exception cref="SessionNotFoundException">Thrown if no session is found.</exception>
        public async Task<AdminSession> GetAdminSession()
            => await _sessionStorage.FetchSessionAsync<AdminSession>(SessionSchemes.ADMIN_SESSION);

        /// <summary>
        /// Updates the user session. Used after making changes to user controlled sessions.
        /// </summary>
        /// <param name="userSession"></param>
        /// <returns></returns>
        public async Task UpdateUserSession(UserSession userSession)
            => await _sessionStorage.StoreSessionAsync(SessionSchemes.USER_SESSION, userSession);

        /// <summary>
        /// Updates the admin's session. Used after making changes to admin controlled sessions.
        /// </summary>
        /// <param name="adminSession"></param>
        /// <returns></returns>
        public async Task UpdateAdminSession(AdminSession adminSession)
            => await _sessionStorage.StoreSessionAsync(SessionSchemes.ADMIN_SESSION, adminSession);

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
