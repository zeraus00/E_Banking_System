using E_BankingSystem.Components;
using Data;
using Data.Seeders;
using Data.Seeders.User;
using Data.Seeders.Finance;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Services;
using ViewModels;
using Microsoft.AspNetCore.Antiforgery;

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
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthenticationStateProvider, NexusAuthenticationStateProvider>();
builder.Services.AddScoped<NexusAuthenticationStateProvider>();
builder.Services.AddScoped<NexusAuthenticationService>();
builder.Services.AddScoped<ClaimsHelperService>();
builder.Services.AddScoped<ClientHomeService>();
builder.Services.AddScoped<CredentialValidationService>();
builder.Services.AddScoped<PageRedirectService>();
builder.Services.AddScoped<SignInService>();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth-token";
        options.LoginPath = "/Login_page";
        options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
        options.AccessDeniedPath = "/"; // add access denied page here.
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
    var dbContext = scope.ServiceProvider.GetRequiredService<EBankingContext>();

    //dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
    // Apply migrations (this will create or update the database schema)
    // dbContext.Database.Migrate();

    // Seed data (this ensures that only new data is added)
    await SeedData(dbContext);
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

app.MapGet("/currentuser", (HttpContext context) =>
{
    var user = context.User;
    return user?.Identity?.Name ?? "No user found";
});


app.MapGet("/login2", (HttpContext ctx) =>
{
    ctx.Response.Headers["set-cookie"] = "auth=usr:anton";
    return "ok";
});
app.MapGet("/get-csrf", (HttpContext context, IAntiforgery antiforgery) =>
{
    var tokens = antiforgery.GetAndStoreTokens(context);
    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!, new CookieOptions
    {
        HttpOnly = false, // Required so JavaScript can read the cookie
        Secure = true,
        SameSite = SameSiteMode.Strict
    });

    return Results.Ok(new { message = "CSRF token set in cookie." });
});


app.MapPost("/login", async (
    LogInViewModel _loginModel, 
    CredentialValidationService _validationService, 
    SignInService _signInService,
    ClaimsHelperService _claimsHelperService,
    PageRedirectService _pageRedirectService) =>
{
    try
    {
        var redirectUrl = string.Empty;
        //  Validate the credentials.
        var email = (_loginModel.Email ?? string.Empty).Trim();
        var password = (_loginModel.Password ?? string.Empty).Trim();
        var userAuth = await _validationService.TryValidateUserAsync(email, password);
        if (userAuth == null)
        {
            //  Handle failed validation
            //  Redirect to log in page.
            //_signInService.RedirectToLogInPage();
            redirectUrl = _pageRedirectService.GetRedirectToLogInPage();
            _pageRedirectService.redirectWithHttpContext(redirectUrl);
            //_httpContext.Response.Redirect(redirectUrl);
            return;
        }

        //  Handle Sign In logic.
        await _signInService.TrySignInAsync(userAuth);

        //	Redirect based on role.
        redirectUrl = _pageRedirectService.GetRedirectBasedOnRole(userAuth.RoleId);
        _pageRedirectService.redirectWithHttpContext(redirectUrl);

        return;
    }
    catch (Exception ex)
    {
        Console.WriteLine("ERRORRRR: " + ex.Message);
        //_signInService.RedirectToLogInPage();

        var redirectUrl = _pageRedirectService.GetRedirectToLogInPage();
        _pageRedirectService.redirectWithHttpContext(redirectUrl);
        return;
        //return Results.Redirect("/Landing_page");
    }
});



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