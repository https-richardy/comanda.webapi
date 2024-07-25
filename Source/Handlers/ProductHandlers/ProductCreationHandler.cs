namespace Comanda.WebApi.Handlers;

public sealed class ProductCreationHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IFileUploadService fileUploadService,
    IValidator<ProductCreationRequest> validator,
    ILogger<ProductCreationHandler> logger
) : IRequestHandler<ProductCreationRequest, Response>
{
    public async Task<Response> Handle(
        ProductCreationRequest request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var category = await categoryRepository.RetrieveByIdAsync(request.CategoryId);
        if (category is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Category not found."
            );

        var product = TinyMapper.Map<Product>(request);

        product.Category = category;
        product.ImagePath = await fileUploadService.UploadFileAsync(request.Image);

        await productRepository.SaveAsync(product);

        logger.LogInformation("Product '{Title}' created successfully.", product.Title);

        return new Response(
            statusCode: StatusCodes.Status201Created,
            message: "Product created successfully."
        );
    }
}