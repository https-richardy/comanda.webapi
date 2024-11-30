namespace Comanda.WebApi.Handlers;

public sealed class FetchCustomerAddressesHandler(
    IAddressRepository addressRepository,
    ICustomerRepository customerRepository,
    IUserContextService userContextService
) : IRequestHandler<FetchCustomerAddressesRequest, Response<IEnumerable<FormattedAddress>>>
{
    public async Task<Response<IEnumerable<FormattedAddress>>> Handle(
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
        var payload = addresses
            .Select(address => TinyMapper.Map<FormattedAddress>(address))
            .ToList();

        return new Response<IEnumerable<FormattedAddress>>(
            data: payload,
            statusCode: StatusCodes.Status200OK,
            message: "addresses retrieved successfully."
        );
    }
}