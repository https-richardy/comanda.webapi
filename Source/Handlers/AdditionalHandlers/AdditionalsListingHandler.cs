namespace Comanda.WebApi.Handlers;

public sealed class AdditionalsListingHandler(
    IAdditionalRepository additionalRepository
) : IRequestHandler<AdditionalsListingRequest, Response<IEnumerable<FormattedAdditional>>>
{
    public async Task<Response<IEnumerable<FormattedAdditional>>> Handle(
        AdditionalsListingRequest request,
        CancellationToken cancellationToken
    )
    {
        var additionals = await additionalRepository
            .RetrieveAllAsync();

        var payload = additionals /* see the additional mapping profile */
            .Select(additional => TinyMapper.Map<FormattedAdditional>(additional));

        return new Response<IEnumerable<FormattedAdditional>>(
            data: payload,
            statusCode: StatusCodes.Status200OK,
            message: "Additionals retrieved successfully."
        );
    }
}