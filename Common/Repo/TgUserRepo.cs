using TestShopApp.Common.Data;

namespace TestShopApp.Common.Repo
{
    public class TgUserRepo : ITgUserRepo
    {
        public async Task<ExecutionResult> AddUser(TgUser data)
        {
            throw new NotImplementedException();
        }

        public async Task<ExecutionResult<TgUser>> GetUser(long userId)
        {
            throw new NotImplementedException();
        }
    }
}
