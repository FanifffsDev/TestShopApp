using TestShopApp.Common.Data;

namespace TestShopApp.Common.Repo;

public interface IGroupRepo
{
    Task<ExecutionResult<Group>> GetGroup(string groupNumber);
    Task<ExecutionResult<IEnumerable<Group>>> GetGroups();
    Task<ExecutionResult<Group>> CreateGroup(string groupNumber, string groupName, long ownerId);
    Task<ExecutionResult<Group>> UpdateGroup(string groupNumber, string groupName, long callerId);
    
    Task<ExecutionResult<Group>> DeleteGroup(string groupNumber, long callerId);
}