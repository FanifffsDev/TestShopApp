using AutoMapper;
using TestShopApp.App.Models.User;
using TestShopApp.Common.Data;

namespace TestShopApp.Common.Repo
{
    public interface IUserRepo
    {
        Task<ExecutionResult<User>> AddUser(User user);
        Task<ExecutionResult<User>> GetUser(long userId);
        Task<ExecutionResult<User>> UpdateUser(long userId, UpdateUserDto updateDto, IMapper mapper);
        
        Task<ExecutionResult<User>> MakeHeadmanOf(long userId, string groupNumber);
    }
}
