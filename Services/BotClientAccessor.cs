using FreemJuniorBot.Abstractions;
using Telegram.Bot;

namespace FreemJuniorBot.Services;

/// <summary>
/// Singleton provider of a Telegram bot client. Reads token from the environment or configuration once.
/// </summary>
public sealed class BotClientAccessor : IBotClientAccessor
{
    public ITelegramBotClient Client { get; }

    public BotClientAccessor(IConfiguration configuration, ILogger<BotClientAccessor> logger)
    {
        var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")
                    ?? configuration["TELEGRAM_BOT_TOKEN"]
                    ?? configuration["Telegram:BotToken"];

        if (string.IsNullOrWhiteSpace(token))
        {
            logger.LogError("TELEGRAM_BOT_TOKEN is not set in environment or configuration.");
            throw new InvalidOperationException("TELEGRAM_BOT_TOKEN is not set in environment or configuration.");
        }

        Client = new TelegramBotClient(token);
    }
}