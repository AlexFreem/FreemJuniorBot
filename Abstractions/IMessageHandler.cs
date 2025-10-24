using Telegram.Bot;
using Telegram.Bot.Types;

namespace FreemJuniorBot.Abstractions;

public interface IMessageHandler
{
    Task HandleAsync(ITelegramBotClient botClient, Update update, CancellationToken ct);
}