using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.RequestDTOs.User;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Common.ResponseDTOs.AuthDTOs;
using ExpenseSplit.Domain.Entities;
using ExpenseSplit.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace ExpenseSplit.Application.Commands.UserCommands;

public class LoginUserCommand : IRequest<BaseResponse<LoginResponse>>
{
    public LoginUserRequest LoginUserRequest { get; set; }
}

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, BaseResponse<LoginResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public LoginUserCommandHandler(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor,
        ITokenService tokenService,
        IMapper mapper)
    {
        _uow = uow;
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;
        _mapper = mapper;
    }
    public async Task<BaseResponse<LoginResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.LoginUserRequest.Email))
            return BaseResponse<LoginResponse>.Fail("Email can not null or empty.");

        var existingUser = await _uow.UserRepository.FindUserbyEmailAsync(request.LoginUserRequest.Email);
        if (existingUser is null)
            throw new Exception($"User not found with email - {request.LoginUserRequest.Email}");

        var isValidPassword = await _uow.UserRepository.ValidatePasswordAsync(existingUser, request.LoginUserRequest.Password);
        if (!isValidPassword)
            throw new Exception("Invalid password.");

        var roles = new List<string>();
        var token = _tokenService.GenerateToken(existingUser.Email, existingUser.Id, roles.ToList());
        if (string.IsNullOrWhiteSpace(token))
            return BaseResponse<LoginResponse>.Fail( "An error occured during token generation.");
        var mappedResponse = _mapper.Map<LoginResponse>(existingUser);
        var csrfToken = Guid.NewGuid().ToString();

        SetToken(token, "AccessToken", 10, false);
        SetToken(csrfToken, "X-CSRF", 12, true);

        return BaseResponse<LoginResponse>.Success(mappedResponse, "Login successfully.");
    }

    private void SetToken(string token, string type, int expirationTime, bool isCSRF)
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(type, token, new CookieOptions
        {
            HttpOnly = !isCSRF,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            Expires = DateTime.UtcNow.AddMinutes(expirationTime)
        });
    }
}
