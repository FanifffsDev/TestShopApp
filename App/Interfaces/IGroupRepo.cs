using TestShopApp.Domain.Base;
using TestShopApp.Telegram;

namespace TestShopApp.App.Interfaces;

public interface IGroupRepo
{
    Task CreateGroup(Group group, CancellationToken ct);
    void UpdateGroup(Group group);
    void DeleteGroup(Group group);
    Task<Group?> GetGroup(string groupNumber, CancellationToken ct);

    Task<bool> SaveChangesAsync(CancellationToken ct);
}