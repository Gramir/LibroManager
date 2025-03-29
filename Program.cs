using LibroManager.Components;
using LibroManager.Components.Account;
using LibroManager.Constants;
using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services;
using LibroManager.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings - Simplificados para evitar problemas de validación
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

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
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Add Authorization policies
builder.Services.AddAuthorization(options =>
{
    // Admin policy
    options.AddPolicy("RequireAdmin", policy =>
        policy.RequireRole(RoleConstants.AdminRole));

    // Librarian policy
    options.AddPolicy("RequireLibrarian", policy =>
        policy.RequireRole(RoleConstants.LibrarianRole, RoleConstants.AdminRole));

    // Libros policies
    options.AddPolicy(RoleConstants.Permissions.Libros.Create, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Libros.Create));
    options.AddPolicy(RoleConstants.Permissions.Libros.Read, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Libros.Read));
    options.AddPolicy(RoleConstants.Permissions.Libros.Update, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Libros.Update));
    options.AddPolicy(RoleConstants.Permissions.Libros.Delete, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Libros.Delete));
    options.AddPolicy(RoleConstants.Permissions.Libros.Manage, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Libros.Manage));

    // Autores policies
    options.AddPolicy(RoleConstants.Permissions.Autores.Create, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Autores.Create));
    options.AddPolicy(RoleConstants.Permissions.Autores.Read, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Autores.Read));
    options.AddPolicy(RoleConstants.Permissions.Autores.Update, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Autores.Update));
    options.AddPolicy(RoleConstants.Permissions.Autores.Delete, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Autores.Delete));

    // Prestamos policies
    options.AddPolicy(RoleConstants.Permissions.Prestamos.Create, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Prestamos.Create));
    options.AddPolicy(RoleConstants.Permissions.Prestamos.Read, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Prestamos.Read));
    options.AddPolicy(RoleConstants.Permissions.Prestamos.Update, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Prestamos.Update));
    options.AddPolicy(RoleConstants.Permissions.Prestamos.Delete, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Prestamos.Delete));
    options.AddPolicy(RoleConstants.Permissions.Prestamos.Manage, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Prestamos.Manage));

    // Estudiantes policies
    options.AddPolicy(RoleConstants.Permissions.Estudiantes.Create, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Estudiantes.Create));
    options.AddPolicy(RoleConstants.Permissions.Estudiantes.Read, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Estudiantes.Read));
    options.AddPolicy(RoleConstants.Permissions.Estudiantes.Update, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Estudiantes.Update));
    options.AddPolicy(RoleConstants.Permissions.Estudiantes.Delete, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Estudiantes.Delete));

    // Ubicaciones policies
    options.AddPolicy(RoleConstants.Permissions.Ubicaciones.Create, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Ubicaciones.Create));
    options.AddPolicy(RoleConstants.Permissions.Ubicaciones.Read, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Ubicaciones.Read));
    options.AddPolicy(RoleConstants.Permissions.Ubicaciones.Update, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Ubicaciones.Update));
    options.AddPolicy(RoleConstants.Permissions.Ubicaciones.Delete, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Ubicaciones.Delete));

    // Categorias policies
    options.AddPolicy(RoleConstants.Permissions.Categorias.Create, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Categorias.Create));
    options.AddPolicy(RoleConstants.Permissions.Categorias.Read, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Categorias.Read));
    options.AddPolicy(RoleConstants.Permissions.Categorias.Update, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Categorias.Update));
    options.AddPolicy(RoleConstants.Permissions.Categorias.Delete, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Categorias.Delete));

    // Users policies
    options.AddPolicy(RoleConstants.Permissions.Users.ManageRoles, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Users.ManageRoles));
    options.AddPolicy(RoleConstants.Permissions.Users.ManageUsers, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Users.ManageUsers));
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
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<IdentityUserAccessor>();

builder.Services.AddScoped<IdentityRedirectManager>();

builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddScoped<IEmailSender<ApplicationUser>, EmailService>();

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
    var roleService = serviceProvider.GetRequiredService<IRoleService>();

    // Asegurar que los roles existan con sus permisos
    await roleService.EnsureDefaultRolesExistAsync();

    // Crear usuario administrador predeterminado si no existe
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

app.MapAdditionalIdentityEndpoints();

app.Run();