namespace Comanda.WebApi.Handlers;

public sealed class ProductDetailHandler(
    IProductRepository repository
) : IRequestHandler<ProductDetailRequest, Response<Product>>
{
    public async Task<Response<Product>> Handle(
        ProductDetailRequest request,
        CancellationToken cancellationToken
    )
    {
        var product = await repository.RetrieveByIdAsync(request.ProductId);
        if (product is null)
            return new Response<Product>(
                data: null,
                statusCode: StatusCodes.Status404NotFound,
                message: "Product not found."
            );

        return new Response<Product>(
            data: product,
            statusCode: StatusCodes.Status200OK,
            message: "Product retrieved successfully."
        );
    }
}