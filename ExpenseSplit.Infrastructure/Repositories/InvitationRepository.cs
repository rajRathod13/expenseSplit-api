using ExpenseSplit.Domain.Entities;
using ExpenseSplit.Domain.Interfaces;
using ExpenseSplit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace ExpenseSplit.Infrastructure.Repositories;

public class InvitationRepository : IInvitationRepository
{
    private readonly ApplicationContext _context;

    public InvitationRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<Invitation> AddInvitationAsync(Invitation invitation, CancellationToken cancellationToken) 
    {
        await _context.Invitations.AddAsync(invitation, cancellationToken);
        return invitation;
    }

    public async Task<List<Invitation>> GetPendingInvitationsAsync(string userId, CancellationToken cancellationToken)
    {
        var invitations = await _context.Invitations
                                     .Where(x => ( x.InvitedUserId == userId) &&
                                                 x.Status == Domain.Enums.InvitationStatus.Pending)
                                     // Group + members (+ each member's User)
                                     .Include(i => i.Group)
                                         .ThenInclude(g => g.UserGroupRefs)
                                     .Include(x => x.InviterUser)
                                     // Nice to have when loading multiple collections:
                                     .AsSplitQuery()               // avoids cartesian explosion
                                     .AsNoTracking()               // if read-only
                                     .ToListAsync(cancellationToken);

        return invitations;
    }

    public async Task<List<Invitation>> GetSentInvitationsAsync(string userId, CancellationToken cancellationToken) {
        var invitations = await _context.Invitations.Where(x => x.InviterId == userId &&
                                                                x.Status == Domain.Enums.InvitationStatus.Pending)
                                                    .Include(g => g.Group)
                                                    .ThenInclude(x => x.UserGroupRefs)
                                                    .Include(x => x.InvitedUser)
                                                    .AsSplitQuery()
                                                    .AsNoTracking()
                                                    .ToListAsync(cancellationToken);

        return invitations;
    }

    public async Task<List<Invitation>> GetAcceptedInvitationsAsync(string userId, CancellationToken cancellationToken)
    {
        var invitations = await _context.Invitations.Where(x => x.InviterId == userId &&
                                                                x.Status == Domain.Enums.InvitationStatus.Accepted)
                                                    .Include(x => x.Group)
                                                    .ThenInclude(g => g.UserGroupRefs)
                                                    .Include(x => x.InvitedUser)
                                                    .ToListAsync(cancellationToken);    
        return invitations;
    }

    public async Task<List<Invitation>> GetRejectedInvitationsAsync(string userId, CancellationToken cancellationToken)
    {
        var invitations = await _context.Invitations
                                                    .Where(x => (x.InviterId == userId) &&
                                                                x.Status == Domain.Enums.InvitationStatus.Rejected)
                                                    .Include(x => x.Group)
                                                    .ThenInclude (g => g.UserGroupRefs)
                                                    .Include (x => x.InvitedUser)
                                                    .ToListAsync(cancellationToken);
        return invitations;
    }

    public async Task<List<Invitation>> GetCancelledInvitationsAsync(string userId, CancellationToken cancellationToken) 
    {
        var invitations = await _context.Invitations.Include(x => x.Group)
                                                    .Where(x => x.InviterId == userId &&
                                                                x.Status == Domain.Enums.InvitationStatus.Cancelled)
                                                    .Include(u => u.InvitedUser)
                                                    .ToListAsync(cancellationToken);
        return invitations;
    }

    public async Task<Invitation> GetInvitationByIdAsync(Guid invitationId, CancellationToken cancellationToken)
    {
        var invitation = await _context.Invitations.FirstOrDefaultAsync(x => x.InvitationId == invitationId, cancellationToken);

        return invitation;
    }

    public Task<bool> HasPendingInvitaionAsync(string invitedUserId, Guid groupId, CancellationToken cancellationToken)
    {
        return _context.Invitations.AnyAsync(x => x.GroupId == groupId &&
                                                  x.InvitedUserId == invitedUserId &&
                                                  x.Status == Domain.Enums.InvitationStatus.Pending,
                                                  cancellationToken);
    }

    public void UpdateInvitationStatus(Invitation invitation)
    {
        _context.Invitations.Update(invitation);
    }

    public void RemoveInvitation(Invitation invitation)
    {
        _context.Invitations.Remove(invitation);
    }
}
