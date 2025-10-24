using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using FreemJuniorBot.Abstractions;

namespace FreemJuniorBot.Hosting;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly ReceiverOptions _receiverOptions = new()
    {
        AllowedUpdates = []
    };

    private readonly IMessageHandler _messageHandler;
    private readonly IErrorHandler _errorHandler;
    private readonly IOwnerIdProvider _ownerIdProvider;

    public Worker(ILogger<Worker> logger, IBotClientAccessor botClientAccessor, IErrorHandler errorHandler, IMessageHandler messageHandler, IOwnerIdProvider ownerIdProvider)
    {
        _logger = logger;
        _errorHandler = errorHandler;
        _messageHandler = messageHandler;
        _botClient = botClientAccessor.Client;
        _ownerIdProvider = ownerIdProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _botClient.StartReceiving(
            updateHandler: _messageHandler.HandleAsync,
            errorHandler: _errorHandler.HandleAsync,
            receiverOptions: _receiverOptions,
            cancellationToken: ct
        );

        await NotifyRestartAsync(ct);
        await Task.Delay(Timeout.Infinite, ct);
    }

    private async Task NotifyRestartAsync(CancellationToken ct)
    {
        try
        {
            var text = $"Приложение перезапущено в {DateTime.Now:G}";

            await _botClient.SendMessage(new ChatId(_ownerIdProvider.OwnerId), text, cancellationToken: ct);
            _logger.LogInformation("Отправлено уведомление о перезапуске пользователю {OwnerId}", _ownerIdProvider.OwnerId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось отправить уведомление о перезапуске");
        }
    }
}