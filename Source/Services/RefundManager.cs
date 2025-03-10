namespace Comanda.WebApi.Services;

public sealed class RefundManager : IRefundManager
{
    public async Task RefundAsync(string paymentIntentId, decimal amount)
    {
        // Due to the payment gateway change, I deleted the code here.
        // I'll implement it later!

        await Task.CompletedTask;
    }

    public async Task RefundOrderAsync(Order order)
    {
        // Due to the payment gateway change, I deleted the code here. 
        // I'll implement it later!

        await Task.CompletedTask;
    }
}