using System.Security.Claims;
using CulinaryBlog.Application.Common.Authorization;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.Features.Recipes.DTOs;
using CulinaryBlog.Domain.ValueObjects;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Application.Features.Recipes.Commands.UpdateRecipe;

public record UpdateRecipeCommand : IRequest<Result<Unit>>
{
    public Guid Id { get; init; }
    public string Title { get; init; } = "";
    public string Description { get; init; } = "";
    public string? Instructions { get; init; }
    public string? Notes { get; init; }
    public int PrepTimeMinutes { get; init; }
    public int CookTimeMinutes { get; init; }
    public int Servings { get; init; }
    public RecipeDifficulty Difficulty { get; init; }
    public Guid CategoryId { get; init; }
    public uint RowVersion { get; init; }
    public RecipeNutrition? Nutrition { get; init; }
    // Null preserves the existing collection, [] explicitly clears it.
    public List<IngredientInput>? Ingredients { get; init; }
    public List<StepInput>? Steps { get; init; }
    public ClaimsPrincipal CurrentUser { get; init; } = default!;
}

public class UpdateRecipeCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateRecipeCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(UpdateRecipeCommand request, CancellationToken ct)
    {
        var recipe = await context.Recipes.Include(r => r.Ingredients).Include(r => r.Steps)
            .AsSplitQuery().FirstOrDefaultAsync(r => r.Id == request.Id, ct);
        if (recipe == null) return Result<Unit>.Failure("NOT_FOUND", "Không tìm thấy công thức.");
        if (!RecipePermission.CanEdit(request.CurrentUser, recipe.AuthorId))
            return Result<Unit>.Failure("FORBIDDEN", "Bạn không có quyền cập nhật công thức này.");
        if (recipe.RowVersion != request.RowVersion)
            return Result<Unit>.Failure("CONCURRENCY_CONFLICT", "Công thức đã được thay đổi. Hãy tải lại trước khi lưu.");
        if (!await context.Categories.AnyAsync(c => c.Id == request.CategoryId, ct))
            return Result<Unit>.Failure("CATEGORY_NOT_FOUND", "Danh mục không tồn tại.");
        if (recipe.Status == RecipeStatus.Published && request.Steps is { Count: 0 })
            return Result<Unit>.Failure("INVALID_STATE", "Công thức đã xuất bản phải có ít nhất một bước.");
        var ingredientIds = request.Ingredients?.Where(i => i.Id.HasValue).Select(i => i.Id!.Value).ToList();
        var stepIds = request.Steps?.Where(s => s.Id.HasValue).Select(s => s.Id!.Value).ToList();
        if (ingredientIds != null && (ingredientIds.Distinct().Count() != ingredientIds.Count || ingredientIds.Any(id => !recipe.Ingredients.Any(i => i.Id == id)))
            || stepIds != null && (stepIds.Distinct().Count() != stepIds.Count || stepIds.Any(id => !recipe.Steps.Any(s => s.Id == id))))
            return Result<Unit>.Failure("INVALID_DATA", "ID nguyên liệu hoặc bước không thuộc công thức này, hoặc bị trùng.");

        recipe.Title = request.Title.Trim();
        recipe.Description = request.Description.Trim();
        if (request.Instructions != null) recipe.Instructions = request.Instructions.Trim();
        if (request.Notes != null) recipe.Notes = request.Notes.Trim();
        recipe.PrepTimeMinutes = request.PrepTimeMinutes;
        recipe.CookTimeMinutes = request.CookTimeMinutes;
        recipe.Servings = request.Servings;
        recipe.Difficulty = request.Difficulty;
        recipe.CategoryId = request.CategoryId;
        recipe.Nutrition = request.Nutrition;
        recipe.UpdatedAt = DateTime.UtcNow; // Forces a root xmin check for all edits.
        if (request.Ingredients != null)
        {
            foreach (var old in recipe.Ingredients.Where(i => !ingredientIds!.Contains(i.Id)).ToList())
                context.RecipeIngredients.Remove(old);
            for (var index = 0; index < request.Ingredients.Count; index++)
            {
                var input = request.Ingredients[index];
                var item = input.Id.HasValue ? recipe.Ingredients.Single(i => i.Id == input.Id) : new RecipeIngredient { RecipeId = recipe.Id };
                item.Name = input.Name.Trim(); item.Quantity = input.Quantity; item.Unit = input.Unit.Trim();
                item.Notes = input.Notes?.Trim(); item.OrderIndex = index + 1;
                if (!input.Id.HasValue) context.RecipeIngredients.Add(item);
            }
        }
        if (request.Steps != null)
        {
            foreach (var old in recipe.Steps.Where(s => !stepIds!.Contains(s.Id)).ToList())
                context.RecipeSteps.Remove(old);
            for (var index = 0; index < request.Steps.Count; index++)
            {
                var input = request.Steps[index];
                var item = input.Id.HasValue ? recipe.Steps.Single(s => s.Id == input.Id) : new RecipeStep { RecipeId = recipe.Id };
                item.StepNumber = index + 1; item.Title = input.Title.Trim(); item.Description = input.Description.Trim();
                item.TimerMinutes = input.TimerMinutes; item.ImageUrl = input.ImageUrl;
                if (!input.Id.HasValue) context.RecipeSteps.Add(item);
            }
        }
        // One EF SaveChanges transaction covers metadata, nutrition, steps and ingredients.
        try { await context.SaveChangesAsync(ct); }
        catch (DbUpdateConcurrencyException) { return Result<Unit>.Failure("CONCURRENCY_CONFLICT", "Công thức đã được thay đổi. Hãy tải lại trước khi lưu."); }
        return Result<Unit>.Success(Unit.Value);
    }
}
