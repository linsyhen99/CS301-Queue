using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using Microsoft.Extensions.Options;
using RabbitMqQueue.Domain.Configurations;
using RabbitMqQueue.Domain.Mappings;
using RabbitMqQueue.Domain.Models;
using RabbitMqQueue.Services.Services.Interfaces;

namespace RabbitMqQueue.Queue.HostedServices;

public class TransactionCSVStreamerHostedService : BackgroundService
{
    private readonly TimeSpan _delay = TimeSpan.FromSeconds(5);
    private readonly IRabbitMqService _rabbitMqService;
    private readonly ILogger<TransactionCSVStreamerHostedService> Logger;
    private readonly IOptions<FilePathOption> _filePathOption;

    public TransactionCSVStreamerHostedService(ILogger<TransactionCSVStreamerHostedService> logger,
        IRabbitMqService rabbitMqService, IOptions<FilePathOption> filePathOption)
    {
        _rabbitMqService = rabbitMqService;
        Logger = logger;
        _filePathOption = filePathOption;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        Logger.LogInformation("[TransactionCSVStreamerHostedService] Starting File Streamer Background Service");

        try
        {
            using (var streamReader = new StreamReader(_filePathOption.Value.FilePath))
            {
                using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    csvReader.Context.RegisterClassMap<TransactionMap>();
                    var records = csvReader.GetRecords<Transaction>().ToList();

                    _rabbitMqService.PublishTransactionRecord(records);
                    Console.WriteLine(records.Count);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(
                $"[TransactionCSVStreamerHostedService] Failed to handle Transaction service with error message: {ex.Message}");
        }

        stopwatch.Stop();
        TimeSpan timeSpan = stopwatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes,
            timeSpan.Seconds, timeSpan.Milliseconds / 10);
        Console.WriteLine("RunTime " + elapsedTime);

        Logger.LogInformation("[TransactionCSVStreamerHostedService] Stopping background service");
        return Task.CompletedTask;
    }
}