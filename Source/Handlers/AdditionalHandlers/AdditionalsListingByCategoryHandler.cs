namespace Comanda.WebApi.Handlers;

public sealed class AdditionalsListingByCategoryHandler(
    IAdditionalRepository additionalRepository,
    ICategoryRepository categoryRepository
) : IRequestHandler<AdditionalsListingByCategoryRequest, Response<IEnumerable<Additional>>>
{
    public async Task<Response<IEnumerable<Additional>>> Handle(
        AdditionalsListingByCategoryRequest request,
        CancellationToken cancellationToken
    )
    {
        var category = await categoryRepository.RetrieveByIdAsync(request.CategoryId);
        if (category is null)
            return new Response<IEnumerable<Additional>>(
                data: null,
                statusCode: StatusCodes.Status404NotFound,
                message: "Category not found."
            );

        var additionals = await additionalRepository.FindAllAsync(additional => additional.Category.Id == request.CategoryId);

        return new Response<IEnumerable<Additional>>(
            data: additionals,
            statusCode: StatusCodes.Status200OK,
            message: "Additionals retrieved successfully."
        );
    }
}