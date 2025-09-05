using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.RequestDTOs.Expense;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Domain.Entities;
using MediatR;

namespace ExpenseSplit.Application.Commands.ExpenseCommands;

public class UpsertExpenseCommand : IRequest<BaseResponse<Guid>>
{
    public UpsertExpenseRequest UpsertExpenseRequest { get; set; }
}

public class UpsertExpenseCommandHandler : IRequestHandler<UpsertExpenseCommand, BaseResponse<Guid>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UpsertExpenseCommandHandler(IUnitOfWork uow,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<BaseResponse<Guid>> Handle(UpsertExpenseCommand request, CancellationToken ct)
    {
        var userId = _currentUserService.CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId))
            return BaseResponse<Guid>.Fail("User not authenticated.");

        var isCreate = !request.UpsertExpenseRequest.ExpenseId.HasValue
                       || request.UpsertExpenseRequest.ExpenseId == Guid.Empty;

        var expenseId = isCreate
            ? Guid.NewGuid()
            : request.UpsertExpenseRequest.ExpenseId!.Value;

        // Guard: user must be in group
        var isMember = await _uow.UserGroupRepository
            .IsGroupMemberAsync(userId, request.UpsertExpenseRequest.GroupId, ct);

        if (!isMember)
            return BaseResponse<Guid>.Fail("You are not a member of this group.");

        // Prepare splits (single source of truth)
        var groupMembers = await _uow.UserGroupRepository
            .GetUserIdsAsync(request.UpsertExpenseRequest.GroupId, ct);

        var splits = PrepareSplitDetails(request.UpsertExpenseRequest, groupMembers, userId, expenseId);
        if (splits is null || splits.Count == 0)
            return BaseResponse<Guid>.Fail("Invalid or incomplete split configuration.");

        if (isCreate)
        {
            // Map only the expense fields (children ignored by profile)
            var expense = _mapper.Map<Expense>(request.UpsertExpenseRequest);
            expense.ExpenseId = expenseId;
            expense.CreatedOn = DateTime.UtcNow;
            expense.CreatedBy = Guid.Parse(userId);

            // Attach the computed children ONCE
            expense.SplitDetails = splits;

            await _uow.ExpenseRepository.AddExpenseAsync(expense, ct);
        }
        else
        {
            // Load and update aggregate root
            var existing = await _uow.ExpenseRepository.GetExpenceByIdAsync(expenseId, ct);
            if (existing is null)
                return BaseResponse<Guid>.Fail($"The expense record with id - {expenseId} not found.");

            existing.Description = request.UpsertExpenseRequest.Description;
            existing.TotalAmount = request.UpsertExpenseRequest.TotalAmount;
            existing.SplitType = request.UpsertExpenseRequest.SplitType;
            existing.UpdatedBy = Guid.Parse(userId);
            existing.UpdatedOn = DateTime.UtcNow;

            // Replace children (two common approaches)

            // A) If your repository has a bulk delete, keep it simple:
            await _uow.SplitDetailRepository.DeleteByExpenseIdAsync(expenseId);

            // Then add the new computed rows
            await _uow.SplitDetailRepository.AddManySplitDetailsAsync(splits, ct);

            // (Alternative B is to load collection and manipulate tracked entities directly.)
        }

        var saved = await _uow.SaveChangesAsync(ct);
        if (saved > 0)
            return BaseResponse<Guid>.Success(expenseId, "Expense saved successfully.");

        return BaseResponse<Guid>.Fail("An error occurred while saving the expense.");
    }

    private List<SplitDetail> PrepareSplitDetails(UpsertExpenseRequest request, List<string> groupMembers, string userId, Guid expenseId)
    {
        var splits = new List<SplitDetail>();

        var splitDetails = request.SplitDetails ?? new List<SplitDetailDTO>();
        var selectedUserIds = splitDetails.Any()
            ? splitDetails.Select(x => x.UserId).ToList()
            : groupMembers;

        switch (request.SplitType)
        {
            case "Equals":
                var share = Math.Round(request.TotalAmount / selectedUserIds.Count, 2);
                foreach (var userIdEntry in selectedUserIds)
                {
                    splits.Add(new SplitDetail
                    {
                        SplitDetailId = Guid.NewGuid(),
                        ExpenseId = expenseId,
                        UserId = userIdEntry,
                        ShareAmount = share,
                        IsSettled = false,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = Guid.Parse(userId)
                    });
                }
                break;

            case "Percentage":
                var totalPercentage = splitDetails.Sum(x => x.Percentage ?? 0);
                if (totalPercentage != 100) return null;

                foreach (var detail in splitDetails)
                {
                    splits.Add(new SplitDetail
                    {
                        SplitDetailId = Guid.NewGuid(),
                        ExpenseId = expenseId,
                        UserId = detail.UserId,
                        Percentage = detail.Percentage,
                        ShareAmount = Math.Round(request.TotalAmount * (detail.Percentage!.Value / 100), 2),
                        IsSettled = false,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = Guid.Parse(userId)
                    });
                }
                break;

            case "Custom":
                var totalCustom = splitDetails.Sum(x => x.ShareAmount ?? 0);
                if (totalCustom != request.TotalAmount) return null;

                foreach (var detail in splitDetails)
                {
                    splits.Add(new SplitDetail
                    {
                        SplitDetailId = Guid.NewGuid(),
                        ExpenseId = expenseId,
                        UserId = detail.UserId,
                        ShareAmount = detail.ShareAmount!.Value,
                        IsSettled = false,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = Guid.Parse(userId)
                    });
                }
                break;

            default:
                return null;
        }

        return splits;
    }
}
