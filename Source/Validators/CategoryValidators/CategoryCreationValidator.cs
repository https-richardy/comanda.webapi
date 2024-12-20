namespace Comanda.WebApi.Validators;

public sealed class CategoryCreationValidator :
    AbstractValidator<CategoryCreationRequest>, IValidator<CategoryCreationRequest>
{
    public CategoryCreationValidator()
    {
        RuleFor(category => category.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters.")
            .MaximumLength(50).WithMessage("Title must be at most 50 characters.");
    }
}