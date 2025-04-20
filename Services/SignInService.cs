using Data;
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
        private readonly ClaimsHelperService _claimsHelperService;
        private readonly NexusAuthenticationService _authenticationService;

        /// <summary>
        /// Constructor for the SignInService.
        /// </summary>
        /// <param name="accessor">accessor to httpcontext manage user session and authentication.</param>
        public SignInService(IHttpContextAccessor accessor, NexusAuthenticationService authenticationService)
        {
            _httpContext = accessor.HttpContext!;
            _claimsHelperService = new();
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Signs in the user by creating an authentication ticket and adding the claims.
        /// </summary>
        /// <param name="userAuth">The model containing the user's authentication details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task TrySignInAsync(UserAuth userAuth)
        {

            var principal = _claimsHelperService.CreateClaimsPrincipal(userAuth);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.Now.AddMinutes(30),
                IsPersistent = true
            };

            //  Sign in the user
            await _httpContext.SignInAsync(_claimsHelperService.cookieScheme, principal, authProperties);
            
            return;
        }
    }
}
