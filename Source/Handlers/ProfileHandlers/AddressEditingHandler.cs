namespace Comanda.WebApi.Handlers;

public sealed class AddressEditingHandler(
    IUserContextService userContextService,
    IAddressRepository addressRepository,
    IAddressService addressService,
    ICustomerRepository customerRepository,
    IValidator<AddressEditingRequest> validator
) : IRequestHandler<AddressEditingRequest, Response>
{
    public async Task<Response> Handle(
        AddressEditingRequest request,
        CancellationToken cancellationToken
    )
    {
        var userIdentifier = userContextService.GetCurrentUserIdentifier();

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        /*
            It is impossible for the 'userIdentifier' to be null at this point
            since the 'ProfileController' restricts access only to authenticated customers.
        */
        #pragma warning disable CS8604, CS8602

        var customer = await customerRepository.FindCustomerByUserIdAsync(userIdentifier);
        var address = customer.Addresses.FirstOrDefault(address => address.Id == request.AddressId);

        if (address is null)
            return new Response(
                statusCode: StatusCodes.Status403Forbidden,
                message: "address does not belong to the customer."
            );

        if (!string.IsNullOrEmpty(request.PostalCode))
        {
            var updatedAddress = await addressService.GetByZipCodeAsync(request.PostalCode);
            if (updatedAddress is not null)
            {
                address.Street = updatedAddress.Street;
                address.City = updatedAddress.City;
                address.State = updatedAddress.State;
                address.PostalCode = updatedAddress.PostalCode;
            }
        }

        address.Number = !string.IsNullOrEmpty(request.Number) ? request.Number : address.Number;
        address.Complement = !string.IsNullOrEmpty(request.Complement) ? request.Complement : address.Complement;
        address.Reference = !string.IsNullOrEmpty(request.Reference) ? request.Reference : address.Reference;

        await addressRepository.UpdateAsync(address);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Address successfully updated."
        );
    }
}