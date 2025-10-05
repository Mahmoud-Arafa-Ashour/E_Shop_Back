using E_Shop.Core.Abstractions;
using E_Shop.Core.Authentications;
using E_Shop.Core.Authorization;
using E_Shop.Core.Persistent;
using E_Shop.Service.IServices;
using E_Shop.Service.Services;
using E_Shop.Service.Swagger;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
namespace E_Shop;

public static class DependencyInjection
{

    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        var conn = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection String 'DefaultConnection' not found.");
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(conn, b => b.MigrationsAssembly("E_Shop")));
        services.AddMappsterServices();
        services.AddAuthconfigServices(configuration);
        services.AddConfigServices();
        services.AddSwaggerGen();
        services.AddCors(
                options => options.AddPolicy("AllowAll", builder =>
                {
                    builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin();
                }));
        services.AddScoped<IAuthServices, AuthServices>();
        services.AddScoped<IUserServices, UserServices>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IProductServices, ProductServices>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddSingleton<IJwtProvidor, JwtProvidor>();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddProblemDetails();
        services.AddHttpContextAccessor();
        //Add Bucket Rate Limiter
        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            rateLimiterOptions.AddTokenBucketLimiter("Token", Options =>
            {
                Options.TokenLimit = 10;
                Options.QueueLimit = 2;
                Options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                Options.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                Options.TokensPerPeriod = 2;
                Options.AutoReplenishment = true;
            });
        });

        //Add Concurrency Rate Limiter
        services.AddRateLimiter(rateLimiterOptions => 
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            rateLimiterOptions.AddConcurrencyLimiter("Concurrency", options => 
            {
                options.PermitLimit = 5;
                options.QueueLimit = 2;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
        });
        return services;
    }
    public static IServiceCollection AddMappsterServices(this IServiceCollection services)
    {
        var mappingConfiguration = TypeAdapterConfig.GlobalSettings;
        mappingConfiguration.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper(mappingConfiguration));
        return services;
    }
    public static IServiceCollection AddConfigServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        services.AddFluentValidationAutoValidation();
        return services;
    }
    public static IServiceCollection AddAuthconfigServices(this IServiceCollection services , IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        services.AddTransient<IAuthorizationHandler, PermissionAttributeHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
            )
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions!.Key)),
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience
                };
            });
        services.Configure<IdentityOptions>(Options =>
        {
            Options.Password.RequiredLength = 8;
            Options.SignIn.RequireConfirmedEmail = true;
            Options.Lockout.MaxFailedAccessAttempts = 5;
            Options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            Options.Lockout.AllowedForNewUsers = true;
        });
        return services;
    }
}
