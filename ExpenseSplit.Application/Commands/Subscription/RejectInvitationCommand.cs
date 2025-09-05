using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.RequestDTOs.Subscription;
using ExpenseSplit.Common.ResponseDTOs;
using MediatR;

namespace ExpenseSplit.Application.Commands.Subscription;

public class RejectInvitationCommand : IRequest<BaseResponse<bool>>
{
    public RejectInvitationRequest RejectInvitationRequest { get; set; }
}

public class RejectInvitationCommandHandler : IRequestHandler<RejectInvitationCommand, BaseResponse<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public RejectInvitationCommandHandler(IUnitOfWork uow,
        ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }
    public async Task<BaseResponse<bool>> Handle(RejectInvitationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId))
            return BaseResponse<bool>.Fail("Unauthorized user.");

        var invitation = await _uow.InvitationRepository.GetInvitationByIdAsync(request.RejectInvitationRequest.InvitationId, cancellationToken);

        if (invitation is null)
            return BaseResponse<bool>.Fail($"Invitation not found with id- {request.RejectInvitationRequest.InvitationId}");

        if (!(invitation.Status == Domain.Enums.InvitationStatus.Pending) )
            return BaseResponse<bool>.Fail("Invalid operation.");

        if (invitation.InvitedUserId != userId)
            return BaseResponse<bool>.Fail("Unauthorized user.");

        invitation.Status = Domain.Enums.InvitationStatus.Rejected;
        invitation.UpdatedOn = DateTime.UtcNow;
        invitation.UpdatedBy = Guid.Parse(userId);

        _uow.InvitationRepository.UpdateInvitationStatus(invitation);
        var result = await _uow.SaveChangesAsync(cancellationToken);
        if (result <= 0)
            return BaseResponse<bool>.Fail("An error occured during rejecting the invitation.");

        return BaseResponse<bool>.Success(true, "Invitation rejected successfully.");
    }
}
