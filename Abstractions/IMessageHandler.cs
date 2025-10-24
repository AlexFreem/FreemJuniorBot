using FreemJuniorBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FreemJuniorBot.Abstractions;

public interface IMessageHandler
{
    Task HandleAsync(ITelegramBotClient botClient, Update update, CancellationToken ct);
    Task HandleHTTPCommand(ITelegramBotClient botClient, CommandRequest commandRequest, CancellationToken ct);
}