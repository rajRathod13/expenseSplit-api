using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Common.ResponseDTOs.GroupDetail;
using MediatR;

namespace ExpenseSplit.Application.Queries.GroupDetail;

public class GetGroupDetailsByUserIdQuery : IRequest<BaseResponse<GroupResponse>>
{
}


public class GetGroupDetailsByUserIdQueryHandler : IRequestHandler<GetGroupDetailsByUserIdQuery, BaseResponse<GroupResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetGroupDetailsByUserIdQueryHandler(IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    public async Task<BaseResponse<GroupResponse>> Handle(GetGroupDetailsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId))
            throw new Exception("User not authenticated.");

        var groupIds = await _uow.UserGroupRepository.GetGroupIdsAsync(userId, cancellationToken);

        var memberInGroups = await _uow.GroupDetailRepository.GetGroupsByGroupIds(groupIds ?? new List<Guid>(), cancellationToken);

        var createdGroups = await _uow.GroupDetailRepository.GetGroupsByUserIdAsync(userId, cancellationToken);

        var response = new GroupResponse();
        var mappedCreatedGroups = _mapper.Map<List<GroupDetailResponse>>(createdGroups);
        var mappedMemberInGroups = _mapper.Map<List<GroupDetailResponse>>(memberInGroups);

        mappedCreatedGroups.ForEach(g => g.IsCreator = true);
        mappedMemberInGroups.ForEach(g => g.IsCreator = false);

        OrderChildren(mappedCreatedGroups);

        response.CreatedGroups = mappedCreatedGroups;
        response.GroupsAsMember = mappedMemberInGroups;

        var message = (!createdGroups.Any() && !memberInGroups.Any())
                                     ? "No groups found for the user."
                                     : "Groups fetched successfully.";

        return BaseResponse<GroupResponse>.Success(response, message);
    }

    private void OrderChildren(List<GroupDetailResponse> groups)
    {
        foreach (var g in groups)
        {
            g.UserGroups = g.UserGroups
                .OrderByDescending(x => x.IsCreator)
                .ThenBy(x => x.UserId)
                .ToList();
        }
    }
}
