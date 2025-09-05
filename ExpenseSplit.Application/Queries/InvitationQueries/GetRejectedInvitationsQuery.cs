using ExpenseSplit.Common.ResponseDTOs.Invitation;
using ExpenseSplit.Common.ResponseDTOs;
using MediatR;
using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;

namespace ExpenseSplit.Application.Queries.InvitationQueries;

public class GetRejectedInvitationsQuery : IRequest<BaseResponse<List<InvitationResponse>>>
{
}

public class GetRejectedInvitationsQueryHandler : IRequestHandler<GetRejectedInvitationsQuery, BaseResponse<List<InvitationResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    public GetRejectedInvitationsQueryHandler(IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    public async Task<BaseResponse<List<InvitationResponse>>> Handle(GetRejectedInvitationsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId))
            return BaseResponse<List<InvitationResponse>>.Fail("Unauthorize user.");

        var invitations = await _uow.InvitationRepository.GetRejectedInvitationsAsync(userId, cancellationToken);
        var mappedResponse = _mapper.Map<List<InvitationResponse>>(invitations);

        var message = mappedResponse.Any() ? "Rejected invitations fetched successfully." : "No records found.";
        return BaseResponse<List<InvitationResponse>>.Success(mappedResponse, message);
    }
}
