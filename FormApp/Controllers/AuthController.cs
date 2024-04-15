using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FormApp.Data;
using FormApp.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FormApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(ILogger<AuthController> logger, AppDbContext context, IConfiguration config)
    {
        _logger = logger;
        _context = context;
        _config = config;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] GoogleAuthSignin googleAuthSignin)
    {
        try
        {
            // Validate google token
            var payload =
                await GoogleJsonWebSignature.ValidateAsync(googleAuthSignin.GoogleToken);

            var user = _context.Users.Where(user => user.GoogleSubject == payload.Subject);

            if (!user.Any())
            {
                _context.Users.Add(new User() { Email = payload.Email, GoogleSubject = payload.Subject });
                await _context.SaveChangesAsync();
            }

            var tokenSting = GenerateJwtToken(user.First());

            return Ok(new { token = tokenSting });
        }
        catch (InvalidJwtException)
        {
            _logger.LogWarning("Invalid Google Token");
            return BadRequest("Invalid Token");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new StatusCodeResult(500);
        }
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            claims: new List<Claim>()
            {
                new ("GoogleSubject", user.GoogleSubject),
                new (ClaimTypes.NameIdentifier, user.Id.ToString())
            },
            expires: DateTime.Now.AddDays(5),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}