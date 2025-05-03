using E_BankingSystem.Components;
using Data;
using Data.Constants;
using Data.Seeders;
using Data.Seeders.User;
using Data.Seeders.Finance;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Services;
using Services.DataManagement;
using System.Security.Claims;
using ViewModels;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// DbContext;
builder.Services.AddDbContextFactory<EBankingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqloptions => sqloptions.CommandTimeout(100))
    );
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();



// Authentication Services
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthenticationStateProvider, NexusAuthenticationStateProvider>();
builder.Services.AddScoped<ClaimsHelperService>();
builder.Services.AddScoped<CredentialValidationService>();
builder.Services.AddScoped<NexusAuthenticationService>();
builder.Services.AddScoped<SignInService>();

// Data services
builder.Services.AddScoped<AdminDataService>();
builder.Services.AddScoped<CredentialFactory>();
builder.Services.AddScoped<DataMaskingService>();
builder.Services.AddScoped<UserDataService>();
builder.Services.AddScoped<RegistrationService>();
builder.Services.AddScoped<TransactionService>();

// Session Management Services
builder.Services.AddScoped<AdminControlledSessionService>();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<SessionStorageService>();
builder.Services.AddScoped<UserControlledSessionService>();
builder.Services.AddScoped<UserSessionService>();

// Misc
builder.Services.AddScoped<PageRedirectService>();



builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth-token";
        options.LoginPath = PageRoutes.LANDING_PAGE;
        options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
        options.AccessDeniedPath = PageRoutes.LANDING_PAGE; // add access denied page here.
        // Ensure the cookie is only sent over HTTPS
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        // Allow the cookie to be sent in cross-site requests
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.HttpOnly = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Apply any pending migrations and seed the database
using (var scope = app.Services.CreateScope())
{
    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<EBankingContext>>();
    await using (var dbContext = await contextFactory.CreateDbContextAsync())
    {

        //dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        // Apply migrations (this will create or update the database schema)
        // dbContext.Database.Migrate();
    }

    // Seed data (this ensures that only new data is added)
    await SeedData(contextFactory);
}





app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseStaticFiles();

app.UseRouting();

//  Middlewares for authentication and authorization.
app.UseAuthentication();
app.UseAuthorization();





app.UseAntiforgery();

//temp
app.MapRazorPages();

//app.UseMiddleware<StatisticMiddleWare>();

/*  ADD MINIMAL APIS HERE   */


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


static async Task SeedData(IDbContextFactory<EBankingContext> contextFactory)
{
    await using (var dbContext = await contextFactory.CreateDbContextAsync())
    {
        AuthSeeder authSeeders = new AuthSeeder(dbContext);
        AccountSeeder accountSeeders = new AccountSeeder(dbContext);
        AccountTypeSeeder accountTypeSeeders = new AccountTypeSeeder(dbContext);
        TransactionTypesSeeder transactionTypesSeeder = new TransactionTypesSeeder(dbContext);
        LoanTypeSeeder loanTypeSeeder = new LoanTypeSeeder(dbContext);
        NameSeeder nameSeeders = new NameSeeder(dbContext);
        UserInfoSeeder userInfoSeeders = new UserInfoSeeder(dbContext);


        // Seed AccountTypes
        await accountTypeSeeders.SeedAccountTypes();
        // Seed StatusTypes
        await accountTypeSeeders.SeedAccountStatusTypes();
        // Seed Accounts
        await accountSeeders.SeedAccounts();
        // Seed TransactionTypes
        await transactionTypesSeeder.SeedTransactionTypes();
        // Seed LoanTypes
        await loanTypeSeeder.SeedLoanTypes();
        // Seed roles
        await authSeeders.SeedRoles();
        // Seed users
        await authSeeders.SeedUsersAuth();

        // Seed Names and UsersInfo
        await userInfoSeeders.SeedUserInfos();

        //await accountSeeders.SeedOrUpdateAtmNumbers();
    }

}