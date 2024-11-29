namespace Comanda.WebApi.Extensions;

[ExcludeFromCodeCoverage]
internal static class MediatorExtension
{
    public static void AddMediator(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
    }
}