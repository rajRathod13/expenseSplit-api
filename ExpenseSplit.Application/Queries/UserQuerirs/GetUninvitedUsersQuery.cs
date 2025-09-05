using AutoMapper;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Common.ResponseDTOs.User;
using MediatR;

namespace ExpenseSplit.Application.Queries.UserQuerirs;

public class GetUninvitedUsersQuery : IRequest<BaseResponse<List<UserResponse>>>
{
    public Guid GroupId { get; set; }
}

public class GetUninvitedUsersQueryhandler : IRequestHandler<GetUninvitedUsersQuery, BaseResponse<List<UserResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUninvitedUsersQueryhandler(IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<BaseResponse<List<UserResponse>>> Handle(GetUninvitedUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var users = await _unitOfWork.UserRepository.GetUnInvitedUsersAsync(request.GroupId, cancellationToken);
            var mappedResponse = _mapper.Map<List<UserResponse>>(users);

            return BaseResponse<List<UserResponse>>.Success(mappedResponse, "Users fetched successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }
}
