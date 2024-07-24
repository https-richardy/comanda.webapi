namespace Comanda.WebApi.Handlers;

public sealed class CategoryDeletionHandler(
    ICategoryRepository categoryRepository,
    ILogger<CategoryDeletionHandler> logger
) : IRequestHandler<CategoryDeletionRequest, Response>
{
    private readonly ICategoryRepository _repository = categoryRepository;
    private readonly ILogger _logger = logger;

    public async Task<Response> Handle(
        CategoryDeletionRequest request,
        CancellationToken cancellationToken
    )
    {
        var category = await _repository.RetrieveByIdAsync(request.CategoryId);
        if (category is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Category not found."
            );

        await _repository.DeleteAsync(category);
        _logger.LogInformation("Category '{Title}' deleted successfully.", category.Name);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Category deleted successfully."
        );
    }
}