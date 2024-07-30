namespace Comanda.WebApi.Handlers;

public sealed class AdditionalCreationHandler(
    IAdditionalRepository additionalRepository,
    IValidator<AdditionalCreationRequest> validator
) : IRequestHandler<AdditionalCreationRequest, Response>
{
    public async Task<Response> Handle(
        AdditionalCreationRequest request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var additional = TinyMapper.Map<Additional>(request);
        await additionalRepository.SaveAsync(additional);

        return new Response(
            statusCode: StatusCodes.Status201Created,
            message: "Additional created successfully."
        );
    }
}