using TestShopApp.Common.Data;

namespace TestShopApp.Common.Repo
{
    public interface ITgUserRepo
    {
        Task<ExecutionResult> AddUser(TgUser data);
        Task<ExecutionResult<TgUser>> GetUser(long userId);
    }
}
