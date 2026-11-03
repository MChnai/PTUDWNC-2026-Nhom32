using System.Linq.Expressions;
using CulinaryBlog.Domain.Entities;

namespace CulinaryBlog.Application.Features.Recipes.DTOs;

public static class RecipeProjection
{
    public static readonly Expression<Func<Recipe, RecipeDto>> Summary = r => new RecipeDto
    {
        Id = r.Id, Title = r.Title, Slug = r.Slug, Summary = r.Description,
        CoverImage = r.Images.OrderByDescending(i => i.IsPrimary).ThenBy(i => i.DisplayOrder).Select(i => i.OriginalUrl).FirstOrDefault(),
        PrepTimeMinutes = r.PrepTimeMinutes, CookTimeMinutes = r.CookTimeMinutes,
        Servings = r.Servings, Difficulty = r.Difficulty, ViewCount = r.ViewCount,
        AverageRating = (double)r.RatingAverage, RatingCount = r.RatingCount,
        CategoryId = r.CategoryId, CategoryName = r.Category != null ? r.Category.Name : "",
        AuthorId = r.AuthorId, AuthorName = r.Author != null ? r.Author.FullName : "",
        PublishedAt = r.PublishedAt ?? r.CreatedAt, CreatedAt = r.CreatedAt
    };
}
