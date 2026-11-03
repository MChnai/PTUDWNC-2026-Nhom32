using System.Security.Claims;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.Common.Authorization;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Application.Features.Recipes.Commands.DeleteStep;

public class DeleteRecipeStepCommand : IRequest<Result<Unit>>
{
    public Guid RecipeId { get; set; }
    public Guid StepId { get; set; }
    public ClaimsPrincipal CurrentUser { get; set; } = null!;
}

public class DeleteRecipeStepCommandHandler : IRequestHandler<DeleteRecipeStepCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public DeleteRecipeStepCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(DeleteRecipeStepCommand request, CancellationToken cancellationToken)
    {
        var recipe = await _context.Recipes.Include(r => r.Steps).FirstOrDefaultAsync(r => r.Id == request.RecipeId, cancellationToken);
        if (recipe == null) return Result<Unit>.Failure("NOT_FOUND", "Không tìm thấy công thức.");
        if (!RecipePermission.CanEdit(request.CurrentUser, recipe.AuthorId))
            return Result<Unit>.Failure("FORBIDDEN", "Bạn không có quyền xóa bước thực hiện.");
        if (recipe.Status == CulinaryBlog.Domain.Enums.RecipeStatus.Published && recipe.Steps.Count <= 1)
            return Result<Unit>.Failure("INVALID_STATE", "Hãy hủy xuất bản trước khi xóa bước cuối cùng.");

        var stepToDelete = await _context.RecipeSteps
            .FirstOrDefaultAsync(s => s.Id == request.StepId && s.RecipeId == request.RecipeId, cancellationToken);

        if (stepToDelete == null)
            return Result<Unit>.Failure("NOT_FOUND", "Không tìm thấy bước thực hiện.");

        var deletedStepNumber = stepToDelete.StepNumber;

        // 1. Xóa bước được chọn
        _context.RecipeSteps.Remove(stepToDelete);

        // 2. Tự động đánh lại số thứ tự (Re-index) cho các bước phía sau
        var subsequentSteps = await _context.RecipeSteps
            .Where(s => s.RecipeId == request.RecipeId && s.StepNumber > deletedStepNumber)
            .ToListAsync(cancellationToken);

        foreach (var step in subsequentSteps)
        {
            step.StepNumber -= 1;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}