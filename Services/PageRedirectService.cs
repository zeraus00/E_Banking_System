using Data.Enums;
using Microsoft.AspNetCore.Components;
namespace Services
{
    public class PageRedirectService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly HttpContext? _httpContext;
        private readonly NavigationManager? _navigationManager;

        public PageRedirectService(IHttpContextAccessor accessor, NavigationManager navigationManager)
        {
            _accessor = accessor;
            _httpContext = _accessor.HttpContext;
            _navigationManager = navigationManager;
        }

        /// <summary>
        /// Redirects the user to the specified url using HttpContext.
        /// </summary>
        /// <param name="url">The url to redirect to.</param>
        public void redirectWithHttpContext(string url)
        {
            if (_httpContext != null) _httpContext.Response.Redirect(url);
            else Console.Write("HttpContext unavailable for redirect.");
        }

        /// <summary>
        /// Redirects the user to the specified url using NavigationManager.
        /// </summary>
        /// <param name="url">The url to redirect to.</param>
        public void redirectWithNavigationManager(string url)
        {
            if (_navigationManager != null) _navigationManager.NavigateTo(_navigationManager.BaseUri + url, true);
            else Console.WriteLine("NavigationManager unavailable for redirect.");
        }

        /// <summary>
        /// Redirects the user based on their role id.
        /// </summary>
        /// <param name="roleId">The role ID of the user.</param>
        public string GetRedirectBasedOnRole(int roleId)
        {
            return roleId switch
            {
                (int)RoleTypes.Administrator => "/",
                (int)RoleTypes.User => "/Client_home",
                (int)RoleTypes.Employee => "/",
                _ => "/Login_page"
            };
        }

        /// <summary>
        /// Redirects the unauthenticated user to the Landing page.
        /// Used when the user fails validation or authentication.
        /// </summary>
        public string GetRedirectToLogInPage()
        {
            return "/Login_page";
        }
    }
}
