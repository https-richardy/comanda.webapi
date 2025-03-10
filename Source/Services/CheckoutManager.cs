namespace Comanda.WebApi.Services;

public sealed class CheckoutManager(
    ISettingsRepository settingsRepository,
    IConfiguration configuration
) : ICheckoutManager
{
    public async Task<CheckoutResponse> CreateCheckoutSessionAsync(Cart cart, Address address)
    {
        var client = new PreferenceClient();
        var clientAddress = configuration.GetValue<string>("ClientSettings:Address");
        var settings = await settingsRepository.GetSettingsAsync();

        var items = cart.Items.Select(item => new PreferenceItemRequest
        {
            Id = item.Id.ToString(),
            Title = item.Product.Title,
            Quantity = item.Quantity,
            CurrencyId = "BRL",
            UnitPrice = item.Product.Price
        }).ToList();

        items.Add(new PreferenceItemRequest
        {
            Title = "Taxa de Entrega",
            Quantity = 1,
            CurrencyId = "BRL",
            UnitPrice = settings.DeliveryFee
        });

        var request = new PreferenceRequest
        {
            Items = items,
            BackUrls = new PreferenceBackUrlsRequest
            {
                Success = $"",
                Failure = $"",
                Pending = $""
            },
            AutoReturn = "approved",
            ExternalReference = cart.Id.ToString(),
            NotificationUrl = configuration["MercadoPago:NotificationUrl"],
            Payer = new PreferencePayerRequest
            {
                Name = cart.Customer.FullName,
                Email = cart.Customer.Account.Email
            },
            PaymentMethods = new PreferencePaymentMethodsRequest
            {
                Installments = 1,
                ExcludedPaymentMethods = new List<PreferencePaymentMethodRequest>
                {
                    new PreferencePaymentMethodRequest { Id = "bolbradesco" },
                    new PreferencePaymentMethodRequest { Id = "pec" }
                }
            }
        };

        var session = await client.CreateAsync(request);
        var response = new CheckoutResponse
        {
            SessionId = session.Id,
            Url = session.InitPoint
        };

        return response;
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