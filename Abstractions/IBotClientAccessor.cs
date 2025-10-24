using Telegram.Bot;

namespace FreemJuniorBot.Abstractions;

/// <summary>
/// Provides access to a single instance of ITelegramBotClient via DI.
/// </summary>
public interface IBotClientAccessor
{
    ITelegramBotClient Client { get; }
}