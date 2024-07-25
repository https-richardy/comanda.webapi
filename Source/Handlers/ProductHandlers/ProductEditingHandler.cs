namespace Comanda.WebApi.Handlers;

public sealed class ProductEditingHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IFileUploadService fileUploadService,
    IValidator<ProductEditingRequest> validator,
    ILogger<ProductEditingHandler> logger
) : IRequestHandler<ProductEditingRequest, Response>
{
    public async Task<Response> Handle(
        ProductEditingRequest request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var existingProduct = await productRepository.RetrieveByIdAsync(request.ProductId);
        if (existingProduct is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Product not found."
            );

        var category = await categoryRepository.RetrieveByIdAsync(request.CategoryId);
        if (category is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Category not found."
            );

        var updatedProduct = TinyMapper.Map<Product>(request);
        updatedProduct.Category = category;

        if (request.Image is not null)
            updatedProduct.ImagePath = await fileUploadService.UploadFileAsync(request.Image);

        /* If no image is uploaded, keep the existing image path. */
        else
            updatedProduct.ImagePath = existingProduct.ImagePath;

        await productRepository.UpdateAsync(updatedProduct);

        logger.LogInformation("Product '{Title}' updated successfully.", existingProduct.Title);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Product updated successfully."
        );
    }
}