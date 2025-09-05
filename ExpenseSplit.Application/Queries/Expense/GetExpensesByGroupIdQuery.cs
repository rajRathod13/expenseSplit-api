using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Common.ResponseDTOs.ExpenseDTO;
using MediatR;

namespace ExpenseSplit.Application.Queries.Expense;

public class GetExpensesByGroupIdQuery : IRequest<BaseResponse<List<ExpenseResponse>>>
{
    public Guid GroupId { get; set; }
    public bool LatestOnly { get; set; }
}

public class GetExpensesByGroupIdQueryHandler : IRequestHandler<GetExpensesByGroupIdQuery, BaseResponse<List<ExpenseResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetExpensesByGroupIdQueryHandler(IUnitOfWork uow,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _uow = uow;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }
    public async Task<BaseResponse<List<ExpenseResponse>>> Handle(GetExpensesByGroupIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId))
            return BaseResponse<List<ExpenseResponse>>.Fail("Unauthorized user.");

        var message = string.Empty;
        var expenses = await _uow.ExpenseRepository.GetExpensesByGroupIdAsync(request.GroupId, request.LatestOnly, cancellationToken);
        message = expenses is null ? "No expenses found" : "Expenses fetched successfully.";

        var mappedResponse = _mapper.Map<List<ExpenseResponse>>(expenses);

        return BaseResponse<List<ExpenseResponse>>.Success(mappedResponse, message);
    }
}
