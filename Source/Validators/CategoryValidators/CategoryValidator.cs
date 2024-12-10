namespace Comanda.WebApi.Validators;

public sealed class CategoryValidator :
    AbstractValidator<Category>,
    IValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(category => category.Name)
            .NotEmpty().WithMessage("Category name cannot be empty.")
            .MinimumLength(3).WithMessage("Category name must be at least 3 characters long.")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.")
            .Matches("^[a-zA-Z\\s]*$").WithMessage("Category name can only contain letters and spaces.");
    }
}