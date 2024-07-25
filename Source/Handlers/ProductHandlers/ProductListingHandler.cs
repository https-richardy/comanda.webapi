namespace Comanda.WebApi.Handlers;

public sealed class ProductListingHandler(
    IProductRepository productRepository,
    IHttpContextAccessor contextAccessor
) : IRequestHandler<ProductListingRequest, Response<PaginationHelper<Product>>>
{
    #pragma warning disable CS8604, CS8600
    public async Task<Response<PaginationHelper<Product>>> Handle(
        ProductListingRequest request,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<Product> products;
        Expression<Func<Product, bool>> predicate = null;

        if (!string.IsNullOrEmpty(request.Title))
        {
            var title = request.Title
                .Trim()
                .Normalize(NormalizationForm.FormD)
                .ToLowerInvariant();

            predicate = predicate ??= product => product.Title.Contains(title);
        }

        if (request.MinPrice.HasValue)
            predicate = predicate ??= product => product.Price >= request.MinPrice.Value;

        if (request.MaxPrice.HasValue)
            predicate = predicate ??= product => product.Price <= request.MaxPrice.Value;

        if (predicate is not null)
        {
            products = await productRepository.PagedAsync(
                predicate: predicate,
                pageNumber: request.Page,
                pageSize: request.PageSize
            );
        }
        else
        {
            products = await productRepository.PagedAsync(
                pageNumber: request.Page,
                pageSize: request.PageSize
            );
        }

        var pagination = new PaginationHelper<Product>(
            data: products,
            pageNumber: request.Page,
            pageSize: request.PageSize,
            httpContext: contextAccessor.HttpContext
        );

        return new Response<PaginationHelper<Product>>(
            data: pagination,
            statusCode: StatusCodes.Status200OK,
            message: "products retrieved successfully."
        );
    }
}