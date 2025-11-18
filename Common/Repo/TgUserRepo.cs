using TestShopApp.Common.Data;

namespace TestShopApp.Common.Repo
{
    public class TgUserRepo(AppDbContext context) : ITgUserRepo
    {
        private readonly AppDbContext _context = context;
        public async Task<ExecutionResult> AddUser(TgUser user)
        {
            await _context.AddAsync(user);
            return new ExecutionResult(await SaveChangesAsync());
        }

        public async Task<ExecutionResult<TgUser>> GetUser(long userId)
        {
            //var res = await _context.Users.FindAsync(userId);

            var res = new TgUser
            {
                Id = 123456789,
                IsPremium = false,
                AllowsWriteToPm = false,
                FirstName = "firstname",
                LastName = "lastname",
                Username = "hzpox",
                PhotoUrl = "https://.....",
                LanguageCode = "ru"
            };
            return new ExecutionResult<TgUser>(res != null, res);
        }

        public async Task<ExecutionResult<TgUser>> UpdateUser(TgUser user)
        {
            _context.Update(user);
            return new ExecutionResult<TgUser>(await SaveChangesAsync(), user);
        }
        
        private async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
