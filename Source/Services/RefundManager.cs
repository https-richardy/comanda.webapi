using Stripe;

namespace Comanda.WebApi.Services;

public sealed class RefundManager(IPaymentRepository paymentRepository, ILogger<RefundManager> logger) : IRefundManager
{
    public async Task<Refund> RefundAsync(string paymentIntentId, decimal amount)
    {
        try
        {
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId);

            var refundService = new RefundService();
            var refundList = refundService.List(new RefundListOptions
            {
                PaymentIntent = paymentIntentId
            });

            var totalRefunded = refundList.Sum(refund => (decimal)refund.Amount / 100);
            var availableForRefund = (decimal)paymentIntent.AmountReceived / 100 - totalRefunded;

            if (amount > availableForRefund)
            {
                logger.LogError("Error: Refund amount ({RefundAmount:C}) is greater than available amount ({AvailableForRefund:C}).", amount, availableForRefund);
                throw new InvalidOperationException($"Refund amount ({amount:C}) is greater than the available amount ({availableForRefund:C}).");
            }

            var options = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId,
                Amount = (long)(amount * 100)
            };

            var service = new RefundService();
            var refund = await service.CreateAsync(options);

            if (refund.Status == "succeeded")
                logger.LogInformation("Refund processed successfully.");
            else
                logger.LogError("Error processing refund: {status}", refund.Status);

            return refund;
        }
        catch (StripeException exception) when (exception.StripeError?.Code == "balance_insufficient")
        {
            logger.LogError($"Error: Insufficient balance to process the refund.");
            throw new InvalidOperationException("There is not enough balance to process the refund.", exception);
        }
        catch (StripeException exception) when (exception.StripeError?.Code == "charge_already_refunded")
        {
            logger.LogError("Error: The payment has already been refunded.");
            throw new InvalidOperationException("That payment has already been refunded.", exception);
        }
        catch (StripeException exception)
        {
            logger.LogError("Stripe Error: {message}", exception.StripeError?.Message);
            throw new InvalidOperationException("An error occurred while processing the refund.", exception);
        }
        catch (Exception exception)
        {
            logger.LogError("Unexpected error: {message}", exception.Message);
            throw new InvalidOperationException("An unexpected error occurred while processing the refund.", exception);
        }
    }

    public async Task<Refund> RefundOrderAsync(Order order)
    {
        var payment = await paymentRepository.FindByOrderIdAsync(order.Id);

        if (payment is null)
        {
            logger.LogError("Error: Payment not found for Order ID {orderId}.", order.Id);
            throw new InvalidOperationException($"Payment not found for Order ID {order.Id}.");
        }

        var refundService = new RefundService();
        var refundList = refundService.List(new RefundListOptions
        {
            PaymentIntent = payment.PaymentIntentId
        });

        var totalRefunded = refundList.Sum(refund => (decimal)refund.Amount / 100);

        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.GetAsync(payment.PaymentIntentId);
        var availableForRefund = (decimal)paymentIntent.AmountReceived / 100 - totalRefunded;

        var refundAmount = Math.Min(order.Total, availableForRefund);

        return await RefundAsync(payment.PaymentIntentId, refundAmount);
    }
}