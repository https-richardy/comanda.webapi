namespace Comanda.WebApi.Handlers;

public sealed class FetchPaymentSessionHandler(ICheckoutManager checkoutManager) : 
    IRequestHandler<FetchPaymentSessionRequest, Session>
{
    public async Task<Session> Handle(
        FetchPaymentSessionRequest request,
        CancellationToken cancellationToken
    )
    {
        var session = await checkoutManager.GetSessionAsync(request.SessionId);
        return session;
    }
}