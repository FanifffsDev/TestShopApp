using TestShopApp.Common.Data;

namespace TestShopApp.Common.Repo
{
    public interface IUserRepo
    {
        Task<ExecutionResult<User>> AddUser(User user);
        Task<ExecutionResult<User>> GetUser(long userId);
        Task<ExecutionResult<User>> UpdateUser(User user);
    }
}
