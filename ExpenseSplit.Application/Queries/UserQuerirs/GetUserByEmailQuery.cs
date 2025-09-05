using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Common.ResponseDTOs.User;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ExpenseSplit.Application.Queries.UserQuerirs;

public class GetUserByEmailQuery : IRequest<BaseResponse<UserResponse>>
{
    //public string Email { get; set; }
}


public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, BaseResponse<UserResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;
    private readonly ICurrentUserService _currentUserService;
    private readonly string _cookieName = "AccessToken";

    public GetUserByEmailQueryHandler(IUnitOfWork uow,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        ITokenService tokenService,
        ICurrentUserService currentUserService)
    {
        _uow = uow;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;
        _currentUserService = currentUserService;
    }
    public async Task<BaseResponse<UserResponse>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var email = _currentUserService.Email;
        if (string.IsNullOrEmpty(email))
            return BaseResponse<UserResponse>.Fail("Unauthorized user.");

        //var email = _tokenService.GetEmailFromToken(token);
        var user = await _uow.UserRepository.FindUserbyEmailAsync(email);
        if (user is null)
            return BaseResponse<UserResponse>.Fail($"No user found with email - {email}");

        var mappedResponse = _mapper.Map<UserResponse>(user);
        return BaseResponse<UserResponse>.Success(mappedResponse, "User fetched successfully.");
    }
}
