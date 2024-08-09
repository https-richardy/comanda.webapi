namespace Comanda.WebApi.Handlers;

public sealed class PaymentProcessingHandler(
    IPaymentRepository paymentRepository,
    ILogger<PaymentProcessingHandler> logger
) : IRequestHandler<PaymentProcessingRequest, Payment>
{
    public async Task<Payment> Handle(
        PaymentProcessingRequest request,
        CancellationToken cancellationToken
    )
    {
        var session = request.Session;
        var order = request.Order;

        var paymentIntentId = session.PaymentIntentId;
        var payment = new Payment
        {
            Order = request.Order,
            PaymentIntentId = paymentIntentId
        };

        logger.LogInformation("Associating payment with order {OrderId} (Payment Intent ID: {PaymentIntentId})", order.Id, paymentIntentId);
        await paymentRepository.SaveAsync(payment);

        return payment;
    }
}
