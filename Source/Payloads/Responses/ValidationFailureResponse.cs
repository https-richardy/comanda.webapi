/* 
    using aliases because of the ambiguous reference between 
    FluentValidation.Results.ValidationFailure and Microsoft.IdentityModel.Tokens.ValidationFailure
*/

using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace Comanda.WebApi.Payloads;

public sealed class ValidationFailureResponse : Response
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<ValidationFailure> Errors { get; set; }

    public ValidationFailureResponse(IEnumerable<ValidationFailure> errors)
    {
        StatusCode = StatusCodes.Status400BadRequest;
        Message = "We couldn't process your request because some fields are missing or invalid. Please review the details and try again.";
        Errors = errors;
    }
}