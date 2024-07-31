namespace Comanda.WebApi.Payloads;

public class Response<TData> : Response
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TData? Data { get; set; }

    public Response() { }

    public Response(TData? data, int statusCode, string? message)
    {
        Data = data;
        StatusCode = statusCode;
        Message = message;
    }
}