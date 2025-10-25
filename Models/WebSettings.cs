using System.ComponentModel.DataAnnotations;

namespace FreemJuniorBot.Models;

/// <summary>
/// Web hosting settings bound from configuration/environment variables.
/// </summary>
public sealed class WebSettings
{
    /// <summary>
    /// ASP.NET Core URLs binding. Bind via section key Web:Urls (env: Web__Urls).
    /// </summary>
    [Required]
    public string Urls { get; init; } = "";
}
