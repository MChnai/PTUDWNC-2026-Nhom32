using CulinaryBlog.Application.Features.Categories.Commands.CreateCategory;
using CulinaryBlog.Application.Features.Categories.Commands.UpdateCategory;
using FluentValidation;
namespace CulinaryBlog.Application.Features.Categories.Commands;
public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{ public CreateCategoryValidator() { RuleFor(x => x.Name).NotEmpty().Length(2, 100); RuleFor(x => x.Description).MaximumLength(2000); } }
public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{ public UpdateCategoryValidator() { RuleFor(x => x.Name).NotEmpty().Length(2, 100); RuleFor(x => x.Description).MaximumLength(2000); } }
