using System.ComponentModel.DataAnnotations;

namespace FreemJuniorBot.Models;

/// <summary>
/// Settings for Telegram Bot bound from configuration/environment variables.
/// </summary>
public sealed class BotSettings
{
    /// <summary>
    /// Telegram bot token. Bind via section key Bot:Token (and env var with same name) or general overlay.
    /// </summary>
    [Required]
    public string Token { get; init; } = "";

    /// <summary>
    /// Owner id.
    /// </summary>
    [Range(1, long.MaxValue, ErrorMessage = "OwnerId must be a positive number")]
    public long OwnerId { get; init; } = 0;
}