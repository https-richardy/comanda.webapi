namespace Comanda.WebApi.Handlers;

public sealed class CategoryEditingHandler(
    ICategoryRepository categoryRepository,
    IValidator<CategoryEditingRequest> validator,
    ILogger<CategoryEditingHandler> logger
) : IRequestHandler<CategoryEditingRequest, Response>
{
    private readonly ICategoryRepository _repository = categoryRepository;
    private readonly IValidator<CategoryEditingRequest> _validator = validator;
    private readonly ILogger _logger = logger;

    public async Task<Response> Handle(
        CategoryEditingRequest request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var category = await _repository.RetrieveByIdAsync(request.CategoryId);
        if (category is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Category not found."
            );

        category.Name = request.Title;
        await _repository.UpdateAsync(category);

        _logger.LogInformation("Category '{Title}' updated successfully.", request.Title);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Category updated successfully."
        );
    }
}