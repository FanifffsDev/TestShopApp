using Telegram.Bot;
using Telegram.Bot.Types;
using TestShopApp.Common.Data;

namespace TestShopApp.Telegram.Commands
{
    public class StartCmd : ICommand
    {
        public string Command => "/start";

        public string Description => "start page";

        public string Format => "/start";

        public bool Admin => false;

        public async Task<ExecutionResult> Execute(ITelegramBotClient bot, AuthUser user, Message msg)
        {
            //Пока не нужно;
            return new ExecutionResult(success: true);
        }
    }
}
