using LegalPark.Exception;
using LegalPark.Helpers;
using LegalPark.Models.DTOs.Request.Notification;
using LegalPark.Models.DTOs.Request.User;
using LegalPark.Models.DTOs.Response.User;
using LegalPark.Models.Entities;
using LegalPark.Repositories.LogVerification;
using LegalPark.Repositories.User;
using LegalPark.Security.Jwt;
using LegalPark.Services.Notification;
using LegalPark.Services.Template;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LegalPark.Services.Auth
{
    public class AuthService : IAuthService
    {
        // Field untuk Dependency Injection.
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly UserManager<LegalPark.Models.Entities.User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly INotificationService _notificationService;
        private readonly ILogVerificationRepository _logVerificationRepository;
        private readonly ITemplateService _templateService;

        /// <summary>
        /// Konstruktor untuk inisialisasi AuthService dengan dependensi yang diperlukan.
        /// </summary>
        /// <param name="userRepository">Repository untuk akses data pengguna.</param>
        /// <param name="jwtService">Layanan untuk membuat token JWT.</param>
        /// <param name="userManager">Layanan untuk mengelola pengguna dan kata sandi.</param>
        public AuthService(IUserRepository userRepository, IJwtService jwtService, UserManager<LegalPark.Models.Entities.User> userManager, RoleManager<IdentityRole<Guid>> roleManager,
            INotificationService notificationService,
            ILogVerificationRepository logVerificationRepository, ITemplateService templateService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _userManager = userManager;
            _roleManager = roleManager;
            _notificationService = notificationService;
            _logVerificationRepository = logVerificationRepository;
            _templateService = templateService;
        }

        /// <summary>
        /// Metode untuk menangani permintaan login pengguna.
        /// </summary>
        /// <param name="request">Objek LoginRequest yang berisi email dan kata sandi.</param>
        /// <returns>IActionResult yang berisi respons sukses atau error.</returns>
        public async Task<IActionResult> Login(LoginRequest request)
        {
            // 1. Cari pengguna berdasarkan email.
            // Gunakan metode findByEmail dari UserRepository yang sudah Anda sediakan.
            var user = await _userRepository.findByEmail(request.Email);

            // 2. Periksa apakah pengguna ditemukan dan kata sandi cocok.
            // Jika pengguna tidak ditemukan atau kata sandi salah, kembalikan respons error.
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                // Menggunakan ResponseHandler yang Anda berikan untuk membuat respons error.
                return ResponseHandler.GenerateResponseError(
                    HttpStatusCode.Unauthorized,
                    new { Message = "Invalid email or password." },
                    "Unauthorized"
                );
            }

            // 3. Jika otentikasi berhasil, buat token dan refresh token.
            // Gunakan JwtService yang Anda sediakan.
            string token = await _jwtService.generateToken(user);
            string refreshToken = await _jwtService.generateRefreshToken(user.Id);

            // 4. Buat objek SignResponse untuk respons sukses.
            // Mengisi data email, role, token, dan refresh token.
            var response = new SignResponse
            {
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token,
                RefreshToken = refreshToken
            };

            // 5. Kembalikan respons sukses menggunakan ResponseHandler.
            return ResponseHandler.GenerateResponseSuccess(response);
        }


        // Metode Register yang diperbaiki
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var user = new LegalPark.Models.Entities.User
            {
                UserName = request.AccountName,
                Email = request.Email,
                AccountName = request.AccountName,
                //Role = Role.USER,
                AccountStatus = AccountStatus.PENDING_VERIFICATION,
                Balance = 100000.00m
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // --- Logika Penambahan Peran (Role) ---
            var roleExists = await _roleManager.RoleExistsAsync("User");
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>("User"));
            }

            // Tambahkan peran "User" ke pengguna
            await _userManager.AddToRoleAsync(user, "User");
            // ----------------------------------------

            // ============ Log Account Verifikasi ========
            var verification = new LogVerification
            {
                Code = GenerateOtp.GenerateRandomNumber(),
                UserId = user.Id,
                Expired = GenerateOtp.GetExpiryDate(),
                IsVerify = false
            };

            // Pertama, tambahkan objek 'verification' ke konteks
            await _logVerificationRepository.AddAsync(verification);

            // Kemudian, panggil SaveChangesAsync() untuk menyimpan perubahan
            await _logVerificationRepository.SaveChangesAsync();

            // ============= Kirim Email (menggunakan Template Service) =================
            // Buat dictionary untuk template variables
            var templateVariables = new Dictionary<string, object>
            {
                { "otp", verification.Code },
                { "name", user.AccountName }
            };

            // Panggil template service untuk memproses template
            string emailBody = await _templateService.ProcessEmailTemplateAsync("email_verification", templateVariables);

            // Kirim email dengan body yang sudah diproses
            var emailRequest = new EmailNotificationRequest
            {
                To = user.Email,
                Subject = "Account Verification",
                Body = emailBody
            };

            await _notificationService.SendEmailNotification(emailRequest);

            // Generate token setelah pengguna berhasil dibuat
            var token = await _jwtService.generateToken(user);

            var response = new RegisterResponse
            {
                Email = user.Email,
                Token = token
            };

            return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "Registration successful.", response);
        }


        // Method baru untuk mengubah peran pengguna.
        public async Task<IActionResult> UpdateUserRole(UpdateRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "User not found.");
            }

            // Periksa apakah peran baru valid (Admin atau User)
            if (request.Role.ToLower() != "admin" && request.Role.ToLower() != "user")
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid role specified.");
            }

            // Dapatkan semua peran pengguna saat ini
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Hapus peran lama
            if (currentRoles.Any())
            {
                var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!result.Succeeded)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, "FAILED", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // Tambahkan peran baru
            var addRoleResult = await _userManager.AddToRoleAsync(user, request.Role);
            if (!addRoleResult.Succeeded)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, "FAILED", string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
            }

            // Generate token baru
            var claims = (await _userManager.GetClaimsAsync(user)).ToList();
            var newClaims = (await _userManager.GetClaimsAsync(user)).ToList();
            newClaims.Add(new Claim(ClaimTypes.Role, request.Role));

            var newJwtToken = await _jwtService.generateToken(user, newClaims);

            return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "SUCCESS", new { Message = "User role updated successfully.", Token = newJwtToken });
        }

        // Metode Logout yang sudah ada
        public async Task<IActionResult> Logout(string email)
        {
            var user = await _userRepository.findByEmail(email);
            if (user == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "User not found.");
            }

            user.Balance = 100000.00m;
            user.UpdatedAt = DateTime.Now;

            // Panggil metode Update secara sinkron, tanpa await
            _userRepository.Update(user);

            // Panggil SaveChangesAsync untuk menyimpan perubahan ke database
            await _userRepository.SaveChangesAsync();

            return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "Logout successful. Your virtual balance has been reset to 100k.", null);
        }

    }
}
