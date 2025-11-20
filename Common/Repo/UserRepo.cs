using TestShopApp.Common.Data;

namespace TestShopApp.Common.Repo
{
    public class UserRepo(AppDbContext context) : IUserRepo
    {
        private readonly AppDbContext _context = context;
        public async Task<ExecutionResult<User>> AddUser(User user)
        {
            await _context.AddAsync(user);
            return new ExecutionResult<User>(await SaveChangesAsync(), user);
        }

        public async Task<ExecutionResult<User>> GetUser(long userId)
        {
            //var res = await _context.Users.FindAsync(userId);

            User res = null;
            
            return new ExecutionResult<User>(res != null, res);
        }

        public async Task<ExecutionResult<User>> UpdateUser(User user)
        {
            _context.Update(user);
            return new ExecutionResult<User>(await SaveChangesAsync(), user);
        }
        
        private async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
