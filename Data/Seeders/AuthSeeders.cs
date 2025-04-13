using Database.Builder;

namespace Data.Seeders
{
    public class AuthSeeders
    {
        // Seed your database with initial data here
        // For example, you can add some default users, roles, etc.
        // context.Users.Add(new User { Name = "Admin", Role = "Administrator" });
        // context.SaveChanges();

        public async Task SeedRoles(EBankingContext context)
        {
            if (!await context.Roles.AnyAsync())
            {
                var authBuilder = new AuthBuilder(context);
                var roleNames = new[] { "Administrator", "User", "Employee"};
                foreach (var roleName in roleNames)
                {
                    var role = new RoleBuilder()
                        .WithRoleName(roleName)
                        .Build();
                    await authBuilder.AddRoleAsync(role);
                }
                await authBuilder.SaveChangesAsync();
            }
        }
    }
}
