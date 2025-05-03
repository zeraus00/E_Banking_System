namespace Data.Constants
{
    public class AccessRoleNames
    {
        public const string PRIMARY_OWNER = "PRIMARY OWNER";
        public const string SECONDARY_OWNER = "SECONDARY OWNER";
        public const string BENEFICIARY = "BENEFICIARY";

        public static List<string> AsList = new List<string>
        {
            PRIMARY_OWNER,
            SECONDARY_OWNER,
            BENEFICIARY
        };
    }
}
