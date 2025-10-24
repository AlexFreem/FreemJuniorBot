using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using FreemJuniorBot.Abstractions;

namespace FreemJuniorBot.Hosting;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly TelegramBotClient _botClient;
    private readonly ReceiverOptions _receiverOptions = new()
    {
        AllowedUpdates = []
    };

    private readonly IMessageHandler _messageHandler;
    private readonly IErrorHandler _errorHandler;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IErrorHandler errorHandler, IMessageHandler messageHandler)
    {
        _logger = logger;
        _errorHandler = errorHandler;
        _messageHandler = messageHandler;

        var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")
                    ?? configuration["TELEGRAM_BOT_TOKEN"]
                    ?? configuration["Telegram:BotToken"];

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("TELEGRAM_BOT_TOKEN is not set in environment or configuration.");
        }

        _botClient = new TelegramBotClient(token);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _botClient.StartReceiving(
            updateHandler: _messageHandler.HandleAsync,
            errorHandler: _errorHandler.HandleAsync,
            receiverOptions: _receiverOptions,
            cancellationToken: ct
        );

        _logger.LogInformation("Бот запущен!");

        await Task.Delay(Timeout.Infinite, ct);
    }
}