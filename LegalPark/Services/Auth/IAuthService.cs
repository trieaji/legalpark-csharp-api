using LegalPark.Models.DTOs.Request.User;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Services.Auth
{
    // Interface untuk layanan otentikasi
    public interface IAuthService
    {
        Task<IActionResult> Login(LoginRequest request);

        
        Task<IActionResult> Register(RegisterRequest request);

        Task<IActionResult> UpdateUserRole(UpdateRoleRequest request);


        Task<IActionResult> Logout(string email);


        // Mendefinisikan kontrak untuk metode pendaftaran pengguna dengan OTP.
        //Task<IActionResult> RegisterWithOtp(RegisterWithOtpRequest request);

        // Mendefinisikan kontrak untuk metode verifikasi OTP.
        //Task<IActionResult> VerifyOtp(VerifyOtpRequest request);

        // Mendefinisikan kontrak untuk metode pengiriman ulang OTP.
        //Task<IActionResult> ResendOtp(ResendOtpRequest request);
    }
}
