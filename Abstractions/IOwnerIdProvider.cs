namespace FreemJuniorBot.Abstractions;

/// <summary>
/// Provides access to the configured Telegram owner/admin id.
/// Reads from environment/configuration once at startup and exposes parsed value.
/// </summary>
public interface IOwnerIdProvider
{
    /// <summary>
    /// Parsed owner id, if configured and valid; otherwise null.
    /// </summary>
    long OwnerId { get; }

    /// <summary>
    /// Raw value taken from environment/configuration, if any.
    /// </summary>
    string? Raw { get; }
}