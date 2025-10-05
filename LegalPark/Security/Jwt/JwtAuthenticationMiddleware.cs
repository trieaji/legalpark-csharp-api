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
            
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                try
                {
                    
                    var principal = jwtService.extractPrincipalFromToken(token);
                    if (principal != null)
                    {
                        
                        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                        if (userIdClaim != null)
                        {
                            var userId = Guid.Parse(userIdClaim.Value);
                            
                            var user = await userRepository.GetByIdAsync(userId);

                            if (user != null)
                            {
                                
                                context.User = principal;
                            }
                        }
                    }
                }
                catch (SecurityTokenExpiredException)
                {
                    
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token has expired.");
                    return;
                }
                catch
                {
                    
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid token.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
