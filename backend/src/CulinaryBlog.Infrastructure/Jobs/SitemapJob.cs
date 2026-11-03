using System.Xml.Linq;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Domain.Enums;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace CulinaryBlog.Infrastructure.Jobs;
public class SitemapJob(IApplicationDbContext db, IWebHostEnvironment env, IConfiguration config, ILogger<SitemapJob> logger)
{
    [AutomaticRetry(Attempts = 2)]
    [DisableConcurrentExecution(600)]
    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        var root = (config["ClientUrl"] ?? "http://localhost:3000").TrimEnd('/');
        var categories = await db.Categories.AsNoTracking().Select(c => c.Slug).ToListAsync(ct);
        var recipes = await db.Recipes.AsNoTracking().Where(r => r.Status == RecipeStatus.Published)
            .Select(r => new { r.Slug, Modified = r.UpdatedAt ?? r.CreatedAt }).ToListAsync(ct);
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        XElement Url(string path, DateTime? date = null) => new(ns + "url", new XElement(ns + "loc", root + path),
            date.HasValue ? new XElement(ns + "lastmod", date.Value.ToString("yyyy-MM-dd")) : null);
        var document = new XDocument(new XElement(ns + "urlset", new[] { "/", "/recipes", "/categories", "/learn", "/about" }.Select(p => Url(p)),
            categories.Select(s => Url("/categories/" + s)), recipes.Select(r => Url("/recipes/" + r.Slug, r.Modified))));
        var output = Path.Combine(env.ContentRootPath, "wwwroot", "sitemap.xml");
        Directory.CreateDirectory(Path.GetDirectoryName(output)!);
        await File.WriteAllTextAsync(output + ".tmp", document.ToString(), ct);
        File.Move(output + ".tmp", output, overwrite: true);
        logger.LogInformation("Sitemap generated with {Count} recipes.", recipes.Count);
    }
}
