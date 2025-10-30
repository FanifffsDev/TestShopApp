using Telegram.Bot;
using Telegram.Bot.Types;
using TestShopApp.Common.Data;

namespace TestShopApp.Telegram.Commands
{
    public interface ICommand
    {
        string Command { get; }
        string Description { get; }
        string Format { get; }
        bool Admin { get; }
        Task<ExecutionResult> Execute(ITelegramBotClient bot, TgUser user, Message msg);
    }
}
