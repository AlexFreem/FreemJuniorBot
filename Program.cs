using FreemJuniorBot.Abstractions;
using FreemJuniorBot.Handlers;
using FreemJuniorBot.Hosting;

namespace FreemJuniorBot;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddSingleton<IErrorHandler, ErrorHandler>();
        builder.Services.AddSingleton<IMessageHandler, MessageHandler>();
        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();
        host.Run();
    }
}