namespace Comanda.WebApi.Validators;

public sealed class ProductCreationValidator : AbstractValidator<ProductCreationRequest>
{
    public ProductCreationValidator()
    {
        RuleFor(product => product.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters.")
            .MaximumLength(100).WithMessage("Title must be at most 100 characters.");

        RuleFor(product => product.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters.")
            .MaximumLength(120).WithMessage("Description must be at most 120 characters.");

        RuleFor(product => product.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.")
            .LessThan(1000).WithMessage("Price must be less than 1000.");

        RuleFor(product => product.CategoryId)
            .GreaterThan(0).WithMessage("Category ID must be greater than 0.");

        RuleFor(product => product.Ingredients)
            .Must(ingredients => ingredients.All(ingredient => ingredient.StandardQuantity > 0))
            .WithMessage("The ingredients must have a standard quantity greater than zero.");
    }
}