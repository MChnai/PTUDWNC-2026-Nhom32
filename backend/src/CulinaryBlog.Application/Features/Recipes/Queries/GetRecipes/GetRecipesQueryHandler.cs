using System.Text.RegularExpressions;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.Features.Recipes.DTOs;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetRecipes;

public class GetRecipesQueryHandler(IApplicationDbContext context) : IRequestHandler<GetRecipesQuery, Result<PaginatedList<RecipeDto>>>
{
    public async Task<Result<PaginatedList<RecipeDto>>> Handle(GetRecipesQuery request, CancellationToken ct)
    {
        var page = Math.Clamp(request.PageNumber, 1, 1000000);
        var size = Math.Clamp(request.PageSize, 1, 50);
        var term = (request.SearchTerm ?? request.Keyword)?.Trim();
        IQueryable<Recipe> query = context.Recipes.AsNoTracking().Where(r => r.Status == RecipeStatus.Published);
        Dictionary<Guid, float>? ranks = null;
        if (!string.IsNullOrWhiteSpace(term))
        {
            if (term.Length < 2 || term.Length > 200)
                return Result<PaginatedList<RecipeDto>>.Failure("INVALID_QUERY", "Từ khóa cần từ 2 đến 200 ký tự.");
            // Tokenize before to_tsquery; SQL parameters never contain SQL syntax.
            var tokens = Regex.Matches(term, @"[\p{L}\p{N}]+", RegexOptions.None, TimeSpan.FromSeconds(1))
                .Select(m => m.Value + ":*").Take(20).ToArray();
            if (tokens.Length == 0) return Result<PaginatedList<RecipeDto>>.Failure("INVALID_QUERY", "Từ khóa cần chứa chữ hoặc số.");
            var tsquery = string.Join(" & ", tokens);
            // Rank is computed in PostgreSQL, without the old function's 1000-row truncation.
            var matches = await context.Database.SqlQuery<SearchHit>($"""
                SELECT id AS "Id", ts_rank(search_vector, to_tsquery('public.vietnamese_unaccent', unaccent({tsquery}))) AS "Rank"
                FROM public.recipes
                WHERE status = 'Published' AND NOT is_deleted
                   AND search_vector @@ to_tsquery('public.vietnamese_unaccent', unaccent({tsquery}))
                """).ToListAsync(ct);
            ranks = matches.ToDictionary(x => x.Id, x => x.Rank);
            var ids = ranks.Keys.ToArray();
            query = query.Where(r => ids.Contains(r.Id));
        }
        if (request.CategoryId.HasValue) query = query.Where(r => r.CategoryId == request.CategoryId);
        if (request.Difficulty.HasValue) query = query.Where(r => r.Difficulty == (RecipeDifficulty)request.Difficulty);
        if (request.MaxTotalTime.HasValue) query = query.Where(r => r.PrepTimeMinutes + r.CookTimeMinutes <= request.MaxTotalTime);
        if (request.MaxCookTime.HasValue) query = query.Where(r => r.CookTimeMinutes <= request.MaxCookTime);
        if (request.MinServings.HasValue) query = query.Where(r => r.Servings >= request.MinServings);
        var asc = request.SortDirection?.Equals("asc", StringComparison.OrdinalIgnoreCase) == true;
        if (ranks != null && request.SortBy is "rank" or "relevance")
        {
            var data = await query.Select(RecipeProjection.Summary).ToListAsync(ct);
            var ordered = asc ? data.OrderBy(x => ranks.GetValueOrDefault(x.Id)) : data.OrderByDescending(x => ranks.GetValueOrDefault(x.Id));
            var items = ordered.ThenBy(x => x.Id).Skip((page - 1) * size).Take(size).ToList();
            foreach (var item in items) item.RelevanceScore = ranks.GetValueOrDefault(item.Id);
            return Result<PaginatedList<RecipeDto>>.Success(new(items, data.Count, page, size));
        }
        IOrderedQueryable<Recipe> sorted = request.SortBy?.ToLowerInvariant() switch
        {
            "title" => asc ? query.OrderBy(r => r.Title) : query.OrderByDescending(r => r.Title),
            "views" => asc ? query.OrderBy(r => r.ViewCount) : query.OrderByDescending(r => r.ViewCount),
            "rating" => asc ? query.OrderBy(r => r.RatingAverage) : query.OrderByDescending(r => r.RatingAverage),
            "cooktime" => asc ? query.OrderBy(r => r.CookTimeMinutes) : query.OrderByDescending(r => r.CookTimeMinutes),
            "totaltime" => asc ? query.OrderBy(r => r.PrepTimeMinutes + r.CookTimeMinutes) : query.OrderByDescending(r => r.PrepTimeMinutes + r.CookTimeMinutes),
            _ => asc ? query.OrderBy(r => r.CreatedAt) : query.OrderByDescending(r => r.CreatedAt)
        };
        var count = await query.CountAsync(ct);
        var list = await sorted.ThenBy(r => r.Id).Select(RecipeProjection.Summary).Skip((page - 1) * size).Take(size).ToListAsync(ct);
        return Result<PaginatedList<RecipeDto>>.Success(new(list, count, page, size));
    }
    public class SearchHit { public Guid Id { get; set; } public float Rank { get; set; } }
}
