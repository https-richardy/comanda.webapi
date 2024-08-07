namespace Comanda.WebApi.Handlers;

public sealed class SettingsDetailsHandler(
    ISettingsRepository settingsRepository
) : IRequestHandler<SettingsDetailsRequest, Response<SettingsFormattedResponse>>
{
    public async Task<Response<SettingsFormattedResponse>> Handle(
        SettingsDetailsRequest request,
        CancellationToken cancellationToken
    )
    {
        var settings = await settingsRepository.GetSettingsAsync();
        var payload = TinyMapper.Map<SettingsFormattedResponse>(settings);

        return new Response<SettingsFormattedResponse>(
            data: payload,
            statusCode: StatusCodes.Status200OK,
            message: "settings information successfully retrieved."
        );
    }
}