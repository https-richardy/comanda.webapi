namespace Comanda.WebApi.Handlers;

public sealed class ProductImageUploadHandler(
    IProductRepository productRepository,
    IFileUploadService fileUploadService
) : IRequestHandler<ProductImageUploadRequest, Response>
{
    public async Task<Response> Handle(
        ProductImageUploadRequest request,
        CancellationToken cancellationToken
    )
    {
        var product = await productRepository.RetrieveByIdAsync(request.ProductId);
        if (product is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Product not found."
            );

        var imagePath = await fileUploadService.UploadFileAsync(request.Image);
        product.ImagePath = imagePath;

        await productRepository.UpdateAsync(product);
        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Product image uploaded successfully."
        );
    }
}