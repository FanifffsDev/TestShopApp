using TestShopApp.Domain.Base;

namespace TestShopApp.App.Interfaces
{
    public interface IUserRepo
    {
        Task AddUser(User user, CancellationToken ct);
        Task<User?> GetUser(long userId, CancellationToken ct);
        Task<IEnumerable<User?>> GetGroupMembers(string groupNumber, CancellationToken ct);
        Task<int> GetStudentCount(string groupNumber, CancellationToken ct);
        Task<User?> GetHeadmanByGroup(string groupNumber, CancellationToken ct);
        void UpdateUser(User updatedUser);

        Task<bool> SaveChangesAsync(CancellationToken ct);
    }
}
