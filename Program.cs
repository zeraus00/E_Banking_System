using E_BankingSystem.Components;
using Data;
using Data.Enums;
using Data.Seeders;
using Data.Seeders.User;
using Data.Seeders.Finance;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Services;
using System.Security.Claims;
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
builder.Services.AddScoped<LogInService>();

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

app.MapPost("/login", async (HttpContext httpContext, IAntiforgery antiforgery, LogInService logInService, LogInViewModel loginModel) =>
{
    try 
    {
        await antiforgery.ValidateRequestAsync(httpContext);
        //httpContext.Response.Headers.SetCookie = "bths=usr:mabudachi";
        var email = (loginModel.Email ?? string.Empty).Trim();
        var password = (loginModel.Password ?? string.Empty).Trim();
        Console.WriteLine(email + " " + password);
        Console.WriteLine(httpContext.Request.Scheme);

        var userAuth = await logInService.TryAuthenticateUserAsync(email, password);
        if (userAuth == null)
        {
            Console.WriteLine("userAuth is null");
            //return Results.Redirect("/Home/Landing_page"); // Ensure no further processing occurs after redirect
            httpContext.Response.Redirect("/Landing_page");
            return;
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, email),
            new Claim(ClaimTypes.Name, email),
            //new Claim("AccountNumber", userAuth.Account!.AccountNumber),
            new Claim(ClaimTypes.Role, userAuth.Role.RoleName)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            AllowRefresh = true,
            ExpiresUtc = DateTimeOffset.Now.AddDays(1),
            IsPersistent = true,
        };

        Console.WriteLine($"IsAuthenticated: {identity.IsAuthenticated}"); // Should be true
        Console.WriteLine($"AuthenticationType: {identity.AuthenticationType}");

        foreach (var claim in principal.Claims)
        {
            Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
        }

        Console.WriteLine("Signing in user...");
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
        Console.WriteLine("Sign-in complete.");

        var user = httpContext.User;
        Console.WriteLine($"User Identity IsAuthenticated: {user.Identity?.IsAuthenticated}");
        Console.WriteLine($"User Identity Name: {user.Identity?.Name}");

        string redirectUrl = userAuth.RoleId switch
        {
            (int)RoleTypes.Administrator => "/",  // URL for Administrator
            (int)RoleTypes.User => "/Client_home", // URL for User
            (int)RoleTypes.Employee => "/",       // URL for Employee
            _ => "/Login_page"  // Default to Login page
        };
        Console.WriteLine(userAuth.Account!.AccountName);
        Console.WriteLine(userAuth.Role.RoleId + ":" + (int)RoleTypes.User);
        Console.WriteLine("Redirect Url: " + redirectUrl);

        //return Results.Redirect(redirectUrl); // Ensure no further processing occurs after redirect
        httpContext.Response.Redirect(redirectUrl);
        return; 
    } catch (Exception ex)
    {
        Console.WriteLine("ERRORRRR: "+ ex.Message);
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