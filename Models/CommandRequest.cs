namespace FreemJuniorBot.Models;

public sealed class CommandRequest
{
    public string? Command { get; init; }
    public string? Value { get; init; }
}