namespace Data.Constants
{
    public static class RoleNames
    {
        public const string ADMINISTRATOR = "Administrator";
        public const string USER = "User";
        public const string EMPLOYEE = "Employee";

        public static List<string> AS_STRING_LIST { get; } = new()
        {
            ADMINISTRATOR,
            USER,
            EMPLOYEE
        };

        public static List<Role> AS_ROLE_LIST { get; } = new()
        {
            new Role { RoleName = ADMINISTRATOR },
            new Role { RoleName = USER },
            new Role { RoleName = EMPLOYEE}
        };
    }
}
