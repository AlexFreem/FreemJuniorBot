using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using FreemJuniorBot.Abstractions;
using FreemJuniorBot.Handlers;
using FreemJuniorBot.Hosting;
using FreemJuniorBot.Models;
using FreemJuniorBot.Services;

var builder = WebApplication.CreateBuilder(args);

// Helper: bind from a named section then overlay root/env and validate data annotations
T BindAndValidate<T>(string sectionName) where T : new()
{
    var instance = new T();
    var section = builder.Configuration.GetSection(sectionName);
    section.Bind(instance); // bind from section first
    builder.Configuration.Bind(instance); // overlay root/env

    var ctx = new ValidationContext(instance);
    var results = new List<ValidationResult>();
    if (!Validator.TryValidateObject(instance, ctx, results, validateAllProperties: true))
    {
        var message = string.Join("; ", results.Select(r => r.ErrorMessage));
        throw new ValidationException($"Configuration for '{typeof(T).Name}' is invalid: {message}");
    }

    return instance;
}

// Settings bound via configuration binder (sections + env) and validated
var botSettings = BindAndValidate<BotSettings>("Bot");
var webSettings = BindAndValidate<WebSettings>("Web");
var databaseSettings = BindAndValidate<DatabaseSettings>("Database");

builder.Services.AddSingleton(botSettings);
builder.Services.AddSingleton(webSettings);
builder.Services.AddSingleton(databaseSettings);

// Services
builder.Services.AddSingleton<IBotClientAccessor, BotClientAccessor>();
builder.Services.AddSingleton<IErrorHandler, ErrorHandler>();
builder.Services.AddSingleton<IMessageHandler, MessageHandler>();
// Command processing service
builder.Services.AddSingleton<ICommandService, CommandService>();
// Owner/Admin id provider is no longer used; BotSettings is used directly for owner id

// Background worker that runs the Telegram bot
builder.Services.AddHostedService<Worker>();

// Configure Kestrel to listen on port 80 by default if ASPNETCORE_URLS not set
if (string.IsNullOrWhiteSpace(webSettings.Urls))
{
    builder.WebHost.UseUrls("http://0.0.0.0:80");
}
else
{
    builder.WebHost.UseUrls(webSettings.Urls);
}

var app = builder.Build();

// Minimal health endpoint
app.MapGet("/healthz", () => Results.Ok("OK"));

// Helper to resolve command/value from query or JSON body
static async Task<(string? command, string? value, string? error)> ResolveCommandAsync(HttpContext context, CancellationToken ct)
{
    var query = context.Request.Query;
    var command = query.TryGetValue("command", out var cq) ? cq.ToString() : null;
    var value = query.TryGetValue("value", out var vq) ? vq.ToString() : null;

    if (!string.IsNullOrWhiteSpace(command) && !string.IsNullOrWhiteSpace(value))
    {
        return (command, value, null);
    }

    // Try to read JSON body: { command: "...", value: "..." }
    try
    {
        // If the body has already been read or is empty, this will just yield null
        var req = await JsonSerializer.DeserializeAsync<CommandRequest>(context.Request.Body, cancellationToken: ct);
        command ??= req?.Command;
        value ??= req?.Value;

        if (string.IsNullOrWhiteSpace(command) || string.IsNullOrWhiteSpace(value))
        {
            return (null, null, "Both 'command' and 'value' must be provided either as query parameters or in JSON body.");
        }

        return (command!, value!, null);
    }
    catch (Exception ex)
    {
        return (null, null, $"Failed to parse body: {ex.Message}");
    }
}

// GET /command supports query params and optionally JSON body
app.MapGet("/command", async (HttpContext context, ICommandService service, CancellationToken ct) =>
{
    var (command, value, error) = await ResolveCommandAsync(context, ct);
    if (command is null || value is null)
    {
        return Results.BadRequest(new { error });
    }

    await service.HandleAsync(new CommandRequest(){Command = command, Value = value}, ct);
    return Results.Ok();
});

// POST /command supports query params and JSON body
app.MapPost("/command", async (HttpContext context, ICommandService service, CancellationToken ct) =>
{
    var (command, value, error) = await ResolveCommandAsync(context, ct);
    if (command is null || value is null)
    {
        return Results.BadRequest(new { error });
    }

    await service.HandleAsync(new CommandRequest(){Command = command, Value = value}, ct);
    return Results.Ok();
});

app.Run();