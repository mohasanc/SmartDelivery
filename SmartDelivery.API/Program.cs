using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Events;
using SmartDelivery.API.Extensions;
using SmartDelivery.API.Middleware;
using SmartDelivery.Application.Extensions;
using SmartDelivery.Domain.Entities;
using SmartDelivery.Infrastructure.Data.Seed;
using SmartDelivery.Infrastructure.Extensions;

namespace SmartDelivery.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
               .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
               .Enrich.FromLogContext()
               .Enrich.WithMachineName()
               .Enrich.WithThreadId()
               .WriteTo.Console(outputTemplate:
                   "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
               .WriteTo.File(
                   path: "logs/smartdelivery-.log",
                   rollingInterval: RollingInterval.Day,
                   retainedFileCountLimit: 30,
                   outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
               .CreateLogger();

            try
            {
                Log.Information("Starting Smart Delivery API...");

                var builder = WebApplication.CreateBuilder(args);

                // Logging
                builder.Host.UseSerilog();

                // Services
                builder.Services.AddApiServices();
                builder.Services.AddApplicationServices();
                builder.Services.AddInfrastructureServices(builder.Configuration);
                builder.Services.AddSwaggerDocumentation();

                // Password Hasher (needed for seeder & auth)
                builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

                var app = builder.Build();

                // Seed Database 
                using (var scope = app.Services.CreateScope())
                {
                    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                    await seeder.SeedAsync();
                }

                // Middleware Pipeline
                app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
                app.UseMiddleware<RequestLoggingMiddleware>();

                if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Demo"))

                {
                    app.UseSwaggerDocumentation();
                }

                app.UseHttpsRedirection();
                app.UseResponseCompression();

                app.UseCors(app.Environment.IsDevelopment() ? "Production" : "AllowAll" );

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers();
                app.MapHealthChecks("/health");

                // Redirect root to Swagger
                app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

                Log.Information("Smart Delivery API started on {Urls}", string.Join(", ", builder.WebHost.GetSetting("urls") ?? "http://localhost:5000"));

                await app.RunAsync();
            }
            catch (Exception ex) when (ex is not HostAbortedException)
            {
                Log.Fatal(ex, "Application terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
