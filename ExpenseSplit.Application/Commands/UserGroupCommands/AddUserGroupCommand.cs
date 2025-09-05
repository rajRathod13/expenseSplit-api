using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Domain.Entities;
using MediatR;

namespace ExpenseSplit.Application.Commands.UserGroupCommands;

public class AddUserGroupCommand : IRequest<BaseResponse<bool>>
{
    public Guid GroupId { get; set; }
}

public class AddUserGroupCommandhandler : IRequestHandler<AddUserGroupCommand, BaseResponse<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public AddUserGroupCommandhandler(IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    public async Task<BaseResponse<bool>> Handle(AddUserGroupCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId))
            return BaseResponse<bool>.Fail("User not authenticated.");

        var group = await _uow.GroupDetailRepository.GetGroupDetailAsync(request.GroupId, cancellationToken);
        if (group is null)
            return BaseResponse<bool>.Fail($"No group found with group id - {request.GroupId}.");

        var userGroup = new UserGroupRef
        {
            UserGroupId = Guid.NewGuid(),
            UserId = userId,
            GroupId = group.GroupId,
            IsCreator = false,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = Guid.Parse(userId)
        };

        var addedGroup = await _uow.UserGroupRepository.AddAsyn(userGroup, cancellationToken);
        var result = await _uow.SaveChangesAsync(cancellationToken);
        if (result > 0)
            return BaseResponse<bool>.Success(true, $"You are now member of group - {group.Title}.");

        return BaseResponse<bool>.Fail("An error occured in the operation.");
    }
}
