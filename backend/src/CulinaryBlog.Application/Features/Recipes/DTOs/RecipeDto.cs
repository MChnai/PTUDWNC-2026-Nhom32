using CulinaryBlog.Domain.Enums;

namespace CulinaryBlog.Application.Features.Recipes.DTOs;

public class RecipeDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? CoverImage { get; set; }
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public int TotalTimeMinutes => PrepTimeMinutes + CookTimeMinutes;
    public RecipeDifficulty Difficulty { get; set; }
    public int ViewCount { get; set; }
    public double AverageRating { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public int Servings { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int RatingCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

