using ExpenseSplit.Common.ResponseDTOs.Invitation;
using ExpenseSplit.Common.ResponseDTOs;
using MediatR;
using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;

namespace ExpenseSplit.Application.Queries.InvitationQueries;

public class GetCancelledInvitationsQuery : IRequest<BaseResponse<List<InvitationResponse>>>
{
}


public class GetCancelledInvitationsQueryHandler : IRequestHandler<GetCancelledInvitationsQuery, BaseResponse<List<InvitationResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    public GetCancelledInvitationsQueryHandler(IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    public async Task<BaseResponse<List<InvitationResponse>>> Handle(GetCancelledInvitationsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId))
            return BaseResponse<List<InvitationResponse>>.Fail("Unauthorize user.");

        var invitations = await _uow.InvitationRepository.GetCancelledInvitationsAsync(userId, cancellationToken);
        var mappedResponse = _mapper.Map<List<InvitationResponse>>(invitations);

        var message = mappedResponse.Any() ? "Cancelled invitations fetched successfully." : "No records found.";
        return BaseResponse<List<InvitationResponse>>.Success(mappedResponse, message);
    }
}
