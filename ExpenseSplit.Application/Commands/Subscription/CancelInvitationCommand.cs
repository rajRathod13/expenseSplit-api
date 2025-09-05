using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.RequestDTOs.Subscription;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Domain.Enums;
using MediatR;

namespace ExpenseSplit.Application.Commands.Subscription;

public class CancelInvitationCommand : IRequest<BaseResponse<bool>>
{
    public CancelInvitationRequest CancelInvitationRequest { get; set; }
}

public class CancelInvitationCommandHandler : IRequestHandler<CancelInvitationCommand, BaseResponse<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public CancelInvitationCommandHandler(IUnitOfWork uow,
        ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }
    public async Task<BaseResponse<bool>> Handle(CancelInvitationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId))
            return BaseResponse<bool>.Fail("Unauthorized user.");

        var invitation = await _uow.InvitationRepository.GetInvitationByIdAsync(request.CancelInvitationRequest.InvitationId, cancellationToken);
        if(invitation is null)
            return BaseResponse<bool>.Fail($"No invitation found with id - {request.CancelInvitationRequest.InvitationId}");

        if(invitation.InviterId != userId)
            return BaseResponse<bool>.Fail("Unauthorized user.");

        if(invitation.Status == InvitationStatus.Accepted || invitation.Status == InvitationStatus.Rejected || invitation.Status == InvitationStatus.Cancelled)
            return BaseResponse<bool>.Fail($"Can not cancell the invitation as it was already {((InvitationStatus)invitation.Status).ToString()}");

        invitation.Status = InvitationStatus.Cancelled;
        invitation.UpdatedBy = Guid.Parse( userId);
        invitation.UpdatedOn = DateTime.UtcNow;

        _uow.InvitationRepository.UpdateInvitationStatus(invitation);
        var result = await _uow.SaveChangesAsync(cancellationToken);
        if(result <= 0)
            return BaseResponse<bool>.Fail("An error occured during cancelling the invitation");

        return BaseResponse<bool>.Success(true, "Invitation cancelled successfully.");
    }
}
