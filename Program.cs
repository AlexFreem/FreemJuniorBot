using FreemJuniorBot.Abstractions;
using FreemJuniorBot.Handlers;
using FreemJuniorBot.Hosting;
using FreemJuniorBot.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddSingleton<IBotClientAccessor, BotClientAccessor>();
builder.Services.AddSingleton<IErrorHandler, ErrorHandler>();
builder.Services.AddSingleton<IMessageHandler, MessageHandler>();

// Background worker that runs the Telegram bot
builder.Services.AddHostedService<Worker>();

// Configure Kestrel to listen on port 80 by default if ASPNETCORE_URLS not set
var urlsFromEnv = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");

if (string.IsNullOrWhiteSpace(urlsFromEnv))
{
    builder.WebHost.UseUrls("http://0.0.0.0:80");
}

var app = builder.Build();

// Minimal health endpoint
app.MapGet("/healthz", () => Results.Ok("OK"));
app.Run();