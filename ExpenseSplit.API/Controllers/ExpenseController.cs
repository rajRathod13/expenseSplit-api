using ExpenseSplit.Application.Commands.ExpenseCommands;
using ExpenseSplit.Application.Queries.Expense;
using ExpenseSplit.Common.RequestDTOs.Expense;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplit.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExpenseController : BaseController
{
    private readonly ISender _sender;

    public ExpenseController(ISender sender)
    {
        _sender = sender;
    }
    [HttpPost("UpsertExpense")]
    [Authorize]
    public async Task<ActionResult> UpsertExpense([FromBody] UpsertExpenseRequest request)
    {
        var response = await _sender.Send(new UpsertExpenseCommand { UpsertExpenseRequest = request });
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
    }

    [HttpGet("GetExpensesByGroupId")]
    [Authorize]
    public async Task<ActionResult> GetExpensesByGroupId([FromQuery] Guid groupId, [FromQuery] bool latestOnly)
    {
        if (groupId == Guid.Empty)
            return BadRequest();

        var response = await _sender.Send(new GetExpensesByGroupIdQuery { GroupId = groupId, LatestOnly = latestOnly });
        return Ok(response);
    }
}
