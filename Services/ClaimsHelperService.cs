using Data.Constants;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Services
{
    /// <summary>
    /// Service for handling claims.
    /// </summary>
    public class ClaimsHelperService
    {
        public string cookieScheme { get; private set; } = CookieAuthenticationDefaults.AuthenticationScheme;
        /// <summary>
        /// An overloaded method that creates a ClaimsPrincipal object from 
        /// the user authentication details and the default authentication scheme.
        /// </summary>
        /// <param name="userAuth">The model containing user authentication details.</param>
        /// <returns>A claims principal object.</returns>
        public ClaimsPrincipal CreateClaimsPrincipal(UserAuth userAuth)
        {
            return CreateClaimsPrincipal(userAuth, cookieScheme);
        }

        /// <summary>
        /// Creates a ClaimsPrincipal object from the user authentication details and
        /// an authentication scheme
        /// </summary>
        /// <param name="userAuth">The model containing user authentication details.</param>
        /// <param name="scheme">The scheme to be used for the claims identity.</param>
        /// <returns>A claims principal object.</returns>
        public ClaimsPrincipal CreateClaimsPrincipal(UserAuth userAuth, string scheme)
        {
            var claims = this.ConvertToClaimsList(userAuth);
            var identity = new ClaimsIdentity(claims, scheme);
            var principal = new ClaimsPrincipal(identity);

            return principal;
        }
        
        /// <summary>
        /// Converts the user authentication information into a list of claims.
        /// </summary>
        /// <param name="userAuth">User authentication details.</param>
        /// <returns>A list of claims representing the user's identity.</returns>
        public List<Claim> ConvertToClaimsList(UserAuth userAuth)
        {
            var roleId = userAuth.RoleId.ToString();
            var userInfoId = (userAuth.UserInfo?.UserInfoId ?? 0).ToString();

            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userAuth.UserAuthId.ToString()),
                new Claim(ClaimTypes.Name, userAuth.Email),
                new Claim(ClaimTypes.Role, userAuth.Role.RoleName),
                new Claim(CustomClaimTypes.ROLE_ID, roleId),
                new Claim(CustomClaimTypes.USERINFO_ID, userInfoId)
            };
        }

        /// <summary>
        /// Retrieves the claim value of the specified type from the claim principal.
        /// </summary>
        /// <param name="user">The ClaimsPrincipal</param>
        /// <param name="claimType">The claim type to be retrieved</param>
        /// <returns>The claim or null if it doesn't exist.</returns>
        public string? GetClaimValue(ClaimsPrincipal user, string claimType)
        {
            return user.FindFirst(c => c.Type == claimType)?.Value;
        }

        /// <summary>
        /// Checks if the user is authenticated.
        /// </summary>
        /// <param name="user">The ClaimsPrincipal.</param>
        /// <returns>True if user is authenticated.</returns>
        public bool IsAuthenticated(ClaimsPrincipal? user)
        {
            return user?.Identity?.IsAuthenticated == true;
        }
    }
}
