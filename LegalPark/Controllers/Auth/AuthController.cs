using LegalPark.Models.DTOs.Request.User;
using LegalPark.Models.Entities;
using LegalPark.Services.Auth;
using LegalPark.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LegalPark.Controllers.Auth
{
    // [ApiController] - Menandakan bahwa ini adalah controller API
    // [Route("api/v1/auth")] - Menentukan base URL untuk controller ini
    [ApiController]
    [Route("api/v1/auth")]
    // Menggunakan atribut untuk Swagger/OpenAPI, jika perlu:
    // [Swashbuckle.AspNetCore.Annotations.SwaggerTag("Auth Controller", Description = "Authentication Service")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        // Dependency Injection:
        // ASP.NET Core menggunakan constructor injection secara default
        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        /// <summary>
        /// Endpoint untuk proses login.
        /// </summary>
        /// <param name="request">Request body berisi kredensial login.</param>
        /// <returns>Respons HTTP.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            return await _authService.Login(request);
        }

        /// <summary>
        /// Endpoint untuk pendaftaran pengguna baru.
        /// </summary>
        /// <param name="request">Request body berisi data pendaftaran.</param>
        /// <returns>Respons HTTP.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            return await _authService.Register(request);
        }

        // Endpoint baru untuk mengubah peran pengguna
        [Authorize(Roles = "Admin")]
        [HttpPost("update-role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateRoleRequest request)
        {
            var result = await _authService.UpdateUserRole(request);
            return result;
        }

        /// <summary>
        /// Endpoint untuk verifikasi akun pengguna.
        /// </summary>
        /// <param name="request">Request body berisi data verifikasi akun.</param>
        /// <returns>Respons HTTP.</returns>
        [Authorize] // [Authorize] setara dengan @PreAuthorize di Spring Security
        [HttpPost("verification-account")]
        public async Task<IActionResult> VerificationAccount([FromBody] AccountVerification request)
        {
            return await _userService.VerificationAccount(request);
        }

        /// <summary>
        /// Endpoint untuk logout.
        /// </summary>
        /// <returns>Respons HTTP.</returns>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Mendapatkan email dari principal yang diautentikasi
            // Di ASP.NET Core, `User.FindFirst(ClaimTypes.Name)` mendapatkan nama/username dari principal.
            var email = User.FindFirst(ClaimTypes.Name)?.Value;

            // Cek jika email tidak ditemukan
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("User is not authenticated or email is missing from claims.");
            }

            return await _authService.Logout(email);
        }
    }

}
