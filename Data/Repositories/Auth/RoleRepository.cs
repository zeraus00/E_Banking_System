namespace Data.Repositories.Auth
{
    /// <summary>
    /// Class for handling CRUD operations in Roles table.
    /// Contains both sync and async versions of each method.
    /// </summary>
    public class RoleRepository : Repository
    {
        public RoleRepository(EBankingContext context) : base(context) { }

    }
    /// <summary>
    /// Builder class for Role
    /// Methods for setting properties of Role
    /// </summary>
    public class RoleBuilder
    {
        private string _roleName = string.Empty;

        public RoleBuilder WithRoleName(string roleName)
        {
            _roleName = roleName.Trim();
            return this;
        }

        /// <summary>
        /// Builds the Role object with the specified properties
        /// </summary>
        /// <returns> Role Object </returns>
        public Role Build()
        {
            return new Role
            {
                RoleName = _roleName
            };
        }
    }
}
