using CulinaryBlog.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace CulinaryBlog.Application.Features.Recipes.Commands.RecordRecipeView;
public record RecordRecipeViewCommand(Guid Id) : IRequest<bool>;
public class RecordRecipeViewCommandHandler(IApplicationDbContext db) : IRequestHandler<RecordRecipeViewCommand, bool>
{
    public async Task<bool> Handle(RecordRecipeViewCommand request, CancellationToken ct) =>
        await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE public.recipes SET view_count = view_count + 1 WHERE id = {request.Id} AND status = 'Published' AND NOT is_deleted", ct) > 0;
}
