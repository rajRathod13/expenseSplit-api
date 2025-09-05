using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.RequestDTOs.Subscription;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Domain.Entities;
using MediatR;

namespace ExpenseSplit.Application.Commands.Subscription;

public class AcceptInvitationCommand : IRequest<BaseResponse<bool>>
{
    public AcceptInvitationRequest AcceptInvitationRequest { get; set; }
}

public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand, BaseResponse<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public AcceptInvitationCommandHandler(IUnitOfWork uow,
        ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }
    public async Task<BaseResponse<bool>> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId))
            return BaseResponse<bool>.Fail("Unauthorized user.");

        var pendingInvitation = await _uow.InvitationRepository.GetInvitationByIdAsync(request.AcceptInvitationRequest.InvitationId, cancellationToken);
        if (pendingInvitation is null)
            return BaseResponse<bool>.Fail($"Invitaion not found with id: {request.AcceptInvitationRequest.InvitationId} ");

        if (pendingInvitation.InvitedUserId != userId)
            return BaseResponse<bool>.Fail("Unauthorized user.");

        if (pendingInvitation.Status != Domain.Enums.InvitationStatus.Pending)
            return BaseResponse<bool>.Fail("Invite not pending.");

        var isMember = await _uow.UserGroupRepository.IsGroupMemberAsync(userId, pendingInvitation.GroupId, cancellationToken);
        if (!isMember)
        {
            var userGroup = new UserGroupRef
            {
                UserGroupId = Guid.NewGuid(),
                UserId = userId,
                GroupId = pendingInvitation.GroupId,
                IsCreator=false,
                CreatedBy = Guid.Parse(userId),
                CreatedOn = DateTime.UtcNow,
            };

            await _uow.UserGroupRepository.AddAsyn(userGroup, cancellationToken);
        }

        pendingInvitation.Status = Domain.Enums.InvitationStatus.Accepted;
        pendingInvitation.UpdatedBy = Guid.Parse(userId);
        pendingInvitation.UpdatedOn = DateTime.UtcNow;

        _uow.InvitationRepository.UpdateInvitationStatus(pendingInvitation);
        var result = await _uow.SaveChangesAsync(cancellationToken);
        if (result <= 0)
            return BaseResponse<bool>.Fail("An error occured during accpeting invitation.");

        return BaseResponse<bool>.Success(true, "Invitation accepted successfully.");
    }
}
