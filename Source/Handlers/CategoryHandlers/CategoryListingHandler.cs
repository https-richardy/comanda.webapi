namespace Comanda.WebApi.Handlers;

public sealed class CategoryListingHandler :
    IRequestHandler<CategoryListingRequest, Response<IEnumerable<FormattedCategory>>>
{
    private readonly ICategoryManager _categoryManager;

    public CategoryListingHandler(ICategoryManager categoryManager)
    {
        _categoryManager = categoryManager;
    }

    public async Task<Response<IEnumerable<FormattedCategory>>> Handle(
        CategoryListingRequest request,
        CancellationToken cancellationToken
    )
    {
        var categories = await _categoryManager.GetAllAsync();
        var payload = categories.Select(category => TinyMapper.Map<FormattedCategory>(category));

        return new Response<IEnumerable<FormattedCategory>>(
            data: payload,
            statusCode: StatusCodes.Status200OK,
            message: "Categories retrieved successfully."
        );
    }
}