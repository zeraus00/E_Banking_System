using Data;
using Data.Constants;
using Data.Enums;
using Data.Models.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;


namespace Services
{
    /// <summary>
    /// Service for handling sign in and authentication.
    /// </summary>
    public class SignInService
    {
        private readonly HttpContext _httpContext;
        string _cookieScheme;

        /// <summary>
        /// Constructor for the SignInService.
        /// </summary>
        /// <param name="httpContext">HttpContext to manage user session and authentication.</param>
        public SignInService(IHttpContextAccessor accessor)
        {
            _httpContext = accessor.HttpContext!;
            _cookieScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }

        /// <summary>
        /// Converts the user authentication information into a list of claims.
        /// </summary>
        /// <param name="userAuth">User authentication details.</param>
        /// <returns>A list of claims representing the user's identity.</returns>
        public List<Claim> ConvertToClaimsList(UserAuth userAuth)
        {
            var roleId = userAuth.RoleId.ToString();
            var accountId = (userAuth.AccountId ?? 0).ToString();
            var userInfoId = (userAuth.UserInfoId ?? 0).ToString();

            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userAuth.UserAuthId.ToString()),
                new Claim(ClaimTypes.Name, userAuth.Email),
                new Claim(ClaimTypes.Role, userAuth.Role.RoleName),
                new Claim(CustomClaimTypes.RoleId, roleId),
                new Claim(CustomClaimTypes.AccountId, accountId),
                new Claim(CustomClaimTypes.UserInfoId, userInfoId)
            };
        }

        /// <summary>
        /// Signs in the user by creating an authentication ticket and adding the claims.
        /// </summary>
        /// <param name="claims">The list of claims representing the user's identity.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task TrySignInAsync(List<Claim> claims)
        {

            var identity = new ClaimsIdentity(claims, _cookieScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.Now.AddMinutes(30),
                IsPersistent = true
            };

            //  Sign in the user
            await _httpContext.SignInAsync(_cookieScheme, principal, authProperties);
            return;
        }

        /// <summary>
        /// Redirects the user based on their role id.
        /// </summary>
        /// <param name="roleId">The role ID of the user.</param>
        public string RedirectBasedOnRole(int roleId)
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
        public string RedirectToLogInPage()
        {
            return "/Login_page";
        }
    }
}
