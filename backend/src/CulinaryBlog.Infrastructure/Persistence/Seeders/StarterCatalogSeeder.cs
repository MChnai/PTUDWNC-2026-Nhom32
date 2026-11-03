using System.Text.Json;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace CulinaryBlog.Infrastructure.Persistence.Seeders;

public class StarterCatalogSeeder(ApplicationDbContext db, IPasswordHasher hasher, IConfiguration config, ILogger<StarterCatalogSeeder> logger)
{
    public class Catalog { public List<Category> Categories { get; set; } = []; public List<ApplicationUser> Authors { get; set; } = []; public List<Recipe> Recipes { get; set; } = []; }
    public async Task SeedAsync(string path)
    {
        if (await db.Recipes.IgnoreQueryFilters().AnyAsync()) { logger.LogInformation("Existing catalog preserved; starter seed skipped."); return; }
        var password = config["Seed:AdminPassword"] ?? throw new InvalidOperationException("Cần Seed__AdminPassword để khởi tạo dữ liệu phát triển.");
        if (password.Length < 12) throw new InvalidOperationException("Mật khẩu khởi tạo cần ít nhất 12 ký tự.");
        var catalog = JsonSerializer.Deserialize<Catalog>(await File.ReadAllTextAsync(path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        foreach (var category in catalog.Categories)
        {
            var existing = await db.Categories.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Name == category.Name || c.Slug == category.Slug);
            if (existing != null) { foreach (var recipe in catalog.Recipes.Where(r => r.CategoryId == category.Id)) recipe.CategoryId = existing.Id; }
            else db.Categories.Add(category);
        }
        foreach (var author in catalog.Authors)
        {
            var existing = await db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Email == author.Email);
            if (existing != null) { foreach (var recipe in catalog.Recipes.Where(r => r.AuthorId == author.Id)) recipe.AuthorId = existing.Id; }
            else { author.PasswordHash = hasher.HashPassword(password); author.EmailConfirmed = true; db.Users.Add(author); }
        }
        var adminEmail = config["Seed:AdminEmail"] ?? "admin@culinary.local";
        if (!await db.Users.IgnoreQueryFilters().AnyAsync(u => u.Email == adminEmail))
            db.Users.Add(new ApplicationUser { Username = "culinary_admin", Email = adminEmail, FullName = "Bếp trưởng Culinary", Role = "Admin", EmailConfirmed = true, PasswordHash = hasher.HashPassword(password) });
        foreach (var recipe in catalog.Recipes) { recipe.Status = RecipeStatus.Published; recipe.PublishedAt = DateTime.UtcNow; db.Recipes.Add(recipe); }
        await db.SaveChangesAsync();
        logger.LogInformation("Starter catalog seeded: {Count} recipes; no existing records overwritten.", catalog.Recipes.Count);
    }
}
