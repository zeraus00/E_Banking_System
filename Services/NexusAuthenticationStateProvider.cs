using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Services
{
    public class NexusAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly ClaimsHelperService _claimsHelperService;
        private AuthenticationState _authenticationState;
        public NexusAuthenticationStateProvider (IHttpContextAccessor accessor, NexusAuthenticationService authenticationService )
        {
            _accessor = accessor;
            _claimsHelperService = new();

            _authenticationState = new AuthenticationState(authenticationService.currentUser);

            authenticationService.UserChanged += (newUser) =>
            {
                _authenticationState = new AuthenticationState(newUser);
                NotifyAuthenticationStateChanged(Task.FromResult(_authenticationState));
            };
        }
        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(_authenticationState);

        public void AuthenticateUser(UserAuth userAuth)
        {
            ClaimsPrincipal principal = _claimsHelperService.CreateClaimsPrincipal(userAuth);
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(principal)));
        }
    }
}
