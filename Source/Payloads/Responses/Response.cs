namespace Comanda.WebApi.Payloads;

public class Response
{
    public int StatusCode { get; set; }
    public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }

    public Response() { }

    public Response(int statusCode, string? message)
    {
        StatusCode = statusCode;
        Message = message;
    }
}