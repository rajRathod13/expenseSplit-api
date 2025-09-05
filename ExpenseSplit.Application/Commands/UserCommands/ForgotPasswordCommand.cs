using ExpenseSplit.Common.RequestDTOs.User;
using ExpenseSplit.Common.ResponseDTOs;
using MediatR;

namespace ExpenseSplit.Application.Commands.UserCommands;

public class ForgotPasswordCommand : IRequest<BaseResponse<string>>
{
    public ForgotPasswordRequest ForgotPasswordRequest { get; set; }
}

public class ForgotPasswordCommandhandler : IRequestHandler<ForgotPasswordCommand, BaseResponse<string>>
{
    private readonly IUnitOfWork _uow;

    public ForgotPasswordCommandhandler(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<BaseResponse<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _uow.UserRepository.FindUserbyEmailAsync(request.ForgotPasswordRequest.Email);
        if (existingUser is null)
            throw new Exception($"User not found with email - {request.ForgotPasswordRequest.Email}");

        var result = await _uow.UserRepository.ForgotPasswordAsync(existingUser);
        if (string.IsNullOrEmpty(result))
            return BaseResponse<string>.Fail("An error occured during generating reset token.");

        return BaseResponse<string>.Success(result, "Please check your mail for reset the password.");
    }
}
