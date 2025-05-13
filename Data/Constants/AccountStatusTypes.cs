namespace Data.Constants
{
    public class AccountStatusTypes
    {
        public const string ACTIVE = "ACTIVE";
        public const string PENDING = "PENDING";
        public const string INACTIVE = "INACTIVE";
        public const string DORMANT = "DORMANT";
        public const string CLOSED = "CLOSED";
        public const string SUSPENDED = "SUSPENDED";
        public const string FROZEN = "FROZEN";
        public const string RESTRICTED = "RESTRICTED";
        public const string DENIED = "DENIED";
        public const string FOR_RE_APPROVAL = "FOR RE-APPROVAL";

        public static List<string> AS_STRING_LIST { get; } = new()
        {
            ACTIVE,
            PENDING,
            INACTIVE,
            DORMANT,
            CLOSED,
            SUSPENDED,
            FROZEN,
            RESTRICTED,
            DENIED,
            FOR_RE_APPROVAL
        };

        public static List<AccountStatusType> AS_ACCOUNT_STATUS_TYPE_LIST { get; } = new()
        {
            new AccountStatusType { AccountStatusTypeName = ACTIVE },
            new AccountStatusType { AccountStatusTypeName = PENDING },
            new AccountStatusType { AccountStatusTypeName = INACTIVE },
            new AccountStatusType { AccountStatusTypeName = DORMANT },
            new AccountStatusType { AccountStatusTypeName = CLOSED },
            new AccountStatusType { AccountStatusTypeName = SUSPENDED },
            new AccountStatusType { AccountStatusTypeName = FROZEN },
            new AccountStatusType { AccountStatusTypeName = RESTRICTED },
            new AccountStatusType { AccountStatusTypeName = DENIED },
            new AccountStatusType { AccountStatusTypeName = FOR_RE_APPROVAL }
        };
    }
}
