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
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var currentSettings = await settingsRepository.GetSettingsAsync();
        var currentSettingsId = currentSettings.Id;

        currentSettings = TinyMapper.Map<Settings>(request);
        currentSettings.Id = currentSettingsId;

        await settingsRepository.UpdateAsync(currentSettings);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "settings successfully updated."
        );
    }
}