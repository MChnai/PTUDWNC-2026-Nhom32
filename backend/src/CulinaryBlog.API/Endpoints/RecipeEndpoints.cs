using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;
using CulinaryBlog.Application.Features.Recipes.Commands.RecordRecipeView;
using CulinaryBlog.API.Models.Requests;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;
using CulinaryBlog.Application.Features.Recipes.Commands.UpdateRecipe;
using CulinaryBlog.Application.Features.Recipes.Commands.ChangeRecipeStatus;
using CulinaryBlog.Application.Features.Recipes.Commands.ArchiveRecipe;
using CulinaryBlog.Application.Features.Recipes.Commands.DeleteRecipe;
using CulinaryBlog.Application.Features.Recipes.Commands.UploadRecipeImage;
using CulinaryBlog.Application.Features.Recipes.Commands.DeleteRecipeImage;
using CulinaryBlog.Application.Features.Recipes.Commands.SetPrimaryImage;
using CulinaryBlog.Application.Features.Recipes.Commands.AddIngredient;
using CulinaryBlog.Application.Features.Recipes.Commands.UpdateIngredient;
using CulinaryBlog.Application.Features.Recipes.Commands.DeleteIngredient;
using CulinaryBlog.Application.Features.Recipes.Commands.AddStep;
using CulinaryBlog.Application.Features.Recipes.Commands.UpdateStep;
using CulinaryBlog.Application.Features.Recipes.Commands.DeleteStep;
using CulinaryBlog.Application.Features.Recipes.DTOs;
using CulinaryBlog.Application.Features.Recipes.Queries.GetRecipes;
using CulinaryBlog.Application.Features.Recipes.Queries.GetRecipeById;
using CulinaryBlog.Application.Features.Recipes.Queries.GetManagedRecipes;
using CulinaryBlog.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.API.Endpoints;

