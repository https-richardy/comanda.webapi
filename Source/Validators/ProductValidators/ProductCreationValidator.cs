namespace Comanda.WebApi.Validators;

public sealed class ProductCreationValidator : AbstractValidator<ProductCreationRequest>
{
    public ProductCreationValidator()
    {
        RuleFor(product => product.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters.");

        RuleFor(product => product.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters.");

        RuleFor(product => product.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(product => product.CategoryId)
            .GreaterThan(0).WithMessage("Category ID must be greater than 0.");
    }
}