using FluentValidation;
using CulinaryBlog.Application.Features.Recipes.Commands.UpdateRecipe;

namespace CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;

public class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().Length(5, 200);
        RuleFor(x => x.Description).MaximumLength(10000);
        RuleFor(x => x.Instructions).MaximumLength(20000);
        RuleFor(x => x.Notes).MaximumLength(4000);
        RuleFor(x => x.PrepTimeMinutes).InclusiveBetween(0, 10080);
        RuleFor(x => x.CookTimeMinutes).InclusiveBetween(0, 10080);
        RuleFor(x => x.Servings).InclusiveBetween(1, 1000);
        RuleFor(x => x.Difficulty).InclusiveBetween(1, 3);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Ingredients).Must(x => x == null || x.Count <= 100);
        RuleFor(x => x.Steps).Must(x => x == null || x.Count <= 100);
        RuleForEach(x => x.Ingredients).ChildRules(i =>
        {
            i.RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            i.RuleFor(x => x.Quantity).GreaterThan(0).LessThan(1000000);
            i.RuleFor(x => x.Unit).NotEmpty().MaximumLength(50);
            i.RuleFor(x => x.Notes).MaximumLength(255);
        });
        RuleForEach(x => x.Steps).ChildRules(s =>
        {
            s.RuleFor(x => x.Title).MaximumLength(200);
            s.RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
            s.RuleFor(x => x.TimerMinutes).InclusiveBetween(0, 10080).When(x => x.TimerMinutes.HasValue);
            s.RuleFor(x => x.ImageUrl).Must(UpdateRecipeCommandValidator.IsSafeImage).MaximumLength(500);
        });
        When(x => x.Nutrition != null, () =>
        {
            RuleFor(x => x.Nutrition!.Calories).InclusiveBetween(0, 100000);
            RuleFor(x => x.Nutrition!.Protein).InclusiveBetween(0, 9999);
            RuleFor(x => x.Nutrition!.Carbohydrates).InclusiveBetween(0, 9999);
            RuleFor(x => x.Nutrition!.Fat).InclusiveBetween(0, 9999);
            RuleFor(x => x.Nutrition!.Fiber).InclusiveBetween(0, 9999).When(x => x.Nutrition!.Fiber.HasValue);
            RuleFor(x => x.Nutrition!.Sugar).InclusiveBetween(0, 9999).When(x => x.Nutrition!.Sugar.HasValue);
        });
    }
}
