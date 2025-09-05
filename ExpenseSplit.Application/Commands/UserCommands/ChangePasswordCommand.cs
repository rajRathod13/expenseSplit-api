using ExpenseSplit.Common.RequestDTOs.User;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExpenseSplit.Application.Commands.UserCommands;

public class ChangePasswordCommand : IRequest<BaseResponse<bool>>
{
    public ChangePasswordRequest ChangePasswordRequest { get; set; }
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, BaseResponse<bool>>
{
    private readonly IUnitOfWork _uow;

    public ChangePasswordCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<BaseResponse<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {

        var existingUser = await _uow.UserRepository.FindUserbyEmailAsync(request.ChangePasswordRequest.Email);
        if (existingUser is null)
            throw new Exception($"User not found with email - {request.ChangePasswordRequest.Email}");

        var isValidPassword = await _uow.UserRepository.ValidatePasswordAsync(existingUser, request.ChangePasswordRequest.OldPassword);
        if (!isValidPassword)
            throw new Exception("The old password is incorrect.");

        var result = await _uow.UserRepository.ChangePasswordAsync(existingUser,
                                               request.ChangePasswordRequest.OldPassword,
                                               request.ChangePasswordRequest.NewPassword);

        if (!result)
            return BaseResponse<bool>.Fail("Password change failed.");

        return BaseResponse<bool>.Success(true, "Password changes successfully. Please try login again.");
    }
}
