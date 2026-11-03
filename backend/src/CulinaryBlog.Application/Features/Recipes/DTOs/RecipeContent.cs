namespace CulinaryBlog.Application.Features.Recipes.DTOs;

public record IngredientInput(Guid? Id, string Name, decimal Quantity, string Unit, string? Notes, int OrderIndex);
public record StepInput(Guid? Id, string Title, string Description, int? TimerMinutes, string? ImageUrl);
