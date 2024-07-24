namespace Comanda.WebApi.Handlers;

public sealed class CategoryListingHandler(
    ICategoryRepository categoryRepository,
    ILogger<CategoryListingHandler> logger
) : IRequestHandler<CategoryListingRequest, Response<IEnumerable<Category>>>
{
    private readonly ICategoryRepository _repository = categoryRepository;
    private readonly ILogger _logger = logger;

    public async Task<Response<IEnumerable<Category>>> Handle(
        CategoryListingRequest request,
        CancellationToken cancellationToken
    )
    {
        var categories = await _repository.RetrieveAllAsync();

        _logger.LogInformation("Categories retrieved successfully.");

        return new Response<IEnumerable<Category>>(
            data: categories,
            statusCode: StatusCodes.Status200OK,
            message: "Categories retrieved successfully."
        );
    }
}