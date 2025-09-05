using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ExpenseSplit.Application;

public static class DependencyServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatRServices()
                .AddMappingService();

        return services;
    }

    public static IServiceCollection AddMediatRServices(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            var assemblies = Assembly.GetExecutingAssembly();
            config.RegisterServicesFromAssemblies(assemblies);
        });
        return services;
    }

    public static IServiceCollection AddMappingService(this IServiceCollection services)
    {
        var assemblies = Assembly.GetExecutingAssembly();
        services.AddAutoMapper(config =>
        {
            config.AddMaps(assemblies);
        });
        return services;
    }
}
