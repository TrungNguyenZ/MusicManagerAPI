using Microsoft.AspNetCore.Identity;
using MusicManager.Models;
using System.Security.Claims;

namespace MusicManager.Services
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(ApplicationUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
