using LegalPark.Exception;
using LegalPark.Helpers;
using LegalPark.Models.DTOs.Request.Notification;
using LegalPark.Models.DTOs.Request.User;
using LegalPark.Models.DTOs.Response.User;
using LegalPark.Models.Entities;
using LegalPark.Repositories.LogVerification;
using LegalPark.Repositories.User;
using LegalPark.Services.Notification;
using LegalPark.Services.Template;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Transactions;

namespace LegalPark.Services.User
{
    public class UserService : IUserService
    {
        // --- Dependency Injection ---
        private readonly InfoAccount _infoAccount;
        private readonly IUserRepository _usersRepository;
        private readonly ILogVerificationRepository _logVerificationRepository;
        private readonly INotificationService _notificationService;
        private readonly ITemplateService _templateService;

        // Constructor untuk injection
        public UserService(
            InfoAccount infoAccount,
            IUserRepository usersRepository,
            ILogVerificationRepository logVerificationRepository,
            INotificationService notificationService,
            ITemplateService templateService)
        {
            _infoAccount = infoAccount;
            _usersRepository = usersRepository;
            _logVerificationRepository = logVerificationRepository;
            _notificationService = notificationService;
            _templateService = templateService;
        }

        // Metode untuk memverifikasi akun pengguna
        public async Task<IActionResult> VerificationAccount(AccountVerification request)
        {
            try
            {
                // Menggunakan `using` untuk TransactionScope agar transaksi dikelola dengan benar
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var currentUser = await _infoAccount.GetAsync();
                    string userEmail = currentUser?.Email;

                    // Mencari log verifikasi berdasarkan email pengguna dan kode OTP yang dimasukkan
                    var verify = await _logVerificationRepository.getByUserAndExp(userEmail, request.Code);

                    // Jika tidak ada log verifikasi yang cocok
                    if (verify == null)
                    {
                        return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Otp yang anda masukan salah");
                    }

                    // Logika pengecekan kedaluwarsa OTP
                    if (verify.Expired != null)
                    {
                        var getExpi = verify.Expired;
                        var currentTime = DateTime.Now;

                        if (IsExpired(getExpi, currentTime))
                        {
                            // Catatan: Logika asli di sini tampaknya aneh karena mengembalikan
                            // respons sukses untuk OTP yang kedaluwarsa. Dalam implementasi
                            // yang lebih baik, mungkin lebih tepat untuk mengembalikan error.
                            // Kode ini mereplikasi logika asli.
                            var newVerification = new LogVerification
                            {
                                UserId = verify.UserId,
                                Code = GenerateOtp.GenerateRandomNumber(),
                                Expired = GenerateOtp.GetExpiryDate(),
                                IsVerify = false
                            };
                            await _logVerificationRepository.AddAsync(newVerification);
                            scope.Complete(); // Menyelesaikan transaksi
                            return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "OTP expired. A new OTP has been generated.");
                        }
                    }

                    // Mengatur status verifikasi menjadi true
                    verify.IsVerify = true;
                    _logVerificationRepository.Update(verify);

                    await _logVerificationRepository.SaveChangesAsync();

                    // Mengambil data pengguna dan memperbarui status akun
                    var user = await _usersRepository.findByEmail(userEmail);
                    if (user == null)
                    {
                        return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "User not found.");
                    }
                    user.AccountStatus = AccountStatus.ACTIVE;
                    _usersRepository.Update(user);

                    await _usersRepository.SaveChangesAsync();

                    // Mengirim notifikasi email verifikasi sukses
                    var templateVariables = new Dictionary<string, object>
                    {
                        { "name", user.AccountName }
                    };
                    string emailBody = await _templateService.ProcessEmailTemplateAsync("email_success_verification", templateVariables);

                    var emailRequest = new EmailNotificationRequest
                    {
                        To = user.Email,
                        Subject = "Success Verification",
                        Body = emailBody
                    };
                    await _notificationService.SendEmailNotification(emailRequest);

                    scope.Complete(); // Menyelesaikan transaksi

                    var responseDto = new UserVerificationResponse
                    {
                        Email = user.Email,
                        AccountName = user.AccountName,
                        AccountStatus = user.AccountStatus.ToString()
                    };

                    return ResponseHandler.GenerateResponseSuccess(responseDto);
                }
            }
            catch (System.Exception e)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, e.Message, "Internal Server Error");
            }
        }

        // Metode untuk memperbarui status akun pengguna
        public async Task<IActionResult> UpdateAccountStatus(string userId, AccountStatus newStatus)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Konversi string userId ke Guid
                if (!Guid.TryParse(userId, out var userGuid))
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid user ID format.");
                }

                var user = await _usersRepository.GetByIdAsync(userGuid);
                if (user == null)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "User not found with ID: " + userId);
                }

                // Validasi transisi status
                if (user.AccountStatus == AccountStatus.BLOCKED && newStatus != AccountStatus.ACTIVE)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Cannot change status from BLOCKED directly, unless unblocked by admin.");
                }

                if (user.AccountStatus == AccountStatus.PENDING_VERIFICATION &&
                    !(newStatus == AccountStatus.ACTIVE || newStatus == AccountStatus.BLOCKED))
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid status transition from PENDING_VERIFICATION.");
                }

                user.AccountStatus = newStatus;
                _usersRepository.Update(user);

                await _usersRepository.SaveChangesAsync();

                scope.Complete(); // Menyelesaikan transaksi
                return ResponseHandler.GenerateResponseSuccess(user);
            }
        }

        // Metode helper untuk memeriksa apakah OTP sudah kedaluwarsa (lebih dari 5 menit)
        private static bool IsExpired(DateTime expiredTime, DateTime currentTime)
        {
            return (currentTime - expiredTime).TotalMinutes >= 5;
        }
    }
}
