using Microsoft.OpenApi.Models;

namespace SmartDelivery.API.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Smart Delivery API",
                    Version = "v1",
                    Description = """
                    Smart Delivery System (Talabat clone).

                    **Roles:**
                    - **Admin** — full system access
                    - **Customer** — place and track orders
                    - **Driver** — accept and deliver orders
                    - **RestaurantOwner** — manage restaurants, menus, and orders

                    **Authentication:** Use the `/api/v1/auth/login` endpoint to obtain a Bearer token,
                    then click **Authorize** and enter: `Bearer <your_token>`
                    """,
                    Contact = new OpenApiContact
                    {
                        Name = "Smart Delivery Team",
                        Email = "itsmohassan@gmail.com"
                    },
                    License = new OpenApiLicense { Name = "MIT" }
                });

                // JWT Bearer security definition
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token below. Example: **eyJhbGci...**"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

                // Include XML comments if available
                var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
                foreach (var xmlFile in xmlFiles)
                    options.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);

                options.EnableAnnotations();
                options.UseInlineDefinitionsForEnums();
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api-docs/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/api-docs/v1/swagger.json", "Smart Delivery API v1");
                options.RoutePrefix = "swagger";
                options.DocumentTitle = "Smart Delivery API";
                options.DisplayRequestDuration();
                options.EnableDeepLinking();
                options.ShowExtensions();
            });

            return app;
        }
    }
}
