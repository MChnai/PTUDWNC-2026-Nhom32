using System.Text.RegularExpressions;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Enums;
using CulinaryBlog.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;

public class CreateRecipeCommandHandler
    : IRequestHandler<CreateRecipeCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateRecipeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(
        CreateRecipeCommand request,
        CancellationToken cancellationToken)
    {
        var categoryExists = await _context.Categories
            .AnyAsync(c =>
                c.Id == request.CategoryId && !c.IsDeleted,
                cancellationToken);

        if (!categoryExists)
            return Result<Guid>.Failure(
                "CategoryNotFound",
                "Danh mục không tồn tại.");

        var authorExists = await _context.Users
            .AnyAsync(u =>
                u.Id == request.AuthorId && !u.IsDeleted,
                cancellationToken);

        if (!authorExists)
            return Result<Guid>.Failure(
                "AuthorNotFound",
                "Tác giả không tồn tại.");

        if (!Enum.IsDefined(
            typeof(RecipeDifficulty),
            request.Difficulty))
        {
            return Result<Guid>.Failure(
                "InvalidDifficulty",
                "Độ khó không hợp lệ.");
        }

        var recipeId = Guid.NewGuid();

        var baseSlug = Regex.Replace(
            request.Title.Trim().ToLowerInvariant(),
            @"[^a-z0-9]+",
            "-"
        ).Trim('-');

        if (string.IsNullOrEmpty(baseSlug))
            baseSlug = "recipe";

        var recipe = new Recipe
        {
            Id = recipeId,
            Title = request.Title.Trim(),
            Slug = $"{baseSlug}-{recipeId:N}",
            Description = request.Description,
            Instructions = request.Instructions,
            Notes = request.Notes,

            PrepTimeMinutes = request.PrepTimeMinutes,
            CookTimeMinutes = request.CookTimeMinutes,
            Servings = request.Servings,

            Difficulty = (RecipeDifficulty)request.Difficulty,
            Status = RecipeStatus.Draft,

            CategoryId = request.CategoryId,
            AuthorId = request.AuthorId,

            CreatedAt = DateTime.UtcNow
        };

        if (request.Nutrition != null)
        {
            recipe.Nutrition = new RecipeNutrition
            {
                Calories = request.Nutrition.Calories,
                Protein = request.Nutrition.Protein,
                Carbohydrates = request.Nutrition.Carbohydrates,
                Fat = request.Nutrition.Fat,
                Fiber = request.Nutrition.Fiber,
                Sugar = request.Nutrition.Sugar
            };
        }

        foreach (var ingredient in request.Ingredients ?? [])
        {
            recipe.Ingredients.Add(new RecipeIngredient
            {
                RecipeId = recipeId,
                Name = ingredient.Name,
                Quantity = ingredient.Quantity,
                Unit = ingredient.Unit,
                Notes = ingredient.Notes,
                OrderIndex = ingredient.OrderIndex
            });
        }

        foreach (var step in request.Steps ?? [])
        {
            recipe.Steps.Add(new RecipeStep
            {
                RecipeId = recipeId,
                StepNumber = step.StepNumber,
                Title = step.Title,
                Description = step.Description,
                TimerMinutes = step.TimerMinutes,
                ImageUrl = step.ImageUrl
            });
        }

        _context.Recipes.Add(recipe);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(recipe.Id);
    }
}