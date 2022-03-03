using RabbitMqQueue.Services.Services;
using RabbitMqQueue.Services.Services.Interfaces;

namespace RabbitMqQueue.Queue.DependencyInjections;

public static class DataAccessStartup
{
    public static void AddDataAccessInjections(this IServiceCollection services)
    {
        services.AddTransient<IRabbitMqService, RabbitMqService>();
    }
}