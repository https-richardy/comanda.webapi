namespace Comanda.WebApi.Services;

public sealed class AddressService : IAddressService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AddressService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }


    public async Task<Address> GetByZipCodeAsync(string zipCode)
    {
        var requestUrl = GetRequestUrl(zipCode);
        var client = _httpClientFactory.CreateClient();

        var response = await client.GetFromJsonAsync<ViaCepResponse>(requestUrl);
        var address = TinyMapper.Map<Address>(response);

        return address;
    }

    private string RemoveNonDigits(string zipCode)
    {
       return zipCode.Replace("-", "").Trim();
    }

    #pragma warning disable CS8600
    private string GetRequestUrl(string zipCode)
    {
        zipCode = RemoveNonDigits(zipCode);

        string viaCepUrl = _configuration["ExternalApis:ViaCepUrl"];
        string requestUrl = $"{viaCepUrl}/ws/{zipCode}/json/";

        return requestUrl;
    }
}