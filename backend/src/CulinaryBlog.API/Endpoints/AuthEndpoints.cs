using System.Security.Claims;
using CulinaryBlog.Application.Features.Auth.Commands.Register;
using CulinaryBlog.Application.Features.Auth.Commands.Login;
using CulinaryBlog.Application.Features.Auth.Commands.RefreshToken;
using CulinaryBlog.Application.Features.Auth.Commands.Logout;
using CulinaryBlog.Application.Features.Auth.Commands.GoogleLogin;
using CulinaryBlog.Application.Features.Auth.Commands.UpdateProfile;
using CulinaryBlog.Application.Features.Auth.Queries.GetCurrentUser;
using CulinaryBlog.Application.Features.Auth.DTOs;
using Google.Apis.Auth;
using MediatR;

namespace CulinaryBlog.API.Endpoints;

public static class AuthEndpoints
{
    public record GoogleRequest(string IdToken);
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/auth").WithTags("Authentication");
        g.MapPost("/register", async (RegisterCommand body, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(body, ct);
            return result.IsSuccess ? Results.Created("/api/v1/auth/me", result) : ApiResults.Error(result);
        }).RequireRateLimiting("auth");
        g.MapPost("/login", async (LoginCommand body, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(body, ct);
            return result.IsSuccess ? Results.Ok(result) : ApiResults.Error(result);
        }).RequireRateLimiting("auth");
        foreach (var path in new[] { "/refresh", "/refresh-token" })
            g.MapPost(path, async (RefreshTokenRequest body, HttpContext http, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new RefreshTokenCommand(body.RefreshToken, http.Connection.RemoteIpAddress?.ToString()), ct);
                return result.IsSuccess ? Results.Ok(result) : ApiResults.Error(result);
            }).RequireRateLimiting("auth");
        g.MapPost("/logout", async (LogoutRequest body, HttpContext http, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new LogoutCommand(body.RefreshToken, http.Connection.RemoteIpAddress?.ToString(),
                Guid.Parse(http.User.FindFirstValue(ClaimTypes.NameIdentifier)!)), ct);
            return result.IsSuccess ? Results.NoContent() : ApiResults.Error(result);
        }).RequireAuthorization();
        g.MapGet("/me", async (ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetCurrentUserQuery(Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!)), ct);
            return result.IsSuccess ? Results.Ok(result) : ApiResults.Error(result);
        }).RequireAuthorization();
        g.MapPatch("/me", async (UpdateProfileRequest body, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdateProfileCommand(Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!), body.FullName, body.AvatarUrl), ct);
            return result.IsSuccess ? Results.Ok(result) : ApiResults.Error(result);
        }).RequireAuthorization();
        g.MapPost("/google", async (GoogleRequest body, IConfiguration config, ISender sender, CancellationToken ct) =>
        {
            var clientId = config["Google:ClientId"];
            if (string.IsNullOrWhiteSpace(clientId)) return Results.Problem(statusCode: 503, detail: "Đăng nhập Google chưa được cấu hình.");
            if (string.IsNullOrWhiteSpace(body.IdToken) || body.IdToken.Length > 16000) return Results.Problem(statusCode: 401, detail: "Google ID token không hợp lệ.");
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(body.IdToken, new GoogleJsonWebSignature.ValidationSettings { Audience = new[] { clientId } });
                if (!payload.EmailVerified) return Results.Problem(statusCode: 401, detail: "Email Google chưa được xác thực.");
                var result = await sender.Send(new GoogleLoginCommand(payload.Subject, payload.Email, payload.Name, payload.Picture), ct);
                return result.IsSuccess ? Results.Ok(result) : ApiResults.Error(result);
            }
            catch (InvalidJwtException) { return Results.Problem(statusCode: 401, detail: "Google ID token không hợp lệ hoặc đã hết hạn."); }
            catch (HttpRequestException) { return Results.Problem(statusCode: 502, detail: "Chưa kết nối được dịch vụ xác thực Google."); }
        }).RequireRateLimiting("auth");
    }
}
