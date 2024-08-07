namespace Comanda.WebApi.Payloads;

public sealed record FetchPaymentSessionRequest: IRequest<Session>
{
    public string SessionId { get; init; }

    public FetchPaymentSessionRequest(string sessionId)
    {
        SessionId = sessionId;
    }
}