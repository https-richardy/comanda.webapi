namespace Comanda.WebApi.Handlers;

public sealed class SettingsEditingHandler(
    ISettingsRepository settingsRepository,
    IValidator<SettingsEditingRequest> validator
) : IRequestHandler<SettingsEditingRequest, Response>
{
    public async Task<Response> Handle(
        SettingsEditingRequest request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var settings = await settingsRepository.GetSettingsAsync();

        settings = TinyMapper.Map<Settings>(request);
        await settingsRepository.UpdateAsync(settings);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "settings successfully updated."
        );
    }
}