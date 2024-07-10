using FormApp.Data;
using FormApp.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FormApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Authenticates a user using Google authentication
    /// </summary>
    /// <param name="googleAuthSignin">The Google authentication string</param>
    /// <returns>A JWT token used for authentication</returns>
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] GoogleAuthSignin googleAuthSignin)
    {
        try
        {
            var tokenString = await authService.GoogleLoginAsync(googleAuthSignin);
            return Ok(new { token = tokenString });
        }
        catch (InvalidJwtException)
        {
            return BadRequest("Invalid Token");
        }
        catch (Exception)
        {
            return new StatusCodeResult(500);
        }
    }
}