using Data.Constants;
using Data.Repositories.Auth;
using Data.Repositories.Finance;
using Helpers;

namespace Data.Seeders
{
    public class AuthSeeder
    {
        // Seed your database with initial data here
        // For example, you can add some default users, roles, etc.
        // context.Users.Add(new User { Name = "Admin", Role = "Administrator" });
        // context.SaveChanges();

        private readonly EBankingContext _context;

        public AuthSeeder(EBankingContext context) {
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
                    await roleRepository.AddAsync(role);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task SeedUsersAuth()
        {
            if (!await _context.UsersAuth.AnyAsync())
            {
                var accountRepository = new AccountRepository(_context);
                var userAuthRepository = new UserAuthRepository(_context);
                var userAuthBuilder = new UserAuthBuilder();

                var users = new List<(int roleId, string userName, string email, string password)>
                {
                    (1, "nexusAdmin", "nexusAdmin@gmail.com", "nexusAdmin123"),
                    (2, "bogartDelaMon", "bogartDelaMonJr@gmail.com", "bogartDelaMonJr123"),
                    (3, "employee", "employee@gmail.com", "Employee123")
                };



                foreach (var (roleId, userName, email, password) in users)
                {
                    userAuthBuilder
                        .WithRoleId(roleId)
                        .WithUserName(userName)
                        .WithEmail(email)
                        .WithPassword(BcryptHelper.HashPassword(password.Trim()));

                    var userAuth = userAuthBuilder.Build();

                    Console.WriteLine("USERAUTH BUILT");

                    await userAuthRepository.AddAsync(userAuth);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task SeedAccessRoles()
        {
            if (!await _context.AccessRoles.AnyAsync())
            {
                foreach(var roleName in AccessRoles.AS_STRING_LIST)
                {
                    AccessRole accessRole = new AccessRole();
                    accessRole.AccessRoleName = roleName;
                    await _context.AccessRoles.AddAsync(accessRole);
                }

                await _context.SaveChangesAsync();
            }    
        }
    }
}
