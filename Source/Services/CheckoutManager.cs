namespace Comanda.WebApi.Services;

public sealed class CheckoutManager(
    ISettingsRepository settingsRepository,
    IConfiguration configuration
) : ICheckoutManager
{
    public async Task<CheckoutResponse> CreateCheckoutSessionAsync(Cart cart, Customer customer, Address address)
    {
        var client = new PreferenceClient();
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

        var successUrl = configuration["MercadoPago:BackUrls:Success"];
        var failureUrl = configuration["MercadoPago:BackUrls:Failure"];
        var pendingUrl = configuration["MercadoPago:BackUrls:Pending"];

        var request = new PreferenceRequest
        {
            Items = items,
            BackUrls = new PreferenceBackUrlsRequest
            {
                Success = successUrl,
                Failure = failureUrl,
                Pending = pendingUrl
            },
            AutoReturn = "approved",
            ExternalReference = cart.Id.ToString(),
            NotificationUrl = configuration["MercadoPago:NotificationUrl"],
            Payer = new PreferencePayerRequest
            {
                Name = customer.FullName,
                Email = customer.Account.Email
            },
            Metadata = new Dictionary<string, object>
            {
                { "CustomerEmail", customer.Account.Email! },
                { "ShippingAddressId", address.Id.ToString() }
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
}