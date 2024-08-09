namespace Comanda.WebApi.Handlers;

public sealed class SuccessfulPaymentHandler(IMediator mediator) : 
    IRequestHandler<SuccessfulPaymentRequest, Response>
{
    public async Task<Response> Handle(
        SuccessfulPaymentRequest request,
        CancellationToken cancellationToken
    )
    {
        var fetchPaymentSessionRequest = new FetchPaymentSessionRequest(request.SessionId);
        var session = await mediator.Send(fetchPaymentSessionRequest);

        var cartId = int.Parse(session.Metadata["cartId"]);
        var shippingAddressId = int.Parse(session.Metadata["shippingAddressId"]);

        var retrieveCartAndAddressRequest = new RetrieveCartAndAddressRequest(cartId, shippingAddressId);
        var (cart, address) = await mediator.Send(retrieveCartAndAddressRequest);

        var orderProcessingRequest = new OrderProcessingRequest { Cart = cart, Address = address };
        var order = await mediator.Send(orderProcessingRequest);

        var paymentProcessingRequest = new PaymentProcessingRequest { Order = order, Session = session };
        await mediator.Send(paymentProcessingRequest);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "payment processed and order successfully created."
        );
    }
}