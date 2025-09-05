using ExpenseSplit.Application.ServiceInterfaces;
using System.Security.Claims;

namespace ExpenseSplit.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public string CurrentUserId => _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;

    public string Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
}
