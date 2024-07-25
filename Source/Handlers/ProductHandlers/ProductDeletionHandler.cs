namespace Comanda.WebApi.Handlers;

public sealed class ProductDeletionHandler(
    IProductRepository productRepository,
    ILogger<ProductDeletionHandler> logger
) : IRequestHandler<ProductDeletionRequest, Response>
{
    public async Task<Response> Handle(
        ProductDeletionRequest request,
        CancellationToken cancellationToken
    )
    {
        var product = await productRepository.RetrieveByIdAsync(request.ProductId);
        if (product is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Product not found."
            );

        await productRepository.DeleteAsync(product);
        logger.LogInformation("Product '{Title}' deleted successfully.", product.Title);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Product deleted successfully."
        );
    }
}