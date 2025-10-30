using Telegram.Bot.Types;

namespace TestShopApp.Telegram.Utils
{
    public static class BotUtils
    {
        public static ReplyParameters Reply(Message msg)
        {
            return new ReplyParameters
            {
                ChatId = msg.Chat.Id,
                MessageId = msg.MessageId
            };
        }
    }
}
