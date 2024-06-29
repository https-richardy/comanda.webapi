namespace Comanda.WebApi.Handlers;

public sealed class GetCustomerCartDetailsHandler(
    UserManager<Account> userManager,
    CartManager cartManager,
    ICustomerRepository customerRepository
) : IRequestHandler<GetCartDetailsRequest, Response<CartResponse>>
{
    public async Task<Response<CartResponse>> Handle(
        GetCartDetailsRequest request,
        CancellationToken cancellationToken
    )
    {
        var account = await userManager.FindByIdAsync(request.UserId);
        if (account is null)
            return new Response<CartResponse>(
                data: null,
                statusCode: StatusCodes.Status404NotFound,
                message: "user not found."
            );

        var customer = await customerRepository.FindSingleAsync(customer => customer.Account.Id == account.Id);
        if (customer is null)
            return new Response<CartResponse>(
                data: null,
                statusCode: StatusCodes.Status404NotFound,
                message: "customer not found."
            );

        try
        {
            var cart = await cartManager.GetCustomerCartDetailsAsync(customer.Id);
            return new Response<CartResponse>(
                data: cart,
                statusCode: StatusCodes.Status200OK,
                message: "cart details retrieved successfully."
            );
        }
        catch (CustomerNotFoundException exception)
        {
            return new Response<CartResponse>(
                data: null,
                statusCode: StatusCodes.Status404NotFound,
                message: exception.Message
            );
        }
    }
}