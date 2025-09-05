using ExpenseSplit.Common.ResponseDTOs.Invitation;
using ExpenseSplit.Common.ResponseDTOs;
using MediatR;
using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;

namespace ExpenseSplit.Application.Queries.InvitationQueries;

public class GetAcceptedInvitationsQuery : IRequest<BaseResponse<List<InvitationResponse>>>
{
}
public class GetAcceptedInvitationsQueryHandler : IRequestHandler<GetAcceptedInvitationsQuery, BaseResponse<List<InvitationResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    public GetAcceptedInvitationsQueryHandler(IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    public async Task<BaseResponse<List<InvitationResponse>>> Handle(GetAcceptedInvitationsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId))
            return BaseResponse<List<InvitationResponse>>.Fail("Unauthorize user.");

        var invitations = await _uow.InvitationRepository.GetAcceptedInvitationsAsync(userId, cancellationToken);
        var mappedResponse = _mapper.Map<List<InvitationResponse>>(invitations);

        var message = mappedResponse.Any() ? "Accepted invitations fetched successfully." : "No records found.";
        return BaseResponse<List<InvitationResponse>>.Success(mappedResponse, message);
    }
}
