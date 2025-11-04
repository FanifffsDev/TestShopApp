using Telegram.Bot;
using TestShopApp.Telegram.Handlers;

namespace TestShopApp
{
    public class BotWorker(ITelegramBotClient botClient, IServiceScopeFactory scopeFactory) : BackgroundService
    {
        private readonly ITelegramBotClient _botClient = botClient;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            return;
            _botClient.StartReceiving(
                updateHandler: async (bot, update, ct) =>
                {
                    using var updateScope = _scopeFactory.CreateScope();
                    var scopedHandler = updateScope.ServiceProvider.GetRequiredService<UpdateHandler>();
                    await scopedHandler.HandleUpdateAsync(bot, update, ct);
                },
                errorHandler: async (bot, exception, ct) =>
                {
                    using var errorScope = _scopeFactory.CreateScope();
                    var scopedHandler = errorScope.ServiceProvider.GetRequiredService<UpdateHandler>();
                    await scopedHandler.HandleErrorAsync(bot, exception, ct);
                },
                cancellationToken: stoppingToken
            );

            var me = await _botClient.GetMe();
            Console.WriteLine($"Бот @{me.Username} запущен с поддержкой параллельной обработки");
        }
    }
}
