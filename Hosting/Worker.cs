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

    public Worker(ILogger<Worker> logger, IBotClientAccessor botClientAccessor, IErrorHandler errorHandler, IMessageHandler messageHandler)
    {
        _logger = logger;
        _errorHandler = errorHandler;
        _messageHandler = messageHandler;
        _botClient = botClientAccessor.Client;
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

    private static string? GetOwnerIdRawFromEnv()
    {
        // Priority: TELEGRAM_OWNER_ID, then fallback: TELEGRAM_ADMIN_ID
        return Environment.GetEnvironmentVariable("TELEGRAM_OWNER_ID")
               ?? Environment.GetEnvironmentVariable("TELEGRAM_ADMIN_ID");
    }

    private async Task NotifyRestartAsync(CancellationToken ct)
    {
        try
        {
            var ownerIdRaw = GetOwnerIdRawFromEnv();

            if (string.IsNullOrWhiteSpace(ownerIdRaw))
            {
                _logger.LogWarning("Переменная окружения TELEGRAM_OWNER_ID/TELEGRAM_ADMIN_ID не задана — уведомление о перезапуске не отправлено");
                return;
            }

            if (!long.TryParse(ownerIdRaw, out var ownerId))
            {
                _logger.LogWarning("Значение переменной окружения TELEGRAM_OWNER_ID/TELEGRAM_ADMIN_ID некорректно: {Value}", ownerIdRaw);
                return;
            }

            var text = $"Приложение перезапущено в {DateTime.Now:G}";

            await _botClient.SendMessage(new ChatId(ownerId), text, cancellationToken: ct);
            _logger.LogInformation("Отправлено уведомление о перезапуске пользователю {OwnerId}", ownerId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось отправить уведомление о перезапуске");
        }
    }
}