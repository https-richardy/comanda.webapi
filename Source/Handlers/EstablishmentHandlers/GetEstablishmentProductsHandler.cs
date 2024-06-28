#pragma warning disable CS8604

namespace Comanda.WebApi.Handlers;

public sealed class GetEstablishmentProductsHandler(
    IEstablishmentRepository establishmentRepository,
    IHttpContextAccessor accessor
) :
    IRequestHandler<GetEstablishmentProductsRequest, Response<PaginationHelper<Product>>>
{
    public async Task<Response<PaginationHelper<Product>>> Handle(
        GetEstablishmentProductsRequest request,
        CancellationToken cancellationToken
    )
    {
        var establishment = await establishmentRepository.RetrieveByIdAsync(request.EstablishmentId);
        if (establishment is null)
        {
            return new Response<PaginationHelper<Product>>(
                data: null,
                statusCode: StatusCodes.Status404NotFound,
                message: $"Establishment with ID '{request.EstablishmentId}' not found."
            );
        }

        var products = await establishmentRepository.GetProductsAsync(request.PageNumber, request.PageSize, request.EstablishmentId);

        return new Response<PaginationHelper<Product>>(
            data: new PaginationHelper<Product>(products, request.PageNumber, request.PageSize, accessor.HttpContext),
            statusCode: StatusCodes.Status200OK,
            message: "Products retrieved successfully."
        );
    }
}
