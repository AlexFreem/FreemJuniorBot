using FreemJuniorBot.Abstractions;
using FreemJuniorBot.Models;
using Telegram.Bot;

namespace FreemJuniorBot.Services;

public sealed class CommandService(ILogger<CommandService> logger, IBotClientAccessor botClientAccessor, IMessageHandler messageHandler) : ICommandService
{
    
    private readonly ITelegramBotClient _botClient = botClientAccessor.Client;
    
    public Task HandleAsync(CommandRequest command, CancellationToken ct)
    {
        return messageHandler.HandleHTTPCommand(_botClient, command, ct);
    }
}