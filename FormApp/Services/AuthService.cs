using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FormApp.Data;
using FormApp.Models;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;

namespace FormApp.Services;

public class AuthService(IConfiguration config, AppDbContext context, ILogger<IAuthService> logger) : IAuthService
{
    public string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            config["Jwt:Issuer"],
            config["Jwt:Issuer"],
            claims: new List<Claim>()
            {
                new ("GoogleSubject", user.GoogleSubject),
                new (ClaimTypes.NameIdentifier, user.Id.ToString())
            },
            expires: DateTime.Now.AddDays(5),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);

    }

    public async Task<string> GoogleLoginAsync(GoogleAuthSignin googleAuthSignin)
    {
        try
        {
            // Validate google token
            var payload =
                await GoogleJsonWebSignature.ValidateAsync(googleAuthSignin.GoogleToken);

            var user = context.Users.Where(user => user.GoogleSubject == payload.Subject);

            if (!user.Any())
            {
                context.Users.Add(new User() { Email = payload.Email, GoogleSubject = payload.Subject });
                await context.SaveChangesAsync();
            }

            return GenerateJwtToken(user.First());;
        }
        catch (InvalidJwtException)
        {
            logger.LogWarning("Invalid Google Token");
            throw new InvalidJwtException("Invalid Token");
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw;
        }
    }
}