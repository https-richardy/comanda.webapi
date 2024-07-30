namespace Comanda.WebApi.Handlers;

public sealed class AdditionalsListingHandler(
    IAdditionalRepository additionalRepository
) : IRequestHandler<AdditionalsListingRequest, Response<IEnumerable<Additional>>>
{
    public async Task<Response<IEnumerable<Additional>>> Handle(
        AdditionalsListingRequest request,
        CancellationToken cancellationToken
    )
    {
        var additionals = await additionalRepository.RetrieveAllAsync();

        return new Response<IEnumerable<Additional>>(
            data: additionals,
            statusCode: StatusCodes.Status200OK,
            message: "Additionals retrieved successfully."
        );
    }
}