namespace Comanda.WebApi.Services;

public sealed class RecommendationService : IRecommendationService
{
    private readonly IOrderHistoryFormatter _orderHistoryFormatter;
    private readonly IMenuFormatter _menuFormatter;
    private readonly IGeminiService _geminiService;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public RecommendationService(
        IOrderHistoryFormatter orderHistoryFormatter,
        IMenuFormatter menuFormatter,
        IGeminiService geminiService,
        IProductRepository productRepository,
        IOrderRepository orderRepository
    )
    {
        _orderHistoryFormatter = orderHistoryFormatter;
        _menuFormatter = menuFormatter;
        _geminiService = geminiService;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public async Task<string> RecommendAsync(Customer customer)
    {
        Expression<Func<Order, bool>> predicate = order => order.Customer.Id == customer.Id;

        var products = await _productRepository.RetrieveAllAsync();
        var orders = await _orderRepository.PagedAsync(
            pageNumber: 1,
            pageSize: 10,
            predicate: predicate
        );

        var formattedHistory = _orderHistoryFormatter.Format(orders);
        var formattedMenu = _menuFormatter.Format(products);

        var prompt = $"Dado os ultimos 10 pedidos do cliente: {formattedHistory}. " +
                     $"E Dado nosso menu: {formattedMenu}." +
                     $"Dê uma sugestão para o Usuário '{customer.FullName}' chame ele, se refira a ele. " +
                     $"Por exemplo: Olá {customer.FullName}, que tal comer hoje um {{suggestion}}.";

        var response = await _geminiService.GenerateContent(prompt);
        return response;
    }
}