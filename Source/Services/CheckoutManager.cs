namespace Comanda.WebApi.Services;

public sealed class CheckoutManager(
    IHostInformationProvider hostInformation,
    ISettingsRepository settingsRepository
) : ICheckoutManager
{
    public async Task<Session> CreateCheckoutSessionAsync(Cart cart, Address address)
    {
        var settings = await settingsRepository.GetSettingsAsync();
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = cart.Items.Select(ToSessionLineItem).ToList(),
            Metadata = new Dictionary<string, string>
            {
                { "cartId", cart.Id.ToString() },
                { "shippingAddressId", address.Id.ToString() }
            },
            Mode = "payment",
            SuccessUrl = $"{hostInformation.HostAddress}/api/checkout/success?sessionId={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{hostInformation.HostAddress}/api/checkout/cancel?sessionId={{CHECKOUT_SESSION_ID}}",
        };

        options.LineItems.Add(new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "BRL",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = "Delivery Fee"
                },
                UnitAmount = (long)(settings.DeliveryFee * 100),
            },
            Quantity = 1
        });

        /*
            I even tried to DI the SessionService, but it seems that StripeClient decided to
            to take an unexpected vacation, deconfiguring everything. So here we are, back to basics
            back to basics and instantiating manually.
        */
        var sessionService = new SessionService();
        var session = await sessionService.CreateAsync(options);

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