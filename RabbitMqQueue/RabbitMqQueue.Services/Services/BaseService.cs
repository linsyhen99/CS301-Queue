using Microsoft.Extensions.Logging;

namespace RabbitMqQueue.Services.Services;

public abstract class BaseService<T> where T : class
{
    protected readonly ILogger<T> Logger;

    protected BaseService(ILogger<T> logger)
    {
        Logger = logger;
    }
}