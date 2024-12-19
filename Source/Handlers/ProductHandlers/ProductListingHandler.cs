namespace Comanda.WebApi.Handlers;

public sealed class ProductListingHandler(
    IProductRepository productRepository,
    IHttpContextAccessor contextAccessor
) : IRequestHandler<ProductListingRequest, Response<PaginationHelper<FormattedProduct>>>
{
    #pragma warning disable CS8604, CS8600
    public async Task<Response<PaginationHelper<FormattedProduct>>> Handle(
        ProductListingRequest request,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<Product> products;
        Expression<Func<Product, bool>> predicate = product => product.IsDeleted == false;

        if (!string.IsNullOrEmpty(request.Title))
        {
            var title = request.Title
                .Trim()
                .ToLowerInvariant();

            predicate = predicate.AndAlso(product => EF.Functions.Like(
                product.Title.ToLower(),
                $"%{title}%"
            ));
        }

        if (request.MinPrice.HasValue)
        {
            predicate = predicate.AndAlso(product => product.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            predicate = predicate.AndAlso(product => product.Price <= request.MaxPrice.Value);
        }

        products = await productRepository.PagedAsync(
            predicate: predicate,
            pageNumber: request.Page,
            pageSize: request.PageSize
        );

        var formattedProducts = products
            .Select(product => (FormattedProduct) product)
            .ToList();

        var pagination = new PaginationHelper<FormattedProduct>(
            data: formattedProducts,
            pageNumber: request.Page,
            pageSize: request.PageSize,
            httpContext: contextAccessor.HttpContext
        );

        return new Response<PaginationHelper<FormattedProduct>>(
            data: pagination,
            statusCode: StatusCodes.Status200OK,
            message: "products retrieved successfully."
        );
    }
}