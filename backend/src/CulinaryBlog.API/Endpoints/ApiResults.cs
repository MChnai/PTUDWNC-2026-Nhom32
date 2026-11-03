using CulinaryBlog.Application.Common.Models;

namespace CulinaryBlog.API.Endpoints;

public static class ApiResults
{
    public static IResult Error<T>(Result<T> result)
    {
        var code = (result.ErrorCode ?? "INVALID_DATA").Replace("_", "").ToUpperInvariant();
        var status = code.Contains("NOTFOUND") ? 404
            : code.Contains("FORBIDDEN") ? 403
            : code.Contains("CONFLICT") || code.Contains("HASRECIPES") ? 409
            : code.Contains("LOCKED") ? 429
            : code.Contains("UNAUTHORIZED") || code.Contains("CREDENTIALS") || code.Contains("TOKEN") || code.Contains("INACTIVE") ? 401
            : code.Contains("FILE") ? 400 : 422;
        return Results.Problem(statusCode: status, title: "Không thể hoàn tất yêu cầu",
            detail: result.ErrorMessage, extensions: new Dictionary<string, object?> { ["code"] = result.ErrorCode });
    }
}
