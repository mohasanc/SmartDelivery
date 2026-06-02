using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SmartDelivery.Application.Interfaces.Repositories;
using SmartDelivery.Application.Interfaces.Services;
using SmartDelivery.Domain.Entities;
using SmartDelivery.Infrastructure.Data;
using SmartDelivery.Infrastructure.Data.Seed;
using SmartDelivery.Infrastructure.Services;
using System.Text;

namespace SmartDelivery.Infrastructure.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            // ------ Database ------
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        //sqlOptions.EnableRetryOnFailure(
                        //maxRetryCount: 5,
                        //maxRetryDelay: TimeSpan.FromSeconds(30),
                        //errorNumbersToAdd: null);
                        sqlOptions.CommandTimeout(60);
                        sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    });
                options.EnableSensitiveDataLogging(false);
            });

            // ------ Caching ------
            services.AddMemoryCache();

            // ------ Repositories & UoW ------
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            // ------ Application Services ------
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ICacheService, CacheService>();

            // ------ Password Hasher ------
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // ------ Seeder ------
            services.AddScoped<DataSeeder>();


            // ------ JWT Authentication ------
            var jwtKey = configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");


            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; // set true in production
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = ctx =>
                        {
                            if (ctx.Exception is SecurityTokenExpiredException)
                                ctx.Response.Headers["Token-Expired"] = "true";
                            return Task.CompletedTask;
                        }
                    };
                });
                
            services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", p => p.RequireRole("Admin"))
            .AddPolicy("CustomerOrAdmin", p => p.RequireRole("Customer", "Admin"))
            .AddPolicy("DriverOrAdmin", p => p.RequireRole("Driver", "Admin"))
            .AddPolicy("RestaurantOrAdmin", p => p.RequireRole("RestaurantOwner", "Admin"));

            return services;
        }
    }
}
