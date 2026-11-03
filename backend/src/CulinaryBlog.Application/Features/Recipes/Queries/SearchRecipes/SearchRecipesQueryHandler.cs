using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.Features.Recipes.DTOs;
using CulinaryBlog.Application.Features.Recipes.Queries.GetRecipes;
using MediatR;
namespace CulinaryBlog.Application.Features.Recipes.Queries.SearchRecipes;
public class SearchRecipesQueryHandler(ISender sender) : IRequestHandler<SearchRecipesQuery, Result<PaginatedList<RecipeDto>>>
{
    public Task<Result<PaginatedList<RecipeDto>>> Handle(SearchRecipesQuery q, CancellationToken ct) => sender.Send(
        new GetRecipesQuery(PageNumber: q.Page, PageSize: q.PageSize, CategoryId: q.CategoryId, Difficulty: q.Difficulty,
            MaxTotalTime: q.MaxTotalTime, SearchTerm: q.Keyword, SortBy: q.SortBy, SortDirection: q.SortDirection), ct);
}
