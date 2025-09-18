using LegalPark.Repositories.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace LegalPark.Security.Jwt
{
    public class JwtAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IJwtService jwtService, IUserRepository userRepository)
        {
            // Ambil token dari header Authorization
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                try
                {
                    // PERBAIKAN: Memanggil metode yang benar, yaitu extractPrincipalFromToken
                    var principal = jwtService.extractPrincipalFromToken(token);
                    if (principal != null)
                    {
                        // Ambil userId dari claim token
                        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                        if (userIdClaim != null)
                        {
                            var userId = Guid.Parse(userIdClaim.Value);
                            // Menggunakan metode GetByIdAsync dari IGenericRepository
                            // yang merupakan implementasi yang benar.
                            var user = await userRepository.GetByIdAsync(userId);

                            if (user != null)
                            {
                                // Jika pengguna ditemukan, set context.User
                                context.User = principal;
                            }
                        }
                    }
                }
                catch (SecurityTokenExpiredException)
                {
                    // Tangani token kedaluwarsa
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token has expired.");
                    return;
                }
                catch
                {
                    // Tangani token tidak valid lainnya
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid token.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
