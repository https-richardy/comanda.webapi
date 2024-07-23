namespace Comanda.WebApi.Validators;

public sealed class CategoryEditingValidator :
    AbstractValidator<CategoryEditingRequest>, IValidator<CategoryEditingRequest>
{
    public CategoryEditingValidator()
    {
        RuleFor(category => category.Title)
            .NotEmpty().WithMessage("Title is required.");

        RuleFor(category => category.Title)
            .MinimumLength(3).WithMessage("Title must be at least 3 characters.");

        RuleFor(category => category.CategoryId)
            .GreaterThan(0).WithMessage("Category ID must be greater than 0.");
    }
}