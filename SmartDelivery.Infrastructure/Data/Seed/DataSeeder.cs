using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartDelivery.Domain.Entities;

namespace SmartDelivery.Infrastructure.Data.Seed
{
    public class DataSeeder 
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<DataSeeder> _logger;

        public DataSeeder(AppDbContext context, IPasswordHasher<User> passwordHasher, ILogger<DataSeeder> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }
        public async Task SeedAsync()
        {
            try
            {
                await _context.Database.MigrateAsync();
                await SeedRolesAsync();
                await SeedUsersAsync();
                await SeedRestaurantsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            if (await _context.Roles.AnyAsync())
                return;

            var roles = new List<Role>
            {
                new() {Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "Admin", Description = "System administrator" },
                new() {Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "Customer", Description = "App customer who places orders" },
                new() {Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "Driver", Description = "Delivery drivers" },
                new() {Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Name = "RestaurantOwner", Description = "Restaurant owner" },
            };

            await _context.Roles.AddRangeAsync(roles);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Roles seeded successfully.");
        }

        private async Task SeedUsersAsync()
        {
            if (await _context.Users.AnyAsync()) return;

            var adminId = Guid.Parse("20000000-0000-0000-0000-000000000001");
            var customerId = Guid.Parse("20000000-0000-0000-0000-000000000002");
            var driverId = Guid.Parse("20000000-0000-0000-0000-000000000003");
            var ownerId = Guid.Parse("20000000-0000-0000-0000-000000000004");

            var adminRoleId = Guid.Parse("10000000-0000-0000-0000-000000000001");
            var customerRoleId = Guid.Parse("10000000-0000-0000-0000-000000000002");
            var driverRoleId = Guid.Parse("10000000-0000-0000-0000-000000000003");
            var ownerRoleId = Guid.Parse("10000000-0000-0000-0000-000000000004");

            var admin = new User { Id = adminId, FirstName = "System", LastName = "Admin", Email = "admin@smartdelivery.com", PhoneNumber = "+201009961956", IsEmailVerified = true };
            admin.PasswordHash = _passwordHasher.HashPassword(admin, "Admin@123456");

            var customer = new User { Id = customerId, FirstName = "Mohamed", LastName = "Hassan", Email = "customer@smartdelivery.com", PhoneNumber = "+201009961956", IsEmailVerified = true };
            customer.PasswordHash = _passwordHasher.HashPassword(customer, "Customer@123456");

            var driver = new User { Id = driverId, FirstName = "Mohamed", LastName = "Hassan", Email = "driver@smartdelivery.com", PhoneNumber = "+201009961956", IsEmailVerified = true };
            driver.PasswordHash = _passwordHasher.HashPassword(driver, "Driver@123456");

            var owner = new User { Id = ownerId, FirstName = "Mohamed", LastName = "Hassan", Email = "owner@smartdelivery.com", PhoneNumber = "+201009961956", IsEmailVerified = true };
            owner.PasswordHash = _passwordHasher.HashPassword(owner, "Owner@123456");

            await _context.Users.AddRangeAsync(admin, customer, driver, owner);

            await _context.UserRoles.AddRangeAsync(
                new UserRole { UserId = adminId, RoleId = adminRoleId },
                new UserRole { UserId = customerId, RoleId = customerRoleId },
                new UserRole { UserId = driverId, RoleId = driverRoleId },
                new UserRole { UserId = ownerId, RoleId = ownerRoleId }
            );

            await _context.SaveChangesAsync();
            _logger.LogInformation("Users seeded successfully.");
        }

        private async Task SeedRestaurantsAsync()
        {
            if (await _context.Restaurants.AnyAsync()) return;


            var ownerId = Guid.Parse("20000000-0000-0000-0000-000000000004");
            var restaurantId = Guid.Parse("30000000-0000-0000-0000-000000000001");
            var categoryId = Guid.Parse("40000000-0000-0000-0000-000000000001");

            var restaurant = new Restaurant
            {
                Id = restaurantId,
                Name = "Burger Palace",
                Description = "The best burgers in town, made with premium Angus beef.",
                PhoneNumber = "+201009961956",
                Email = "info@burgerpalace.com",
                Address = "123 Tahrir Square",
                City = "Cairo",
                Latitude = 30.0444,
                Longitude = 31.2357,
                DeliveryFee = 15,
                EstimatedDeliveryMinutes = 30,
                Rating = 4.5,
                TotalRatings = 120,
                IsOpen = true,
                OpeningTime = "09:00",
                ClosingTime = "23:00",
                OwnerId = ownerId
            };

            var category = new Category
            {
                Id = categoryId,
                Name = "Burgers",
                Description = "Our signature burgers",
                DisplayOrder = 1,
                RestaurantId = restaurantId
            };

            var menuItems = new List<MenuItem>
            {
                new() { Name = "Classic Burger", Description = "Juicy beef patty with lettuce, tomato, and our secret sauce", Price = 89, CategoryId = categoryId, RestaurantId = restaurantId, PreparationTimeMinutes = 15, Calories = 650 },
                new() { Name = "Double Smash Burger", Description = "Two smashed patties, double cheese, caramelized onions", Price = 129, CategoryId = categoryId, RestaurantId = restaurantId, PreparationTimeMinutes = 18, Calories = 950 },
                new() { Name = "Veggie Burger", Description = "Plant-based patty with fresh vegetables", Price = 79, CategoryId = categoryId, RestaurantId = restaurantId, PreparationTimeMinutes = 12, Calories = 420, IsVegetarian = true, IsVegan = true },
                new() { Name = "Crispy Fries", Description = "Golden crispy seasoned fries", Price = 35, CategoryId = categoryId, RestaurantId = restaurantId, PreparationTimeMinutes = 8, Calories = 380, IsVegetarian = true, IsVegan = true },
            };

            await _context.Restaurants.AddAsync(restaurant);
            await _context.Categories.AddAsync(category);
            await _context.MenuItems.AddRangeAsync(menuItems);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Restaurant seed data created successfully.");
        }

    }
}
