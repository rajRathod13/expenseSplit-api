using ExpenseSplit.Domain.Entities;

namespace ExpenseSplit.Domain.Interfaces;

public interface IUserGroupRepository : IDependencyMarkerRepository
{
    Task<UserGroupRef> AddAsyn(UserGroupRef entity, CancellationToken cancellationToken);

    //Task<List<UserGroupRef>> GetUserGroupsAsync(string userId);
    IQueryable<UserGroupRef> GetUserGroupsAsync(string userId);
    Task<List<Guid>> GetGroupIdsAsync(string userId, CancellationToken cancellationToken);
    Task<List<string>> GetUserIdsAsync(Guid groupId, CancellationToken cancellationToken);
    Task RemoveUserAsync(string userId, Guid groupId, CancellationToken cancellationToken);

    Task<bool> IsGroupMemberAsync(string userId, Guid groupId, CancellationToken cancellationToken);
    Task<bool> IsCreatorAsync(string userId,Guid groupId, CancellationToken cancellationToken);
}
