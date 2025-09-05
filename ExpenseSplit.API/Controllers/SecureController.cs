using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplit.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SecureController : BaseController
{
    [HttpGet("ping")]
    [Authorize] 
    public IActionResult Ping()
    {
        return Ok(new { isAuthenticated = true, message = "Pong" });
    }
}
