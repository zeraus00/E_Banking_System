using E_BankingSystem.Components;
using Data;
using Data.Seeders;
using Data.Seeders.User;
using Data.Seeders.Finance;
using Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// DbContext
builder.Services.AddDbContext<EBankingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqloptions => sqloptions.CommandTimeout(100)));


builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// Authentication Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<LogInService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_token";
        options.LoginPath = "/Login_page";
        options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
        options.AccessDeniedPath = "/"; // add access denied page here.
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Apply any pending migrations and seed the database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EBankingContext>();

    //dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
    // Apply migrations (this will create or update the database schema)
    // dbContext.Database.Migrate();

    // Seed data (this ensures that only new data is added)
    await SeedData(dbContext);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

//  Middlewares for authentication and authorization.
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


static async Task SeedData(EBankingContext context)
{


    AuthSeeder authSeeders = new AuthSeeder(context);
    AccountSeeder accountSeeders = new AccountSeeder(context);
    AccountTypeSeeder accountTypeSeeders = new AccountTypeSeeder(context);
    NameSeeder nameSeeders = new NameSeeder(context);
    UserInfoSeeder userInfoSeeders = new UserInfoSeeder(context);

    
    // Seed AccountTypes
    await accountTypeSeeders.SeedAccountTypes();
    // Seed Accounts
    await accountSeeders.SeedAccounts();
    // Seed Names and UsersInfo
    await userInfoSeeders.SeedUserInfos();
    // Seed roles
    await authSeeders.SeedRoles();
    // Seed users
    await authSeeders.SeedUsersAuth();
}