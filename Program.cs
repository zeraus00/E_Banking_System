using E_BankingSystem.Components;
using Data;
using Database.Builder;
using Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<EBankingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqloptions => sqloptions.CommandTimeout(100)));
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


var app = builder.Build();

// Apply any pending migrations and seed the database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EBankingContext>();

    dbContext.Database.EnsureCreated();
    // Apply migrations (this will create or update the database schema)
    // dbContext.Database.Migrate();

    // Seed data (this ensures that only new data is added)
    SeedData(dbContext);
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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


void SeedData(EBankingContext context)
{
    // Seed your database with initial data here
    // For example, you can add some default users, roles, etc.
    // context.Users.Add(new User { Name = "Admin", Role = "Administrator" });
    // context.SaveChanges();

    if (!context.EmployeesAuth.Any())
    {
        var authBuilder = new AuthBuilder(context);
        var employeeAuth = new EmployeeAuthBuilder();

        employeeAuth
            .WithUserName("admin")
            .WithPassword("admin")
            .WithEmail("admin@gmail.com");
        authBuilder.AddEmployeeAuth(employeeAuth.Build());
    }
}