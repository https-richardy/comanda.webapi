namespace Comanda.WebApi.Handlers;

public sealed class AdditionalsListingByCategoryHandler(
    IAdditionalRepository additionalRepository,
    ICategoryRepository categoryRepository
) : IRequestHandler<AdditionalsListingByCategoryRequest, Response<IEnumerable<FormattedAdditional>>>
{
    public async Task<Response<IEnumerable<FormattedAdditional>>> Handle(
        AdditionalsListingByCategoryRequest request,
        CancellationToken cancellationToken
    )
    {
        var category = await categoryRepository.RetrieveByIdAsync(request.CategoryId);
        if (category is null)
            return new Response<IEnumerable<FormattedAdditional>>(
                data: null,
                statusCode: StatusCodes.Status404NotFound,
                message: "Category not found."
            );

        var additionals = await additionalRepository
            .FindAllAsync(additional => additional.Category.Id == request.CategoryId);

        var payload = additionals
            .Select(additional => TinyMapper.Map<FormattedAdditional>(additional))
            .ToList();

        return new Response<IEnumerable<FormattedAdditional>>(
            data: payload,
            statusCode: StatusCodes.Status200OK,
            message: "Additionals retrieved successfully."
        );
    }
}