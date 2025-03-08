using LibroManager.Components;
using LibroManager.Data.Context;
using LibroManager.Services;
using LibroManager.Services.Interfaces;
using LibroManager.Repositories;
using LibroManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LibroManager.Models;
using LibroManager.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Add Authorization policies
builder.Services.AddAuthorization(options =>
{
    // Admin policy
    options.AddPolicy("RequireAdmin", policy =>
        policy.RequireRole(RoleConstants.AdminRole));

    // Bibliotecario policy
    options.AddPolicy("RequireBibliotecario", policy =>
        policy.RequireRole(RoleConstants.BibliotecarioRole, RoleConstants.AdminRole));

    // Entity-specific policies
    options.AddPolicy("ManageLibros", policy =>
        policy.RequireClaim(RoleConstants.Permissions.Libros.Manage));
        
    options.AddPolicy("ManagePrestamos", policy =>
        policy.RequireClaim(RoleConstants.Permissions.Prestamos.Manage));
        
    options.AddPolicy("ManageUsers", policy =>
        policy.RequireClaim(RoleConstants.Permissions.Users.ManageUsers));
});

// Add Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add Services
builder.Services.AddScoped<ILibroValidationService, LibroValidationService>();
builder.Services.AddScoped<ILibroService, LibroService>();
builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IEstudianteService, EstudianteService>();
builder.Services.AddScoped<IPrestamoService, PrestamoService>();
builder.Services.AddScoped<IUbicacionService, UbicacionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add authentication & authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Initialize roles and admin user
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Create roles if they don't exist
    var roles = new[] { RoleConstants.AdminRole, RoleConstants.BibliotecarioRole };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Create default admin user if it doesn't exist
    var adminEmail = "admin@libromanager.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            NombreCompleto = "Administrador",
            EmailConfirmed = true,
            FechaCreacion = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, RoleConstants.AdminRole);
        }
    }
}

app.Run();
