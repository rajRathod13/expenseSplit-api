using ExpenseSplit.Domain.Entities;
using ExpenseSplit.Domain.Interfaces;
using ExpenseSplit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplit.Infrastructure.Repositories;

public class GroupDetailRepository : IGroupDetailRepository
{
    private readonly ApplicationContext _context;

    public GroupDetailRepository(ApplicationContext context)
    {
        _context = context;
    }
    public async Task<GroupDetail> AddAsync(GroupDetail entity, CancellationToken cancellationToken)
    {
        await _context.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<List<GroupDetail>> GetGroupDetailsAsync(CancellationToken cancellationToken) => await _context.GroupDetails.ToListAsync(cancellationToken);

    public async Task<GroupDetail> GetGroupDetailAsync(Guid id, CancellationToken cancellationToken)
    {
        var group = await _context.GroupDetails
                                    .Include(x => x.User)
                                    .Include(x => x.UserGroupRefs)
                                    .FirstOrDefaultAsync(x => x.GroupId == id, cancellationToken);
        return group;
    }

    public async Task<List<GroupDetail>> GetGroupsByGroupIds(List<Guid> groupIds, CancellationToken cancellationToken)
    {
        var groups = await _context.GroupDetails
                                   .Include(u => u.User)
                                   .Include(g => g.UserGroupRefs)
                                   .Where(x => groupIds.Contains(x.GroupId))
                                   .ToListAsync(cancellationToken);
        return groups;
    }

    //public IQueryable<GroupDetail> GetGroupsByUserId(string userId) 
    //{
    //    return _context.GroupDetails.Where(x => x.Id == userId);
    //}

    public async Task<List<GroupDetail>> GetGroupsByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await _context.GroupDetails
                             .Include(u => u.User)
                             .Include(g => g.UserGroupRefs)
                             .Where(g => g.UserId == userId)
                             .ToListAsync(cancellationToken);
    }

    public void DeleteGroup(GroupDetail group) =>
        _context.GroupDetails.Remove(group);
}
