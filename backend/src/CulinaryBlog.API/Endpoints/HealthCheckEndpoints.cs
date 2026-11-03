using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace CulinaryBlog.API.Endpoints;
public static class HealthCheckEndpoints
{
    public static void MapHealthCheckEndpoints(this IEndpointRouteBuilder app)
    {
        static HealthCheckOptions Options(Func<HealthCheckRegistration, bool> predicate) => new()
        {
            Predicate = predicate,
            ResponseWriter = async (http, report) => await http.Response.WriteAsJsonAsync(new
            { status = report.Status.ToString(), checks = report.Entries.ToDictionary(x => x.Key, x => x.Value.Status.ToString()) })
        };
        app.MapHealthChecks("/health", Options(_ => true));
        app.MapHealthChecks("/health/live", Options(x => x.Tags.Contains("live")));
        app.MapHealthChecks("/health/ready", Options(x => x.Tags.Contains("ready")));
    }
}
