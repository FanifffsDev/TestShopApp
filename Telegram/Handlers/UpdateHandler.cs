using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TestShopApp.App.Interfaces;
using TestShopApp.Telegram.Commands;

namespace TestShopApp.Telegram.Handlers
{
    public class UpdateHandler(IConfiguration config, IServiceScopeFactory scopeFactory)
    {
        private readonly IConfiguration _config = config;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
        {
            try
            {
                if (update.Type != UpdateType.Message || update.Message?.From?.IsBot == true || update.Message == null)
                {
                    return Task.CompletedTask;
                }

                Console.WriteLine($"Message: {update.Message.Chat.Id} - {update.Message.MessageId}");

                if (update.Message.Text != null &&
                    update.Message.Chat.Type == ChatType.Private)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            using var messageScope = _scopeFactory.CreateScope();
                            var scopedUserRepo = messageScope.ServiceProvider.GetRequiredService<IUserRepo>();
                            var scopedBot = messageScope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
                            var scopedCommandProvider = messageScope.ServiceProvider.GetRequiredService<CommandProvider>();
                            var scopedMessageHandler = new MessageHandler(scopedBot, scopedCommandProvider);

                            await ProcessMessageAsync(bot, update.Message, scopedUserRepo, scopedMessageHandler);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка обработки сообщения: {ex}");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в HandleUpdateAsync: {ex}");
            }

            return Task.CompletedTask;
        }

        private async Task ProcessMessageAsync(ITelegramBotClient bot, Message message, IUserRepo userRepo, MessageHandler messageHandler)
        {
            if (message.From == null) return;

            /*var res = await userRepo.GetUser(message.From.Id);

            if (res.success)
            {
                await userRepo.UpdateName(message.From.Id, message.From.FirstName, message.From.Username == null ? "" : "@" + message.From.Username);
            }
            else
            {
                if (string.IsNullOrEmpty(_config["GroupId"])) return;

                var user = await bot.GetChatMember(Int64.Parse(_config["GroupId"]!), message.From.Id);
                var userRes = await userRepo.AddUser(new Userdata
                {
                    UserId = user.User.Id,
                    Username = user.User.Username == null ? "" : "@" + user.User.Username,
                    Name = user.User.FirstName,
                    Balance = 0,
                    Role = "Участник"
                });

                if (!userRes.success)
                {
                    return;
                }
            }*/

            await messageHandler.Handle(message, null);
        }

        public Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
        {
            Console.WriteLine($"Ошибка: {exception}");
            return Task.CompletedTask;
        }
    }
}
