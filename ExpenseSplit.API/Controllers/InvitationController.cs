using ExpenseSplit.Application.Commands.Subscription;
using ExpenseSplit.Application.Queries.InvitationQueries;
using ExpenseSplit.Common.RequestDTOs.Subscription;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplit.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvitationController : BaseController
{
    private readonly ISender _sender;

    public InvitationController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Route("SendInvitation")]
    [Authorize]
    public async Task<ActionResult> SendInvitation([FromBody] InvitationRequest request)
    {
        var response = await _sender.Send(new SendInvitationCommand { InvitationRequest = request });
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status500InternalServerError, response);

    }

    [HttpPost("AcceptInvitation")]
    [Authorize]
    public async Task<ActionResult> AcceptInvitation([FromBody] AcceptInvitationRequest request)
    {
        var response = await _sender.Send(new AcceptInvitationCommand { AcceptInvitationRequest = request });
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
    }

    [HttpPost("RejectInvitation")]
    [Authorize]
    public async Task<ActionResult> RejectInvitation([FromBody] RejectInvitationRequest request) 
    {
        var response = await _sender.Send(new RejectInvitationCommand { RejectInvitationRequest = request });
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
    }

    [HttpPost("CancelInvitation")]
    [Authorize]
    public async Task<ActionResult> CancelInvitation([FromBody] CancelInvitationRequest request)
    {
        var response = await _sender.Send(new CancelInvitationCommand { CancelInvitationRequest = request });
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
    }

    [HttpGet("GetPendingInvitations")]
    [Authorize]
    public async Task<ActionResult> GetPendingInvitations() 
    {
        var response = await _sender.Send(new GetPendingInvitationsQuery());
        return Ok(response);
    }

    [HttpGet("GetSentInvitations")]
    [Authorize]
    public async Task<ActionResult> GetSentInvitations()
    {
        var response = await _sender.Send(new GetSentInvitatonQuery());
        return Ok(response);
    }

    [HttpGet("GetAcceptedInvitations")]
    [Authorize]
    public async Task<ActionResult> GetAcceptedInvitations()
    {
        var response = await _sender.Send(new GetAcceptedInvitationsQuery());
        return Ok(response);
    }

    [HttpGet("GetRejectedInvitations")]
    [Authorize]
    public async Task<ActionResult> GetRejectedInvitations()
    {
        var response = await _sender.Send(new GetRejectedInvitationsQuery());
        return Ok(response);
    }

    [HttpGet("GetCancelledInvitations")]
    [Authorize]
    public async Task<ActionResult> GetCancelldInvitations()
    {
        var response = await _sender.Send(new GetCancelledInvitationsQuery());
        return Ok(response);
    }
}
