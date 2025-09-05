using AutoMapper;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Common.ResponseDTOs.User;
using MediatR;

namespace ExpenseSplit.Application.Queries.GroupDetail;

public class GetGroupUsersByGroupIdQuery : IRequest<BaseResponse<List<UserResponse>>>
{
    public Guid GroupId { get; set; }
}

public class GetGroupUsersByGroupIdQueryHandler : IRequestHandler<GetGroupUsersByGroupIdQuery, BaseResponse<List<UserResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetGroupUsersByGroupIdQueryHandler(IUnitOfWork uow,
        IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }
    public async Task<BaseResponse<List<UserResponse>>> Handle(GetGroupUsersByGroupIdQuery request, CancellationToken cancellationToken)
    {
        var userIds = await _uow.UserGroupRepository.GetUserIdsAsync(request.GroupId, cancellationToken);

        var users = await _uow.UserRepository.GetUsersByGroupAsync(request.GroupId, cancellationToken);
        var mappedResponse = _mapper.Map<List<UserResponse>>(users);
        var message = !users.Any() ? "No users found related to this group." : "Group users fetched successfully.";

        return BaseResponse<List<UserResponse>>.Success(mappedResponse,message);
    }
}
