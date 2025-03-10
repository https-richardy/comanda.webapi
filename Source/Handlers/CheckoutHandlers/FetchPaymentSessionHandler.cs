namespace Comanda.WebApi.Handlers;

public sealed class FetchPreferenceHandler(ILogger<FetchPreferenceHandler> logger) :
    IRequestHandler<FetchPreferenceRequest, Preference>
{
    public async Task<Preference> Handle(
        FetchPreferenceRequest request,
        CancellationToken cancellationToken
    )
    {
        var client = new PreferenceClient();
        var preference = await client.GetAsync(
            id: request.PreferenceId,
            cancellationToken: cancellationToken
        );

        logger.LogInformation("Preference {Id} found successfully!", request.PreferenceId);
        return preference;
    }
}