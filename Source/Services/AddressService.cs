namespace Comanda.WebApi.Services;

public sealed class AddressService(HttpClient httpClient) : IAddressService
{
    public async Task<Address> GetByZipCodeAsync(string zipCode)
    {
        zipCode = RemoveNonDigits(zipCode);

        var response = await httpClient.GetFromJsonAsync<ViaCepResponse>(requestUri: $"/ws/{zipCode}/json/");
        var address = TinyMapper.Map<Address>(response);

        return address;
    }

    private string RemoveNonDigits(string zipCode)
    {
       return zipCode.Replace("-", "").Trim();
    }
}