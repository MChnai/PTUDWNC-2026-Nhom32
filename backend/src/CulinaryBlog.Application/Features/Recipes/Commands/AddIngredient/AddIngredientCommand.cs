using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.Features.Recipes.DTOs;
using MediatR;
using System.Security.Claims;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Common.Authorization;
using CulinaryBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Application.Features.Recipes.Commands.AddIngredient;

public record AddIngredientCommand : IRequest<Result<RecipeIngredientDto>>
{
    public Guid RecipeId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public int SortOrder { get; init; }
    public ClaimsPrincipal CurrentUser { get; init; } = default!;
}

public class AddIngredientCommandHandler
    : IRequestHandler<AddIngredientCommand, Result<RecipeIngredientDto>>
{
    private readonly IApplicationDbContext _context;

    public AddIngredientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RecipeIngredientDto>> Handle(
        AddIngredientCommand request,
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

        if (string.IsNullOrWhiteSpace(request.Name) || request.Quantity <= 0)
            return Result<RecipeIngredientDto>.Failure(
                "INVALID_DATA", "Tên và số lượng nguyên liệu không hợp lệ.");

        var ingredient = new RecipeIngredient
        {
            RecipeId = request.RecipeId,
            Name = request.Name.Trim(),
            Quantity = request.Quantity,
            Unit = request.Unit,
            Notes = request.Notes,
            OrderIndex = request.SortOrder
        };

        _context.RecipeIngredients.Add(ingredient);
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