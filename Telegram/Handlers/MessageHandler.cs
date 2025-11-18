using Telegram.Bot;
using Telegram.Bot.Types;
using TestShopApp.Common.Data;
using TestShopApp.Telegram.Commands;
using TestShopApp.Telegram.Utils;

namespace TestShopApp.Telegram.Handlers
{
    public class MessageHandler(ITelegramBotClient bot, CommandProvider commandProvider)
    {
        private readonly ITelegramBotClient _bot = bot;
        private readonly CommandProvider _commandProvider = commandProvider;
        public async Task Handle(Message message, TgUser user)
        {        
            if (message.From == null || string.IsNullOrEmpty(message.Text)) return;

            /*var user = await _userRepo.GetUser(message.From.Id);

            if (!user.success) { return; }

            if (message.Text.StartsWith("/"))
            {
                var cmd = _commandProvider.GetCommand(message.Text.Split(" ")[0].Replace("@plebee_bot", "").Replace("@plebeetestbot", ""));

                if (cmd == null)
                {
                    await _bot.SendMessage(message.Chat.Id, "Я такого не знаю",
                        replyParameters: BotUtils.Reply(message));
                    return;
                }

                // Выполняем команду синхронно в рамках уже созданного scope
                try
                {
                    var res = await cmd.Execute(_bot, user.Value, message);

                    if (!res.success)
                    {
                        Console.WriteLine($"Command {cmd.Command} failed: {res.message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка выполнения команды {cmd.Command}: {ex}");
                }
                return;
            }*/

            await _bot.SendMessage(message.Chat.Id, "Стартовое сообщение",
                        replyParameters: BotUtils.Reply(message));
            return;
        }
    }
}