public static class RecipeEndpoints
{
    public static void MapRecipeEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/recipes").WithTags("Recipes");
        g.MapGet("/", List).CacheOutput("public-recipes");
        g.MapGet("/search", List);
        g.MapGet("/manage", async (ClaimsPrincipal user, int? page, int? pageNumber, int? pageSize, RecipeStatus? status,
            Guid? categoryId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetManagedRecipesQuery(user, page ?? pageNumber ?? 1,
                Math.Clamp(pageSize ?? 12, 1, 50), status, categoryId), ct);
            return result.IsSuccess ? Results.Ok(result) : ApiResults.Error(result);
        }).RequireAuthorization("Author");
        g.MapGet("/by-slug/{slug}", async (string slug, ClaimsPrincipal user, IApplicationDbContext db, ISender sender, HttpContext http, CancellationToken ct) =>
        {
            var id = await db.Recipes.Where(r => r.Slug == slug).Select(r => (Guid?)r.Id).FirstOrDefaultAsync(ct);
            return id.HasValue ? await Detail(id.Value, user, sender, http, ct) : Results.NotFound();
        });
        g.MapGet("/{id:guid}", Detail);
        g.MapGet("/{slug}", async (string slug, ClaimsPrincipal user, IApplicationDbContext db, ISender sender, HttpContext http, CancellationToken ct) =>
        {
            var id = await db.Recipes.Where(r => r.Slug == slug).Select(r => (Guid?)r.Id).FirstOrDefaultAsync(ct);
            return id.HasValue ? await Detail(id.Value, user, sender, http, ct) : Results.NotFound();
        });
        g.MapPost("/{id:guid}/view", async (Guid id, HttpContext http, IMemoryCache cache, ISender sender, CancellationToken ct) =>
        {
            var key = $"view:{id}:{http.Connection.RemoteIpAddress}";
            if (cache.TryGetValue(key, out _)) return Results.NoContent();
            if (!await sender.Send(new RecordRecipeViewCommand(id), ct)) return Results.NotFound();
            cache.Set(key, true, TimeSpan.FromMinutes(1));
            return Results.NoContent();
        });
        g.MapPost("/", async (CreateRecipeCommand body, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(body with { AuthorId = UserId(user) }, ct);
            return result.IsSuccess ? Results.Created($"/api/v1/recipes/{result.Value}", result) : ApiResults.Error(result);
        }).RequireAuthorization("Author");
        g.MapPut("/{id:guid}", async (Guid id, UpdateRecipeRequest r, HttpContext http, ISender sender, CancellationToken ct) =>
        {
            if (!uint.TryParse(http.Request.Headers.IfMatch.ToString().Trim('"'), out var version))
                return Results.Problem(statusCode: 428, detail: "Cần If-Match chứa RowVersion để lưu công thức.");
            var result = await sender.Send(new UpdateRecipeCommand { Id = id, Title = r.Title, Description = r.Description,
                Instructions = r.Instructions, Notes = r.Notes, PrepTimeMinutes = r.PrepTimeMinutes, CookTimeMinutes = r.CookTimeMinutes,
                Servings = r.Servings, Difficulty = r.Difficulty, CategoryId = r.CategoryId, RowVersion = version,
                Nutrition = r.Nutrition, Ingredients = r.Ingredients, Steps = r.Steps, CurrentUser = http.User }, ct);
            return result.IsSuccess ? await Detail(id, http.User, sender, http, ct) : ApiResults.Error(result);
        }).RequireAuthorization("Author");
        foreach (var action in new[] { "publish", "unpublish" })
        {
            var target = action == "publish" ? RecipeStatus.Published : RecipeStatus.Draft;
            g.MapPatch($"/{{id:guid}}/{action}", async (Guid id, HttpContext http, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new ChangeRecipeStatusCommand { Id = id, Status = target, CurrentUser = http.User }, ct);
                return result.IsSuccess ? await Detail(id, http.User, sender, http, ct) : ApiResults.Error(result);
            }).RequireAuthorization("Author");
        }
        g.MapPatch("/{id:guid}/status", async (Guid id, ChangeRecipeStatusRequest r, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new ChangeRecipeStatusCommand { Id = id, Status = r.Status, CurrentUser = user }, ct);
            return result.IsSuccess ? Results.NoContent() : ApiResults.Error(result);
        }).RequireAuthorization("Author");
        g.MapPatch("/{id:guid}/archive", async (Guid id, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new ArchiveRecipeCommand { Id = id, IsArchived = true, CurrentUser = user }, ct);
            return result.IsSuccess ? Results.NoContent() : ApiResults.Error(result);
        }).RequireAuthorization("Author");
        g.MapPatch("/{id:guid}/unarchive", async (Guid id, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new ArchiveRecipeCommand { Id = id, IsArchived = false, CurrentUser = user }, ct);
            return result.IsSuccess ? Results.NoContent() : ApiResults.Error(result);
        }).RequireAuthorization("Author");
        g.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteRecipeCommand(id, user), ct);
            return result.IsSuccess ? Results.NoContent() : ApiResults.Error(result);
        }).RequireAuthorization("Author");

        g.MapPost("/{id:guid}/images", async (Guid id, IFormFile file, [FromForm] string? altText,
            [FromForm] bool? isPrimary, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UploadRecipeImageCommand { RecipeId = id, File = file,
                AltText = altText, IsPrimary = isPrimary ?? false, CurrentUser = user }, ct);
            return result.IsSuccess ? Results.Created($"/api/v1/recipes/{id}/images/{result.Value!.Id}", result) : ApiResults.Error(result);
        }).RequireAuthorization("Author").RequireRateLimiting("upload").DisableAntiforgery();
        g.MapPatch("/{id:guid}/images/{imageId:guid}/primary", async (Guid id, Guid imageId, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new SetPrimaryRecipeImageCommand(id, imageId, user), ct);
            return result.IsSuccess ? Results.NoContent() : ApiResults.Error(result);
        }).RequireAuthorization("Author");
        g.MapDelete("/{id:guid}/images/{imageId:guid}", async (Guid id, Guid imageId, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteRecipeImageCommand(id, imageId, user), ct);
            return result.IsSuccess ? Results.NoContent() : ApiResults.Error(result);
        }).RequireAuthorization("Author");

        // Child read endpoints use the same visibility rules as the parent detail.
        g.MapGet("/{id:guid}/ingredients", async (Guid id, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetRecipeByIdQuery(id, user), ct);
            return result.IsSuccess ? Results.Ok(result.Value!.Ingredients) : ApiResults.Error(result);
        });
        g.MapGet("/{id:guid}/steps", async (Guid id, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetRecipeByIdQuery(id, user), ct);
            return result.IsSuccess ? Results.Ok(result.Value!.Steps) : ApiResults.Error(result);
        });
        g.MapPost("/{id:guid}/ingredients", async (Guid id, CreateIngredientRequest r, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new AddIngredientCommand { RecipeId = id, Name = r.Name, Quantity = r.Quantity,
                Unit = r.Unit, Notes = r.Notes, SortOrder = r.SortOrder, CurrentUser = user }, ct);
            return result.IsSuccess ? Results.Created($"/api/v1/recipes/{id}/ingredients/{result.Value!.Id}", result) : ApiResults.Error(result);
        }).RequireAuthorization("Author");
        g.MapPut("/{id:guid}/ingredients/{ingredientId:guid}", async (Guid id, Guid ingredientId, UpdateIngredientRequest r, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdateIngredientCommand { RecipeId = id, IngredientId = ingredientId,
                Name = r.Name, Quantity = r.Quantity, Unit = r.Unit, Notes = r.Notes, SortOrder = r.SortOrder, CurrentUser = user }, ct);
            return result.IsSuccess ? Results.Ok(result) : ApiResults.Error(result);
        }).RequireAuthorization("Author");
        g.MapDelete("/{id:guid}/ingredients/{ingredientId:guid}", async (Guid id, Guid ingredientId, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteIngredientCommand { RecipeId = id, IngredientId = ingredientId, CurrentUser = user }, ct);
            return result.IsSuccess ? Results.NoContent() : ApiResults.Error(result);
        }).RequireAuthorization("Author");
        g.MapPost("/{id:guid}/steps", async (Guid id, CreateRecipeStepRequest r, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var text = string.IsNullOrWhiteSpace(r.Description) ? r.Instruction : r.Description;
            var result = await sender.Send(new AddRecipeStepCommand { RecipeId = id, Title = r.Title, Instruction = text,
                Description = text, TimerMinutes = r.TimerMinutes, ImageUrl = r.ImageUrl, CurrentUser = user }, ct);
            return result.IsSuccess ? Results.Created($"/api/v1/recipes/{id}/steps/{result.Value!.Id}", result) : ApiResults.Error(result);
        }).RequireAuthorization("Author");
        g.MapPut("/{id:guid}/steps/{stepId:guid}", async (Guid id, Guid stepId, UpdateRecipeStepRequest r, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var text = string.IsNullOrWhiteSpace(r.Description) ? r.Instruction : r.Description;
            var result = await sender.Send(new UpdateRecipeStepCommand { RecipeId = id, StepId = stepId, Title = r.Title,
                Description = text, Instruction = text, TimerMinutes = r.TimerMinutes, ImageUrl = r.ImageUrl, CurrentUser = user }, ct);
            return result.IsSuccess ? Results.NoContent() : ApiResults.Error(result);
        }).RequireAuthorization("Author");
        g.MapDelete("/{id:guid}/steps/{stepId:guid}", async (Guid id, Guid stepId, ClaimsPrincipal user, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteRecipeStepCommand { RecipeId = id, StepId = stepId, CurrentUser = user }, ct);
            return result.IsSuccess ? Results.NoContent() : ApiResults.Error(result);
        }).RequireAuthorization("Author");
    }
    private static async Task<IResult> Detail(Guid id, ClaimsPrincipal user, ISender sender, HttpContext http, CancellationToken ct)
    {
        var result = await sender.Send(new GetRecipeByIdQuery(id, user), ct);
        if (!result.IsSuccess) return ApiResults.Error(result);
        http.Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        http.Response.Headers.CacheControl = "no-store";
        return Results.Ok(result);
    }
    private static async Task<IResult> List(HttpRequest req, ISender sender, CancellationToken ct)
    {
        var q = req.Query;
        int? Number(string key) => int.TryParse(q[key], out var x) ? x : null;
        var term = q["q"].FirstOrDefault() ?? q["keyword"].FirstOrDefault() ?? q["searchTerm"].FirstOrDefault();
        var difficulty = q["difficulty"].FirstOrDefault();
        int? difficultyValue = null;
        if (!string.IsNullOrWhiteSpace(difficulty))
        {
            if (!Enum.TryParse<RecipeDifficulty>(difficulty, true, out var parsed) || !Enum.IsDefined(parsed))
                return Results.Problem(statusCode: 422, detail: "Độ khó không hợp lệ.");
            difficultyValue = (int)parsed;
        }
        if (req.Path.Value!.EndsWith("/search") && string.IsNullOrWhiteSpace(term))
            return Results.Problem(statusCode: 422, detail: "Cần từ khóa có ít nhất 2 ký tự.");
        var sort = q["sort"].FirstOrDefault();
        var sortBy = sort?.TrimStart('-') ?? q["sortBy"].FirstOrDefault() ?? (term != null ? "rank" : "createdAt");
        var direction = sort != null ? (sort.StartsWith('-') ? "desc" : "asc") : q["sortDirection"].FirstOrDefault() ?? q["sortOrder"].FirstOrDefault() ?? "desc";
        if (!new[] { "createdat", "publishedat", "title", "cooktime", "totaltime", "views", "rating", "rank", "relevance" }.Contains(sortBy.ToLowerInvariant())
            || direction is not ("asc" or "desc")) return Results.Problem(statusCode: 422, detail: "Tiêu chí sắp xếp không hợp lệ.");
        Guid? categoryId = null;
        if (q.ContainsKey("categoryId"))
        {
            if (!Guid.TryParse(q["categoryId"], out var category)) return Results.Problem(statusCode: 422, detail: "Danh mục không hợp lệ.");
            categoryId = category;
        }
        foreach (var key in new[] { "page", "pageNumber", "pageSize", "maxTotalTime", "maxCookTime", "minServings" })
            if (q.ContainsKey(key) && (!Number(key).HasValue || Number(key) <= 0 || Number(key) > 1000000))
                return Results.Problem(statusCode: 422, detail: $"Tham số {key} phải là số nguyên dương, tối đa 1000000.");
        var result = await sender.Send(new GetRecipesQuery(PageNumber: Number("page") ?? Number("pageNumber") ?? 1,
            PageSize: Number("pageSize") ?? 12, CategoryId: categoryId, Difficulty: difficultyValue,
            MaxTotalTime: Number("maxTotalTime"), SearchTerm: term, SortBy: sortBy, SortDirection: direction,
            MaxCookTime: Number("maxCookTime"), MinServings: Number("minServings")), ct);
        return result.IsSuccess ? Results.Ok(result) : ApiResults.Error(result);
    }
    private static Guid UserId(ClaimsPrincipal user) => Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
