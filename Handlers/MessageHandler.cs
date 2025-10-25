using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using FreemJuniorBot.Abstractions;
using FreemJuniorBot.Models;

namespace FreemJuniorBot.Handlers;

/// <summary>
/// Обработчик сообщений. Содержит обработчики для уже существующих кейсов сообщений (текст и фото)
/// и маршрутизирует входящие обновления.
/// </summary>
public sealed class MessageHandler(ILogger<MessageHandler> logger, BotSettings botSettings) : IMessageHandler
{
    private readonly long _ownerId = botSettings.OwnerId;

    /// <summary>
    /// Точка входа: маршрутизация по типам сообщений.
    /// </summary>
    public async Task HandleAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        if (update.Message is not { Chat: not null } message)
        {
            return;
        }

        try
        {
            switch (message.Type)
            {
                case MessageType.Text:
                    await HandleTextAsync(botClient, message, ct);
                    break;
                case MessageType.Photo:
                    await HandlePhotoAsync(botClient, message, ct);
                    break;
                default:
                    // Другие типы пока не обрабатываем
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обработке обновления");
        }
    }

    public async Task HandleHTTPCommand(ITelegramBotClient botClient, CommandRequest command, CancellationToken ct)
    {
        try
        {
            await botClient.SendMessage(_ownerId, $"{command.Command}: {command.Value}", cancellationToken: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при отправке команды");
        }
    }

    /// <summary>
    /// Обработчик текстовых сообщений.
    /// </summary>
    private static async Task HandleTextAsync(ITelegramBotClient botClient, Message message, CancellationToken ct)
    {
        await botClient.SendMessage(message.Chat, $"Вы прислали {message.Text}", cancellationToken: ct);
    }

    /// <summary>
    /// Обработчик фото-сообщений.
    /// </summary>
    private static async Task HandlePhotoAsync(ITelegramBotClient botClient, Message message, CancellationToken ct)
    {
        await botClient.SendMessage(message.Chat, "Вы прислали фото", cancellationToken: ct);
        if (!string.IsNullOrWhiteSpace(message.Caption))
        {
            await botClient.SendMessage(message.Chat, $"Вы также прислали описание: {message.Caption}", cancellationToken: ct);
        }
    }
}