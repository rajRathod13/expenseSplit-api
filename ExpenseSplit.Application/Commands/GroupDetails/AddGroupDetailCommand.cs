using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.RequestDTOs.Group;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Common.ResponseDTOs.GroupDetail;
using ExpenseSplit.Domain.Entities;
using MediatR;

namespace ExpenseSplit.Application.Commands.GroupDetails;

public class AddGroupDetailCommand : IRequest<BaseResponse<GroupDetailResponse>>
{
    public AddGroupRequest AddGroupRequest { get; set; }
}

public class AddGroupDetailCommandHandler : IRequestHandler<AddGroupDetailCommand, BaseResponse<GroupDetailResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public AddGroupDetailCommandHandler(IUnitOfWork uow,
        IFileService fileService,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _uow = uow;
        _fileService = fileService;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    public async Task<BaseResponse<GroupDetailResponse>> Handle(AddGroupDetailCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId))
            throw new Exception("User not authenticated. please login first.");

        var groupDetailId = Guid.NewGuid();
        var filePath = await _fileService.SaveFileAsync(request.AddGroupRequest.GroupImageFile, "GroupDetail", groupDetailId.ToString());

        var mappedEntity = _mapper.Map<GroupDetail>(request.AddGroupRequest);
        mappedEntity.GroupId = groupDetailId;
        mappedEntity.UserId = userId;
        mappedEntity.GroupImage = filePath;
        mappedEntity.CreatedOn = DateTime.UtcNow;
        mappedEntity.CreatedBy = Guid.Parse(userId);

        var result = await _uow.GroupDetailRepository.AddAsync(mappedEntity, cancellationToken);

        var userGroup = new UserGroupRef
        {
            UserGroupId = Guid.NewGuid(),
            UserId = userId,
            GroupId = result.GroupId,
            IsCreator = true,
            CreatedBy = Guid.Parse(userId),
            CreatedOn = DateTime.UtcNow,
        };
        var userGroupResponse = await _uow.UserGroupRepository.AddAsyn(userGroup, cancellationToken);
        //result.UserGroupRefs.Add(userGroupResponse);
        await _uow.SaveChangesAsync(cancellationToken);
        var mappedResponse = _mapper.Map<GroupDetailResponse>(result);
        if (result is not null)
            return BaseResponse<GroupDetailResponse>.Success(mappedResponse, "Group details inserted successfully.");

        return BaseResponse<GroupDetailResponse>.Fail("An error occured during insertion operation.");
    }
}
