namespace Comanda.WebApi.Handlers;

public sealed class AddressEditingHandler :
    IRequestHandler<AddressEditingRequest, Response>
{
    private readonly IAddressManager _addressManager;
    private readonly IUserContextService _userContextService;
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<AddressEditingRequest> _validator;

    public AddressEditingHandler(
        IAddressManager addressManager,
        IUserContextService userContextService,
        ICustomerRepository customerRepository,
        IValidator<AddressEditingRequest> validator
    )
    {
        _addressManager = addressManager;
        _userContextService = userContextService;
        _customerRepository = customerRepository;
        _validator = validator;
    }

    public async Task<Response> Handle(
        AddressEditingRequest request,
        CancellationToken cancellationToken
    )
    {
        var userIdentifier = _userContextService.GetCurrentUserIdentifier();

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);


        var customer = await _customerRepository.FindCustomerByUserIdAsync(userIdentifier!);
        var address = customer!.Addresses.FirstOrDefault(address => address.Id == request.AddressId);

        if (address is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "address not found"
            );

        if (!string.IsNullOrEmpty(request.PostalCode))
        {
            var updatedAddress = await _addressManager.FetchAddressByZipCodeAsync(request.PostalCode);
            if (updatedAddress is not null)
            {
                address.Street = updatedAddress.Street;
                address.City = updatedAddress.City;
                address.State = updatedAddress.State;
                address.PostalCode = updatedAddress.PostalCode;
            }
        }

        /*
            the following fields are optional.
            if they are defined in the request, we will update the corresponding values in the 'address' object.
            otherwise, the existing values will be retained.
        */


        address.Number = !string.IsNullOrEmpty(request.Number) ?
            request.Number :
            address.Number;

        address.Complement = !string.IsNullOrEmpty(request.Complement) ?
            request.Complement :
            address.Complement;

        address.Reference = !string.IsNullOrEmpty(request.Reference) ?
            request.Reference :
            address.Reference;

        await _addressManager.UpdateAddressAsync(address);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Address successfully updated."
        );
    }
}