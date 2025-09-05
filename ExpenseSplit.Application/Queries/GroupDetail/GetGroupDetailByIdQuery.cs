using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Common.ResponseDTOs.GroupDetail;
using MediatR;

namespace ExpenseSplit.Application.Queries.GroupDetail;

public class GetGroupDetailByIdQuery : IRequest<BaseResponse<GroupDetailResponse>>
{
    public Guid GroupId { get; set; }
}

public class GetGroupDetailByIdQueryHandler : IRequestHandler<GetGroupDetailByIdQuery, BaseResponse<GroupDetailResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetGroupDetailByIdQueryHandler(IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    public async Task<BaseResponse<GroupDetailResponse>> Handle(GetGroupDetailByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId))
            return BaseResponse<GroupDetailResponse>.Fail("User not authenticated.");

        var groupDetail = await _uow.GroupDetailRepository.GetGroupDetailAsync(request.GroupId, cancellationToken);
        var userGrpoup = groupDetail.UserGroupRefs
                                    .FirstOrDefault(x => x.UserId == userId && x.GroupId == request.GroupId);

        var isCreator = userGrpoup.IsCreator;
        var mappdeResponse = _mapper.Map<GroupDetailResponse>(groupDetail);
        mappdeResponse.IsCreator = isCreator;
        if (groupDetail is null)
            return BaseResponse<GroupDetailResponse>.Success(null, $"No record found with id - {request.GroupId}");

        return BaseResponse<GroupDetailResponse>.Success(mappdeResponse, "Group detail fetched successfully.");
    }
}
