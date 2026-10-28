using System.Security.Claims;
using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Domain.Enums;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetManagedRecipes;

public record GetManagedRecipesQuery(
    ClaimsPrincipal CurrentUser,
    int PageNumber = 1,
    int PageSize = 20,
    RecipeStatus? Status = null,
    Guid? CategoryId = null
) : IRequest<Result<ManagedRecipesResponse>>;

public class ManagedRecipeDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? Summary { get; set; }
    public string? CoverImage { get; set; }

    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public int Servings { get; set; }
    public RecipeDifficulty Difficulty { get; set; }
    public RecipeStatus  Status { get; set; }

    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = "";

    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = "";

    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }

    public int ViewCount { get; set; }
    public double AverageRating { get; set; }
    public int RatingCount { get; set; }
}

public class ManagedRecipesResponse
{
    public List<ManagedRecipeDto> Items { get; set; } = [];

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public int DraftCount { get; set; }
    public int PublishedCount { get; set; }
    public int ArchivedCount { get; set; }
}