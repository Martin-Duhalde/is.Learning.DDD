/// MIT License © 2025 Martín Duhalde + ChatGPT
///
/// Hot Links:
/// 
///         https://localhost:7263/swagger
///         
///         https://localhost:7263/scalar
///
///         https://localhost:7263/health
///         
///         https://localhost:7263/alive
///         

using CarRental.Core.Interfaces;
using CarRental.Infrastructure.Auth;
using CarRental.Infrastructure.Databases;
using CarRental.Infrastructure.Email;
using CarRental.Infrastructure.Extensions;
using CarRental.UseCases.Common.Behaviors;
using CarRental.UseCases.Rentals.Create;

using FluentValidation;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Scalar.AspNetCore;

using Serilog;

using System.Reflection;
using System.Text;

/************************  builder  ************************/

var builder = WebApplication.CreateBuilder(args);

/// 🔧 Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();  /// ⬅️ Reemplaza el logger por defecto

/// Controllers, endpoint

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

/// Crear las migraciones por proveedor  
///
///		Proveedor			appsettings.json
///
///		📦 SqlServer        "DatabaseProvider": "SqlServer"
///		📦 SQLite           "DatabaseProvider": "SQLite"
///		📦 PostgreSQL       "DatabaseProvider": "PostgreSQL"

var dbProvider = builder.Configuration["DatabaseProvider"];

/// GitHub Actions
var isGitHub = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";

/// Databases
builder.Services.AddDbContext<CarRentalDbContext>(options =>
{
    switch (dbProvider)
    {
        case "SqlServer":       /**/ options.UseSqlServer(  /**/ builder.Configuration.GetConnectionString("SqlServer"));       /**/ break;
        case "PostgreSQL":      /**/ options.UseNpgsql(     /**/ builder.Configuration.GetConnectionString("PostgreSQL"));      /**/ break;
        case "SQLite_HitHub":   /**/ options.UseSqlite(     /**/ builder.Configuration.GetConnectionString("SQLite_HitHub"));   /**/ break;
        //case "SQLite":        /**/ options.UseSqlite(     /**/ builder.Configuration.GetConnectionString("SQLite"));          /**/ break;
        case "SQLite":
            if (isGitHub)
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("SQLite_HitHub")); 
            }
            else
            {
                var dbPath = Path.GetFullPath(Path.Combine(
                builder.Environment.ContentRootPath, "..", "CarRental.Infrastructure", "Databases", "SQLite", "CarRental.db"));
                options.UseSqlite($"Data Source={dbPath}");
            }
            break;
        default:
            throw new InvalidOperationException("Database provider not supported.");
    }

    Log.Information("Using database provider: {Provider}", dbProvider);
});

bool fakeEmail = true;

/// Email Service
if (fakeEmail)   /**/ builder.Services.AddScoped<IEmailService, FakeEmailSender>();
if (!fakeEmail)  /**/ builder.Services.AddScoped<IEmailService, MimeKitEmailSender>();


/// Infrastructure DependencyInjection Adds
/// 
///     💾 Identity: 
///     
///          services.AddIdentity ApplicationUser  IdentityRole
/// 
///     💾 AuthService
///     
///         services.AddScoped IAuthService 
///     
///     💾 Repositories
///     
///          EfServiceRepository 
///          EfCarRepository     
///          EfRentalRepository  
///          EfServiceRepository 
///          
CarRental.Infrastructure.DependencyInjection.AddInfrastructure(builder.Services);

/// Add MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

/// Register Asemblys for MediatR: 
///     CarRental.UseCases
///     CarRental.Application
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateRentalCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CarRental.UseCases.Cars.GetAll.ListAllCarsQuery).Assembly);
});

/// 🔐 JWT Auth
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme   /**/ = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme      /**/ = JwtBearerDefaults.AuthenticationScheme;
})

