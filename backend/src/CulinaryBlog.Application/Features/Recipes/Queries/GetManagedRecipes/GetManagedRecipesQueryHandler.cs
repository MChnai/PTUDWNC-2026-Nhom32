using System.Security.Claims;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetManagedRecipes;

public class GetManagedRecipesQueryHandler
    : IRequestHandler<GetManagedRecipesQuery, Result<ManagedRecipesResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetManagedRecipesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ManagedRecipesResponse>> Handle(
        GetManagedRecipesQuery request,
        CancellationToken ct)
    {
        var user = request.CurrentUser;

        if (user.Identity?.IsAuthenticated != true)
        {
            return Result<ManagedRecipesResponse>.Failure(
                "UNAUTHORIZED", "Bạn chưa đăng nhập.");
        }

        var isAdmin = user.IsInRole("Admin");
        var isAuthor = user.IsInRole("Author");

        if (!isAdmin && !isAuthor)
        {
            return Result<ManagedRecipesResponse>.Failure(
                "FORBIDDEN", "Bạn không có quyền quản lý công thức.");
        }

        var userIdClaim = user.FindFirst(
            ClaimTypes.NameIdentifier)?.Value;

        if (!isAdmin && !Guid.TryParse(userIdClaim, out _))
        {
            return Result<ManagedRecipesResponse>.Failure(
                "UNAUTHORIZED", "Token không có User ID hợp lệ.");
        }

        Guid.TryParse(userIdClaim, out var userId);

        // Admin: tất cả Recipe.
        // Author: chỉ Recipe của mình.
        var baseQuery = _context.Recipes
            .AsNoTracking()
            .Where(r => !r.IsDeleted);

        if (!isAdmin)
        {
            baseQuery = baseQuery.Where(
                r => r.AuthorId == userId);
        }

        // Thống kê trước khi lọc và phân trang
        var draftCount = await baseQuery.CountAsync(
            r => r.Status == RecipeStatus.Draft, ct);

        var publishedCount = await baseQuery.CountAsync(
            r => r.Status == RecipeStatus.Published, ct);

        var archivedCount = await baseQuery.CountAsync(
            r => r.Status == RecipeStatus.Archived, ct);

        // Bộ lọc bảng quản trị
        var query = baseQuery;

        if (request.Status.HasValue)
        {
            query = query.Where(
                r => r.Status == request.Status.Value);
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(
                r => r.CategoryId == request.CategoryId.Value);
        }

        var page = Math.Max(1, request.PageNumber);
        var size = Math.Clamp(request.PageSize, 1, 100);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .ThenBy(r => r.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(r => new ManagedRecipeDto
            {
                Id = r.Id,
                Title = r.Title,
                Slug = r.Slug,
                Summary = r.Description,

                CoverImage =
                    r.Images.Where(i => i.IsPrimary)
                        .Select(i => i.OriginalUrl)
                        .FirstOrDefault()
                    ?? r.Images
                        .Select(i => i.OriginalUrl)
                        .FirstOrDefault(),

                PrepTimeMinutes = r.PrepTimeMinutes,
                CookTimeMinutes = r.CookTimeMinutes,
                Servings = r.Servings,
                Difficulty = r.Difficulty,
                Status = r.Status,

                CategoryId = r.CategoryId,
                CategoryName = r.Category != null
                    ? r.Category.Name : "",

                AuthorId = r.AuthorId,
                AuthorName = r.Author != null
                    ? r.Author.FullName : "",

                CreatedAt = r.CreatedAt,
                PublishedAt = r.PublishedAt,

                ViewCount = r.ViewCount,
                AverageRating = (double)r.RatingAverage,
                RatingCount = r.RatingCount
            })
            .ToListAsync(ct);

        return Result<ManagedRecipesResponse>.Success(
            new ManagedRecipesResponse
            {
                Items = items,
                PageNumber = page,
                PageSize = size,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(
                    totalCount / (double)size),

                DraftCount = draftCount,
                PublishedCount = publishedCount,
                ArchivedCount = archivedCount
            });
    }
}