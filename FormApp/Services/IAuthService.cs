using FormApp.Data;
using FormApp.Models;

namespace FormApp.Services;

public interface IAuthService
{
    string GenerateJwtToken(User user);
    Task<string> GoogleLoginAsync(GoogleAuthSignin googleAuthSignin);
}