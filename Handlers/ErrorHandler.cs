using Telegram.Bot;
using Telegram.Bot.Exceptions;
using FreemJuniorBot.Abstractions;

namespace FreemJuniorBot.Handlers;

public sealed class ErrorHandler(ILogger<ErrorHandler> logger) : IErrorHandler
{
    public Task HandleAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        logger.LogError("{ErrorMessage}", errorMessage);
        return Task.CompletedTask;
    }
}