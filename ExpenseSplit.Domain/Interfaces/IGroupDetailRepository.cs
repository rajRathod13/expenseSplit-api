using ExpenseSplit.Domain.Entities;

namespace ExpenseSplit.Domain.Interfaces;

public interface IGroupDetailRepository : IDependencyMarkerRepository
{
    Task<GroupDetail> AddAsync(GroupDetail entity, CancellationToken cancellationToken);

    Task<List<GroupDetail>> GetGroupDetailsAsync(CancellationToken cancellationToken);
    Task<GroupDetail> GetGroupDetailAsync(Guid id, CancellationToken cancellationToken);
    //IQueryable<GroupDetail> GetGroupsByUserId(string userId);
    Task<List<GroupDetail>> GetGroupsByGroupIds(List<Guid> groupIds, CancellationToken cancellationToken);

    Task<List<GroupDetail>> GetGroupsByUserIdAsync(string userId, CancellationToken cancellationToken);
    void DeleteGroup(GroupDetail group);
}
