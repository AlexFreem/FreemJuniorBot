using Telegram.Bot;

namespace FreemJuniorBot.Abstractions;

public interface IErrorHandler
{
    Task HandleAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken);
}