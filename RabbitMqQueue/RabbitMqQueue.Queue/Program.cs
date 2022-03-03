using RabbitMqQueue.Queue.DependencyInjections;
using RabbitMqQueue.Queue.HostedServices;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console(theme: AnsiConsoleTheme.Literate);
});

builder.Services.AddConfigurationVariables(builder.Environment, builder.Configuration);
builder.Services.AddDataAccessInjections();

builder.Services.AddHostedService<TransactionCSVStreamerHostedService>();

var app = builder.Build();
app.Run();