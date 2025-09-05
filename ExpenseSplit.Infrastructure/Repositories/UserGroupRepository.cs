using ExpenseSplit.Domain.Entities;
using ExpenseSplit.Domain.Interfaces;
using ExpenseSplit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplit.Infrastructure.Repositories;

public class UserGroupRepository : IUserGroupRepository
{
    private readonly ApplicationContext _context;

    public UserGroupRepository(ApplicationContext context)
    {
        _context = context;
    }
    public async Task<UserGroupRef> AddAsyn(UserGroupRef entity, CancellationToken cancellationToken)
    {
        await _context.AddAsync(entity, cancellationToken);
        return entity;
    }

    public IQueryable<UserGroupRef> GetUserGroupsAsync(string userId)
    {
        var userGroups = _context.UserGroups
                                 .Where(x => x.UserId == userId && x.IsCreator);
        return userGroups;
    }

    public async Task<List<Guid>> GetGroupIdsAsync(string userId, CancellationToken cancellationToken)
    {
        var userGroupIds = await _context.UserGroups
                                         .Where(x => x.UserId == userId && !x.IsCreator)
                                         .Select(x => x.GroupId)
                                         .ToListAsync(cancellationToken);
        return userGroupIds;
    }

    public async Task<List<string>> GetUserIdsAsync(Guid groupId, CancellationToken cancellationToken)
    {
        var userIds = await _context.UserGroups
                                    .Where(x => x.GroupId == groupId)
                                    .Select(x => x.UserId)
                                    .ToListAsync(cancellationToken);
        return userIds;
    }

    public async Task<bool> IsGroupMemberAsync(string userId, Guid groupId, CancellationToken cancellationToken)
    {
        return await _context.UserGroups.AnyAsync(x => x.UserId == userId &&
                                                     x.GroupId == groupId,
                                                     cancellationToken);
    }

    public async Task RemoveUserAsync(string userId, Guid groupId, CancellationToken cancellationToken)
    {
        var userGroup = await _context.UserGroups.FirstOrDefaultAsync(x => (x.UserId == userId && x.GroupId == groupId), cancellationToken);
        if (userGroup is not null)
        {
            _context.Remove(userGroup);
        }
    }

    public async Task<bool> IsCreatorAsync(string userId, Guid groupId, CancellationToken cancellationToken)
    {
        return await _context.UserGroups.AnyAsync(x => x.UserId == userId &&
                                                     x.GroupId == groupId &&
                                                     x.IsCreator,
                                                     cancellationToken);
    }
}
