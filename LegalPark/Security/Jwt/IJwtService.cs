using LegalPark.Models.Entities;
using System.Security.Claims;

namespace LegalPark.Security.Jwt
{
    public interface IJwtService
    {
        Task<string> generateToken(User user, IList<Claim> claims = null);
        Task<string> generateRefreshToken(Guid userId);
        bool isTokenValid(string token, User user);
        ClaimsPrincipal extractPrincipalFromToken(string token);
    }
}
