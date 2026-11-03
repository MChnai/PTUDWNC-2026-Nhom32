using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.Features.Categories.DTOs;
using CulinaryBlog.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace CulinaryBlog.Application.Features.Categories.Queries.GetCategories;
public record GetCategoriesQuery : IRequest<Result<List<CategoryDto>>>;
public class GetCategoriesQueryHandler(IApplicationDbContext db) : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
{
    public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken ct) =>
        Result<List<CategoryDto>>.Success(await db.Categories.AsNoTracking().OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Slug, c.Description,
                c.Recipes.Count(r => r.Status == RecipeStatus.Published))).ToListAsync(ct));
}
