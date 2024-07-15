namespace Comanda.WebApi.Payloads;

public sealed class ValidationFailureResponse : Response
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ValidationResult Errors { get; set; }

    public ValidationFailureResponse(string message, ValidationResult errors)
    {
        StatusCode = StatusCodes.Status400BadRequest;
        Message = message;
        Errors = errors;
    }
}