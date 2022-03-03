using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMqQueue.Domain.Configurations;
using RabbitMqQueue.Domain.Constants;
using RabbitMqQueue.Domain.Models;
using RabbitMqQueue.Services.Services.Interfaces;

namespace RabbitMqQueue.Services.Services;

public class RabbitMqService : BaseService<RabbitMqService>, IRabbitMqService
{
    private readonly IOptions<RabbitMqOption> _rabbitMqOption;
    private readonly IOptions<TransactionProcessorOption> _transactionProcessorOption;
    private IConnection _connection;
    private IModel _transactionChannel;

    public RabbitMqService(ILogger<RabbitMqService> logger, IOptions<RabbitMqOption> rabbitMqOption,
        IOptions<TransactionProcessorOption> transactionProcessorOption) : base(logger)
    {
        _rabbitMqOption = rabbitMqOption;
        _transactionProcessorOption = transactionProcessorOption;

        CreateConnection();
        CreateExchangesAndQueues();
    }

    private void CreateConnection()
    {
        Logger.LogInformation("[RabbitMqService:CreateConnection] Preparing RabbitMQ connection");

        var connectionFactory = new ConnectionFactory
        {
            HostName = _rabbitMqOption.Value.HostName,
            UserName = _rabbitMqOption.Value.UserName,
            Password = _rabbitMqOption.Value.Password,
            VirtualHost = _rabbitMqOption.Value.VHost,
            Port = _rabbitMqOption.Value.Port,

            RequestedHeartbeat = TimeSpan.Zero
        };

        _connection = connectionFactory.CreateConnection();
        _transactionChannel = _connection.CreateModel();
        _transactionChannel.ConfirmSelect();

        Logger.LogInformation("[RabbitMqService:CreateConnection] RabbitMQ connection established");
    }

    private void CreateExchangesAndQueues()
    {
        Logger.LogInformation("[RabbitMqService:CreateExchangesAndQueues] Preparing exchange and queues");

        _transactionChannel.ExchangeDeclare(RabbitMqConstant.TransactionExchange, ExchangeType.Direct);
        _transactionChannel.QueueDeclare(RabbitMqConstant.TransactionQueue, true, false, false);
        _transactionChannel.QueueBind(RabbitMqConstant.TransactionQueue, RabbitMqConstant.TransactionExchange,
            RabbitMqConstant.TransactionQueue);

        Logger.LogInformation("[RabbitMqService:CreateExchangesAndQueues] Exchange and queues created");
    }

    public void PublishTransactionRecord(List<Transaction> transactionRecords)
    {
        var exchangeProps = _transactionChannel.CreateBasicProperties();
        exchangeProps.Persistent = true;

        foreach (var transaction in transactionRecords)
        {
            var transactionSerialized = JsonSerializer.SerializeToUtf8Bytes(transaction);

            _transactionChannel.BasicPublish(RabbitMqConstant.TransactionExchange,
                RabbitMqConstant.TransactionQueue, exchangeProps, transactionSerialized);
        }
    }
}