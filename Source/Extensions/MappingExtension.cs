namespace Comanda.WebApi.Extensions;

public static class MappingExtension
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

        #region mappings for address requests

        TinyMapper.Bind<ViaCepResponse, Address>(config =>
        {
            config.Bind(source: source => source.Logradouro, target: target => target.Street);
            config.Bind(source: source => source.Bairro, target: target => target.Neighborhood);
            config.Bind(source: source => source.Localidade, target: target => target.City);
            config.Bind(source: source => source.UF, target: target => target.State);
            config.Bind(source: source => source.Cep, target: target => target.PostalCode);
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

        #endregion
    }
}