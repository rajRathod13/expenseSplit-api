using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.ResponseDTOs;
using MediatR;

namespace ExpenseSplit.Application.Commands.GroupDetails;

public class DeleteGroupCommand : IRequest<BaseResponse<bool>>
{
    public Guid GroupId { get; set; }
}

public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, BaseResponse<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public DeleteGroupCommandHandler(IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    public async Task<BaseResponse<bool>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId))
            return BaseResponse<bool>.Fail("User not authenticated.");

        var group = await _uow.GroupDetailRepository.GetGroupDetailAsync(request.GroupId, cancellationToken);
        if(group is null)
            return BaseResponse<bool>.Fail($"No group found with id - {request.GroupId}.");

        var userGroup = group.UserGroupRefs
                             .FirstOrDefault(x => x.UserId == userId && x.GroupId == request.GroupId);

        if (userGroup == null || !userGroup.IsCreator)
            return BaseResponse<bool>.Fail("You are not authorized to delete this group.");

        _uow.GroupDetailRepository.DeleteGroup(group);
        var result = await _uow.SaveChangesAsync(cancellationToken);
        if (result > 0)
            return BaseResponse<bool>.Success(true, "Group deleted successfully.");

        return BaseResponse<bool>.Fail("An error occured during deleting the group.");
    }
}
