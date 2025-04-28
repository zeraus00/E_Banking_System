using Data.Repositories.Auth;
using Data.Repositories.Finance;

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

                    Console.WriteLine("USERAUTH BUILT");

                    await userAuthRepository.AddAsync(userAuth);
                }

                await _context.SaveChangesAsync();
                var account = await accountRepository.GetAccountByIdAsync(1);
                var account2 = await accountRepository.GetAccountByIdAsync(2);
                var userAuth2 = await userAuthRepository.GetUserAuthByIdAsync(2);

                account!.UsersAuth.Add(userAuth2!);
                account2!.UsersAuth.Add(userAuth2!);
                userAuth2!.Accounts.Add(account!);
                userAuth2!.Accounts.Add(account2!);

                await _context.SaveChangesAsync();
            }
        }
    }
}
