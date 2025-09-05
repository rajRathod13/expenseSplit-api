using ExpenseSplit.Common.ResponseDTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ExpenseSplit.Application.Commands.UserCommands;

public class LogoutUserCommand : IRequest<BaseResponse<bool>>
{
}

public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, BaseResponse<bool>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LogoutUserCommandHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<BaseResponse<bool>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        DeleteCookie("AccessToken", isCsrf: false);
        DeleteCookie("X-CSRF", isCsrf: true);
        return BaseResponse<bool>.Success(true,"Logout Successfully.");
    }

    private void DeleteCookie(string name, bool isCsrf)
    {
        var resp = _httpContextAccessor.HttpContext?.Response;
        if (resp is null) return;

        // Overwrite with expired cookie using the SAME attributes used when setting
        var opts = new CookieOptions
        {
            HttpOnly = !isCsrf,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            Expires = DateTimeOffset.UnixEpoch
        };

        // Two-step is safest across browsers
        resp.Cookies.Append(name, "", opts);
        resp.Cookies.Delete(name, new CookieOptions
        {
            HttpOnly = !isCsrf,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        });
    }
}
