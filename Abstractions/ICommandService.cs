using FreemJuniorBot.Models;

namespace FreemJuniorBot.Abstractions;

public interface ICommandService
{
    /// <summary>
    /// Process incoming command/value pair.
    /// </summary>
    Task HandleAsync(CommandRequest command, CancellationToken ct);
}