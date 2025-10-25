using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using FreemJuniorBot.Abstractions;
using FreemJuniorBot.Models;

namespace FreemJuniorBot.Hosting;

public sealed class Worker(
    ILogger<Worker> logger,
    IBotClientAccessor botClientAccessor,
    IErrorHandler errorHandler,
    IMessageHandler messageHandler,
    BotSettings botSettings)
    : BackgroundService
{
    private readonly ITelegramBotClient _botClient = botClientAccessor.Client;
    private readonly long _ownerId = botSettings.OwnerId;
    private readonly ReceiverOptions _receiverOptions = new()
    {
        AllowedUpdates = []
    };

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _botClient.StartReceiving(
            updateHandler: messageHandler.HandleAsync,
            errorHandler: errorHandler.HandleAsync,
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

            await _botClient.SendMessage(new ChatId(_ownerId), text, cancellationToken: ct);
            logger.LogInformation("Отправлено уведомление о перезапуске пользователю {OwnerId}", _ownerId);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Не удалось отправить уведомление о перезапуске");
        }
    }
}