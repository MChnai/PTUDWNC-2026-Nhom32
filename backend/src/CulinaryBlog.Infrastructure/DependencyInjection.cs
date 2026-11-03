using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Common.Metrics;
using CulinaryBlog.Infrastructure.Persistence;
using CulinaryBlog.Infrastructure.Persistence.Seeders;
using CulinaryBlog.Infrastructure.Services;
using CulinaryBlog.Infrastructure.Storage;
using CulinaryBlog.Infrastructure.Jobs;
using CulinaryBlog.Infrastructure.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Minio;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
namespace CulinaryBlog.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connection = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Cần ConnectionStrings__DefaultConnection.");
        services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(connection, n => n.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        services.AddScoped<IApplicationDbContext>(p => p.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
        services.Configure<MinioOptions>(config.GetSection("Minio"));
        var minio = config.GetSection("Minio").Get<MinioOptions>() ?? new();
        services.AddMinio(c => c.WithEndpoint(minio.Endpoint).WithCredentials(minio.AccessKey, minio.SecretKey).WithSSL(minio.UseSSL).Build());
        if (config["Storage:Provider"]?.Equals("Minio", StringComparison.OrdinalIgnoreCase) == true) services.AddScoped<IFileStorageService, MinioFileStorageService>();
        else services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IStorageService, MinioStorageService>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IImageInspector, ImageInspector>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IWelcomeEmailJobService, WelcomeEmailJobService>();
        services.AddScoped<IThumbnailJobService, ThumbnailJobService>();
        services.AddScoped<SitemapJob>();
        services.AddScoped<ISitemapJobService, SitemapJobService>();
        var checks = services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
            .AddDbContextCheck<ApplicationDbContext>("postgresql", tags: ["ready"]);
        var redis = config.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redis)) checks.AddRedis(redis, "redis", tags: ["ready"]);
        if (config["Storage:Provider"]?.Equals("Minio", StringComparison.OrdinalIgnoreCase) == true) checks.AddCheck<MinioHealthCheck>("minio", tags: ["ready"]);
        var telemetry = services.AddOpenTelemetry().ConfigureResource(r => r.AddService("CulinaryBlog.API"));
        telemetry.WithTracing(t =>
        {
            t.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddEntityFrameworkCoreInstrumentation();
            if (Uri.TryCreate(config["OpenTelemetry:OtlpEndpoint"], UriKind.Absolute, out var endpoint)) t.AddOtlpExporter(o => o.Endpoint = endpoint);
        });
        telemetry.WithMetrics(m =>
        {
            m.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddMeter(CulinaryBlogMetrics.MeterName);
            if (Uri.TryCreate(config["OpenTelemetry:OtlpEndpoint"], UriKind.Absolute, out var endpoint)) m.AddOtlpExporter(o => o.Endpoint = endpoint);
        });
        return services;
    }
}
