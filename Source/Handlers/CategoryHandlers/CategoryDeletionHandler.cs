namespace Comanda.WebApi.Handlers;

public sealed class CategoryDeletionHandler :
    IRequestHandler<CategoryDeletionRequest, Response>
{
    private readonly ICategoryManager _categoryManager;
    private readonly ILogger _logger;

    public CategoryDeletionHandler(
        ICategoryManager categoryManager,
        ILogger<CategoryDeletionHandler> logger
    )
    {
        _categoryManager = categoryManager;
        _logger = logger;
    }

    public async Task<Response> Handle(
        CategoryDeletionRequest request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryManager.GetAsync(request.CategoryId);
        if (category is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Category not found."
            );

        await _categoryManager.DeleteAsync(category);
        _logger.LogInformation("Category '{Title}' deleted successfully.", category.Name);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Category deleted successfully."
        );
    }
}