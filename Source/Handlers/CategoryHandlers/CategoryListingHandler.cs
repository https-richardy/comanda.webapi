namespace Comanda.WebApi.Handlers;

public sealed class CategoryListingHandler(
    ICategoryRepository categoryRepository,
    ILogger<CategoryListingHandler> logger
) : IRequestHandler<CategoryListingRequest, Response<IEnumerable<FormattedCategory>>>
{
    private readonly ICategoryRepository _repository = categoryRepository;
    private readonly ILogger _logger = logger;

    public async Task<Response<IEnumerable<FormattedCategory>>> Handle(
        CategoryListingRequest request,
        CancellationToken cancellationToken
    )
    {
        var categories = await _repository.RetrieveAllAsync();
        var payload = categories.Select(category => TinyMapper.Map<FormattedCategory>(category));

        return new Response<IEnumerable<FormattedCategory>>(
            data: payload,
            statusCode: StatusCodes.Status200OK,
            message: "Categories retrieved successfully."
        );
    }
}