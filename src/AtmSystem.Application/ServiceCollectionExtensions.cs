using AtmSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AtmSystem.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAtmService, AtmService>();
        services.AddScoped<ISessionService, SessionService>();
        return services;
    }
}