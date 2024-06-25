namespace Comanda.WebApi.Extensions;

public static class MappingExtension
{
    public static void AddMapping(this IServiceCollection services)
    {
        #region mappings for accounts requests

        TinyMapper.Bind<AccountRegistrationRequest, Account>(config =>
        {
            config.Bind(source: source => source.Name, target: target => target.UserName);
            config.Bind(source: source => source.Email, target: target => target.Email);
        });

        #endregion

        #region mappings for establishments requests

        TinyMapper.Bind<CreateEstablishmentRequest, Establishment>(config =>
        {
            config.Bind(source: source => source.EstablishmentName, target: target => target.Name);
        });

        #endregion
    }
}