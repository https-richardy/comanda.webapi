namespace Comanda.WebApi.Services;

public sealed class CheckoutManager(IHostInformationProvider hostInformation) : ICheckoutManager
{
    public Task<Session> CreateCheckoutSessionAsync(Cart cart)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = cart.Items.Select(ToSessionLineItem).ToList(),
            Metadata = new Dictionary<string, string>
            {
                { "cartId", cart.Id.ToString() }
            },
            Mode = "payment",
            SuccessUrl = $"{hostInformation.HostAddress}/api/checkout/success?sessionId={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{hostInformation.HostAddress}/api/checkout/cancel?sessionId={{CHECKOUT_SESSION_ID}}"
        };

        /*
            I even tried to DI the SessionService, but it seems that StripeClient decided to
            to take an unexpected vacation, deconfiguring everything. So here we are, back to basics
            back to basics and instantiating manually.
        */
        var sessionService = new SessionService();
        var session = sessionService.CreateAsync(options);

        return session;
    }

    public async Task<Session> GetSessionAsync(string sessionId)
    {
        var sessionService = new SessionService();
        var retrievedSession = await sessionService.GetAsync(sessionId);

        return retrievedSession;
    }


    private SessionLineItemOptions ToSessionLineItem(CartItem item)
    {
        return new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "BRL",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = item.Product.Title
                },
                UnitAmount = (long)(item.Total * 100),
            },
            Quantity = item.Quantity
        };
    }
}