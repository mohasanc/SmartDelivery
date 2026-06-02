using FluentValidation;
using FluentValidation.AspNetCore;
using SmartDelivery.API.Filters;
using SmartDelivery.Application.Validators.Auth;

namespace SmartDelivery.API.Extensions
{
    public static class ApiServiceExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                // Suppress default model state validation — handled by FluentValidation filter
                options.SuppressModelStateInvalidFilter = true;
            });

            // FluentValidation
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });

                options.AddPolicy("Production", policy =>
                {
                    policy.WithOrigins(
                            "https://smartdelivery.com",
                            "https://app.smartdelivery.com",
                            "https://admin.smartdelivery.com")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // Response compression
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            // Health checks
            services.AddHealthChecks();

            return services;
        }
    }
}
