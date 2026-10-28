using System.Security.Claims;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.Common.Authorization;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Application.Features.Recipes.Commands.UpdateStep;

public class UpdateRecipeStepCommand : IRequest<Result<Unit>>
{
    public Guid RecipeId { get; set; }
    public Guid StepId { get; set; }
    public string Instruction { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? TimerMinutes { get; set; }
    public ClaimsPrincipal CurrentUser { get; set; } = null!;
}

public class UpdateRecipeStepCommandHandler : IRequestHandler<UpdateRecipeStepCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public UpdateRecipeStepCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(UpdateRecipeStepCommand request, CancellationToken cancellationToken)
    {
        var recipe = await _context.Recipes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                r => r.Id == request.RecipeId,
                cancellationToken
            );

        if (recipe == null)
        {
            return Result<Unit>.Failure(
                "NOT_FOUND",
                "Không tìm thấy công thức."
            );
        }

        if (!RecipePermission.CanEdit(
            request.CurrentUser,
            recipe.AuthorId))
        {
            return Result<Unit>.Failure(
                "FORBIDDEN",
                "Bạn không có quyền sửa bước thực hiện."
            );
        }
        var step = await _context.RecipeSteps
            .FirstOrDefaultAsync(s => s.Id == request.StepId && s.RecipeId == request.RecipeId, cancellationToken);

        if (step == null)
            return Result<Unit>.Failure("NOT_FOUND", "Không tìm thấy bước thực hiện.");

        step.Title = request.Title.Trim();
        step.Description = request.Instruction;
        step.TimerMinutes = request.TimerMinutes;
        step.ImageUrl = request.ImageUrl;
        step.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}