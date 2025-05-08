using System.Collections.Generic;

namespace Data.Constants
{
    public class AccessRoles
    {
        public const string PRIMARY_OWNER = "PRIMARY OWNER";
        public const string SECONDARY_OWNER = "SECONDARY OWNER";
        public const string BENEFICIARY = "BENEFICIARY";

        public static List<string> AS_STRING_LIST { get; } = new List<string>
        {
            PRIMARY_OWNER,
            SECONDARY_OWNER,
            BENEFICIARY
        };

        public static List<AccessRole> AS_ACCESS_ROLE_LIST { get; } = new()
        {
            new AccessRole { AccessRoleName = PRIMARY_OWNER },
            new AccessRole { AccessRoleName = SECONDARY_OWNER },
            new AccessRole {  AccessRoleName = BENEFICIARY }
        };
    }
}
