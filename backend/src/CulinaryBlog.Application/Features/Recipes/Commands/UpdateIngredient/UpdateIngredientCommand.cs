using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.Features.Recipes.DTOs;
using MediatR;
using System.Security.Claims;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Common.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Application.Features.Recipes.Commands.UpdateIngredient;

public record UpdateIngredientCommand : IRequest<Result<RecipeIngredientDto>>
{
    public Guid RecipeId { get; init; }
    public Guid IngredientId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public int SortOrder { get; init; }
    public ClaimsPrincipal CurrentUser { get; init; } = default!;
}

public class UpdateIngredientCommandHandler
    : IRequestHandler<UpdateIngredientCommand, Result<RecipeIngredientDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateIngredientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RecipeIngredientDto>> Handle(
        UpdateIngredientCommand request,
        CancellationToken ct)
    {
        var recipe = await _context.Recipes
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.RecipeId, ct);

        if (recipe == null)
            return Result<RecipeIngredientDto>.Failure(
                "NOT_FOUND", "Không tìm thấy công thức.");

        if (!RecipePermission.CanEdit(request.CurrentUser, recipe.AuthorId))
            return Result<RecipeIngredientDto>.Failure(
                "FORBIDDEN", "Bạn không có quyền sửa công thức.");

        var ingredient = await _context.RecipeIngredients
            .FirstOrDefaultAsync(i =>
                i.Id == request.IngredientId &&
                i.RecipeId == request.RecipeId, ct);

        if (ingredient == null)
            return Result<RecipeIngredientDto>.Failure(
                "NOT_FOUND", "Không tìm thấy nguyên liệu.");

        if (string.IsNullOrWhiteSpace(request.Name) || request.Quantity <= 0)
            return Result<RecipeIngredientDto>.Failure(
                "INVALID_DATA", "Tên và số lượng nguyên liệu không hợp lệ.");

        ingredient.Name = request.Name.Trim();
        ingredient.Quantity = request.Quantity;
        ingredient.Unit = request.Unit;
        ingredient.Notes = request.Notes;
        ingredient.OrderIndex = request.SortOrder;
        ingredient.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return Result<RecipeIngredientDto>.Success(
            new RecipeIngredientDto
            {
                Id = ingredient.Id,
                RecipeId = ingredient.RecipeId,
                Name = ingredient.Name,
                Quantity = ingredient.Quantity,
                Unit = ingredient.Unit ?? string.Empty,
                Notes = ingredient.Notes,
                SortOrder = ingredient.OrderIndex
            });
    }
}