using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using CulinaryBlog.API.Endpoints;
using CulinaryBlog.API.Middleware;
using CulinaryBlog.API.Middlewares;
using CulinaryBlog.Application;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Infrastructure;
using CulinaryBlog.Infrastructure.Services;
using CulinaryBlog.Infrastructure.Persistence.Seeders;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, config) => config.ReadFrom.Configuration(ctx.Configuration).Enrich.FromLogContext().WriteTo.Console());
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
    .WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? ["http://localhost:3000"])
    .AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("ETag", "X-Correlation-ID")));
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        context.Request.Path.StartsWithSegments("/api")
        ? RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ =>
            new FixedWindowRateLimiterOptions { Window = TimeSpan.FromMinutes(1), PermitLimit = 100, QueueLimit = 0 })
        : RateLimitPartition.GetNoLimiter("non-api"));
    options.AddPolicy("auth", context => RateLimitPartition.GetSlidingWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ => new SlidingWindowRateLimiterOptions
        { Window = TimeSpan.FromMinutes(1), SegmentsPerWindow = 6,
          PermitLimit = builder.Configuration.GetValue("RateLimits:AuthPermitLimit", 10), QueueLimit = 0 }));
    options.AddPolicy("upload", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ => new FixedWindowRateLimiterOptions
        { Window = TimeSpan.FromMinutes(1), PermitLimit = 5, QueueLimit = 0 }));
    options.OnRejected = async (context, ct) =>
    {
        var seconds = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry) ? Math.Ceiling(retry.TotalSeconds) : 60;
        context.HttpContext.Response.Headers.RetryAfter = seconds.ToString(System.Globalization.CultureInfo.InvariantCulture);
        await Results.Problem(statusCode: 429, detail: $"Bạn thao tác hơi nhanh. Vui lòng thử lại sau {seconds} giây.").ExecuteAsync(context.HttpContext);
    };
});
var jwt = builder.Configuration.GetSection("Jwt");
var secret = jwt["Key"] ?? throw new InvalidOperationException("Cần cấu hình Jwt__Key.");
if (Encoding.UTF8.GetByteCount(secret) < 32) throw new InvalidOperationException("Jwt__Key phải dài ít nhất 32 byte.");
if (!builder.Environment.IsDevelopment() && secret.StartsWith("DEVELOPMENT_"))
    throw new InvalidOperationException("Cần thay Jwt__Key trước khi chạy Production.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"], ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)) { KeyId = "CulinaryBlogSecretKeyId" },
        ClockSkew = TimeSpan.FromSeconds(15), RoleClaimType = ClaimTypes.Role
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Author", policy => policy.RequireAuthenticatedUser().RequireRole("Author", "Admin"));
    options.AddPolicy("VerifiedAuthor", policy => policy.RequireAuthenticatedUser().RequireRole("Author", "Admin").RequireClaim("email_verified", "true"));
});
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("public-recipes", p => p.Expire(TimeSpan.FromSeconds(60)).SetVaryByQuery("*").Tag("recipes"));
    options.AddPolicy("public-categories", p => p.Expire(TimeSpan.FromHours(1)).Tag("recipes"));
});
if (!string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("Redis")))
    builder.Services.AddStackExchangeRedisOutputCache(options =>
    { options.Configuration = builder.Configuration.GetConnectionString("Redis"); options.InstanceName = "culinary:"; });
builder.Services.AddHangfire(config =>
{
    config.UseSimpleAssemblyNameTypeSerializer().UseRecommendedSerializerSettings();
    if (builder.Configuration.GetValue<bool>("Jobs:InMemory")) config.UseInMemoryStorage();
    else config.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
});
builder.Services.AddHangfireServer();
builder.Services.AddScoped<IBackgroundJobService, HangfireJobService>();
builder.Services.AddScoped<IFileCleanupService, FileCleanupService>();
builder.Services.AddScoped<IRecipeThumbnailJob, RecipeThumbnailJob>();
builder.Services.AddScoped<StarterCatalogSeeder>();

var app = builder.Build();
if (builder.Configuration.GetValue<bool>("Seed:Enabled"))
{
    if (!app.Environment.IsDevelopment()) throw new InvalidOperationException("Starter seed chỉ dành cho Development.");
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<StarterCatalogSeeder>().SeedAsync(Path.Combine(app.Environment.ContentRootPath, "Data", "starter-catalog.json"));
}
Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, "wwwroot"));
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();
app.Use(async (http, next) =>
{
    http.Response.Headers["X-Content-Type-Options"] = "nosniff";
    http.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    if (http.Request.Path.StartsWithSegments("/api/v1/auth")) http.Response.Headers.CacheControl = "no-store";
    await next();
    if (http.Request.Path.StartsWithSegments("/api/v1") && http.Response.StatusCode < 400
        && http.Request.Method is "POST" or "PUT" or "PATCH" or "DELETE")
        await http.RequestServices.GetRequiredService<IOutputCacheStore>().EvictByTagAsync("recipes", http.RequestAborted);
});
app.UseStaticFiles();
app.UseAuthorization();
app.UseRateLimiter();
app.UseOutputCache();
var api = app.MapGroup("/api/v1");
api.MapAuthEndpoints();
api.MapCategoryEndpoints();
api.MapRecipeEndpoints();
api.MapFileEndpoints();
api.MapJobEndpoints();
app.MapHealthCheckEndpoints();
app.MapHangfireDashboard("/hangfire", new DashboardOptions { Authorization = [new AdminDashboardAuthorization()] }).RequireAuthorization(policy => policy.RequireRole("Admin"));
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
if (!builder.Configuration.GetValue<bool>("Jobs:DisableRecurring"))
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<ISitemapJobService>().ScheduleRecurringSitemapJob();
}
app.Run();

public partial class Program;
