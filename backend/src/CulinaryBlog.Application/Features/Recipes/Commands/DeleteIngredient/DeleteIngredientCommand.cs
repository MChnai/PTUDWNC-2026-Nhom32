using CulinaryBlog.Application.Common.Models;
using MediatR;
using System.Security.Claims;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Common.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Application.Features.Recipes.Commands.DeleteIngredient;

public record DeleteIngredientCommand : IRequest<Result<bool>>
{
    public Guid RecipeId { get; init; }
    public Guid IngredientId { get; init; }
    public ClaimsPrincipal CurrentUser { get; init; } = default!;
}

public class DeleteIngredientCommandHandler
    : IRequestHandler<DeleteIngredientCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteIngredientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(
        DeleteIngredientCommand request,
        CancellationToken ct)
    {
        var recipe = await _context.Recipes
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.RecipeId, ct);

        if (recipe == null)
            return Result<bool>.Failure(
                "NOT_FOUND", "Không tìm thấy công thức.");

        if (!RecipePermission.CanEdit(request.CurrentUser, recipe.AuthorId))
            return Result<bool>.Failure(
                "FORBIDDEN", "Bạn không có quyền sửa công thức.");

        var ingredient = await _context.RecipeIngredients
            .FirstOrDefaultAsync(i =>
                i.Id == request.IngredientId &&
                i.RecipeId == request.RecipeId, ct);

        if (ingredient == null)
            return Result<bool>.Failure(
                "NOT_FOUND", "Không tìm thấy nguyên liệu.");

        _context.RecipeIngredients.Remove(ingredient);
        await _context.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }
}