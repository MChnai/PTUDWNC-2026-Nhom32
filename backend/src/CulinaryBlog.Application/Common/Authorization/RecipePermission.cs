using System.Security.Claims;

namespace CulinaryBlog.Application.Common.Authorization;

public static class RecipePermission
{
    public static bool CanEdit(
        ClaimsPrincipal user,
        Guid authorId)
    {
        if (user.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        if (user.IsInRole("Admin"))
        {
            return true;
        }

        var userIdClaim = user
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(
            userIdClaim,
            out var currentUserId
        ) && currentUserId == authorId;
    }
}