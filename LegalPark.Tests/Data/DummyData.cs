using LegalPark.Helpers;
using LegalPark.Models.DTOs.Request.User;
using LegalPark.Models.Entities;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace LegalPark.Tests.Data
{
    // Kelas statis untuk menyimpan semua data dummy yang akan digunakan dalam pengujian.
    public static class DummyData
    {
        // Pengguna dummy untuk skenario pengujian yang berhasil.
        public static User DummyUser = new()
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"), // Hash kata sandi untuk pengujian.
            AccountStatus = AccountStatus.Active,
            Role = Role.User,
            CreatedAt = DateTime.UtcNow
        };

        // Pengguna dummy dengan status yang belum diverifikasi.
        public static User DummyUnverifiedUser = new()
        {
            Id = Guid.NewGuid(),
            Email = "unverified@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
            AccountStatus = AccountStatus.Unverified,
            Role = Role.User,
            CreatedAt = DateTime.UtcNow
        };

        // Objek permintaan login yang berhasil.
        public static LoginRequest DummyLoginRequest = new()
        {
            Email = "test@example.com",
            Password = "Password123"
        };

        // Objek permintaan login yang gagal (kata sandi salah).
        public static LoginRequest DummyLoginRequestWrongPassword = new()
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        // Objek permintaan registrasi.
        public static RegisterRequest DummyRegisterRequest = new()
        {
            Email = "newuser@example.com",
            Password = "NewPassword123"
        };

        // Objek permintaan untuk memperbarui peran.
        public static UpdateRoleRequest DummyUpdateRoleRequest = new()
        {
            Id = DummyUser.Id,
            Role = Role.Admin
        };

        // Objek tanggapan otentikasi.
        public static AuthResponse DummyAuthResponse = new()
        {
            IsSuccess = true,
            Message = "Authentication successful"
        };

        // Kumpulan klaim untuk token JWT.
        public static List<Claim> DummyClaims = new()
    {
        new Claim(ClaimTypes.NameIdentifier, DummyUser.Id.ToString()),
        new Claim(ClaimTypes.Role, DummyUser.Role.ToString())
    };

        // Log verifikasi dummy.
        public static LogVerification DummyLogVerification = new()
        {
            Id = Guid.NewGuid(),
            UserId = DummyUnverifiedUser.Id,
            Code = new GenerateOtp().Generate(),
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddMinutes(10)
        };
    }
}
