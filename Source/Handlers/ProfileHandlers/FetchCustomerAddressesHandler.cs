namespace Comanda.WebApi.Handlers;

public sealed class FetchCustomerAddressesHandler(
    IAddressRepository addressRepository,
    ICustomerRepository customerRepository,
    IUserContextService userContextService
) : IRequestHandler<FetchCustomerAddressesRequest, Response<IEnumerable<Address>>>
{
    public async Task<Response<IEnumerable<Address>>> Handle(
        FetchCustomerAddressesRequest request,
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

        var addresses = await addressRepository.GetAddressesByCustomerIdAsync(customer.Id);
        return new Response<IEnumerable<Address>>(
            data: addresses,
            statusCode: StatusCodes.Status200OK,
            message: "addresses retrieved successfully."
        );
    }
}