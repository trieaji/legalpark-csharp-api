using LegalPark.Models.DTOs.Request.User;
using LegalPark.Models.Entities;
using LegalPark.Services.Auth;
using LegalPark.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LegalPark.Controllers.Auth
{
    
    [ApiController]
    [Route("api/v1/auth")]
    
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        
        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            return await _authService.Login(request);
        }

        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            return await _authService.Register(request);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPost("update-role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateRoleRequest request)
        {
            var result = await _authService.UpdateUserRole(request);
            return result;
        }

        
        [Authorize] 
        [HttpPost("verification-account")]
        public async Task<IActionResult> VerificationAccount([FromBody] AccountVerification request)
        {
            return await _userService.VerificationAccount(request);
        }

        
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            
            var email = User.FindFirst(ClaimTypes.Name)?.Value;

            
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("User is not authenticated or email is missing from claims.");
            }

            return await _authService.Logout(email);
        }
    }

}
