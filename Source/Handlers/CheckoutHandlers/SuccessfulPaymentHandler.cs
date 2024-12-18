namespace Comanda.WebApi.Handlers;

public sealed class SuccessfulPaymentHandler(IMediator mediator, ISettingsRepository settingsRepository) :
    IRequestHandler<SuccessfulPaymentRequest, Response<OrderConfirmation>>
{
    public async Task<Response<OrderConfirmation>> Handle(
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

        var settings = await settingsRepository.GetSettingsAsync();
        var payload = new OrderConfirmation
        {
            OrderId = order.Id,
            TotalPaid = (long)session.AmountTotal! / 100m,
            EstimatedDeliveryTimeInMinutes = settings.EstimatedDeliveryTimeInMinutes
        };

        return new Response<OrderConfirmation>(
            data: payload,
            statusCode: StatusCodes.Status200OK,
            message: "payment processed and order successfully created."
        );
    }
}