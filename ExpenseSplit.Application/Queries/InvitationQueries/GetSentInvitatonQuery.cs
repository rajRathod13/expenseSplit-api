using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Common.ResponseDTOs.Invitation;
using MediatR;

namespace ExpenseSplit.Application.Queries.InvitationQueries;

public class GetSentInvitatonQuery : IRequest<BaseResponse<List<InvitationResponse>>>
{
}

public class GetSentInvitatonQueryHandler : IRequestHandler<GetSentInvitatonQuery, BaseResponse<List<InvitationResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetSentInvitatonQueryHandler(IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    public async Task<BaseResponse<List<InvitationResponse>>> Handle(GetSentInvitatonQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId))
            return BaseResponse<List<InvitationResponse>>.Fail("Unauthorize user.");

        var invitations = await _uow.InvitationRepository.GetSentInvitationsAsync(userId, cancellationToken);
        var mappedResponse = _mapper.Map<List<InvitationResponse>>(invitations);

        var message = mappedResponse.Any() ? "Sent invitations fetched successfully." : "No records found.";
        return BaseResponse<List<InvitationResponse>>.Success(mappedResponse, message);
    }
}
