using Data.Constants;
using Data.Enums;
using Microsoft.AspNetCore.Components;
using Services.AuthenticationManagement;
using System.Security.Claims;

namespace Services
{
    /// <summary>
    /// Handles redirection logic.
    /// </summary>
    public class PageRedirectService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly HttpContext? _httpContext;
        private readonly NavigationManager? _navigationManager;
        private readonly ClaimsHelperService _claimsHelper;
        public PageRedirectService(IHttpContextAccessor accessor, NavigationManager navigationManager, ClaimsHelperService claimsHelper)
        {
            _accessor = accessor;
            _httpContext = _accessor.HttpContext;
            _navigationManager = navigationManager;
            _claimsHelper = claimsHelper;
        }
        /// <summary>
        /// Redirects the user to the landing page if not authenticated.
        /// </summary>
        /// <param name="user">The claims principal of the user.</param>
        public void RedirectIfNotAuthenticated(ClaimsPrincipal? user)
        {
            try
            {
                if (!_claimsHelper.IsAuthenticated(user))
                {
                    this.redirectWithNavigationManager();
                }
            } catch (Exception)
            {
                this.redirectWithHttpContext();
            }
        }
        /// <summary>
        /// Redirects the user based on their role.
        /// </summary>
        /// <param name="user"></param>
        public string GetRedirectBasedOnRole(ClaimsPrincipal user) 
        {
            int roleId = Convert.ToInt32(user.FindFirst(c => c.Type == CustomClaimTypes.ROLE_ID)?.Value ?? "0");
            return this.GetRedirectBasedOnRole(roleId);
        }
        /// <summary>
        /// Redirects the user to the specified url using HttpContext.
        /// </summary>
        /// <param name="url">The url to redirect to.</param>
        public void redirectWithHttpContext(string url = "/")
        {
            if (_httpContext != null) _httpContext.Response.Redirect(url);
            else throw new InvalidOperationException("HttpContext unavailable for redirect.");
        }
        /// <summary>
        /// Redirects the user to the specified url using NavigationManager.
        /// </summary>
        /// <param name="url">The url to redirect to.</param>
        public void redirectWithNavigationManager(string url = "")
        {
            if (_navigationManager != null) _navigationManager.NavigateTo(_navigationManager.BaseUri + url.Replace("/", ""), true);
            else throw new InvalidOperationException("NavigationManager unavailable for redirect.");
        }
        /// <summary>
        /// Gets the redirect url based on the role id.
        /// </summary>
        /// <param name="roleId">The role ID of the user.</param>
        private string GetRedirectBasedOnRole(int roleId)
        {
            return roleId switch
            {
                (int)RoleTypeIDs.Administrator => PageRoutes.DASHBOARD,
                (int)RoleTypeIDs.User => PageRoutes.CLIENT_HOME,       //  Client Home
                (int)RoleTypeIDs.Employee => "/",
                _ => PageRoutes.LANDING_PAGE                         //  Landing Page
            };
        }
        /// <summary>
        /// Redirects the unauthenticated user to the Landing page.
        /// Used when the user fails validation or authentication.
        /// </summary>
        public string GetRedirectToLogInPage()
        {
            return PageRoutes.LOG_IN_PAGE;
        }
    }
}
