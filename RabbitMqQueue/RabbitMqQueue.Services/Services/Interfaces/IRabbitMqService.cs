using RabbitMqQueue.Domain.Models;

namespace RabbitMqQueue.Services.Services.Interfaces;

public interface IRabbitMqService
{
    void PublishTransactionRecord(List<Transaction> transactionRecords);
}