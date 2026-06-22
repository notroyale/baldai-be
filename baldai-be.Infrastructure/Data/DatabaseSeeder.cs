using baldai_be.Domain.Entities;
using baldai_be.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace baldai_be.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();

        if (!await context.Users.AnyAsync())
        {
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@lumen.com",
                FirstName = "Lumen",
                LastName = "Admin",
                PasswordHash = "hashedpassword", // Not a real hash for dev
                Role = UserRole.Moderator,
                JoinedDate = DateTime.UtcNow
            };
            var sellerUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "seller@lumen.com",
                FirstName = "Vintage",
                LastName = "Seller",
                PasswordHash = "hashedpassword",
                Role = UserRole.Seller,
                JoinedDate = DateTime.UtcNow
            };
            await context.Users.AddRangeAsync(adminUser, sellerUser);
            await context.SaveChangesAsync();
        }

        if (!await context.Products.AnyAsync())
        {
            var seller = await context.Users.FirstAsync(u => u.Role == UserRole.Seller);

            var products = new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    SellerId = seller.Id,
                    Title = "Cesca B32 Side Chair",
                    Description = "Marcel Breuer, 1928",
                    Category = "Seating",
                    Price = 850m,
                    IsNew = true,
                    Status = ProductStatus.Active,
                    ImagePaths = "[\"https://images.unsplash.com/photo-1503602642458-232111445657?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80\"]",
                    Dimensions = new Dimensions { Length = 45, Width = 45, Height = 80 },
                    Shipping = ShippingOption.Both
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    SellerId = seller.Id,
                    Title = "Coffee Table",
                    Description = "Isamu Noguchi",
                    Category = "Tables",
                    Price = 2200m,
                    IsNew = false,
                    Status = ProductStatus.Active,
                    ImagePaths = "[\"https://images.unsplash.com/photo-1532372320572-cda25653a26d?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80\"]",
                    Dimensions = new Dimensions { Length = 120, Width = 80, Height = 40 },
                    Shipping = ShippingOption.Both
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    SellerId = seller.Id,
                    Title = "Eames Lounge Chair",
                    Description = "Herman Miller, 1970s",
                    Category = "Seating",
                    Price = 5200m,
                    IsNew = false,
                    Status = ProductStatus.Active,
                    ImagePaths = "[\"https://images.unsplash.com/photo-1567538096630-e0c55bd6374c?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80\"]",
                    Dimensions = new Dimensions { Length = 85, Width = 85, Height = 80 },
                    Shipping = ShippingOption.Both
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    SellerId = seller.Id,
                    Title = "Camaleonda Sofa Section",
                    Description = "Mario Bellini, 1970",
                    Category = "Seating",
                    Price = 4100m,
                    IsNew = false,
                    Status = ProductStatus.Active,
                    ImagePaths = "[\"https://images.unsplash.com/photo-1586023492125-27b2c045efd7?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80\"]",
                    Dimensions = new Dimensions { Length = 90, Width = 90, Height = 70 },
                    Shipping = ShippingOption.Both
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    SellerId = seller.Id,
                    Title = "'The Chair' Set (4)",
                    Description = "Hans Wegner, 1949",
                    Category = "Seating",
                    Price = 12000m,
                    IsNew = false,
                    Status = ProductStatus.Active,
                    ImagePaths = "[\"https://images.unsplash.com/photo-1616486029423-aaa4789e8c9a?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80\"]",
                    Dimensions = new Dimensions { Length = 50, Width = 50, Height = 75 },
                    Shipping = ShippingOption.Both
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    SellerId = seller.Id,
                    Title = "Brass Display Vitrine",
                    Description = "Italian Modernist, 1960s",
                    Category = "Storage",
                    Price = 3250m,
                    IsNew = false,
                    Status = ProductStatus.Active,
                    ImagePaths = "[\"https://images.unsplash.com/photo-1600585154340-be6161a56a0c?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80\"]",
                    Dimensions = new Dimensions { Length = 100, Width = 40, Height = 180 },
                    Shipping = ShippingOption.Both
                }
            };
            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }

        if (!await context.Products.AnyAsync(p => p.Title == "Test Product"))
        {
            var seller = await context.Users.FirstAsync(u => u.Role == UserRole.Seller);
            var testProduct = new Product
            {
                Id = Guid.NewGuid(),
                SellerId = seller.Id,
                Title = "Test Product",
                Description = "This is a test product created for testing purposes.",
                Category = "Tables",
                Price = 99.99m,
                IsNew = true,
                Status = ProductStatus.Active,
                ImagePaths = "[\"https://images.unsplash.com/photo-1555041469-a586c61ea9bc?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80\"]",
                Dimensions = new Dimensions { Length = 10, Width = 10, Height = 10 },
                Shipping = ShippingOption.Both
            };
            context.Products.Add(testProduct);
            await context.SaveChangesAsync();
        }
    }
}
