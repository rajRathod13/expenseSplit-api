using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Common.ResponseDTOs.Invitation;
using MediatR;

namespace ExpenseSplit.Application.Queries.InvitationQueries;

public class GetPendingInvitationsQuery : IRequest<BaseResponse<List<InvitationResponse>>>
{
}

public class GetPendingInvitationsQueryHandler : IRequestHandler<GetPendingInvitationsQuery, BaseResponse<List<InvitationResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetPendingInvitationsQueryHandler(IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    public async Task<BaseResponse<List<InvitationResponse>>> Handle(GetPendingInvitationsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId))
            return BaseResponse<List<InvitationResponse>>.Fail("Unauthorize user.");

        var invitations = await _uow.InvitationRepository.GetPendingInvitationsAsync(userId, cancellationToken);
        var mappedResponse = _mapper.Map<List<InvitationResponse>>(invitations);

        var message = mappedResponse.Any() ? "Pending invitations fetched successfully." : "No records found.";
        return BaseResponse<List<InvitationResponse>>.Success(mappedResponse, message);
    }
}

