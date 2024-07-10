namespace Comanda.WebApi.Payloads;

public record ViaCepResponse
{
    public string Cep { get; init; }
    public string Logradouro { get; init; }
    public string Complemento { get; init; }
    public string Bairro { get; init; }
    public string Localidade { get; init; }
    public string UF { get; init; }
    public string Ibge { get; init; }
    public string Gia { get; init; }
    public string Ddd { get; init; }
    public string Siafi { get; init; }
}