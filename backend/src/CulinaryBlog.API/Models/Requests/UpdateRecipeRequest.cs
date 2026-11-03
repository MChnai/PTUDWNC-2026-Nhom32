using CulinaryBlog.Application.Features.Recipes.DTOs;
using CulinaryBlog.Domain.Enums;
using CulinaryBlog.Domain.ValueObjects;

namespace CulinaryBlog.API.Models.Requests;

public record UpdateRecipeRequest(string Title, string Description, int PrepTimeMinutes, int CookTimeMinutes,
    int Servings, RecipeDifficulty Difficulty, Guid CategoryId, RecipeNutrition? Nutrition,
    List<IngredientInput>? Ingredients, List<StepInput>? Steps, string? Instructions = null, string? Notes = null);
