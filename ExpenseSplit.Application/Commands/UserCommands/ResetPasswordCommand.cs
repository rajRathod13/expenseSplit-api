using ExpenseSplit.Common.RequestDTOs.User;
using ExpenseSplit.Common.ResponseDTOs;
using MediatR;

namespace ExpenseSplit.Application.Commands.UserCommands;

public class ResetPasswordCommand : IRequest<BaseResponse<bool>>
{
    public ResetPasswordRequest ResetPasswordRequest { get; set; }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, BaseResponse<bool>>
{
    private readonly IUnitOfWork _uow;

    public ResetPasswordCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<BaseResponse<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _uow.UserRepository.FindUserbyEmailAsync(request.ResetPasswordRequest.Email);
        if (existingUser is null)
            throw new Exception($"User not found with email - {request.ResetPasswordRequest.Email}");

        var result = await _uow.UserRepository.ResetPasswordAsync(existingUser,
                                               request.ResetPasswordRequest.ResetToken,
                                               request.ResetPasswordRequest.NewPassword);

        if (!result)
            return BaseResponse<bool>.Fail("An error occured during reseting the password.");

        return BaseResponse<bool>.Success(true, "Password reset successfully. Please try login.");
    }
}
