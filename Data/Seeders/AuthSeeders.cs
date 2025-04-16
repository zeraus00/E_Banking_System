using Data.Repositories.Auth;

namespace Data.Seeders
{
    public class AuthSeeders
    {
        // Seed your database with initial data here
        // For example, you can add some default users, roles, etc.
        // context.Users.Add(new User { Name = "Admin", Role = "Administrator" });
        // context.SaveChanges();

        private readonly EBankingContext _context;

        public AuthSeeders(EBankingContext context) {
            _context = context;
        }
        public async Task SeedRoles()
        {
            if (!await _context.Roles.AnyAsync())
            {
                var roleRepository = new RoleRepository(_context);
                var roleNames = new[] { "Administrator", "User", "Employee"};
                foreach (var roleName in roleNames)
                {
                    var role = new RoleBuilder()
                        .WithRoleName(roleName)
                        .Build();
                    await roleRepository.AddRoleAsync(role);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task SeedUsersAuth()
        {
            if (!await _context.UsersAuth.AnyAsync())
            {
                var userAuthRepository = new UserAuthRepository(_context);
                var userAuthBuilder = new UserAuthBuilder();

                var users = new List<(int roleId, string userName, string email, string password)>
                {
                    (1, "admin", "admin@gmail.com", "admin123"),
                    (2, "user", "user@gmail.com", "user123"),
                    (3, "employee", "employee@gmail.com", "employee123")
                };

                foreach (var (roleId, userName, email, password) in users)
                {
                    userAuthBuilder
                        .WithRoleId(roleId)
                        .WithUserName(userName)
                        .WithEmail(email)
                        .WithPassword(password);
                    var userAuth = userAuthBuilder.Build();
                    await userAuthRepository.AddUserAuthAsync(userAuth);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
