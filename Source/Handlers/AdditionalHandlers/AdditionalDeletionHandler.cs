namespace Comanda.WebApi.Handlers;

public sealed class AdditionalDeletionHandler(
    IAdditionalRepository additionalRepository
) : IRequestHandler<AdditionalDeletionRequest, Response>
{
    public async Task<Response> Handle(
        AdditionalDeletionRequest request,
        CancellationToken cancellationToken
    )
    {
        var additional = await additionalRepository.RetrieveByIdAsync(request.AdditionalId);
        if (additional is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Additional not found."
            );

        await additionalRepository.DeleteAsync(additional);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Additional deleted successfully."
        );
    }
}