namespace Comanda.WebApi.Handlers;

public sealed class ProductCreationHandler(
    IProductRepository productRepository,
    IIngredientRepository ingredientRepository,
    ICategoryRepository categoryRepository,
    IValidator<ProductCreationRequest> validator,
    ILogger<ProductCreationHandler> logger
) : IRequestHandler<ProductCreationRequest, Response<ProductCreationResponse>>
{
    public async Task<Response<ProductCreationResponse>> Handle(
        ProductCreationRequest request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse<ProductCreationResponse>(errors: validationResult.Errors);

        var category = await categoryRepository.RetrieveByIdAsync(request.CategoryId);
        if (category is null)
            return new Response<ProductCreationResponse>(
                data: null,
                statusCode: StatusCodes.Status404NotFound,
                message: "Category not found."
            );

        var product = TinyMapper.Map<Product>(request);
        product.Category = category;

        await productRepository.SaveAsync(product);

        var ingredients = new List<ProductIngredient>();
        foreach (var payload in request.Ingredients)
        {
            var ingredient = await ingredientRepository.RetrieveByIdAsync(payload.IngredientId);
            if (ingredient is null)
                return new Response<ProductCreationResponse>(
                    data: null,
                    statusCode: StatusCodes.Status404NotFound,
                    message: $"Ingredient with ID '{payload.IngredientId}' not found."
                );

            ingredients.Add(new ProductIngredient
            {
                Product = product,
                Ingredient = ingredient,
                StandardQuantity = payload.StandardQuantity
            });
        }

        foreach (var ingredient in ingredients)
        {
            product.Ingredients.Add(ingredient);
        }

        await productRepository.UpdateAsync(product);

        logger.LogInformation("Product '{Title}' created successfully.", product.Title);

        return new Response<ProductCreationResponse>(
            data: new ProductCreationResponse { ProductId = product.Id },
            statusCode: StatusCodes.Status201Created,
            message: "Product created successfully."
        );
    }
}