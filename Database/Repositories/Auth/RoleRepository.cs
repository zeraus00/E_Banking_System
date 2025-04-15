using Data;

namespace Database.Repositories.Auth
{
    /// <summary>
    /// Class for handling CRUD operations in Roles table.
    /// Contains both sync and async versions of each method.
    /// 
    /// NOTE: There is no persistent saving here. You MUST call
    /// DbContext.SaveChanges or DbContext.SaveChangesAsync externally.
    /// </summary>
    public class RoleRepository
    {
        private readonly EBankingContext _context;

        public RoleRepository(EBankingContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Adds a new entry to the roles table.
        /// </summary>
        /// <param name="role"></param>
        public void AddRoleSync(Role role)
        {
            _context.Set<Role>().Add(role);
        }

        /// <summary>
        /// Adds a new entry to the roles table.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task AddRoleAsync(Role role)
        {
            await _context.Set<Role>().AddAsync(role);
        }

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
