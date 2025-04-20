using System.Security.Claims;

namespace Services
{
    public class NexusAuthenticationService
    {
        //private readonly IHttpContextAccessor _accessor;
        //private readonly HttpContext? _context;

        public event Action<ClaimsPrincipal>? UserChanged;
        private ClaimsPrincipal? _currentUser;

        public ClaimsPrincipal currentUser
        {
            get { return _currentUser ?? /*_context?.User ??*/ new ClaimsPrincipal(); }
            set
            {
                _currentUser = value;
                if (UserChanged != null)
                {
                    UserChanged(_currentUser);
                }
            }
        }

        //public NexusAuthenticationService(IHttpContextAccessor accessor)
        //{
        //    _accessor = accessor;
        //    _context = accessor.HttpContext;
        //}
    }
}