.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer                  /**/ = true,
        ValidateAudience                /**/ = true,
        ValidateLifetime                /**/ = true,
        ValidateIssuerSigningKey        /**/ = true,
        ValidIssuer                     /**/ = jwtSettings["Issuer"],
        ValidAudience                   /**/ = jwtSettings["Audience"],
        IssuerSigningKey                /**/ = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                                        /**/   jwtSettings["SecretKey"]!))
    };
});

// 👇 AutoMapper 15.0.1
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<CreateRentalMappingProfile>();
});

/// FluentValidation
///  1. Registrar todos los validadores de los ensamblados
builder.Services.AddValidatorsFromAssemblyContaining<CreateRentalCommandValidator>();
/// 2. Registrar los handlers y agregar el pipeline de validación
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<CreateRentalCommand>();  /// Handlers
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));                 /// 👈  Middleware de validación
});


/// AddOpenApi (AddScalar and Swagger)
builder.Services.AddOpenApi();

/// Documentación, Openapi
/// Agregar servicios de Swagger con JWT y comentarios XML
builder.Services.AddSwaggerGen(c =>
{
    /// Configurar documento Swagger
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CarRental.API", Version = "v1" });

    /// Habilitar comentarios XML para la documentación de Swagger (asegurate de habilitar en el csproj)

    var basePath = AppContext.BaseDirectory;
    c.IncludeXmlComments(Path.Combine(basePath, "CarRental.API.xml"));
    c.IncludeXmlComments(Path.Combine(basePath, "CarRental.UseCases.xml"));

    /// Habilito el botón de Autorización en Swagger para JWT
    /// Habilitar JWT  Bearer Authorization en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name            /**/ = "Authorization",
        Type            /**/ = SecuritySchemeType.ApiKey,
        Scheme          /**/ = "Bearer",
        BearerFormat    /**/ = "JWT",
        In              /**/ = ParameterLocation.Header,
        Description     /**/ = "Ingrese 'Bearer' seguido del token JWT. Ejemplo: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


/// Infrastructure.Extensions.ServiceExtensions.AddServiceDefaults(...)
/// Agrega:
///     Telemetría 
///     Health Checks  
///     Service discovery
builder.AddServiceDefaults();


/************************  App  ************************/

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    /// Un seeder (sembrador) es un bloque de código que se ejecuta una sola vez 
    /// al iniciar la aplicación para preparar datos esenciales en la base,
    /// como roles, admin inicial, etc.

    /// En este caso, sirve para crear los roles "Admin" y "User" en la
    /// tabla AspNetRoles si no existen aún, usando el RoleManager.

    var serviceProvider /**/ = scope.ServiceProvider;
    var logger          /**/ = serviceProvider.GetRequiredService<ILogger<Program>>();

    await AuthSeeder.SeedRolesAsync(serviceProvider, logger); /// en: CarRental.Infrastructure.Auth
}

if (app.Environment.IsDevelopment())
{
    ///  Swagger / scalar 
    app.UseSwagger();

    /// Expone: https://localhost:7263/swagger/index.html
    app.UseSwaggerUI();

    /// Configurar documento 
    /// Genera el JSON en /openapi/v1.json
    app.MapOpenApi();

    /// Scalar: neva versión tipo swagger. próxima V3
    /// Expone: https://localhost:7263/scalar/v1
    ///         https://localhost:7263/openapi/v1.json
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Registra tu middleware global antes de la autenticación y autorización
app.UseMiddleware<CarRental.API.Middlewares.ErrorHandlingMiddleware>(); // ⬅️ antes de Authentication
app.UseAuthentication(); // ⬅️ antes de Authorization
app.UseAuthorization();

app.MapControllers();

// Health checks y endpoints por defecto
app.MapDefaultEndpoints(); /// MapHealthChecks: /health  /alive

Log.Information("Aplicación iniciada correctamente");

app.Run();

// Make the implicit Program.cs class public, so integration tests can reference the correct assembly for host building
public partial class Program { }
