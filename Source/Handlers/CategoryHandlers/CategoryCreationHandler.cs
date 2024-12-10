namespace Comanda.WebApi.Handlers;

public sealed class CategoryCreationHandler :
    IRequestHandler<CategoryCreationRequest, Response>
{
    private readonly ICategoryManager _categoryManager;
    private readonly IValidator<CategoryCreationRequest> _validator;
    private readonly ILogger _logger;

    public CategoryCreationHandler(
        ICategoryManager categoryManager,
        IValidator<CategoryCreationRequest> validator,
        ILogger<CategoryCreationHandler> logger
    )
    {
        _categoryManager = categoryManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Response> Handle(
        CategoryCreationRequest request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var category = new Category { Name = request.Title };
        await _categoryManager.CreateAsync(category);

        _logger.LogInformation("Category '{Title}' created successfully.", request.Title);

        return new Response(
            statusCode: StatusCodes.Status201Created,
            message: "Category created successfully."
        );
    }
}