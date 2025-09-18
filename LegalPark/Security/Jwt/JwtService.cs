using LegalPark.Models.Entities; // Untuk model User
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration; // Untuk membaca SECRET_KEY dari appsettings.json
using Microsoft.IdentityModel.Tokens; // Pustaka utama untuk token
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt; // Menggantikan io.jsonwebtoken.Jwts
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace LegalPark.Security.Jwt
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _signInKey;
        private readonly UserManager<User> _userManager;

        public JwtService(IConfiguration configuration, UserManager<User> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;

            var jwtSecret = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new InvalidOperationException("JWT SecretKey not configured in appsettings.json.");
            }
            _signInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            
        }

        private ClaimsPrincipal getClaimsPrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signInKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };
            try
            {
                return tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            }
            catch
            {
                return null;
            }
        }

        private bool isTokenExpired(string token)
        {
            try
            {
                var principal = getClaimsPrincipal(token);
                var expirationClaim = principal?.FindFirst(JwtRegisteredClaimNames.Exp);
                if (expirationClaim == null) return true;

                var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim.Value)).UtcDateTime;
                return expirationTime < DateTime.UtcNow;
            }
            catch (SecurityTokenExpiredException)
            {
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool isTokenValid(string token, User user)
        {
            try
            {
                var principal = getClaimsPrincipal(token);
                if (principal == null) return false;

                var userIdFromToken = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                return userIdFromToken != null && userIdFromToken.Equals(user.Id.ToString()) && !isTokenExpired(token);
            }
            catch (SecurityTokenExpiredException)
            {
                throw;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> generateToken(User user, IList<Claim> claims = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            //var claims = new List<Claim>
            //{
            //    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            //    new Claim(JwtRegisteredClaimNames.Email, user.Email),
            //    new Claim(ClaimTypes.Role, user.Role.ToString()),
            //};

            // Jika klaim tidak disediakan (null), ambil klaim dari UserManager
            if (claims == null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                claims = (await _userManager.GetClaimsAsync(user)).ToList();
                foreach (var userRole in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole));
                }
            }

            // Pastikan klaim dasar (sub dan email) selalu ada
            if (!claims.Any(c => c.Type == JwtRegisteredClaimNames.Sub))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
            }
            if (!claims.Any(c => c.Type == JwtRegisteredClaimNames.Email))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            }
            if (!claims.Any(c => c.Type == ClaimTypes.Name))
            {
                claims.Add(new Claim(ClaimTypes.Name, user.Email)); // supaya bisa dipakai User.FindFirst(ClaimTypes.Name)
            }

            if (!claims.Any(c => c.Type == ClaimTypes.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email)); // opsi lain, lebih natural
            }


            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(2),
                SigningCredentials = new SigningCredentials(_signInKey, SecurityAlgorithms.HmacSha256),
                Issuer = issuer,
                Audience = audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return await Task.FromResult(tokenHandler.WriteToken(token));
        }

        public async Task<string> generateRefreshToken(Guid userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(2),
                SigningCredentials = new SigningCredentials(_signInKey, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return await Task.FromResult(tokenHandler.WriteToken(token));
        }

        public ClaimsPrincipal extractPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signInKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                return tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            }
            catch
            {
                return null;
            }
        }
    }
}
