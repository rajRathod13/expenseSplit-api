using ExpenseSplit.Domain.Entities;

namespace ExpenseSplit.Domain.Interfaces;

public interface IInvitationRepository : IDependencyMarkerRepository
{
    Task<Invitation> AddInvitationAsync(Invitation invitation, CancellationToken cancellationToken);

    void RemoveInvitation(Invitation invitation); 

    void UpdateInvitationStatus(Invitation invitation);

    Task<List<Invitation>> GetPendingInvitationsAsync(string userId, CancellationToken cancellationToken);
    Task<List<Invitation>> GetSentInvitationsAsync(string userId, CancellationToken cancellationToken);
    Task<List<Invitation>> GetAcceptedInvitationsAsync(string userId, CancellationToken cancellationToken);
    Task<List<Invitation>> GetRejectedInvitationsAsync(string userId, CancellationToken cancellationToken);
    Task<List<Invitation>> GetCancelledInvitationsAsync(string userId, CancellationToken cancellationToken);
    Task<Invitation> GetInvitationByIdAsync(Guid invitationId, CancellationToken cancellationToken);
    Task<bool> HasPendingInvitaionAsync(string invitedUserId, Guid groupId, CancellationToken cancellationToken);
}
