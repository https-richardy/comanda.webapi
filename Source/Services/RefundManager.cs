using Stripe;

namespace Comanda.WebApi.Services;

public sealed class RefundManager(ILogger<RefundManager> logger) : IRefundManager
{
    public async Task<Refund> RefundAsync(string paymentItentId, decimal amount)
    {
        try
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = paymentItentId,
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
        /* TODO: implement customized exceptions. */
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
}