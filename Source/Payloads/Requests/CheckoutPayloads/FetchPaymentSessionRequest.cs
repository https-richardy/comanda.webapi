namespace Comanda.WebApi.Payloads;

public sealed record FetchPreferenceRequest: IRequest<Preference>
{
    public string PreferenceId { get; init; }

    public FetchPreferenceRequest(string preferenceId)
    {
        PreferenceId = preferenceId;
    }
}