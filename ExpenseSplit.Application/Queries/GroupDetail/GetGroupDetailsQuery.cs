using AutoMapper;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Common.ResponseDTOs.GroupDetail;
using MediatR;

namespace ExpenseSplit.Application.Queries.GroupDetail;

public class GetGroupDetailsQuery : IRequest<BaseResponse<List<GroupDetailResponse>>>
{
}

public class GetGroupDetailsQueryHandler : IRequestHandler<GetGroupDetailsQuery, BaseResponse<List<GroupDetailResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetGroupDetailsQueryHandler(IUnitOfWork uow,
        IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }
    public async Task<BaseResponse<List<GroupDetailResponse>>> Handle(GetGroupDetailsQuery request, CancellationToken cancellationToken)
    {
        var groupDetails = await _uow.GroupDetailRepository.GetGroupDetailsAsync(cancellationToken);
        var mappedResponse = _mapper.Map<List<GroupDetailResponse>>(groupDetails);
        var message = mappedResponse.Count > 0 ? "Group details fetched successfully." : "No records found.";

        return BaseResponse<List<GroupDetailResponse>>.Success(mappedResponse, message);
    }
}
