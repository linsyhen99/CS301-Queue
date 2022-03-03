namespace RabbitMqQueue.Queue.HostedServices;

public abstract class BaseHostedService<T> : BackgroundService where T : class
{
    protected readonly ILogger<T> Logger;

    protected BaseHostedService(ILogger<T> logger)
    {
        Logger = logger;
    }
}