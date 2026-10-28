using System.Security.Claims;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.Features.Recipes.DTOs;
using CulinaryBlog.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetRecipeById;


// Query
public record GetRecipeByIdQuery(
    Guid Id,
    ClaimsPrincipal CurrentUser
) : IRequest<Result<RecipeDetailDto>>;


// Handler
public class GetRecipeByIdQueryHandler
    : IRequestHandler<GetRecipeByIdQuery, Result<RecipeDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRecipeByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RecipeDetailDto>> Handle(
        GetRecipeByIdQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Lấy thông tin người đang truy cập
        var isAuthenticated =
            request.CurrentUser.Identity?.IsAuthenticated == true;

        var userIdClaim = request.CurrentUser
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var hasUserId = Guid.TryParse(
            userIdClaim,
            out var currentUserId
        );

        var isAdmin =
            isAuthenticated &&
            request.CurrentUser.IsInRole("Admin");


        // 2. Truy vấn Recipe
        // Published: mọi người được xem.
        // Draft / Archived: chỉ chủ sở hữu hoặc Admin được xem.
        var recipe = await _context.Recipes
            .AsNoTracking()
            .AsSplitQuery()
            .Include(r => r.Category)
            .Include(r => r.Author)
            .Include(r => r.Ingredients)
            .Include(r => r.Steps)
            .Include(r => r.Images)
            .Where(r =>
                r.Id == request.Id &&
                !r.IsDeleted &&
                (
                    r.Status == RecipeStatus.Published ||
                    (
                        isAuthenticated &&
                        (
                            isAdmin ||
                            (hasUserId && r.AuthorId == currentUserId)
                        )
                    )
                )
            )
            .FirstOrDefaultAsync(cancellationToken);


        // 3. Không tìm thấy hoặc không có quyền xem
        if (recipe == null)
        {
            return Result<RecipeDetailDto>.Failure(
                "NOT_FOUND",
                "Không tìm thấy công thức hoặc bạn không có quyền xem."
            );
        }


        // 4. Chuyển Entity sang DTO
        var dto = new RecipeDetailDto
        {
            // Thông tin cơ bản
            Id = recipe.Id,
            Title = recipe.Title,
            Slug = recipe.Slug,
            Description = recipe.Description,
            Instructions = recipe.Instructions,
            Notes = recipe.Notes,

            // Thời gian
            PrepTimeMinutes = recipe.PrepTimeMinutes,
            CookTimeMinutes = recipe.CookTimeMinutes,
            Servings = recipe.Servings,
            Difficulty = (int)recipe.Difficulty,

            // Trạng thái
            Status = (int)recipe.Status,
            PublishedAt = recipe.PublishedAt,

            // Thống kê
            ViewCount = recipe.ViewCount,
            RatingAverage = recipe.RatingAverage,
            RatingCount = recipe.RatingCount,

            // Danh mục
            CategoryId = recipe.CategoryId,

            Category = recipe.Category == null
                ? null
                : new RecipeDetailCategoryDto(
                    recipe.Category.Id,
                    recipe.Category.Name,
                    recipe.Category.Slug
                ),

            // Tác giả
            AuthorId = recipe.AuthorId,

            Author = recipe.Author == null
                ? null
                : new RecipeDetailAuthorDto(
                    recipe.Author.Id,
                    recipe.Author.FullName,
                    recipe.Author.AvatarUrl,
                    recipe.Author.Bio
                ),


            // Nguyên liệu
            Ingredients = recipe.Ingredients
                .OrderBy(i => i.OrderIndex)
                .Select(i => new RecipeDetailIngredientDto(
                    i.Id,
                    i.RecipeId,
                    i.Name,
                    i.Quantity,
                    i.Unit,
                    i.Notes,
                    i.OrderIndex
                ))
                .ToList(),


            // Các bước thực hiện
            Steps = recipe.Steps
                .OrderBy(s => s.StepNumber)
                .Select(s => new RecipeDetailStepDto(
                    s.Id,
                    s.RecipeId,
                    s.StepNumber,
                    s.Title,
                    s.Description,
                    s.TimerMinutes,
                    s.ImageUrl
                ))
                .ToList(),


            // Hình ảnh
            Images = recipe.Images
                .OrderBy(i => i.DisplayOrder)
                .Select(i => new RecipeDetailImageDto(
                    i.Id,
                    i.RecipeId,
                    i.OriginalUrl,
                    i.ThumbnailUrl,
                    i.AltText,
                    i.IsPrimary,
                    i.DisplayOrder
                ))
                .ToList(),


            // Dinh dưỡng
            Nutrition = recipe.Nutrition == null
                ? null
                : new RecipeDetailNutritionDto(
                    recipe.Nutrition.Calories,
                    recipe.Nutrition.Protein,
                    recipe.Nutrition.Carbohydrates,
                    recipe.Nutrition.Fat,
                    recipe.Nutrition.Fiber,
                    recipe.Nutrition.Sugar,
                    recipe.Nutrition.Sodium
                ),


            // Metadata
            CreatedAt = recipe.CreatedAt,
            UpdatedAt = recipe.UpdatedAt,

            // Optimistic Concurrency
            RowVersion = recipe.RowVersion
        };


        // 5. Trả kết quả
        return Result<RecipeDetailDto>.Success(dto);
    }
}