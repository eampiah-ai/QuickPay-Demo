using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Login(string username, string password)
    {
        return string.Equals(username, password) ? Ok() : Unauthorized();
    }
}