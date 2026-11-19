using AtmSystem.Application.Ports;
using AtmSystem.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AtmSystem.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IAccountRepository, InMemoryAccountRepository>();
        services.AddSingleton<ISessionRepository, InMemorySessionRepository>();
        services.AddSingleton<IOperationRepository, InMemoryOperationRepository>();

        return services;
    }
}