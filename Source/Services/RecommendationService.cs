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

        var prompt = $@"
        Dado os últimos 10 pedidos do cliente: {(orders.Any() ? _orderHistoryFormatter.Format(orders) : "Nenhum histórico de pedidos encontrado.")}.
        E dado nosso menu: {(products.Any() ? _menuFormatter.Format(products) : "Nenhum item disponível no menu atualmente.")}.
        Sugira uma refeição para o usuário '{customer.FullName}', chamando-o pelo nome.
        Se não houver histórico de pedidos, recomende um item do menu.
        Se não houver itens no menu, envie uma mensagem genérica.
        Por exemplo: Olá {customer.FullName}, que tal experimentar hoje um {{suggestion}}.
        Induza-o a compra, chamando para ação. E justifique a sugestão.";

        var response = await _geminiService.GenerateContent(prompt);
        return response;
    }
}