namespace CulinaryBlog.Application.Features.Recipes.DTOs;

public class CreateRecipeStepRequest
{
    public string Instruction { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? TimerMinutes { get; set; }
    public string? ImageUrl { get; set; }
}
