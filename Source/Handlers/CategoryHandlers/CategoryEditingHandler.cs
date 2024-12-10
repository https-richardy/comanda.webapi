namespace Comanda.WebApi.Handlers;

public sealed class CategoryEditingHandler :
    IRequestHandler<CategoryEditingRequest, Response>
{
    private readonly ICategoryManager _categoryManager;
    private readonly IValidator<CategoryEditingRequest> _validator;
    private readonly ILogger _logger;

    public CategoryEditingHandler(
        ICategoryManager categoryManager,
        IValidator<CategoryEditingRequest> validator,
        ILogger<CategoryEditingHandler> logger
    )
    {
        _categoryManager = categoryManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Response> Handle(
        CategoryEditingRequest request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var category = await _categoryManager.GetAsync(request.CategoryId);
        if (category is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Category not found."
            );

        category.Name = request.Title;
        await _categoryManager.UpdateAsync(category);

        _logger.LogInformation("Category '{Title}' updated successfully.", request.Title);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Category updated successfully."
        );
    }
}