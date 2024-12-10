namespace Comanda.WebApi.Handlers;

public sealed class NewAddressRegistrationHandler :
    IRequestHandler<NewAddressRegistrationRequest, Response>
{
    private readonly IUserContextService _userContextService;
    private readonly IValidator<NewAddressRegistrationRequest> _validator;
    private readonly IAddressManager _addressManager;
    private readonly ICustomerRepository _customerRepository;

    public NewAddressRegistrationHandler(
        IUserContextService userContextService,
        IValidator<NewAddressRegistrationRequest> validator,
        IAddressManager addressManager,
        ICustomerRepository customerRepository
    )
    {
        _userContextService = userContextService;
        _validator = validator;
        _addressManager = addressManager;
        _customerRepository = customerRepository;
    }

    public async Task<Response> Handle(
        NewAddressRegistrationRequest request,
        CancellationToken cancellationToken
    )
    {
        var userIdentifier = _userContextService.GetCurrentUserIdentifier();

        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var address = await _addressManager.FetchAddressByZipCodeAsync(request.PostalCode);
        var customer = await _customerRepository.FindCustomerByUserIdAsync(userIdentifier!);

        if (address is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "The zip code / postal code was not found."
            );

        address.Number = !string.IsNullOrEmpty(request.Number) ?
            request.Number :
            address.Number;

        address.Complement = !string.IsNullOrEmpty(request.Complement) ?
            request.Complement :
            address.Complement;

        address.Reference = !string.IsNullOrEmpty(request.Reference) ?
            request.Reference :
            address.Reference;

        if (customer is not null)
        {
            customer.Addresses.Add(address);

            await _addressManager.CreateAddressAsync(address);
            await _customerRepository.UpdateAsync(customer);
        }

        return new Response(
            statusCode: StatusCodes.Status201Created,
            message: "address successfully registered."
        );
    }
}