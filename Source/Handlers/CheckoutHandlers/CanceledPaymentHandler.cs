namespace Comanda.WebApi.Handlers;

public sealed class CanceledPaymentHandler(
    ICheckoutManager checkoutManager,
    ILogger<CanceledPaymentHandler> logger
) : IRequestHandler<CanceledPaymentRequest, Response>
{
    public async Task<Response> Handle(
        CanceledPaymentRequest request,
        CancellationToken cancellationToken
    )
    {
        var session = await checkoutManager.GetSessionAsync(request.SessionId);
        logger.LogInformation("The payment session with the ID '{sessionId}' has been canceled by the customer.", session.Id);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: string.Empty
        );
    }
}