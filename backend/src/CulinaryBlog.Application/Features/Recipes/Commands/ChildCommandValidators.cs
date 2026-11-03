using FluentValidation;
using CulinaryBlog.Application.Features.Recipes.Commands.AddIngredient;
using CulinaryBlog.Application.Features.Recipes.Commands.UpdateIngredient;
using CulinaryBlog.Application.Features.Recipes.Commands.AddStep;
using CulinaryBlog.Application.Features.Recipes.Commands.UpdateStep;
using CulinaryBlog.Application.Features.Recipes.Commands.UpdateRecipe;

namespace CulinaryBlog.Application.Features.Recipes.Commands;
public class AddIngredientValidator : AbstractValidator<AddIngredientCommand>
{
    public AddIngredientValidator() { RuleFor(x => x.Name).NotEmpty().MaximumLength(100); RuleFor(x => x.Unit).NotEmpty().MaximumLength(50); RuleFor(x => x.Quantity).GreaterThan(0).LessThan(1000000); RuleFor(x => x.Notes).MaximumLength(255); }
}
public class UpdateIngredientValidator : AbstractValidator<UpdateIngredientCommand>
{
    public UpdateIngredientValidator() { RuleFor(x => x.Name).NotEmpty().MaximumLength(100); RuleFor(x => x.Unit).NotEmpty().MaximumLength(50); RuleFor(x => x.Quantity).GreaterThan(0).LessThan(1000000); RuleFor(x => x.Notes).MaximumLength(255); }
}
public class AddStepValidator : AbstractValidator<AddRecipeStepCommand>
{
    public AddStepValidator() { RuleFor(x => x.Instruction).NotEmpty().MaximumLength(2000); RuleFor(x => x.Title).MaximumLength(200); RuleFor(x => x.TimerMinutes).InclusiveBetween(0, 10080).When(x => x.TimerMinutes.HasValue); RuleFor(x => x.ImageUrl).Must(UpdateRecipeCommandValidator.IsSafeImage).MaximumLength(500); }
}
public class UpdateStepValidator : AbstractValidator<UpdateRecipeStepCommand>
{
    public UpdateStepValidator() { RuleFor(x => x.Instruction).NotEmpty().MaximumLength(2000); RuleFor(x => x.Title).MaximumLength(200); RuleFor(x => x.TimerMinutes).InclusiveBetween(0, 10080).When(x => x.TimerMinutes.HasValue); RuleFor(x => x.ImageUrl).Must(UpdateRecipeCommandValidator.IsSafeImage).MaximumLength(500); }
}
