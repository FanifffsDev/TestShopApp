using TestShopApp.Common.Data;

namespace TestShopApp.Common.Repo;

public interface IGroupRepo
{
    Task<ExecutionResult<Group>> CreateGroup(Group group);
    Task<ExecutionResult<Group>> UpdateGroup(string groupNumber, string groupName, long callerId);
    Task<ExecutionResult<Group>> DeleteGroup(string groupNumber, long callerId);


    Task<ExecutionResult<Group>> GetGroup(string groupNumber);
    Task<ExecutionResult<IEnumerable<User>>> GetGroupMembers(string groupNumber);
    //Task<ExecutionResult<IEnumerable<Group>>> GetGroups();



    Task<ExecutionResult> AddStudentToGroup(long studentId, string groupNumber);
    Task<ExecutionResult> RemoveStudentFromGroup(long studentId);
    Task<ExecutionResult<int>> GetStudentCount(string groupNumber);
}