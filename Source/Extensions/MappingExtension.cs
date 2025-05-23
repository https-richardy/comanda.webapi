namespace Comanda.WebApi.Extensions;

internal static class MappingExtension
{
    public static void AddMapping(this IServiceCollection services)
    {
        #region mappings for identity requests

        TinyMapper.Bind<AccountRegistrationRequest, Account>(config =>
        {
            config.Bind(source: source => source.Name, target: target => target.UserName);
            config.Bind(source: source => source.Email, target: target => target.Email);
        });

        #endregion

        #region address mapping profiles

        TinyMapper.Bind<ViaCepResponse, Address>(config =>
        {
            config.Bind(source: source => source.Logradouro, target: target => target.Street);
            config.Bind(source: source => source.Bairro, target: target => target.Neighborhood);
            config.Bind(source: source => source.Localidade, target: target => target.City);
            config.Bind(source: source => source.UF, target: target => target.State);
            config.Bind(source: source => source.Cep, target: target => target.PostalCode);
        });

        TinyMapper.Bind<Address, FormattedAddress>(config =>
        {
            config.Bind(source: source => source.Id, target: target => target.Id);
            config.Bind(source: source => source.Street, target: target => target.Street);
            config.Bind(source: source => source.Number, target: target => target.Number);
            config.Bind(source: source => source.City, target: target => target.City);
            config.Bind(source: source => source.State, target: target => target.State);
            config.Bind(source: source => source.Neighborhood, target: target => target.Neighborhood);
            config.Bind(source: source => source.PostalCode, target: target => target.PostalCode);
            config.Bind(source: source => source.Complement, target: target => target.Complement);
            config.Bind(source: source => source.Reference, target: target => target.Reference);
        });

        #endregion

        #region mappings for product requests

        TinyMapper.Bind<ProductCreationRequest, Product>(config =>
        {
            config.Bind(source: source => source.Title, target: target => target.Title);
            config.Bind(source: source => source.Description, target: target => target.Description);
            config.Bind(source: source => source.Price, target: target => target.Price);
        });

        TinyMapper.Bind<ProductEditingRequest, Product>(config =>
        {
            config.Bind(source: source => source.ProductId, target: target => target.Id);
            config.Bind(source: source => source.Title, target: target => target.Title);
            config.Bind(source: source => source.Description, target: target => target.Description);
            config.Bind(source: source => source.Price, target: target => target.Price);
        });

        #endregion

        #region category mapping profiles

        TinyMapper.Bind<Category, FormattedCategory>(config =>
        {
            config.Bind(source: source => source.Id, target: target => target.Id);
            config.Bind(source: source => source.Name, target: target => target.Name);
        });

        #endregion


        #region mappings for additional requests

        TinyMapper.Bind<AdditionalCreationRequest, Additional>(config =>
        {
            config.Bind(source: source => source.Name, target: target => target.Name);
            config.Bind(source: source => source.Price, target: target => target.Price);
        });


        TinyMapper.Bind<AdditionalEditingRequest, Additional>(config =>
        {
            config.Bind(source: source => source.AdditionalId, target: target => target.Id);
            config.Bind(source: source => source.Name, target: target => target.Name);
            config.Bind(source: source => source.Price, target: target => target.Price);
        });

        TinyMapper.Bind<Additional, FormattedAdditional>(config =>
        {
            config.Bind(source: source => source.Id, target: target => target.Id);
            config.Bind(source: source => source.Name, target: target => target.Name);

            /* nested type mapping */
            config.Bind(source: source => source.Category.Id, target: target => target.Category.Id);
            config.Bind(source: source => source.Category.Name, target: target => target.Category.Name);
        });

        #endregion

        #region ingredient mapping profiles

        TinyMapper.Bind<Ingredient, BasicIngredientDetail>(config =>
        {
            config.Bind(source: source => source.Id, target: target => target.Id);
            config.Bind(source: source => source.Name, target: target => target.Name);
        });

        #endregion

        #region mapping for order

        TinyMapper.Bind<CartItemAdditional, OrderItemAdditional>(config =>
        {
            config.Bind(source => source.Quantity, target: target => target.Quantity);
            config.Bind(source: source => source.Additional, target: target => target.Additional);
        });

        #endregion

        #region mappings for settings

        TinyMapper.Bind<Settings, SettingsFormattedResponse>(config =>
        {
            config.Bind(source: source => source.AcceptAutomatically, target: target => target.AcceptAutomatically);
            config.Bind(source: source => source.MaxConcurrentAutomaticOrders, target: target => target.MaxConcurrentAutomaticOrders);
            config.Bind(source: source => source.EstimatedDeliveryTimeInMinutes, target: target => target.EstimatedDeliveryTimeInMinutes);
            config.Bind(source: source => source.DeliveryFee, target: target => target.DeliveryFee);
        });

        TinyMapper.Bind<SettingsEditingRequest, Settings>(config =>
        {
            config.Bind(source: source => source.AcceptAutomatically, target: target => target.AcceptAutomatically);
            config.Bind(source: source => source.MaxConcurrentAutomaticOrders, target: target => target.MaxConcurrentAutomaticOrders);
            config.Bind(source: source => source.EstimatedDeliveryTimeInMinutes, target: target => target.EstimatedDeliveryTimeInMinutes);
            config.Bind(source: source => source.DeliveryFee, target: target => target.DeliveryFee);
        });

        #endregion
    }
}