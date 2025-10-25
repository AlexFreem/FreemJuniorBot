using FreemJuniorBot.Abstractions;
using FreemJuniorBot.Models;
using Telegram.Bot;

namespace FreemJuniorBot.Services;

/// <summary>
/// Singleton provider of a Telegram bot client. Reads token from BotSettings once.
/// </summary>
public sealed class BotClientAccessor : IBotClientAccessor
{
    public ITelegramBotClient Client { get; }

    public BotClientAccessor(BotSettings botSettings, ILogger<BotClientAccessor> logger)
    {
        var token = botSettings.Token;

        Client = new TelegramBotClient(token);
    }
}