using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs.Auth;
using CulinaryBlog.Application.Features.Auth.Commands.Login;
using CulinaryBlog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using CulinaryBlog.Application.Common.Models; 
using CulinaryBlog.Application.Features.Auth.DTOs;
using CulinaryBlog.Application.Features.Auth.Commands.RefreshToken;
using System.Security.Claims;

namespace CulinaryBlog.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth")
                       .WithTags("Authentication");

        // POST: Register
        group.MapPost("/register", async (
            RegisterRequest request, 
            IApplicationDbContext dbContext, 
            IPasswordHasher passwordHasher,
            IWelcomeEmailJobService welcomeEmailJobService) => 
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            
            var existingUser = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail || u.Username == request.Username);
                
            if (existingUser != null)
            {
                return Results.BadRequest(new { Message = "Email hoặc Username đã được sử dụng." });
            }

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                FullName = request.Username,
                PasswordHash = passwordHasher.HashPassword(request.Password),
                Role = "Author", 
                CreatedAt = DateTime.UtcNow
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            // 2. BỔ SUNG DÒNG NÀY: Kích hoạt Fire-and-Forget Job gửi Email chào mừng (FR-JOB-001)
            welcomeEmailJobService.EnqueueWelcomeEmail(user.Email, user.Username);

            return Results.Created($"/api/auth/users/{user.Id}", new 
            { 
                Message = "Đăng ký thành công!", 
                UserId = user.Id,
                user.Username,
                user.Email,
                user.FullName,
                user.Role
            });
        })
        .WithSummary("Đăng ký tài khoản mới (POST)");

        // GET: All Users
        group.MapGet("/users", async (IApplicationDbContext dbContext) =>
        {
            var users = await dbContext.Users
                .AsNoTracking()
                .Where(u => !u.IsDeleted)
                .Select(u => new 
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.FullName,
                    u.Role,
                    u.CreatedAt
                })
                .ToListAsync();

            return Results.Ok(new { Total = users.Count, Data = users });
        })
        .WithSummary("Lấy danh sách tất cả người dùng (GET)")
        .RequireAuthorization(
            policy => policy.RequireRole("Admin")
        );

        // GET: User by ID
        group.MapGet("/users/{id:guid}", async (
            Guid id,
            ClaimsPrincipal currentUser,
            IApplicationDbContext dbContext) =>
        {
            var userIdClaim = currentUser
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var isOwner =
                Guid.TryParse(userIdClaim, out var currentUserId) &&
                currentUserId == id;

            var isAdmin = currentUser.IsInRole("Admin");

            if (!isOwner && !isAdmin)
            {
                return Results.Forbid();
            }

            // Lấy thông tin người dùng từ PostgreSQL
            var user = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == id && !u.IsDeleted)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.FullName,
                    u.Role,
                    u.AvatarUrl,
                    u.Bio,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync();

            // Không tìm thấy người dùng
            if (user == null)
            {
                return Results.NotFound(
                    new { Message = "Không tìm thấy người dùng." }
                );
            }

            // Trả về dữ liệu thật
            return Results.Ok(user);
        })
        .WithSummary("Lấy thông tin người dùng theo ID")
        .RequireAuthorization();

        // FR-AUTH-002: Login
        group.MapPost("/login", async (
            [FromBody] LoginCommand command, 
            [FromServices] ISender sender, 
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);

            if (!result.IsSuccess)
            {
                return Results.BadRequest(new 
                { 
                    code = result.ErrorCode, 
                    message = result.ErrorMessage 
                });
            }

            return Results.Ok(result);
        })
        .WithName("Login")
        .AllowAnonymous();

        // FR-AUTH-004: Refresh Access Token
        group.MapPost("/refresh-token", async (
            [FromBody] RefreshTokenRequest request,
            HttpContext httpContext,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            // Lấy IP thật của client (xử lý trường hợp chạy sau Reverse Proxy như Nginx/Cloudflare)
            var ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                            ?? httpContext.Connection.RemoteIpAddress?.ToString()
                            ?? "127.0.0.1";

            var command = new RefreshTokenCommand(request.RefreshToken, ipAddress);
            var result = await mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                // Trả về 401 Unauthorized nếu token invalid/expired hoặc dính Reuse Attack
                return Results.Json(result, statusCode: StatusCodes.Status401Unauthorized);
            }

            return Results.Ok(result);
        })
        .WithName("RefreshToken")
        .Produces<Result<AuthResponseDto>>(StatusCodes.Status200OK)
        .Produces<Result<AuthResponseDto>>(StatusCodes.Status401Unauthorized);
        
    }
}
