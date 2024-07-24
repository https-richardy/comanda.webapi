namespace Comanda.WebApi.Handlers;

public sealed class CategoryDetailHandler(
    ICategoryRepository categoryRepository,
    ILogger<CategoryDetailHandler> logger
) : IRequestHandler<CategoryDetailRequest, Response<Category>>
{
    private readonly ICategoryRepository _repository = categoryRepository;
    private readonly ILogger _logger = logger;

    public async Task<Response<Category>> Handle(
        CategoryDetailRequest request,
        CancellationToken cancellationToken
    )
    {
        var category = await _repository.RetrieveByIdAsync(request.CategoryId);
        if (category is null)
            return new Response<Category>(
                data: null,
                statusCode: StatusCodes.Status404NotFound,
                message: "Category not found."
            );

        _logger.LogInformation("Category '{Title}' retrieved successfully.", category.Name);

        return new Response<Category>(
            data: category,
            statusCode: StatusCodes.Status200OK,
            message: "Category retrieved successfully."
        );
    }
}