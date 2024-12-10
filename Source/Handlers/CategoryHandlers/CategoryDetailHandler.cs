namespace Comanda.WebApi.Handlers;

public sealed class CategoryDetailHandler :
    IRequestHandler<CategoryDetailRequest, Response<Category>>
{
    private readonly ICategoryManager _categoryManager;
    private readonly ILogger _logger;

    public CategoryDetailHandler(
        ICategoryManager categoryManager,
        ILogger<CategoryDetailHandler> logger
    )
    {
        _categoryManager = categoryManager;
        _logger = logger;
    }

    public async Task<Response<Category>> Handle(
        CategoryDetailRequest request,
        CancellationToken cancellationToken
    )
    {
        var category = await _categoryManager.GetAsync(request.CategoryId);
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