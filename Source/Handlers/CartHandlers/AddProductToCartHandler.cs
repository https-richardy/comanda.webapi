namespace Comanda.WebApi.Handlers;

public sealed class AddProductToCartHandler(
    UserManager<Account> userManager,
    CartManager cartManager,
    ICustomerRepository customerRepository
) : IRequestHandler<AddProductToCartRequest, Response>
{
    public async Task<Response> Handle(
        AddProductToCartRequest request,
        CancellationToken cancellationToken
    )
    {
        var account = await userManager.FindByIdAsync(request.UserId);
        if (account is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "user not found."
            );

        var customer = await customerRepository.FindSingleAsync(customer => customer.Account.Id == account.Id);
        if (customer is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "customer not found."
            );

        try
        {
            await cartManager.AddItemAsync(
                customerId: customer.Id,
                productId: request.ProductId,
                quantity: request.Quantity,
                establishmentId: request.EstablishmentId
            );

            return new Response(
                statusCode: StatusCodes.Status201Created,
                message: "product added to cart successfully."
            );
        }
        catch (CustomerNotFoundException exception)
        {
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: exception.Message
            );
        }
        catch (EstablishmentNotFoundException exception)
        {
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: exception.Message
            );
        }
        catch (EstablishmentProductNotFoundException exception)
        {
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: exception.Message
            );
        }
    }
}