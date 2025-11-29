using Telegram.Bot;
using Telegram.Bot.Types;
using TestShopApp.Domain.Base;

namespace TestShopApp.Telegram.Commands
{
    public interface ICommand
    {
        string Command { get; }
        string Description { get; }
        string Format { get; }
        bool Admin { get; }
        Task<ExecutionResult> Execute(ITelegramBotClient bot, AuthUser user, Message msg);
    }
}
