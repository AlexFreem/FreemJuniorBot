using FreemJuniorBot.Abstractions;

namespace FreemJuniorBot.Services;

/// <summary>
/// Reads TELEGRAM_OWNER_ID (priority) or TELEGRAM_ADMIN_ID once at startup and exposes parsed OwnerId.
/// </summary>
public sealed class OwnerIdProvider : IOwnerIdProvider
{
    public long OwnerId { get; } = 0;
    public string? Raw { get; }

    public OwnerIdProvider(IConfiguration configuration, ILogger<OwnerIdProvider> logger)
    {
        // Priority: env TELEGRAM_OWNER_ID, then env TELEGRAM_ADMIN_ID, then configuration keys
        Raw = Environment.GetEnvironmentVariable("TELEGRAM_OWNER_ID")
              ?? Environment.GetEnvironmentVariable("TELEGRAM_ADMIN_ID")
              ?? configuration["TELEGRAM_OWNER_ID"]
              ?? configuration["TELEGRAM_ADMIN_ID"]
              ?? configuration["Telegram:OwnerId"]
              ?? configuration["Telegram:AdminId"];

        if (!string.IsNullOrWhiteSpace(Raw) && long.TryParse(Raw, out var parsed))
        {
            OwnerId = parsed;
            logger.LogInformation("Owner/Admin id loaded: {OwnerId}", OwnerId);
        }
        else if (!string.IsNullOrWhiteSpace(Raw))
        {
            logger.LogError("Owner/Admin id value is invalid: {Value}", Raw);
        }
        else
        {
            logger.LogInformation("Owner/Admin id not configured (TELEGRAM_OWNER_ID/TELEGRAM_ADMIN_ID)");
        }
    }
}