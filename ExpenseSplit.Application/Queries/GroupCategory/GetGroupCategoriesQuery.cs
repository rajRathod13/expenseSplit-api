using AutoMapper;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Common.ResponseDTOs.GroupCategory;
using MediatR;

namespace ExpenseSplit.Application.Queries.GroupCategory;

public class GetGroupCategoriesQuery : IRequest<BaseResponse<List<GroupCategoryResponse>>>
{
}

public class GetGroupCategoriesQueryHandler : IRequestHandler<GetGroupCategoriesQuery, BaseResponse<List<GroupCategoryResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetGroupCategoriesQueryHandler(IUnitOfWork uow,
        IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }
    public async Task<BaseResponse<List<GroupCategoryResponse>>> Handle(GetGroupCategoriesQuery request, CancellationToken cancellationToken)
    {
        var groupCategories = await _uow.GroupCategoryMasterRepository.GetAllAsync();
        var mappedResponse = _mapper.Map<List<GroupCategoryResponse>>(groupCategories);
        return BaseResponse<List<GroupCategoryResponse>>.Success(mappedResponse, "Categories fetched successfully.");

    }
}
