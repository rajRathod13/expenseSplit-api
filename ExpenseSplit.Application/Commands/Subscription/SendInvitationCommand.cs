using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.RequestDTOs.Subscription;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Domain.Entities;
using MediatR;
using System.Text;

namespace ExpenseSplit.Application.Commands.Subscription;

public class SendInvitationCommand : IRequest<BaseResponse<string>>
{
    public InvitationRequest InvitationRequest { get; set; }
}

public class SendInvitationCommandHandler : IRequestHandler<SendInvitationCommand, BaseResponse<string>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public SendInvitationCommandHandler(IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    public async Task<BaseResponse<string>> Handle(SendInvitationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId))
            throw new Exception("User not authenticated.");

        var group = await _uow.GroupDetailRepository.GetGroupDetailAsync(request.InvitationRequest.GroupId, cancellationToken);
        //var invitationLink = string.Empty;
        if (group is null)
            return BaseResponse<string>.Fail($"No group found for groupid - {request.InvitationRequest.GroupId}.");

        var isCreator = await _uow.UserGroupRepository.IsCreatorAsync(userId, request.InvitationRequest.GroupId, cancellationToken);
        if (!isCreator)
            return BaseResponse<string>.Fail("Only group creator can invite.");

        var isMember = await _uow.UserGroupRepository.IsGroupMemberAsync(request.InvitationRequest.InvitedUserId,request.InvitationRequest.GroupId, cancellationToken); 
        if (isMember)
            return BaseResponse<string>.Fail("User is already a member.");

        var invitation = new Invitation
        {
            InvitationId = Guid.NewGuid(),
            InviterId = userId,
            InvitedUserId = request.InvitationRequest.InvitedUserId,
            GroupId = group.GroupId,
            Status = Domain.Enums.InvitationStatus.Pending,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = Guid.Parse(userId)
        };

        var createdInvitation = await _uow.InvitationRepository.AddInvitationAsync(invitation, cancellationToken);
        var result = await _uow.SaveChangesAsync(cancellationToken);
        if(result <=0)
            return BaseResponse<string>.Fail("An error occured during sending invitation.");


        var invitationLink = GenerateInvitationLink(group.GroupId);
        var message = string.IsNullOrWhiteSpace(invitationLink) ? "Error occured during generation of invitation link." : "Invitation link send successfully to the invited user mail. This is post request.";

        return BaseResponse<string>.Success(invitationLink, message);
    }

    private string GenerateInvitationLink(Guid groupId)
    {
        if (groupId == Guid.Empty || string.IsNullOrWhiteSpace(groupId.ToString()))
            return string.Empty;

        var link = new StringBuilder();
        link.Append($"https://localhost:7207/api/subscription/acceptInvitation?groupId={groupId}");

        return link.ToString();
    }
}
