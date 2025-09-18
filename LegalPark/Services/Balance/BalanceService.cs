using LegalPark.Exception;
using LegalPark.Models.DTOs.Request.Balance;
using LegalPark.Models.DTOs.Response.Balance;
using LegalPark.Models.Entities;
using LegalPark.Repositories.User;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LegalPark.Services.Balance
{
    public class BalanceService : IBalanceService
    {
        private readonly IUserRepository _usersRepository;
        private readonly ILogger<BalanceService> _logger;

        public BalanceService(IUserRepository usersRepository, ILogger<BalanceService> logger)
        {
            _usersRepository = usersRepository;
            _logger = logger;
        }

        public async Task<IActionResult> DeductBalance(DeductBalanceRequest request)
        {
            _logger.LogInformation("Attempting to deduct balance for user ID: {UserId} with amount: {Amount}", request.UserId, request.Amount);

            try
            {
                // Validasi GUID
                if (!Guid.TryParse(request.UserId, out Guid userIdGuid))
                {
                    _logger.LogWarning("Deduct balance failed: Invalid user ID format: {UserId}", request.UserId);
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid User ID format.");
                }

                var user = await _usersRepository.GetByIdAsync(userIdGuid);
                if (user == null)
                {
                    _logger.LogWarning("Deduct balance failed: User not found with ID: {UserId}", request.UserId);
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"User not found with ID: {request.UserId}");
                }

                // Cek status akun: hanya ACTIVE yang bisa melakukan pembayaran
                if (user.AccountStatus != AccountStatus.ACTIVE)
                {
                    _logger.LogWarning("Deduct balance failed: Account is not active for transactions. User ID: {UserId}, Current status: {Status}", user.Id, user.AccountStatus.ToString());
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.Forbidden, "FAILED", $"Account is not active or verified for transactions. Current status: {user.AccountStatus}");
                }

                if (user.Balance < request.Amount)
                {
                    _logger.LogWarning("Deduct balance failed: Insufficient balance for user ID: {UserId}. Current: {Current}, Attempted deduct: {Attempted}", user.Id, user.Balance, request.Amount);
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", $"Insufficient balance. Current balance: {user.Balance}");
                }

                var oldBalance = user.Balance;
                user.Balance -= request.Amount;
                user.UpdatedAt = DateTime.UtcNow;

                _usersRepository.Update(user);
                await _usersRepository.SaveChangesAsync();

                _logger.LogInformation("Balance deducted successfully for user ID: {UserId}. Old: {Old}, Deducted: {Deducted}, New: {New}", user.Id, oldBalance, request.Amount, user.Balance);

                var response = new BalanceResponse
                {
                    UserId = user.Id.ToString(),
                    CurrentBalance = user.Balance,
                    Status = "SUCCESS",
                    Message = "Balance deducted successfully.",
                    Timestamp = DateTime.UtcNow
                };
                return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "Balance deducted successfully.", response);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error deducting balance for user ID {UserId}: {Message}", request.UserId, e.Message);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, "FAILED", $"Failed to deduct balance: {e.Message}");
            }
        }

        public async Task<IActionResult> AddBalance(AddBalanceRequest request)
        {
            _logger.LogInformation("Attempting to add balance for user ID: {UserId} with amount: {Amount}", request.UserId, request.Amount);

            try
            {
                // Validasi GUID
                if (!Guid.TryParse(request.UserId, out Guid userIdGuid))
                {
                    _logger.LogWarning("Add balance failed: Invalid user ID format: {UserId}", request.UserId);
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid User ID format.");
                }

                var user = await _usersRepository.GetByIdAsync(userIdGuid);
                if (user == null)
                {
                    _logger.LogWarning("Add balance failed: User not found with ID: {UserId}", request.UserId);
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"User not found with ID: {request.UserId}");
                }

                var oldBalance = user.Balance;
                user.Balance += request.Amount;
                user.UpdatedAt = DateTime.UtcNow;

                _usersRepository.Update(user);
                await _usersRepository.SaveChangesAsync();

                _logger.LogInformation("Balance added successfully for user ID: {UserId}. Old: {Old}, Added: {Added}, New: {New}", user.Id, oldBalance, request.Amount, user.Balance);

                var response = new BalanceResponse
                {
                    UserId = user.Id.ToString(),
                    CurrentBalance = user.Balance,
                    Status = "SUCCESS",
                    Message = "Balance added successfully.",
                    Timestamp = DateTime.UtcNow
                };
                return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "Balance added successfully.", response);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error adding balance for user ID {UserId}: {Message}", request.UserId, e.Message);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, "FAILED", $"Failed to add balance: {e.Message}");
            }
        }

        public async Task<IActionResult> GetUserBalance(string userId)
        {
            try
            {
                // Validasi GUID
                if (!Guid.TryParse(userId, out Guid userIdGuid))
                {
                    _logger.LogWarning("Get user balance failed: Invalid user ID format: {UserId}", userId);
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid User ID format.");
                }

                var user = await _usersRepository.GetByIdAsync(userIdGuid);
                if (user == null)
                {
                    _logger.LogWarning("Get user balance failed: User not found with ID {UserId}", userId);
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "User not found.");
                }

                var currentBalance = user.Balance;
                var response = new BalanceResponse
                {
                    UserId = user.Id.ToString(),
                    CurrentBalance = currentBalance,
                    Status = "SUCCESS",
                    Message = "User balance retrieved successfully.",
                    Timestamp = DateTime.UtcNow
                };
                return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "User balance retrieved successfully.", response);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "Error retrieving balance for user {UserId}: {Message}", userId, e.Message);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, "FAILED", $"Failed to retrieve balance: {e.Message}");
            }
        }
    }
}
