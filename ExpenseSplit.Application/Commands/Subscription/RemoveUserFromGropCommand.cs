using ExpenseSplit.Common.RequestDTOs.Subscription;
using ExpenseSplit.Common.ResponseDTOs;
using MediatR;

namespace ExpenseSplit.Application.Commands.Subscription;

public class RemoveUserFromGropCommand : IRequest<BaseResponse<bool>>
{
    public RemoveUserRequest RemoveUserRequest { get; set; }
}

public class RemoveUserFromGropCommandHandler : IRequestHandler<RemoveUserFromGropCommand, BaseResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveUserFromGropCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<BaseResponse<bool>> Handle(RemoveUserFromGropCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.UserGroupRepository.RemoveUserAsync(request.RemoveUserRequest.UserId, request.RemoveUserRequest.GroupId, cancellationToken);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (result > 0)
        {
            return BaseResponse<bool>.Success(true, "User removed successfully.");
        }

        return BaseResponse<bool>.Fail("An error occured during removal of user from group.");
    }
}
