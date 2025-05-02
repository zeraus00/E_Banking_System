using ViewModels.RoleControlledSessions;

namespace ViewModels.Sessions
{
    public abstract class SessionModel
    {
        public Dictionary<string, RoleControlledSession?> ControlledSessions { get; set; } = new();
    }
}
