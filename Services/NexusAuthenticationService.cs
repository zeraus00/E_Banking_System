using System.Security.Claims;

namespace Services
{
    public class NexusAuthenticationService
    {
        public event Action<ClaimsPrincipal>? UserChanged;
        private ClaimsPrincipal? _currentUser;

        public ClaimsPrincipal currentUser
        {
            get { return _currentUser ?? new(); }
            set
            {
                currentUser = value;
                if (UserChanged != null)
                {
                    UserChanged(currentUser);
                }
            }
        }
    }
}
