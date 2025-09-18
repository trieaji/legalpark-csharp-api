using LegalPark.Models.DTOs.Request.User;
using LegalPark.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LegalPark.Services.User
{
    public interface IUserService
    {
        Task<IActionResult> VerificationAccount(AccountVerification request);
        Task<IActionResult> UpdateAccountStatus(string userId, AccountStatus newStatus);
    }
}
