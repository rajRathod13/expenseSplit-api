using ExpenseSplit.Domain.Entities;
using ExpenseSplit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using ExpenseSplit.Domain.Interfaces;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.ConfigurationSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Net;
using ExpenseSplit.Application;
using System.Reflection;
using ExpenseSplit.Domain;
using ExpenseSplit.Infrastructure.Repositories;
using System.IdentityModel.Tokens.Jwt;

namespace ExpenseSplit.Infrastructure;

public static class DependencyServices
{
    public static IServiceCollection AddInsfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbConnection(configuration)
                .AddIdentityServices()
                .AddConfigurationSettings(configuration)
                .AddAuthenticationServices(configuration)
                .AddAuthorization()
                .AddDependentServices();
        //.AddExternalServiecs();

        return services;
    }

    private static IServiceCollection AddDependentServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddImplementationsForInterfaces(new[] { typeof(IUserRepository).Assembly, typeof(ITokenService).Assembly }, typeof(UserRepository).Assembly);

        return services;
    }

    //private static IServiceCollection AddExternalServiecs(this IServiceCollection services)
    //{
    //    //services.AddScoped<IFileService, FileService>()
    //    //        .AddScoped<ITokenService, TokenService>();

    //    services.AddImplementationsForInterfaces(new[] { typeof(ITokenService).Assembly },typeof(TokenService).Assembly);

    //    return services;
    //}

    private static IServiceCollection AddDbConnection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(options =>
                            options.UseSqlServer(configuration.GetConnectionString("ExpenseSplitDb")));
        return services;
    }

    private static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>();

        return services;
    }
    private static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(options =>
        {
            options.SecretKey = configuration.GetSection("JWTSettings:SecretKey").Value;
            options.Issuer = configuration.GetSection("JWTSettings:Issuer").Value;
            options.Audience = configuration.GetSection("JWTSettings:Audience").Value;
            options.TokenExpirationMinutes = Convert.ToInt32(configuration.GetSection("JWTSettings:TokenExpirationMinutes").Value);
        });

        return services;
    }
    private static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer("Bearer", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JWTSettings:Issuer"],
                ValidAudience = configuration["JWTSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:SecretKey"])),
                //RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["AccessToken"]; // Extract JWT from HTTP-Only Cookie
                    Console.WriteLine("Extracted Token from Cookie: " + context.Token);
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = c =>
                {
                    //if (c.Exception is SecurityTokenExpiredException)
                    //{
                    //    c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    //    c.Response.ContentType = "application/json";
                    //    c.Response.Headers["Status-Code"] = HttpStatusCode.Unauthorized.ToString();
                    //    var result = "The Token is expired.";
                    //    return c.Response.WriteAsync(result);
                    //}
                    //else
                    //{
                    return Task.CompletedTask; ;
                    //}
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Response.ContentType = "application/json";
                    var result = "You are not authorized to access this resource.";
                    return context.Response.WriteAsync(result);
                },
            };
            
        });

        return services;
    }

    public static IServiceCollection AddImplementationsForInterfaces(
    this IServiceCollection services,
    Assembly[] interfaceAssemblies,
    Assembly implementationAssembly)
    {
        var interfaceTypes = interfaceAssemblies
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                t.IsInterface &&
                (typeof(IDependencyMarkerRepository).IsAssignableFrom(t) ||
                 typeof(IDependencyMarkerService).IsAssignableFrom(t)))
            .ToList();

        var implementationTypes = implementationAssembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .ToList();

        foreach (var iface in interfaceTypes)
        {
            var impl = implementationTypes.FirstOrDefault(c => iface.IsAssignableFrom(c));
            if (impl != null)
            {
                services.AddScoped(iface, impl);
                Console.WriteLine($"✔️ Registered {iface.Name} → {impl.Name}");
            }
            else
            {
                Console.WriteLine($"⚠️ WARNING: No implementation found for {iface.FullName}");
            }
        }

        return services;
    }

}
