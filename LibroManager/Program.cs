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
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add rate limiting for authentication endpoints
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("AuthPolicy", rateLimiterOptions =>
    {
        rateLimiterOptions.PermitLimit = 5;
        rateLimiterOptions.Window = TimeSpan.FromMinutes(5);
        rateLimiterOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        rateLimiterOptions.QueueLimit = 0;
    });
});

// Add MudBlazor
builder.Services.AddMudServices();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings - Enhanced for security
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
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Will be Secure in HTTPS
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.ExpireTimeSpan = TimeSpan.FromHours(8); // Reduced from 1 day to 8 hours
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Add Authorization policies
builder.Services.AddAuthorizationBuilder()
                                 // Add Authorization policies
                                 .AddPolicy("RequireAdmin", policy =>
        policy.RequireRole(RoleConstants.AdminRole))
                                 // Add Authorization policies
                                 .AddPolicy("RequireLibrarian", policy =>
        policy.RequireRole(RoleConstants.LibrarianRole, RoleConstants.AdminRole))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Libros.Create, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Libros.Create))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Libros.Read, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Libros.Read))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Libros.Update, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Libros.Update))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Libros.Delete, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Libros.Delete))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Libros.Manage, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Libros.Manage))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Autores.Create, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Autores.Create))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Autores.Read, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Autores.Read))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Autores.Update, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Autores.Update))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Autores.Delete, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Autores.Delete))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Prestamos.Create, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Prestamos.Create))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Prestamos.Read, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Prestamos.Read))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Prestamos.Update, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Prestamos.Update))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Prestamos.Delete, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Prestamos.Delete))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Prestamos.Manage, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Prestamos.Manage))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Estudiantes.Create, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Estudiantes.Create))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Estudiantes.Read, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Estudiantes.Read))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Estudiantes.Update, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Estudiantes.Update))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Estudiantes.Delete, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Estudiantes.Delete))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Ubicaciones.Create, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Ubicaciones.Create))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Ubicaciones.Read, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Ubicaciones.Read))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Ubicaciones.Update, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Ubicaciones.Update))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Ubicaciones.Delete, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Ubicaciones.Delete))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Categorias.Create, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Categorias.Create))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Categorias.Read, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Categorias.Read))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Categorias.Update, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Categorias.Update))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Categorias.Delete, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Categorias.Delete))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Users.ManageRoles, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Users.ManageRoles))
                                 // Add Authorization policies
                                 .AddPolicy(RoleConstants.Permissions.Users.ManageUsers, policy =>
        policy.RequireClaim("Permission", RoleConstants.Permissions.Users.ManageUsers));

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
    
    // Add security headers for production
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Append("Content-Security-Policy", 
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: https:; " +
            "font-src 'self'; " +
            "connect-src 'self'");
        await next();
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Enable rate limiting
app.UseRateLimiter();

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
        // Generate a secure random password for the default admin user
        var securePassword = GenerateSecurePassword();
        
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            NombreCompleto = "Administrador",
            EmailConfirmed = true,
            FechaCreacion = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(adminUser, securePassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, RoleConstants.AdminRole);
            
            // Log the generated password for first-time setup (remove in production)
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Default admin user created with email: {AdminEmail}. " +
                             "Initial password: {Password}. " + 
                             "Change this password immediately after first login!", 
                             adminEmail, securePassword);
        }
    }
}

// Helper method to generate secure passwords
static string GenerateSecurePassword()
{
    const string lowercase = "abcdefghijklmnopqrstuvwxyz";
    const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; 
    const string digits = "0123456789";
    const string special = "!@#$%^&*";
    const string allChars = lowercase + uppercase + digits + special;
    
    var random = new Random();
    var password = new char[12];
    
    // Ensure at least one character from each required category
    password[0] = lowercase[random.Next(lowercase.Length)];
    password[1] = uppercase[random.Next(uppercase.Length)];
    password[2] = digits[random.Next(digits.Length)];
    password[3] = special[random.Next(special.Length)];
    
    // Fill the rest with random characters
    for (int i = 4; i < password.Length; i++)
    {
        password[i] = allChars[random.Next(allChars.Length)];
    }
    
    // Shuffle the password array
    for (int i = password.Length - 1; i > 0; i--)
    {
        int j = random.Next(i + 1);
        (password[i], password[j]) = (password[j], password[i]);
    }
    
    return new string(password);
}

app.MapAdditionalIdentityEndpoints();

app.Run();