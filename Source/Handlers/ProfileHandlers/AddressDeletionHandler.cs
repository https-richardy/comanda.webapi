namespace Comanda.WebApi.Handlers;

public sealed class AddressDeletionHandler(
    IUserContextService userContextService,
    ICustomerRepository customerRepository,
    IAddressRepository addressRepository
) : IRequestHandler<AddressDeletionRequest, Response>
{
    public async Task<Response> Handle(
        AddressDeletionRequest request,
        CancellationToken cancellationToken
    )
    {
        /*
            It is impossible for the 'userIdentifier' to be null at this point
            since the 'ProfileController' restricts access only to authenticated customers.
        */
        #pragma warning disable CS8604, CS8602

        var userIdentifier = userContextService.GetCurrentUserIdentifier();
        var customer = await customerRepository.FindCustomerByUserIdAsync(userIdentifier);

        var address = customer.Addresses.FirstOrDefault(address => address.Id == request.AddressId);
        if (address is null)
        {
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "address not found."
            );
        }

        await addressRepository.DeleteAsync(address);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "address successfully deleted."
        );
    }
}
