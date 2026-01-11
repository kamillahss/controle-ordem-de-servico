

using Microsoft.Extensions.DependencyInjection;

namespace OsService.Services;

public static class ServicesServiceCollection
{
    public static IServiceCollection AddServicesLayer(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServicesServiceCollection).Assembly));
        return services;
    }
}
