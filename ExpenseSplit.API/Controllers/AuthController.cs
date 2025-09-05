using ExpenseSplit.Application.Commands.UserCommands;
using ExpenseSplit.Common.RequestDTOs.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseSplit.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseController
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }

    [HttpPost]
    [Route("Register")]
    public async Task<ActionResult> RegisterUser([FromForm] RegisterUserRequest request)
    {
        var response = await _sender.Send(new RegisterUserCommand { RegisterUserRequest = request });
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
    }

    [HttpPost]
    [Route("Login")]
    public async Task<ActionResult> LoginUser([FromBody] LoginUserRequest request)
    {
        var response = await _sender.Send(new LoginUserCommand { LoginUserRequest = request });
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status401Unauthorized, response);
    }
    
    [HttpPost]
    [Route("Logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        var response = await _sender.Send(new LogoutUserCommand());
        if (response.IsSuccess)
            return Ok(response);

        return StatusCode(StatusCodes.Status500InternalServerError, new { IsSuccess = response.IsSuccess, Message = response.Message });
    }

    [HttpPost]
    [Route("ForgotPassword")]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request) 
    {
        var temp = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var response = await _sender.Send(new ForgotPasswordCommand { ForgotPasswordRequest = request });
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status404NotFound, response);
    }

    [HttpPost]
    [Route("ResetPassword")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request) 
    {
        var response = await _sender.Send(new ResetPasswordCommand { ResetPasswordRequest = request });
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status400BadRequest, response);
    }

    [HttpPost]
    [Route("ChangePassword")]
    [Authorize]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var response =await _sender.Send(new ChangePasswordCommand { ChangePasswordRequest = request });
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status400BadRequest, response);
    }
}
