using Hangfire.Dashboard;
namespace CulinaryBlog.API.Middleware;
public class AdminDashboardAuthorization : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => context.GetHttpContext().User.IsInRole("Admin");
}
