namespace Data.Constants
{
    public class AccountStatusTypeNames
    {
        public const string ACTIVE = "ACTIVE";
        public const string PENDING = "PENDING";
        public const string INACTIVE = "INACTIVE";
        public const string DORMANT = "DORMANT";
        public const string CLOSED = "CLOSED";
        public const string SUSPENDED = "SUSPENDED";
        public const string FROZEN = "FROZEN";
        public const string RESTRICTED = "RESTRICTED";

        public static readonly List<string> AccountStatusTypeList = new()
        {
            ACTIVE,
            PENDING,
            INACTIVE,
            DORMANT,
            CLOSED,
            SUSPENDED,
            FROZEN,
            RESTRICTED
        };
    }
}
