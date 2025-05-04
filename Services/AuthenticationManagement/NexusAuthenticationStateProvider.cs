using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Services.AuthenticationManagement
{
    public class NexusAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ClaimsHelperService _claimsHelper;
        private AuthenticationState _authenticationState;
        public NexusAuthenticationStateProvider(ClaimsHelperService claimsHelper, NexusAuthenticationService authenticationService)
        {
            _claimsHelper = claimsHelper;



            authenticationService.UserChanged += (newUser) =>
            {
                _authenticationState = new AuthenticationState(newUser);
                NotifyAuthenticationStateChanged(Task.FromResult(_authenticationState));
            };

            _authenticationState = new AuthenticationState(authenticationService.currentUser);
        }
        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(_authenticationState);

        public void AuthenticateUser(UserAuth userAuth)
        {
            ClaimsPrincipal principal = _claimsHelper.CreateClaimsPrincipal(userAuth);
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(principal)));
        }
    }
}
