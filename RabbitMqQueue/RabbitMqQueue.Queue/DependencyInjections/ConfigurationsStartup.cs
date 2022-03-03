using RabbitMqQueue.Domain.Configurations;

namespace RabbitMqQueue.Queue.DependencyInjections;

public static class ConfigurationsStartup
{
    public static void AddConfigurationVariables(this IServiceCollection services, IHostEnvironment hostEnvironment,
        IConfiguration configuration)
    {
        var rabbitMqConfig = GetRabbitMqConfigurations(hostEnvironment, configuration);
        var transactionConfig = GetTransactionProcessorOption(hostEnvironment, configuration);
        var filePathConfig = GetFilePathOption(hostEnvironment, configuration);

        if (string.IsNullOrEmpty(rabbitMqConfig.HostName) || string.IsNullOrEmpty(rabbitMqConfig.UserName) ||
            string.IsNullOrEmpty(rabbitMqConfig.Password) || string.IsNullOrEmpty(rabbitMqConfig.VHost))
            throw new InvalidDataException("Invalid RabbitMQ configuration provided");

        if (string.IsNullOrEmpty(filePathConfig))
            throw new InvalidDataException("Invalid file path provided");

        services.Configure<RabbitMqOption>(options =>
        {
            options.HostName = rabbitMqConfig.HostName;
            options.UserName = rabbitMqConfig.UserName;
            options.Password = rabbitMqConfig.Password;
            options.VHost = rabbitMqConfig.VHost;
            options.Port = rabbitMqConfig.Port;
        });

        services.Configure<TransactionProcessorOption>(options =>
        {
            options.TransactionPerMessage = transactionConfig.TransactionPerMessage;
        });

        services.Configure<FilePathOption>(options =>
        {
            options.FilePath = filePathConfig;
        });
    }

    private static RabbitMqOption GetRabbitMqConfigurations(IHostEnvironment hostEnvironment,
        IConfiguration configuration)
    {
        if (!hostEnvironment.IsProduction())
        {
            if (!int.TryParse(configuration["rabbitMq:Port"], out var devPort))
                throw new InvalidCastException("RabbitMQ port is not a number");

            return new RabbitMqOption
            {
                HostName = configuration["RabbitMq:HostName"],
                UserName = configuration["RabbitMq:UserName"],
                Password = configuration["RabbitMq:Password"],
                VHost = configuration["RabbitMq:VirtualHost"],
                Port = devPort
            };
        }

        if (!int.TryParse(Environment.GetEnvironmentVariable("rmqPort"), out var prodPort))
            throw new InvalidCastException("RabbitMQ port is not a number");

        return new RabbitMqOption
        {
            HostName = Environment.GetEnvironmentVariable("rmqHostname") ?? string.Empty,
            UserName = Environment.GetEnvironmentVariable("rmqUsername") ?? string.Empty,
            Password = Environment.GetEnvironmentVariable("rmqPassword") ?? string.Empty,
            VHost = Environment.GetEnvironmentVariable("rmqVhost") ?? string.Empty,
            Port = prodPort
        };
    }

    private static TransactionProcessorOption GetTransactionProcessorOption(IHostEnvironment hostEnvironment,
        IConfiguration configuration)
    {
        if (!hostEnvironment.IsProduction())
        {
            if (!int.TryParse(configuration["TransactionProcessor:TransactionsPerMessage"],
                    out var devTransactionPerMessage))
                throw new InvalidCastException("Number of transactions per message is not a number");

            return new TransactionProcessorOption
            {
                TransactionPerMessage = devTransactionPerMessage
            };
        }

        if (!int.TryParse(Environment.GetEnvironmentVariable("transactionsPerMessage"),
                out var prodTransactionPerMessage))
            throw new InvalidCastException("Number of transactions per message is not a number");

        return new TransactionProcessorOption
        {
            TransactionPerMessage = prodTransactionPerMessage
        };
    }

    private static string GetFilePathOption(IHostEnvironment hostEnvironment, IConfiguration configuration)
    {
        if (!hostEnvironment.IsProduction())
            return configuration["FilePath:TransactionFilePath"];

        return Environment.GetEnvironmentVariable("transactionFilePath") ?? string.Empty;
    }
}