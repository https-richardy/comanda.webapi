namespace Comanda.WebApi.Handlers;

public sealed class CategoryCreationHandler(
    ICategoryRepository categoryRepository,
    IValidator<CategoryCreationRequest> validator,
    ILogger<CategoryCreationHandler> logger
) : IRequestHandler<CategoryCreationRequest, Response>
{
    private readonly ICategoryRepository _repository = categoryRepository;
    private readonly IValidator<CategoryCreationRequest> _validator = validator;
    private readonly ILogger _logger = logger;

    public async Task<Response> Handle(
        CategoryCreationRequest request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var category = new Category { Name = request.Title };
        await _repository.SaveAsync(category);

        _logger.LogInformation("Category '{Title}' created successfully.", request.Title);

        return new Response(
            statusCode: StatusCodes.Status201Created,
            message: "Category created successfully."
        );
    }
}